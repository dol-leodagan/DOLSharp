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

using DOL.GS.Rivers;

namespace DOL.GS.ClientData
{
	/// <summary>
	/// Point in a Zone
	/// </summary>
	public sealed class ZonePoint : Tuple<byte, byte>
	{
		public byte X { get { return Item1; } }
		public byte Y { get { return Item2; } }
		public ZonePoint(byte X, byte Y)
			: base(X, Y)
		{
		}
		public ZonePoint(int X, int Y)
			: this((byte)X, (byte)Y)
		{
		}
	}
	
	/// <summary>
	/// River Height and Type
	/// </summary>
	public sealed class RiverData : Tuple<ushort, eRiverType>
	{
		public ushort Height { get { return Item1; } }
		public eRiverType Type { get { return Item2; } }
		public RiverData(ushort Height, eRiverType Type)
			: base(Height, Type)
		{
		}
		
		public static RiverData FromIntString(int Height, string Type)
		{
			switch(Type.ToLower())
			{
				case "lava":
					return new RiverData((ushort)Height, eRiverType.Lava);
				case "swamp":
					return new RiverData((ushort)Height, eRiverType.Swamp);
				default:
					return new RiverData((ushort)Height, eRiverType.River);
			}
		}
	}
}
