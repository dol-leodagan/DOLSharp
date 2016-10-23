/*
 * DAWN OF LIGHT - The first free open source DAoC server emulator
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 *
 */
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading;

using DOL.Database;
using DOL.Events;
using DOL.GS.Keeps;
using DOL.GS.Utils;
using DOL.GS.ServerProperties;

using log4net;
using KDSharp.KDTree;
using KDSharp.DistanceFunctions;

namespace DOL.GS
{
	/// <summary>
	/// This class represents a region in DAOC. A region is everything where you
	/// need a loadingscreen to go there. Eg. whole Albion is one Region, Midgard and
	/// Hibernia are just one region too. Darkness Falls is a region. Each dungeon, city
	/// is a region ... you get the clue. Each Region can hold an arbitary number of
	/// Zones! Camelot Hills is one Zone, Tir na Nog is one Zone (and one Region)...
	/// </summary>
    public class Region
    {
        /// <summary>
        /// Defines a logger for this class.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region Constructors
        /// <summary>
        /// Create a new instance of <see cref="Region"/>.
        /// </summary>
        /// <param name="time">TimeManager to which this Region is registered.</param>
        /// <param name="data">The Region Data Configuration.</param>
        public Region(GameTimer.TimeManager time, RegionData data)
        {
            RegionData = data;
            m_timeManager = time;
            
            m_objects = new GameObject[MINIMUM_OBJECT_ARRAY_SIZE];
            m_numPlayer = 0;
            m_objectsInRegion = 0;
            m_nextObjectSlot = 0;

            m_zones = new ReaderWriterList<Zone>(1);
            m_ZoneAreas = new ushort[64][];
            m_ZoneAreasCount = new ushort[64];
            for (int i = 0; i < 64; i++)
            {
                m_ZoneAreas[i] = new ushort[AbstractArea.MAX_AREAS_PER_ZONE];
            }

            m_Areas = new Dictionary<ushort, IArea>();


            List<string> list = null;

            if (ServerProperties.Properties.DEBUG_LOAD_REGIONS != string.Empty)
                list = ServerProperties.Properties.DEBUG_LOAD_REGIONS.SplitCSV(true);

            if (list != null && list.Count > 0)
            {
                m_loadObjects = false;

                foreach (string region in list)
                {
                    if (region.ToString() == ID.ToString())
                    {
                        m_loadObjects = true;
                        break;
                    }
                }
            }

            list = ServerProperties.Properties.DISABLED_REGIONS.SplitCSV(true);
            foreach (string region in list)
            {
                if (region.ToString() == ID.ToString())
                {
                    m_isDisabled = true;
                    break;
                }
            }

            list = ServerProperties.Properties.DISABLED_EXPANSIONS.SplitCSV(true);
            foreach (string expansion in list)
            {
                if (expansion.ToString() == RegionData.Expansion.ToString())
                {
                    m_isDisabled = true;
                    break;
                }
            }
        }
        #endregion
        
        #region Static Default
        /// <summary>
        /// This is the minimumsize for object array that is allocated when
        /// the first object is added to the region must be dividable by 32 (optimization)
        /// </summary>
        protected static readonly int MINIMUM_OBJECT_ARRAY_SIZE = 256;
        
        /// <summary>
        /// Default Bucket Size for BSP Tree
        /// </summary>
        protected static readonly int TREE_DEFAULT_BUCKET_CAPACITY = 32;
        
        /// <summary>
        /// Default Offset used for Region acting as Dungeon
        /// </summary>
        protected static readonly int DUNGEON_REGION_DEFAULT_OFFSET = 8192;
        
        /// <summary>
        /// Default Zone Count used for Region acting as Dungeon
        /// </summary>
        protected static readonly int DUNGEON_REGION_DEFAULT_ZONECOUNT = 1;
        #endregion
        
        #region GameObject Storage
        /// <summary>
        /// Object to lock when changing objects in the array
        /// </summary>
        readonly object ObjectsSyncLock = new object();

        /// <summary>
        /// This holds all objects inside this region. Their index = their id!
        /// </summary>
        GameObject[] m_objects;

        /// <summary>
        /// Returns a Snapshot Array of Objects in this region 
        /// </summary>
        public GameObject[] Objects
        {
            get
            {
                lock (ObjectsSyncLock)
                {
                    return m_objects.ToArray();
                }
            }
        }

        /// <summary>
        /// This holds a counter with the absolute count of all objects that are actually in this region
        /// </summary>
        int m_objectsInRegion;

        /// <summary>
        /// Total number of objects in this region
        /// </summary>
        public virtual int TotalNumberOfObjects { get { return m_objectsInRegion; } }

        /// <summary>
        /// Number of players in the region
        /// </summary>
        int m_numPlayer;

        /// <summary>
        /// Number of players in the region
        /// </summary>
        public virtual int NumPlayers { get { return m_numPlayer; } }

        /// <summary>
        /// Available Region Slots
        /// </summary>
        readonly SortedList<int, int> AvailableSlots = new SortedList<int, int>();
        
        /// <summary>
        /// This holds the highest index slot in which an object is stored.
        /// </summary>
        int m_nextObjectSlot;
        #endregion
        
        #region Geometry Storage
        /// <summary>
        /// KDTree for Static Object Storage, no Translation
        /// </summary>
        protected readonly KDTree<GameObject> m_StaticObjectTree = new KDTree<GameObject>(new SquaredEuclideanDistanceFunction(), 3, TREE_DEFAULT_BUCKET_CAPACITY);
        
        /// <summary>
        /// KDTree for Moving Object Storage, using Speed Translation and GameTimer
        /// </summary>
        protected readonly KDTree<GameObject> m_MovingObjectTree = new KDTree<GameObject>(new SquaredEuclideanDistanceWithTranslation(() => GameTimer.GetTickCount(), 3), 7, TREE_DEFAULT_BUCKET_CAPACITY);
        #endregion
        
        #region Region Variables
        /// <summary>
        /// Data Object Holding Region Configuration.
        /// </summary>
        public RegionData RegionData { get; protected set; }
        
        /// <summary>
        /// Holds all the Zones inside this Region
        /// </summary>
        readonly ReaderWriterList<Zone> m_zones;

        /// <summary>
        /// A snapshot of all Zones within this Region
        /// </summary>
        public IList<Zone> Zones { get { return m_zones.ToArray(); } }
        
