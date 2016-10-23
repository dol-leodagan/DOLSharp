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

namespace DOL.GS.Rivers
{
	/// <summary>
	/// RiverManager Implement how Game Object are checked for Underwater state.
	/// </summary>
	public sealed class RiverManager : IRiverManager
	{
		/// <summary>
		/// Not Under Water
		/// </summary>
		private static readonly Tuple<bool, eRiverType> NotUnderWater = new Tuple<bool, eRiverType>(false, eRiverType.River);
		
		/// <summary>
		/// Under Water
		/// </summary>
		private static readonly Tuple<bool, eRiverType> UnderWater = new Tuple<bool, eRiverType>(true, eRiverType.River);
		
		/// <summary>
		/// Create new instance of <see cref="RiverManager"/>
		/// </summary>
		public RiverManager()
		{
		}
		
		public Tuple<bool, eRiverType> IsUnderWater(GameObject obj)
		{
			if (obj.CurrentRegion == null || obj.CurrentZone == null)
				return NotUnderWater;
			
			var X = obj.X;
			var Y = obj.Y;
			var Z = obj.Z;
			var Waterlevel = obj.CurrentZone.Waterlevel;
			
			// Special land areas below the waterlevel in NF
			if (obj.CurrentRegion.Skin == 163)
			{
				// Mount Collory
				if ((Y > 664000) && (Y < 670000) && (X > 479000) && (X < 488000)) return NotUnderWater;
				if ((Y > 656000) && (Y < 664000) && (X > 472000) && (X < 488000)) return NotUnderWater;
				if ((Y > 624000) && (Y < 654000) && (X > 468500) && (X < 488000)) return NotUnderWater;
				if ((Y > 659000) && (Y < 683000) && (X > 431000) && (X < 466000)) return NotUnderWater;
				if ((Y > 646000) && (Y < 659001) && (X > 431000) && (X < 460000)) return NotUnderWater;
				if ((Y > 624000) && (Y < 646001) && (X > 431000) && (X < 455000)) return NotUnderWater;
				if ((Y > 671000) && (Y < 683000) && (X > 431000) && (X < 471000)) return NotUnderWater;
				// Breifine
				if ((Y > 558000) && (Y < 618000) && (X > 456000) && (X < 479000)) return NotUnderWater;
				// Cruachan Gorge
				if ((Y > 586000) && (Y < 618000) && (X > 360000) && (X < 424000)) return NotUnderWater;
				if ((Y > 563000) && (Y < 578000) && (X > 360000) && (X < 424000)) return NotUnderWater;
				// Emain Macha
				if ((Y > 505000) && (Y < 555000) && (X > 428000) && (X < 444000)) return NotUnderWater;
				// Hadrian's Wall
				if ((Y > 500000) && (Y < 553000) && (X > 603000) && (X < 620000)) return NotUnderWater;
				// Snowdonia
				if ((Y > 633000) && (Y < 678000) && (X > 592000) && (X < 617000)) return NotUnderWater;
				if ((Y > 662000) && (Y < 678000) && (X > 581000) && (X < 617000)) return NotUnderWater;
				// Sauvage Forrest
				if ((Y > 584000) && (Y < 615000) && (X > 626000) && (X < 681000)) return NotUnderWater;
				// Uppland
				if ((Y > 297000) && (Y < 353000) && (X > 610000) && (X < 652000)) return NotUnderWater;
				// Yggdra
				if ((Y > 408000) && (Y < 421000) && (X > 671000) && (X < 693000)) return NotUnderWater;
				if ((Y > 364000) && (Y < 394000) && (X > 674000) && (X < 716000)) return NotUnderWater;
			}

			return Z < Waterlevel ? UnderWater : NotUnderWater;
		}
	}
}
