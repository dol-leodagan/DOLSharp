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

using DOL.Language;
using DOL.GS.ServerProperties;

namespace DOL.GS
{
	/// <summary>
	/// A Game Gravestone For storing XP Loss from Death of player.
	/// </summary>
	public class GameGravestone : GameStaticItem
	{
	    #region Model Properties
        [ServerProperty("gravestone", "albion_gravestone_model", "Albion Player's Gravestone Model", 145)]	    
        public static ushort ALBION_GRAVESTONE_MODEL;

        [ServerProperty("gravestone", "midgard_gravestone_model", "Midgard Player's Gravestone Model", 636)]
        public static ushort MIDGARD_GRAVESTONE_MODEL;

        [ServerProperty("gravestone", "hibernia_gravestone_model", "Hibernia Player's Gravestone Model", 637)]
        public static ushort HIBERNIA_GRAVESTONE_MODEL;
	    #endregion
	    
	    /// <summary>
	    /// Create a new Instance of <see cref="GameGravestone"/> with given XPValue.
	    /// </summary>
	    /// <param name="Player">Player for which gravestone is created.</param>
	    /// <param name="XPValue">XP Value of the gravestone.</param>
	    public GameGravestone(GamePlayer Player, long XPValue)
		{
			//Objects should NOT be saved back to the DB
			//as standard! We want our mobs/items etc. at
			//the same startingspots when we restart!
			m_saveInDB = false;
			
			m_name = LanguageMgr.GetTranslation(Player.Client.Account.Language, "GameGravestone.GameGravestone.Grave", Player.Name);
			
			m_Heading = Player.Heading;
			m_x = Player.X;
			m_y = Player.Y;
			m_z = Player.Z;
			CurrentRegionID = Player.CurrentRegionID;
			
			m_level = 0;

			switch (Player.Realm)
			{
			    case eRealm.Albion:
			        m_model = ALBION_GRAVESTONE_MODEL;
			        break;
			    case eRealm.Midgard:
			        m_model = MIDGARD_GRAVESTONE_MODEL;
			        break;
			    case eRealm.Hibernia:
			        m_model = HIBERNIA_GRAVESTONE_MODEL;
			        break;
			}

			this.XPValue = XPValue;
			m_InternalID = Player.InternalID;	// gravestones use the player unique id for themself
		}

		/// <summary>
		/// returns the xpvalue of this gravestone
		/// </summary>
		public long XPValue { get; private set; }
		
        public override void Delete()
        {
            XPValue = 0;
            base.Delete();
        }
	}
}