        protected object m_lockAreas = new object();

        /// <summary>
        /// Holds all the Areas inside this Region
        /// 
        /// ZoneID, AreaID, Area
        ///
        /// Areas can be registed to a reagion via AddArea
        /// and events will be thrown if players/npcs/objects enter leave area
        /// </summary>
        private Dictionary<ushort, IArea> m_Areas;

        protected Dictionary<ushort, IArea> Areas
        {
            get { return m_Areas; }
        }

        /// <summary>
        /// Cache for zone area mapping to quickly access all areas within a certain zone
        /// </summary>
        protected ushort[][] m_ZoneAreas;

        /// <summary>
        /// /// Cache for number of items in m_ZoneAreas array.
        /// </summary>
        protected ushort[] m_ZoneAreasCount;

        /// <summary>
        /// last relocation time
        /// </summary>
        private long m_lastRelocationTime;

        /// <summary>
        /// The region time manager
        /// </summary>
        protected readonly GameTimer.TimeManager m_timeManager;
        
        /// <summary>
        /// The Region Mob's Respawn Timer Collection
        /// </summary>
        protected readonly ConcurrentDictionary<GameNPC, int> m_mobsRespawning = new ConcurrentDictionary<GameNPC, int>();

        #endregion
                
        #region Region's Properties
        /// <summary>
        /// The Region Name eg. Region000
        /// </summary>
        public virtual string Name { get { return RegionData.Name; } }

        /// <summary>
        /// The Region Description eg. Cursed Forest
        /// </summary>
        public virtual string Description { get { return RegionData.Description; } }
        
        /// <summary>
        /// The ID of the Region eg. 21
        /// </summary>
        public virtual ushort ID { get { return RegionData.Id; } }

        /// <summary>
        /// The Region Server IP ... for future use
        /// </summary>
        public string ServerIP { get { return RegionData.Ip; } }

        /// <summary>
        /// The Region Server Port ... for future use
        /// </summary>
        public ushort ServerPort { get { return RegionData.Port; } }
        
        /// <summary>
        /// Is this Region Frontier ?
        /// TODO: What's the use of this variable with RvR flag ??
        /// </summary>
        public virtual bool IsFrontier
        {
            get { return RegionData.IsFrontier; }
            set { RegionData.IsFrontier = value; }
        }

        /// <summary>
        /// Is the Region a temporary instance
        /// </summary>
        public virtual bool IsInstance { get { return false; } }

        /// <summary>
        /// Is this region a standard DAoC region or a custom server region
        /// TODO: What's the use of this Flag ??
        /// </summary>
        public virtual bool IsCustom { get { return false; } }

        /// <summary>
        /// Gets whether this Region is a dungeon or not
        /// </summary>
        public virtual bool IsDungeon
        {
            get { return m_zones.Count == DUNGEON_REGION_DEFAULT_ZONECOUNT && m_zones[0].XOffset == DUNGEON_REGION_DEFAULT_OFFSET && m_zones[0].YOffset == DUNGEON_REGION_DEFAULT_OFFSET; }
        }

        /// <summary>
        /// Get the region expansion (we use client expansion + 1)
        /// </summary>
        public virtual int Expansion { get { return RegionData.Expansion + 1; } }

        /// <summary>
        /// Get the water level in this region
        /// </summary>
        public virtual int WaterLevel { get { return RegionData.WaterLevel; } }

        /// <summary>
        /// Gets or Sets diving flag for region
        /// Note: This flag should normally be checked at the zone level
        /// </summary>
        public virtual bool IsRegionDivingEnabled { get { return RegionData.DivingEnabled; } }

        /// <summary>
        /// Does this region contain housing?
        /// </summary>
        public virtual bool HousingEnabled { get { return RegionData.HousingEnabled; } }

        /// <summary>
        /// Should this region use the housing manager?
        /// Standard regions always use the housing manager if housing is enabled, custom regions might not.
        /// </summary>
        public virtual bool UseHousingManager { get { return HousingEnabled; } }
        

        /// <summary>
        /// Gets last relocation time
        /// </summary>
        public long LastRelocationTime
        {
            get { return m_lastRelocationTime; }
        }

        /// <summary>
        /// Gets the region time manager
        /// </summary>
        public virtual GameTimer.TimeManager TimeManager
        {
            get { return m_timeManager; }
        }

        /// <summary>
        /// Gets the current region time in milliseconds
        /// </summary>
        public virtual long Time
        {
            get { return m_timeManager.CurrentTime; }
        }

        protected bool m_isDisabled = false;
        /// <summary>
        /// Is this region disabled
        /// </summary>
        public virtual bool IsDisabled
        {
            get { return m_isDisabled; }
        }

        protected bool m_loadObjects = true;
        /// <summary>
        /// Will this region load objects
        /// </summary>
        public virtual bool LoadObjects
        {
            get { return m_loadObjects; }
        }

        //Dinberg: Added this for instances.
        /// <summary>
        /// Added to allow instances; the 'appearance' of the region, the map the GameClient uses.
        /// </summary>
        public virtual ushort Skin
        {
            get { return ID; }
        }

        /// <summary>
        /// Should this region respond to time manager send requests
        /// Normally yes, might be disabled for some instances.
        /// </summary>
        public virtual bool UseTimeManager
        {
            get { return true; }
            set { }
        }


        /// <summary>
        /// Each region can return it's own game time
        /// By default let WorldMgr handle it
        /// </summary>
        public virtual uint GameTime
        {
            get { return WorldMgr.GetCurrentGameTime(); }
            set { }
        }


        /// <summary>
        /// Get the day increment for this region.
        /// By default let WorldMgr handle it
        /// </summary>
        public virtual uint DayIncrement
        {
            get { return WorldMgr.GetDayIncrement(); }
            set { }
        }
        #endregion

        #region Constructor



        /// <summary>
        /// What to do when the region collapses.
        /// This is called when instanced regions need to be closed
        /// </summary>
        public virtual void OnCollapse()
        {
            // TODO Lock Correctly !!
            //Delete objects
            foreach (GameObject obj in m_objects)
            {
                if (obj != null)
                {
                    obj.Delete();
                    RemoveObject(obj);
                    obj.CurrentRegion = null;
                }
            }

            m_objects = new GameObject[MINIMUM_OBJECT_ARRAY_SIZE];
            m_objectsInRegion = 0;
            m_numPlayer = 0;
            m_nextObjectSlot = 0;
            AvailableSlots.Clear();

            foreach (Zone z in m_zones)
                z.Delete();

            m_zones.Clear();
            GameEventMgr.RemoveAllHandlersForObject(this);
        }


