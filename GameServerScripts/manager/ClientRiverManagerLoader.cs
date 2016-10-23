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
using System.Reflection;
using System.Collections.Generic;

using DOL.GS;
using DOL.GS.ClientData;
using DOL.Events;

using log4net;

namespace DOL.GSS.Manager.Rivers
{
	/// <summary>
	/// Script ClientRiverManager Loader.
	/// </summary>
	public static class ClientRiverManagerLoader
	{
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// Try to Load Client River Manager when Starting Server. 
		/// </summary>
		[ScriptLoadedEvent]
		public static void OnScriptLoad(DOLEvent e, object sender, EventArgs args)
		{
			if (log.IsInfoEnabled)
				log.Info("Trying to Start Client-Based RiverManager...");
			
			var GameServerIntance = sender as GameServer;
			
			if (GameServerIntance != null)
			{
				if (GameServerIntance.ClientData.IsReady)
				{
					IDictionary<ushort, byte[,]> BitMapRivers;
					IDictionary<ushort, IDictionary<byte, RiverData>> ZoneRiverIndex;
					GameServerIntance.ClientData.LoadBitMapRivers(out BitMapRivers, out ZoneRiverIndex);
					var clientRiverManager = new ClientRiverManager(BitMapRivers, ZoneRiverIndex);
					GameServerIntance.WorldManager.ChangeRiverManager(clientRiverManager);
					
					if (log.IsInfoEnabled)
						log.Info("Client River Manager configured as default River Manager !");
				}
				else
				{
					if (log.IsInfoEnabled)
						log.Info("Client Data is not available, skipping...");
				}
			}
			else
			{
				if (log.IsWarnEnabled)
					log.Warn("GameServerInstance is null, skipping...");
			}
		}
	}
}
