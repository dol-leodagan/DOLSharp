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

using DOL.GS.ClientPacket;

using log4net;

namespace DOL.GS.PacketHandler.Client.v168
{
    [PacketHandlerAttribute(PacketHandlerType.TCP, eClientPackets.MarketSearchRequest, "Handles player market search", eClientStatus.PlayerInGame)]
    public class PlayerMarketSearchRequestHandler : IPacketHandler
    {
        /// <summary>
        /// Defines a logger for this class.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public void HandlePacket(GameClient client, GSPacketIn packet)
		{
			if (client == null || client.Player == null)
				return;

			if ((client.Player.TargetObject is IGameInventoryObject) == false)
				return;

			MarketSearch.SearchData search = new MarketSearch.SearchData();
			
			var marketSearch = client.Version >= GameClient.eClientVersion.Version198
			    ? new MarketSearchPacket_198(packet)
			    : new MarketSearchPacket(packet);

			search.name = marketSearch.Filter;
			search.slot = (int)marketSearch.Slot;
			search.skill = (int)marketSearch.Skill;
			search.resist = (int)marketSearch.Resist;
			search.bonus = (int)marketSearch.Bonus;
			search.hp = (int)marketSearch.HealthPoint;
			search.power = (int)marketSearch.Power;
			search.proc = (int)marketSearch.Proc;
			search.qtyMin = (int)marketSearch.QuantityMin;
			search.qtyMax = (int)marketSearch.QuantityMax;
			search.levelMin = (int)marketSearch.LevelMin;
			search.levelMax = (int)marketSearch.LevelMax;
			search.priceMin = (int)marketSearch.PriceMin;
			search.priceMax = (int)marketSearch.PriceMax;
			search.visual = (int)marketSearch.Visual;
			search.page = (byte)marketSearch.Page;

			var market190 = marketSearch as MarketSearchPacket_198;
			if (market190 != null)
			{
				// Dunnerholl 2009-07-28 Version 1.98 introduced new options to Market search. 12 Bytes were added, but only 7 are in usage so far in my findings.
				// update this, when packets change and keep in mind, that this code reflects only the 1.98 changes
				search.armorType = market190.ArmorType; // page is now used for the armorType (still has to be logged, i just checked that 2 means leather, 0 = standard
				search.damageType = market190.DamageType; // 1=crush, 2=slash, 3=thrust
				search.playerCrafted = market190.PlayerCrafted; // 1 = show only Player crafted, 0 = all
			}

			search.clientVersion = client.Version.ToString();

			(client.Player.TargetObject as IGameInventoryObject).SearchInventory(client.Player, search);
		}
    }
}