        #endregion

        /// <summary>
        /// Handles players leaving this region via a zonepoint
        /// </summary>
        /// <param name="player"></param>
        /// <param name="zonePoint"></param>
        /// <returns>false to halt processing of this request</returns>
        public virtual bool OnZonePoint(GamePlayer player, ZonePoint zonePoint)
        {
            return true;
        }

        #region Properties

        public virtual bool IsRvR
        {
            get
            {
                switch (RegionData.Id)
                {
                    case 163://new frontiers
                    case 165://cathal valley
                    case 233://Sumoner hall
                    case 234://1to4BG
                    case 235://5to9BG
                    case 236://10to14BG
                    case 237://15to19BG
                    case 238://20to24BG
                    case 239://25to29BG
                    case 240://30to34BG
                    case 241://35to39BG
                    case 242://40to44BG and Test BG
                    case 244://Frontiers RvR dungeon
                    case 249://Darkness Falls - RvR dungeon
                    case 489://lvl5-9 Demons breach
                        return true;
                    default:
                        return false;
                }
            }
        }


        /// <summary>
        /// Create the appropriate GameKeep for this region
        /// </summary>
        /// <returns></returns>
        public virtual AbstractGameKeep CreateGameKeep()
        {
            return new GameKeep();
        }

        /// <summary>
        /// Create the appropriate GameKeepTower for this region
        /// </summary>
        /// <returns></returns>
        public virtual AbstractGameKeep CreateGameKeepTower()
        {
            return new GameKeepTower();
        }

        /// <summary>
        /// Create the appropriate GameKeepComponent for this region
        /// </summary>
        /// <returns></returns>
        public virtual GameKeepComponent CreateGameKeepComponent()
        {
            return new GameKeepComponent();
        }

        /// <summary>
        /// Determine if the current time is AM.
        /// </summary>
        public virtual bool IsAM
        {
            get
            {
                if (IsPM)
                    return false;
                return true;
            }
        }

        private bool m_isPM;
        /// <summary>
        /// Determine if the current time is PM.
        /// </summary>
        public virtual bool IsPM
        {
            get
            {
                uint cTime = GameTime;

                uint hour = cTime / 1000 / 60 / 60;
                bool pm = false;

                if (hour == 0)
                {
                    hour = 12;
                }
                else if (hour == 12)
                {
                    pm = true;
                }
                else if (hour > 12)
                {
                    hour -= 12;
                    pm = true;
                }
                m_isPM = pm;

                return m_isPM;
            }
            set { m_isPM = value; }
        }

        private bool m_isNightTime;
        /// <summary>
        /// Determine if current time is between 6PM and 6AM, can be used for conditional spells.
        /// </summary>
        public virtual bool IsNightTime
        {
            get
            {
                uint cTime = GameTime;

                uint hour = cTime / 1000 / 60 / 60;
                bool pm = false;

                if (hour == 0)
                {
                    hour = 12;
                }
                else if (hour == 12)
                {
                    pm = true;
                }
                else if (hour > 12)
                {
                    hour -= 12;
                    pm = true;
                }

                if (pm && hour >= 6)
                    m_isNightTime = true;

                if (!pm && hour <= 5)
                    m_isNightTime = true;

                if (!pm && hour == 12) //Special Handling for Midnight.
                    m_isNightTime = true;

                if (!pm && hour >= 6)
                    m_isNightTime = false;

                if (pm && hour < 6)
                    m_isNightTime = false;

                return m_isNightTime;
            }
            set { m_isNightTime = value; }
        }

        public virtual ConcurrentDictionary<GameNPC, int> MobsRespawning
        {
        	get
        	{
        		return m_mobsRespawning;
        	}
        }
        
        #endregion

        #region Methods

        /// <summary>
        /// Starts the RegionMgr
        /// </summary>
        public void StartRegionMgr()
        {
            m_timeManager.Start();
            GameEventMgr.Notify(RegionEvent.RegionStart, this);
        }

        /// <summary>
        /// Stops the RegionMgr
        /// </summary>
        public void StopRegionMgr()
        {
            m_timeManager.Stop();
            GameEventMgr.Notify(RegionEvent.RegionStop, this);
        }

        /// <summary>
        /// Reallocates objects array with given size
        /// </summary>
        /// <param name="count">The size of new objects array, limited by MAXOBJECTS</param>
        public virtual void PreAllocateRegionSpace(int count)
        {
            if (count > Properties.REGION_MAX_OBJECTS)
                count = Properties.REGION_MAX_OBJECTS;
            
            lock (ObjectsSyncLock)
            {
                if (m_objects.Length >= count)
                    return;
                
                Array.Resize<GameObject>(ref m_objects, count);
            }
        }

