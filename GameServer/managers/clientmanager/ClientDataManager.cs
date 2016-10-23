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
using System.Linq;

using DaocClientLib;

using log4net;

namespace DOL.GS.ClientData
{
	/// <summary>
	/// ClientDataManager Handle Extracted Data from Embedded Client files 
	/// </summary>
	public sealed class ClientDataManager
	{		
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// Client Wrapper for Retrieving Information
		/// </summary>
		private ClientDataWrapper ClientWrapper { get; set; }
		
		/// <summary>
		/// Ready State of the Client Data Manager
		/// </summary>
		public bool IsReady { get; private set; }

		/// <summary>
		/// Create an instance of <see cref="ClientDataManager"/>
		/// </summary>
		/// <param name="GameServerInstance">GameServerInstance using this Wrapper</param>
		public ClientDataManager(GameServer GameServerInstance)
		{
			if (GameServerInstance == null)
				throw new ArgumentNullException("GameServerInstance");

			var clientpath = GameServerInstance.Configuration.ClientDirPath;
			
			if (string.IsNullOrWhiteSpace(clientpath))
			{
				if (log.IsInfoEnabled)
					log.Info("GameServer Configuration <ClientDirPath /> is Empty, Skipping Client Wrapper Loading...");
				
				return;
			}
			
			try
			{
				ClientWrapper = new ClientDataWrapper(clientpath);
				IsReady = true;
			}
			catch (Exception ex)
			{
				if (log.IsWarnEnabled)
					log.WarnFormat("Error While Loading Client Data from Client Dir Path {0}:{2}{1}", clientpath, Environment.NewLine, ex);
			}
		}
		
		#region Data Access
		/// <summary>
		/// Load BitMap Indexed Rivers
		/// </summary>
		public void LoadBitMapRivers(out IDictionary<ushort, byte[,]> BitMapRivers, out IDictionary<ushort, IDictionary<byte, RiverData>> ZoneRiverIndex)
		{
			var geometries = ClientWrapper.ZonesGeometry;
			var zonesIndex = new Dictionary<ushort, IDictionary<byte, RiverData>>();
			BitMapRivers = ClientWrapper.ZonesData/*.Where(zone => zone.ID == 81 || zone.ID == 38 || zone.ID == 138)*/
				.Select(zone => {
				        	// Select With Proxy Zone ID
				        	var zoneGeom = geometries[zone.IsProxyZone ? zone.ProxyZone : zone.ID];
				        	if (zoneGeom != null)
				        	{
				        		try
				        		{
				        			// Try Storing Water BitMap
				        			var waters = zoneGeom.WaterIndexMap;
				        			
				        			if (waters.Length > 0)
				        			{
					        			// Add to River Indexes
					        			var rivers = zoneGeom.Rivers.ToDictionary(river => (byte)river.ID, river => RiverData.FromIntString(Math.Max(0, river.Height - 40), river.Type));
					        			zonesIndex.Add((ushort)zone.ID, rivers);
	
					        			if (log.IsInfoEnabled)
					        				log.InfoFormat("Loaded River Bitmap for Zone {0} (ID {1}) with {2} river(s)!", zone.Name, zone.ID, rivers.Count);
					        			
					        			return new { ZoneID = zone.ID, Bitmap = waters };
				        			}
				        			
			        				if (log.IsWarnEnabled)
			        					log.WarnFormat("Could not retrieve correct Bitmap from Client File for Zone {0} (ID {1}) with {2} river(s)...", zone.Name, zone.ID, zoneGeom.Rivers.Count());
				        		}
				        		catch (Exception ex)
				        		{
				        			if (log.IsWarnEnabled)
				        				log.WarnFormat("Could not load River BitMap for Zone {0} (ID {1})...{2}{3}", zone.Name, zone.ID, Environment.NewLine, ex);
				        		}
				        	}
				        	else
				        	{
				        		if (log.IsWarnEnabled)
				        			log.WarnFormat("No Zone Data for Zone {0} (ID {1}", zone.Name, zone.ID);
				        	}

				        	return new { ZoneID = zone.ID, Bitmap = (byte[,])null };
				        })
				.Where(item => item.Bitmap != null).ToDictionary(item => (ushort)item.ZoneID, item => item.Bitmap);
			
			ZoneRiverIndex = zonesIndex;
		}
		#endregion
	}
}
