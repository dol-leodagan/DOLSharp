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
namespace DOL.GS
{
    using System;
    using System.Reflection;
    using System.Linq;
    using System.Collections.Generic;
    
    using DOL.Database;
    
    using log4net;

    /// <summary>
    /// Region Manager Handle Region Loading, and GameObject Spawning inside.
    /// </summary>
    public sealed class RegionManager
    {
        /// <summary>
        /// Defines a logger for this class.
        /// </summary>
        static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Dictionary Holding All Region Configuration from Database.
        /// </summary>
        readonly Dictionary<ushort, RegionData> RegionsData;
        
        /// <summary>
        /// Dictionary Holding All Instanced Region of the World
        /// </summary>
        readonly ReaderWriterDictionary<ushort, Region> Regions;
        
        /// <summary>
        /// Array of Time Managers for CPU Distribution
        /// </summary>
        readonly GameTimer.TimeManager[] TimeManagers;

        /// <summary>
        /// Create a new Instance of <see cref="RegionManager"/>
        /// </summary>
        /// <param name="Database">Database Connector</param>
        /// <param name="TimeManagersCount">Number of Time Manager to Spawn</param>
        public RegionManager(IObjectDatabase Database, int TimeManagersCount)
        {
            //Load Database Regions
            RegionsData = Database.SelectAllObjects<DBRegions>()
                .Select(dbr => new RegionData
                        {
                            Id = dbr.RegionID,
                            Name = dbr.Name,
                            Description = dbr.Description,
                            Ip = dbr.IP,
                            Port = dbr.Port,
                            WaterLevel = dbr.WaterLevel,
                            DivingEnabled = dbr.DivingEnabled,
                            HousingEnabled = dbr.HousingEnabled,
                            Expansion = dbr.Expansion,
                            ClassType = dbr.ClassType,
                            IsFrontier = dbr.IsFrontier,
                            DedicatedTimeManager = dbr.DedicatedTimeManager,
                            Zones = dbr.Zones == null
                                ? new ZoneData[0]
                                : dbr.Zones.Select(dbz => new ZoneData
                                                   {
                                                       ZoneID = (ushort)dbz.ZoneID,
                                                       OffX = (byte)dbz.OffsetX,
                                                       OffY = (byte)dbz.OffsetY,
                                                       Height = (byte)dbz.Height,
                                                       Width = (byte)dbz.Width,
                                                       Description = dbz.Name,
                                                       DivingFlag = dbz.DivingFlag,
                                                       WaterLevel = dbz.WaterLevel,
                                                       IsLava = dbz.IsLava,
                                                       BonusXP = dbz.Experience,
                                                       BonusRP = dbz.Realmpoints,
                                                       BonusBP = dbz.Bountypoints,
                                                       BonusCoin = dbz.Coin,
                                                       Realm = (eRealm)Enum.Parse(typeof(eRealm), dbz.Realm.ToString()),
                                                   }).ToArray(),
                        }).ToDictionary(kv => kv.Id, kv => kv);
            
            // Check for Missing Zones !
            if (log.IsWarnEnabled)
                foreach (var regionEntry in RegionsData.Where(kv => kv.Value.Zones.Length < 1))
                    log.WarnFormat("Region {0} (ID={1}) Have no Zones! No Object can be added...", regionEntry.Value.Description, regionEntry.Value.Id);
            
            // Create Time Managers
            TimeManagers = new GameTimer.TimeManager[TimeManagersCount+RegionsData.Count(ent => ent.Value.DedicatedTimeManager)];
            
            for (int times = 0 ; times < TimeManagersCount ; times++)
            {
                TimeManagers[times] = new GameTimer.TimeManager(string.Format("TimeManager_{0}", times));
            }

            // Create Regions
            Regions = new ReaderWriterDictionary<ushort, Region>();

            var generalTimer = 0;
            var dedicatedTimer = 0;
            
            foreach (var regionEntry in RegionsData)
            {
                var entry = regionEntry;
                try
                {
                    GameTimer.TimeManager timer = null;
                    if (regionEntry.Value.DedicatedTimeManager)
                    {
                        timer = new GameTimer.TimeManager(string.Format("TimeManager_{0}", entry.Value.Name));
                        TimeManagers[TimeManagersCount + dedicatedTimer] = timer;
                        dedicatedTimer++;
                    }
                    else
                    {
                        timer = TimeManagers[generalTimer % TimeManagersCount];
                        generalTimer++;
                    }
                    
                    Regions.Add(entry.Key, RegionHelper.Create(timer, entry.Value));
                }
                catch (Exception ex)
                {
                    if (log.IsErrorEnabled)
                        log.ErrorFormat("Could not Instantiate Region {0} (ID={1})\n{2}", entry.Value.Name, entry.Value.Id, ex);
                }
            }
        }
    }
}