        /// <summary>
        /// Loads the region from database
        /// </summary>
        /// <param name="mobObjs"></param>
        /// <param name="mobCount"></param>
        /// <param name="merchantCount"></param>
        /// <param name="itemCount"></param>
        /// <param name="bindCount"></param>
        public virtual void LoadFromDatabase(Mob[] mobObjs, ref long mobCount, ref long merchantCount, ref long itemCount, ref long bindCount)
        {
            if (!LoadObjects)
                return;

            Assembly gasm = Assembly.GetAssembly(typeof(GameServer));
            var staticObjs = GameServer.Database.SelectObjects<WorldObject>("`Region` = @Region", new QueryParameter("@Region", ID));
            var bindPoints = GameServer.Database.SelectObjects<BindPoint>("`Region` = @Region", new QueryParameter("@Region", ID));
            int count = mobObjs.Length + staticObjs.Count;
            if (count > 0) PreAllocateRegionSpace(count + 100);
            int myItemCount = staticObjs.Count;
            int myMobCount = 0;
            int myMerchantCount = 0;
            int myBindCount = bindPoints.Count;
            string allErrors = string.Empty;

            if (mobObjs.Length > 0)
            {
                foreach (Mob mob in mobObjs)
                {
                    GameNPC myMob = null;
                    string error = string.Empty;
  
                    // Default Classtype
                    string classtype = ServerProperties.Properties.GAMENPC_DEFAULT_CLASSTYPE;
                    
                    // load template if any
                    INpcTemplate template = null;
                    if(mob.NPCTemplateID != -1)
                    {
                    	template = NpcTemplateMgr.GetTemplate(mob.NPCTemplateID);
                    }
                    

                    if (Properties.USE_NPCGUILDSCRIPTS && mob.Guild.Length > 0 && mob.Realm >= 0 && mob.Realm <= (int)eRealm._Last)
                    {
                        Type type = ScriptMgr.FindNPCGuildScriptClass(mob.Guild, (eRealm)mob.Realm);
                        if (type != null)
                        {
                            try
                            {
                                
                                myMob = (GameNPC)type.Assembly.CreateInstance(type.FullName);
                               	
                            }
                            catch (Exception e)
                            {
                                if (log.IsErrorEnabled)
                                    log.Error("LoadFromDatabase", e);
                            }
                        }
                    }

  
                    if (myMob == null)
                    {
                    	if(template != null && template.ClassType != null && template.ClassType.Length > 0 && template.ClassType != Mob.DEFAULT_NPC_CLASSTYPE && template.ReplaceMobValues)
                    	{
                			classtype = template.ClassType;
                    	}
                        else if (mob.ClassType != null && mob.ClassType.Length > 0 && mob.ClassType != Mob.DEFAULT_NPC_CLASSTYPE)
                        {
                            classtype = mob.ClassType;
                        }

                        try
                        {
                            myMob = (GameNPC)gasm.CreateInstance(classtype, false);
                        }
                        catch
                        {
                            error = classtype;
                        }

                        if (myMob == null)
                        {
                            foreach (Assembly asm in ScriptMgr.Scripts)
                            {
                                try
                                {
                                    myMob = (GameNPC)asm.CreateInstance(classtype, false);
                                    error = string.Empty;
                                }
                                catch
                                {
                                    error = classtype;
                                }

                                if (myMob != null)
                                    break;
                            }

                            if (myMob == null)
                            {
                                myMob = new GameNPC();
                                error = classtype;
                            }
                        }
                    }

                    if (!allErrors.Contains(error))
                        allErrors += " " + error + ",";

                    if (myMob != null)
                    {
                        try
                        {
                            myMob.LoadFromDatabase(mob);

                            if (myMob is GameMerchant)
                            {
                                myMerchantCount++;
                            }
                            else
                            {
                                myMobCount++;
                            }
                        }
                        catch (Exception e)
                        {
                            if (log.IsErrorEnabled)
                                log.Error("Failed: " + myMob.GetType().FullName + ":LoadFromDatabase(" + mob.GetType().FullName + ");", e);
                            throw;
                        }

                        myMob.AddToWorld();
                    }
                }
            }

            if (staticObjs.Count > 0)
            {
                foreach (WorldObject item in staticObjs)
                {
                    GameStaticItem myItem;
                    if (!string.IsNullOrEmpty(item.ClassType))
                    {
                        myItem = gasm.CreateInstance(item.ClassType, false) as GameStaticItem;
                        if (myItem == null)
                        {
                            foreach (Assembly asm in ScriptMgr.Scripts)
                            {
                                try
                                {
                                    myItem = (GameStaticItem)asm.CreateInstance(item.ClassType, false);
                                }
                                catch { }
                                if (myItem != null)
                                    break;
                            }
                            if (myItem == null)
                                myItem = new GameStaticItem();
                        }
                    }
                    else
                        myItem = new GameStaticItem();

                    myItem.LoadFromDatabase(item);
                    myItem.AddToWorld();
                    //						if (!myItem.AddToWorld())
                    //							log.ErrorFormat("Failed to add the item to the world: {0}", myItem.ToString());
                }
            }

            foreach (BindPoint point in bindPoints)
            {
                AddArea(new Area.BindArea("bind point", point));
            }

            if (myMobCount + myItemCount + myMerchantCount + myBindCount > 0)
            {
                if (log.IsInfoEnabled)
                    log.Info(String.Format("Region: {0} ({1}) loaded {2} mobs, {3} merchants, {4} items {5} bindpoints, from DB ({6})", Description, ID, myMobCount, myMerchantCount, myItemCount, myBindCount, TimeManager.Name));

                log.Debug("Used Memory: " + GC.GetTotalMemory(false) / 1024 / 1024 + "MB");

                if (allErrors != string.Empty)
                    log.Error("Error loading the following NPC ClassType(s), GameNPC used instead:" + allErrors.TrimEnd(','));

                Thread.Sleep(0);  // give up remaining thread time to other resources
            }
            Interlocked.Add(ref mobCount, myMobCount);
            Interlocked.Add(ref merchantCount, myMerchantCount);
            Interlocked.Add(ref itemCount, myItemCount);
            Interlocked.Add(ref bindCount, myBindCount);
        }

        /// <summary>
        /// Adds an object to the region and assigns the object an id.
        /// </summary>
        /// <param name="obj">A GameObject to be added to the region.</param>
        /// <returns>True if Object Successfully added.</returns>
        internal bool AddObject(GameObject obj)
        {
            Zone zone = GetZone(obj.X, obj.Y);
            if (zone == null)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("Zone not found for Object: {0} (ID={1}) in Region {2} (ID={3})", obj.Name, obj.InternalID, Description, ID);
            }

