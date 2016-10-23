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
using System.Collections.Generic;

using DOL.GS;
using DOL.GS.ClientData;
using DOL.GS.Rivers;

namespace DOL.GSS.Manager.Rivers
{
	/// <summary>
	/// Description of ClientRiverManager.
	/// </summary>
	public sealed class ClientRiverManager : IRiverManager
	{
		/// <summary>
		/// Not Under Water
		/// </summary>
		private static readonly Tuple<bool, eRiverType> NotUnderWater = new Tuple<bool, eRiverType>(false, eRiverType.River);

		/// <summary>
		/// Water Bitmap are 256*256 pixel for each zones.
		/// </summary>
		private const double WATER_BITMAP_SCALE = 256.0 / 65536;
		
		/// <summary>
		/// Client Rivers BitMap Indexed
		/// </summary>
		private IDictionary<ushort, byte[,]> BitMapRivers { get; set; }
		
		private IDictionary<ushort, IDictionary<byte, RiverData>> ZoneRiverIndex { get; set; }
		
		/// <summary>
		/// Create a new Instance of <see cref="ClientRiverManager"/>
		/// </summary>
		/// <param name="BitMapRivers"></param>
		/// <param name="ZoneRiverIndex"></param>
		public ClientRiverManager(IDictionary<ushort, byte[,]> BitMapRivers, IDictionary<ushort, IDictionary<byte, RiverData>> ZoneRiverIndex)
		{
			this.BitMapRivers = BitMapRivers;
			this.ZoneRiverIndex = ZoneRiverIndex;
		}
		
		/// <summary>
		/// Check if this GameObject is UnderWater given Current Zone Data.
		/// </summary>
		/// <param name="obj">GameObject to Check</param>
		/// <returns>Return if Underwater and Type of River</returns>
		public Tuple<bool, eRiverType> IsUnderWater(GameObject obj)
		{
			var zone = obj.CurrentZone;
			var currentX = obj.X;
			var currentY = obj.Y;
			var currentZ = obj.Z;
			
			if (zone != null)
			{
				byte[,] zoneRivers;
				if (BitMapRivers.TryGetValue(zone.ZoneSkinID, out zoneRivers))
				{
					var waterX = (byte)Math.Floor((currentX - zone.XOffset) * WATER_BITMAP_SCALE);
					var waterY = (byte)Math.Floor((currentY - zone.YOffset) * WATER_BITMAP_SCALE);
					
					if (waterX < zoneRivers.GetLength(0) && waterY < zoneRivers.GetLength(1))
					{
						var riverIndex = zoneRivers[waterX, waterY];
						
						// River Index start at 0 (black), 255 is white
						if (riverIndex < 255)
						{
							IDictionary<byte, RiverData> rivers;
							if (ZoneRiverIndex.TryGetValue(zone.ZoneSkinID, out rivers))
							{
								RiverData river;
								if (rivers.TryGetValue(riverIndex, out river))
									return currentZ < river.Height ? new Tuple<bool, eRiverType>(true, river.Type) : NotUnderWater;
							}
						}
					}
				}
				else
				{
					// zone have no registered bitmap, revert to zone WaterLevel
					return currentZ < zone.Waterlevel ? new Tuple<bool, eRiverType>(true, eRiverType.River) : NotUnderWater;
				}
			}
			
			return NotUnderWater;
		}
	}
}