            lock (ObjectsSyncLock)
            {
                // Check for errors
                if (obj.ObjectID != -1)
                {
                    if (obj.ObjectID < m_objects.Length && obj == m_objects[obj.ObjectID - 1])
                    {
                        log.WarnFormat("Object is already in Region {1} (ID={2}): {0}", obj, Description, ID);
                        return false;
                    }
                    
                    log.WarnFormat("Object: {0} (ID={1}) should be added to {2} (ID={3}) but had already an OID({4}) => not added\n{5}",
                                   obj.Name, obj.InternalID, Description, ID, obj.ObjectID, Environment.StackTrace);
                    return false;
                }

                //Assign a new id
                var index = m_nextObjectSlot;
                var fromAvailable = false;
                
                var availableCount = AvailableSlots.Count;
                if (availableCount > 0)
                {
                    // Pick An Available Slot !
                    index = AvailableSlots.Values[availableCount - 1];
                    fromAvailable = true;
                }
                else
                {
                    if (index >= m_objects.Length)
                    {
                        int size = (int)(m_objects.Length * 1.20);
                        if (size < m_objects.Length + MINIMUM_OBJECT_ARRAY_SIZE)
                            size = m_objects.Length + MINIMUM_OBJECT_ARRAY_SIZE;
                        if (size > Properties.REGION_MAX_OBJECTS)
                            size = Properties.REGION_MAX_OBJECTS;
                        
                        PreAllocateRegionSpace(size);
                    }
                    
                    if (index >= m_objects.Length)
                    {
                        // No available slot
                        if (log.IsErrorEnabled)
                            log.ErrorFormat("Can't add new object - Region '{0}' (ID={1}) (Object: {2}); OID is above maximum {3} ", Description, ID, obj, index);
                        
                        return false;
                    }
                }
                
                if (index < 0)
                {
                    log.WarnFormat("There was an unexpected problem while adding Object {0} (ID={1}) to Region {2} (ID={3})", obj.Name, obj.InternalID, Description, ID);
                    return false;
                }

                // If we found a slot add the object
                if (m_objects[index] == null)
                {
                    m_objects[index] = obj;
                    
                    if (fromAvailable)
                        AvailableSlots.RemoveAt(availableCount - 1);
                    else
                        ++m_nextObjectSlot;
                    
                    m_objectsInRegion++;
                    obj.ObjectID = index + 1;

                    if (obj is GamePlayer)
                        ++m_numPlayer;
                    
                    return true;
                }
                
                // No available slot
                if (log.IsErrorEnabled)
                    log.ErrorFormat("Can't add new object - Region '{0}' (ID={1}) (Object: {2}); OID is used by another object: {3}", Description, ID, obj, index);
                
                return false;
            }
        }

        /// <summary>
        /// Removes the object with the specified ID from the region
        /// </summary>
        /// <param name="obj">A GameObject to be removed from the region</param>
        internal void RemoveObject(GameObject obj)
        {
            lock (ObjectsSyncLock)
            {
                int index = obj.ObjectID - 1;
                
                if (index < 0)
                    return;

                GameObject inPlace = m_objects[index];
                
                if (inPlace == null)
                {
                    if (log.IsErrorEnabled)
                        log.ErrorFormat("Region RemoveObject conflict! Object: {0} (OID={1}), was not found in slot of Region {2} (ID={3})\n{4}", obj.Name, obj.ObjectID, Description, ID, Environment.StackTrace);
                    return;
                }
                
                if (obj != inPlace)
                {
                    if (log.IsErrorEnabled)
                        log.ErrorFormat("Region RemoveObject conflict! Object: {0} (OID={1}) in Region {2} (ID={3}), there was an other object found in slot (Name: {4}, ID: {5}, State: {6})\n{7}",
                                        obj.Name, obj.ObjectID, Description, ID, inPlace.Name, inPlace.InternalID, inPlace.ObjectState, Environment.StackTrace);
                    return;
                }

                m_objects[index] = null;
                
                // Remove index from valid Slot
                if (index == m_nextObjectSlot - 1)
                {
                    --m_nextObjectSlot;
                    
                    // Compact Object Collection
                    while (AvailableSlots.Count > 0 && AvailableSlots.Values[0] == m_nextObjectSlot - 1)
                    {
                        --m_nextObjectSlot;
                        AvailableSlots.RemoveAt(0);
                    }
                }
                else
                {
                    AvailableSlots.Add(-index, index);
                }
                
                obj.ObjectID = -1; // invalidate object id
                
                m_objectsInRegion--;
                
                if (obj is GamePlayer)
                    --m_numPlayer;
            }
        }

        /// <summary>
        /// Searches for players gravestone in this region
        /// </summary>
        /// <param name="player"></param>
        /// <returns>the found gravestone or null</returns>
        public GameGravestone FindGraveStone(GamePlayer player)
        {
            lock (ObjectsSyncLock)
            {
                return m_objects.OfType<GameGravestone>().FirstOrDefault(grave => grave.InternalID == player.InternalID);
            }
        }

        /// <summary>
        /// Gets the object with the specified ID
        /// </summary>
        /// <param name="id">The ID of the object to get</param>
        /// <returns>The object with the specified ID, null if it didn't exist</returns>
        public GameObject GetObject(ushort id)
        {
            if (m_objects == null || id <= 0 || id > m_objects.Length)
                return null;
            return m_objects[id - 1];
        }

        /// <summary>
        /// Returns the zone that contains the specified x and y values
        /// </summary>
        /// <param name="x">X value for the zone you're retrieving</param>
        /// <param name="y">Y value for the zone you're retrieving</param>
        /// <returns>The zone you're retrieving or null if it couldn't be found</returns>
        public Zone GetZone(int x, int y)
        {
            int varX = x;
            int varY = y;
            foreach (Zone zone in m_zones)
            {
                if (zone.XOffset <= varX && zone.YOffset <= varY && (zone.XOffset + zone.Width) > varX && (zone.YOffset + zone.Height) > varY)
                    return zone;
            }
            return null;
        }
        
        /// <summary>
        /// TODO This should probably not exists...
        /// </summary>
        public void AddZone(Zone Zone) { m_zones.Add(Zone); }

        /// <summary>
        /// Gets the X offset for the specified zone
        /// </summary>
        /// <param name="x">X value for the zone's offset you're retrieving</param>
        /// <param name="y">Y value for the zone's offset you're retrieving</param>
        /// <returns>The X offset of the zone you specified or 0 if it couldn't be found</returns>
        public int GetXOffInZone(int x, int y)
        {
            Zone z = GetZone(x, y);
            if (z == null)
                return 0;
            return x - z.XOffset;
        }

        /// <summary>
        /// Gets the Y offset for the specified zone
        /// </summary>
        /// <param name="x">X value for the zone's offset you're retrieving</param>
        /// <param name="y">Y value for the zone's offset you're retrieving</param>
        /// <returns>The Y offset of the zone you specified or 0 if it couldn't be found</returns>
        public int GetYOffInZone(int x, int y)
        {
            Zone z = GetZone(x, y);
            if (z == null)
                return 0;
            return y - z.YOffset;
        }

        /// <summary>
        /// Check if this region is a capital city
        /// </summary>
        /// <returns>True, if region is a capital city, else false</returns>
        public virtual bool IsCapitalCity
        {
            get
            {
                switch (this.Skin)
                {
                    case 10: return true; // Camelot City
                    case 101: return true; // Jordheim
                    case 201: return true; // Tir na Nog
                    default: return false;
                }
            }
        }

        /// <summary>
        /// Check if this region is a housing zone
        /// </summary>
        /// <returns>True, if region is a housing zone, else false</returns>
        public virtual bool IsHousing
        {
            get
            {
                switch (this.Skin) // use the skin of the region
                {
                    case 2: return true; 	// Housing alb
                    case 102: return true; 	// Housing mid
                    case 202: return true; 	// Housing hib
                    default: return false;
                }
            }
        }

        /// <summary>
        /// Check if the given region is Atlantis.
        /// </summary>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public static bool IsAtlantis(int regionId)
        {
            return (regionId == 30 || regionId == 73 || regionId == 130);
        }

        #endregion

        #region Area

        /// <summary>
        /// Adds an area to the region and updates area-zone cache
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public virtual IArea AddArea(IArea area)
        {
            lock (m_lockAreas)
            {
                ushort nextAreaID = 0;

                foreach (ushort areaID in m_Areas.Keys)
                {
                    if (areaID >= nextAreaID)
                    {
                        nextAreaID = (ushort)(areaID + 1);
                    }
                }

                area.ID = nextAreaID;
                m_Areas.Add(area.ID, area);

                int zonePos = 0;
                foreach (Zone zone in Zones)
                {
                    if (area.IsIntersectingZone(zone))
                    	m_ZoneAreas[zonePos][m_ZoneAreasCount[zonePos]++] = area.ID;
                    
                    zonePos++;
                }
                return area;
            }
        }

        /// <summary>
        /// Removes an area from the list of areas and updates area-zone cache
        /// </summary>
        /// <param name="area"></param>
        public virtual void RemoveArea(IArea area)
        {
            lock (m_lockAreas)
            {
                if (m_Areas.ContainsKey(area.ID) == false)
                {
                    return;
                }

                m_Areas.Remove(area.ID);
                int ZoneCount = Zones.Count;

                for (int zonePos = 0; zonePos < ZoneCount; zonePos++)
                {
                    for (int areaPos = 0; areaPos < m_ZoneAreasCount[zonePos]; areaPos++)
                    {
                        if (m_ZoneAreas[zonePos][areaPos] == area.ID)
                        {
                            // move the remaining m_ZoneAreas array one to the left

                            for (int i = areaPos; i < m_ZoneAreasCount[zonePos] - 1; i++)
                            {
                                m_ZoneAreas[zonePos][i] = m_ZoneAreas[zonePos][i + 1];
                            }

                            m_ZoneAreasCount[zonePos]--;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the areas for given location,
        /// less performant than getAreasOfZone so use other on if possible
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public virtual IList<IArea> GetAreasOfSpot(IPoint3D point)
        {
            Zone zone = GetZone(point.X, point.Y);
            return GetAreasOfZone(zone, point);
        }

        /// <summary>
        /// Gets the areas for a certain spot,
        /// less performant than getAreasOfZone so use other on if possible
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public virtual IList<IArea> GetAreasOfSpot(int x, int y, int z)
        {
            Zone zone = GetZone(x, y);
            Point3D p = new Point3D(x, y, z);
            return GetAreasOfZone(zone, p);
        }

        public virtual IList<IArea> GetAreasOfZone(Zone zone, IPoint3D p)
        {
            return GetAreasOfZone(zone, p, true);
        }

        /// <summary>
        /// Gets the areas for a certain spot
        /// </summary>
        /// <param name="zone"></param>
        /// <param name="p"></param>
        /// <param name="checkZ"></param>
        /// <returns></returns>
        public virtual IList<IArea> GetAreasOfZone(Zone zone, IPoint3D p, bool checkZ)
        {
            lock (m_lockAreas)
            {
                int zoneIndex = Zones.IndexOf(zone);
                var areas = new List<IArea>();

                if (zoneIndex >= 0)
                {
                    try
                    {
                        for (int i = 0; i < m_ZoneAreasCount[zoneIndex]; i++)
                        {
                            IArea area = (IArea)m_Areas[m_ZoneAreas[zoneIndex][i]];
                            if (area.IsContaining(p, checkZ))
                            {
                                areas.Add(area);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error("GetArea exception.Area count " + m_ZoneAreasCount[zoneIndex], e);
                    }
                }

                return areas;
            }
        }

        public virtual IList<IArea> GetAreasOfZone(Zone zone, int x, int y, int z)
        {
            lock (m_lockAreas)
            {
                int zoneIndex = Zones.IndexOf(zone);
                var areas = new List<IArea>();

                if (zoneIndex >= 0)
                {
                    try
                    {
                        for (int i = 0; i < m_ZoneAreasCount[zoneIndex]; i++)
                        {
                            IArea area = (IArea)m_Areas[m_ZoneAreas[zoneIndex][i]];
                            if (area.IsContaining(x, y, z))
                                areas.Add(area);
                        }
                    }
                    catch (Exception e)
                    {
                        log.Error("GetArea exception.Area count " + m_ZoneAreasCount[zoneIndex], e);
                    }
                }
                return areas;
            }
        }

        #endregion

        #region Object in Radius (Added by Konik & WitchKing)

        #region New Get in radius

        /// <summary>
        /// Gets objects in a radius around a point
        /// </summary>
        /// <param name="type">OBJECT_TYPE (0=item, 1=npc, 2=player)</param>
        /// <param name="x">origin X</param>
        /// <param name="y">origin Y</param>
        /// <param name="z">origin Z</param>
        /// <param name="radius">radius around origin</param>
        /// <param name="withDistance">Get an ObjectDistance enumerator</param>
        /// <returns>IEnumerable to be used with foreach</returns>
        protected IEnumerable GetInRadius(Zone.eGameObjectType type, int x, int y, int z, ushort radius, bool withDistance, bool ignoreZ)
        {
            // check if we are around borders of a zone
            Zone startingZone = GetZone(x, y);

            if (startingZone != null)
            {
                ArrayList res = startingZone.GetObjectsInRadius(type, x, y, z, radius, new ArrayList(), ignoreZ);

                uint sqRadius = (uint)radius * radius;

                foreach (var currentZone in m_zones)
                {
                    if ((currentZone != startingZone)
                        && (currentZone.TotalNumberOfObjects > 0)
                        && CheckShortestDistance(currentZone, x, y, sqRadius))
                    {
                        res = currentZone.GetObjectsInRadius(type, x, y, z, radius, res, ignoreZ);
                    }
                }

                //Return required enumerator
                IEnumerable tmp = null;
                if (withDistance)
                {
                    switch (type)
                    {
                        case Zone.eGameObjectType.ITEM:
                            tmp = new ItemDistanceEnumerator(x, y, z, res);
                            break;
                        case Zone.eGameObjectType.NPC:
                            tmp = new NPCDistanceEnumerator(x, y, z, res);
                            break;
                        case Zone.eGameObjectType.PLAYER:
                            tmp = new PlayerDistanceEnumerator(x, y, z, res);
                            break;
                        case Zone.eGameObjectType.DOOR:
                            tmp = new DoorDistanceEnumerator(x, y, z, res);
                            break;
                        default:
                            tmp = new EmptyEnumerator();
                            break;
                    }
                }
                else
                {
                    tmp = new ObjectEnumerator(res);
                }
                return tmp;
            }
            else
            {
                if (log.IsDebugEnabled)
                {
                    log.Error("GetInRadius starting zone is null for (" + type + ", " + x + ", " + y + ", " + z + ", " + radius + ") in Region ID=" + ID);
                }
                return new EmptyEnumerator();
            }
        }


        /// <summary>
        /// get the shortest distance from a point to a zone
        /// </summary>
        /// <param name="zone">The zone to check</param>
        /// <param name="x">X value of the point</param>
        /// <param name="y">Y value of the point</param>
        /// <param name="squareRadius">The square radius to compare the distance with</param>
        /// <returns>True if the distance is shorter false either</returns>
        private static bool CheckShortestDistance(Zone zone, int x, int y, uint squareRadius)
        {
            //  coordinates of zone borders
            int xLeft = zone.XOffset;
            int xRight = zone.XOffset + zone.Width;
            int yTop = zone.YOffset;
            int yBottom = zone.YOffset + zone.Height;
            long distance = 0;

            if ((y >= yTop) && (y <= yBottom))
            {
                int xdiff = Math.Min(FastMath.Abs(x - xLeft), FastMath.Abs(x - xRight));
                distance = (long)xdiff * xdiff;
            }
            else
            {
                if ((x >= xLeft) && (x <= xRight))
                {
                    int ydiff = Math.Min(FastMath.Abs(y - yTop), FastMath.Abs(y - yBottom));
                    distance = (long)ydiff * ydiff;
                }
                else
                {
                    int xdiff = Math.Min(FastMath.Abs(x - xLeft), FastMath.Abs(x - xRight));
                    int ydiff = Math.Min(FastMath.Abs(y - yTop), FastMath.Abs(y - yBottom));
                    distance = (long)xdiff * xdiff + (long)ydiff * ydiff;
                }
            }

            return (distance <= squareRadius);
        }

        /// <summary>
        /// Gets Items in a radius around a spot
        /// </summary>
        /// <param name="x">origin X</param>
        /// <param name="y">origin Y</param>
        /// <param name="z">origin Z</param>
        /// <param name="radius">radius around origin</param>
        /// <param name="withDistance">Get an ObjectDistance enumerator</param>
        /// <returns>IEnumerable to be used with foreach</returns>
        public IEnumerable GetItemsInRadius(int x, int y, int z, ushort radius, bool withDistance)
        {
            return GetInRadius(Zone.eGameObjectType.ITEM, x, y, z, radius, withDistance, false);
        }

        /// <summary>
        /// Gets NPCs in a radius around a spot
        /// </summary>
        /// <param name="x">origin X</param>
        /// <param name="y">origin Y</param>
        /// <param name="z">origin Z</param>
        /// <param name="radius">radius around origin</param>
        /// <param name="withDistance">Get an ObjectDistance enumerator</param>
        /// <returns>IEnumerable to be used with foreach</returns>
        public IEnumerable GetNPCsInRadius(int x, int y, int z, ushort radius, bool withDistance, bool ignoreZ)
        {
            return GetInRadius(Zone.eGameObjectType.NPC, x, y, z, radius, withDistance, ignoreZ);
        }

        /// <summary>
        /// Gets Players in a radius around a spot
        /// </summary>
        /// <param name="x">origin X</param>
        /// <param name="y">origin Y</param>
        /// <param name="z">origin Z</param>
        /// <param name="radius">radius around origin</param>
        /// <param name="withDistance">Get an ObjectDistance enumerator</param>
        /// <returns>IEnumerable to be used with foreach</returns>
        public IEnumerable GetPlayersInRadius(int x, int y, int z, ushort radius, bool withDistance, bool ignoreZ)
        {
            return GetInRadius(Zone.eGameObjectType.PLAYER, x, y, z, radius, withDistance, ignoreZ);
        }

        /// <summary>
        /// Gets Doors in a radius around a spot
        /// </summary>
        /// <param name="x">origin X</param>
        /// <param name="y">origin Y</param>
        /// <param name="z">origin Z</param>
        /// <param name="radius">radius around origin</param>
        /// <param name="withDistance">Get an ObjectDistance enumerator</param>
        /// <returns>IEnumerable to be used with foreach</returns>
        public virtual IEnumerable GetDoorsInRadius(int x, int y, int z, ushort radius, bool withDistance)
        {
            return GetInRadius(Zone.eGameObjectType.DOOR, x, y, z, radius, withDistance, false);
        }

        #endregion

        #region Enumerators

        #region EmptyEnumerator

        /// <summary>
        /// An empty enumerator returned when no objects are found
        /// close to a certain range
        /// </summary>
        public class EmptyEnumerator : IEnumerator, IEnumerable
        {
            /// <summary>
            /// Implementation of the IEnumerable interface
            /// </summary>
            /// <returns>An Enumeration Interface of this class</returns>
            public IEnumerator GetEnumerator()
            {
                return this;
            }

            /// <summary>
            /// Implementation of the IEnumerator interface
            /// </summary>
            /// <returns>Always false to prevent Current</returns>
            public bool MoveNext()
            {
                return false;
            }

            /// <summary>
            /// Implementation of the IEnumerator interface,
            /// always returns null because it shouldn't be
            /// called at all.
            /// </summary>
            public object Current
            {
                get { return null; }
            }

            /// <summary>
            /// Implementation of the IEnumerator interface
            /// </summary>
            public void Reset()
            {
            }
        }

        #endregion

        #region ObjectEnumerator

        /// <summary>
        /// An enumerator over GameObjects. Used to enumerate over
        /// certain objects and do some testing before returning an
        /// object.
        /// </summary>
        public class ObjectEnumerator : IEnumerator, IEnumerable
        {
            /// <summary>
            /// Counter to the current object
            /// </summary>
            protected int m_current = -1;

            protected GameObject[] elements = null;
            //protected ArrayList elements = null;

            protected object m_currentObj = null;

            protected int m_count;

            public IEnumerator GetEnumerator()
            {
                return this;
            }

            public ObjectEnumerator(ArrayList objectSet)
            {
                //objectSet.DumpInfo();
                elements = new GameObject[objectSet.Count];
                objectSet.CopyTo(elements);
                m_count = elements.Length;
            }


            /// <summary>
            /// Get the next GameObjcte from the zone subset created in constructor
            /// and by restrictuing according distance
            /// </summary>
            /// <returns>The Next GameObject of this Enumerator</returns>
            public virtual bool MoveNext()
            {
                /*********NEW GET IN RADIUS SYSTEM ADDED BY KONIK**********/
                m_currentObj = null;
                bool found = false;
                do
                {
                    m_current++;
                    // break if no more object
                    if (m_current < m_count)
                    {
                        // get the object
                        //GameObject obj = (GameObject) elements[m_current];
                        GameObject obj = elements[m_current];
                        if (found = ((obj != null && ((int)obj.ObjectState) == (int)GameObject.eObjectState.Active)))
                        {
                            m_currentObj = obj;
                        }
                    }
                } while (m_current < m_count && !found);
                return found;
            }

            /// <summary>
            /// Returns the current Object in the Enumerator
            /// </summary>
            public virtual object Current
            {
                get { return m_currentObj; }
            }

            /// <summary>
            /// Resets the Enumerator
            /// </summary>
            public void Reset()
            {
                m_currentObj = null;
                m_current = -1;
            }
        }

        #endregion

        #region XXXDistanceEnumerator

        public abstract class DistanceEnumerator : ObjectEnumerator
        {
            protected int m_X;
            protected int m_Y;
            protected int m_Z;

            public DistanceEnumerator(int x, int y, int z, ArrayList elements)
                : base(elements)
            {
                m_X = x;
                m_Y = y;
                m_Z = z;
            }
        }

        /// <summary>
        /// This enumerator returns the object and the distance towards the object
        /// </summary>
        public class PlayerDistanceEnumerator : DistanceEnumerator
        {
            public PlayerDistanceEnumerator(int x, int y, int z, ArrayList elements)
                : base(x, y, z, elements)
            {
            }

            public override object Current
            {
                get
                {
                    GamePlayer obj = (GamePlayer)m_currentObj;
                    return new PlayerDistEntry(obj, obj.GetDistanceTo(new Point3D(m_X, m_Y, m_Z)));
                }
            }
        }

        /// <summary>
        /// This enumerator returns the object and the distance towards the object
        /// </summary>
        public class NPCDistanceEnumerator : DistanceEnumerator
        {
            public NPCDistanceEnumerator(int x, int y, int z, ArrayList elements)
                : base(x, y, z, elements)
            {
            }

            public override object Current
            {
                get
                {
                    GameNPC obj = (GameNPC)m_currentObj;
                    return new NPCDistEntry(obj, obj.GetDistanceTo(new Point3D(m_X, m_Y, m_Z)));
                }
            }
        }

        /// <summary>
        /// This enumerator returns the object and the distance towards the object
        /// </summary>
        public class ItemDistanceEnumerator : DistanceEnumerator
        {
            public ItemDistanceEnumerator(int x, int y, int z, ArrayList elements)
                : base(x, y, z, elements)
            {
            }

            public override object Current
            {
                get
                {
                    GameStaticItem obj = (GameStaticItem)m_currentObj;
                    return new ItemDistEntry(obj, obj.GetDistanceTo(new Point3D(m_X, m_Y, m_Z)));
                }
            }
        }

        /// <summary>
        /// This enumerator returns the object and the distance towards the object
        /// </summary>
        public class DoorDistanceEnumerator : DistanceEnumerator
        {
            public DoorDistanceEnumerator(int x, int y, int z, ArrayList elements)
                : base(x, y, z, elements)
            {
            }

            public override object Current
            {
                get
                {
                    IDoor obj = (IDoor)m_currentObj;
                    return new DoorDistEntry(obj, obj.GetDistance(new Point3D(m_X, m_Y, m_Z)));
                }
            }
        }

        #endregion

        #endregion

        #region Automatic relocation

        public void Relocate()
        {
        	foreach (var zone in m_zones)
        	{
        		zone.Relocate(null);
        	}
        	
        	m_lastRelocationTime = DateTime.Now.Ticks / (10 * 1000);
        }

        #endregion

        #endregion

    }
	#region Helpers classes

	/// <summary>
	/// Holds a Object and it's distance towards the center
	/// </summary>
	public class PlayerDistEntry
	{
		public PlayerDistEntry(GamePlayer o, int distance)
		{
			Player = o;
			Distance = distance;
		}

		public GamePlayer Player;
		public int Distance;
	}

	/// <summary>
	/// Holds a Object and it's distance towards the center
	/// </summary>
	public class NPCDistEntry
	{
		public NPCDistEntry(GameNPC o, int distance)
		{
			NPC = o;
			Distance = distance;
		}

		public GameNPC NPC;
		public int Distance;
	}

	/// <summary>
	/// Holds a Object and it's distance towards the center
	/// </summary>
	public class ItemDistEntry
	{
		public ItemDistEntry(GameStaticItem o, int distance)
		{
			Item = o;
			Distance = distance;
		}

		public GameStaticItem Item;
		public int Distance;
	}

	/// <summary>
	/// Holds a Object and it's distance towards the center
	/// </summary>
	public class DoorDistEntry
	{
		public DoorDistEntry(IDoor d, int distance)
		{
			Door = d;
			Distance = distance;
		}

		public IDoor Door;
		public int Distance;
	}

	#endregion
}
