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
using System.Collections;

using DOL.Database;
using DOL.GS.ClientPacket;

namespace DOL.GS.PacketHandler.Client.v168
{
	[PacketHandlerAttribute(PacketHandlerType.TCP, eClientPackets.ModifyTrade, "Player Accepts Trade", eClientStatus.PlayerInGame)]
	public class PlayerModifyTradeHandler : IPacketHandler
	{
		public void HandlePacket(GameClient client, GSPacketIn packet)
		{
		    var tradePacket = new ModifyTradePacket(packet);
		    
			byte isok = tradePacket.Code;
			byte repair = tradePacket.Repair;
			byte combine = tradePacket.Combine;

			ITradeWindow trade = client.Player.TradeWindow;
			if (trade == null)
				return;

			if (isok == 0)
			{
				trade.CloseTrade();
			}
			else if(isok == 1)
			{
				if(trade.Repairing != (repair == 1)) trade.Repairing = (repair == 1);
				if(trade.Combine != (combine == 1)) trade.Combine = (combine == 1);
				
				ArrayList tradeSlots = new ArrayList(10);
				for (int i = 0 ; i < 10 ; i++)
				{
				    int slotPosition = tradePacket.Slots[i];
					InventoryItem item = client.Player.Inventory.GetItem((eInventorySlot)slotPosition);
					if(item != null && ((item.IsDropable && item.IsTradable) || (client.Player.CanTradeAnyItem || client.Player.TradeWindow.Partner.CanTradeAnyItem)))
					{
						tradeSlots.Add(item);
					}
				}
				
				trade.TradeItems = tradeSlots;

				long money = Money.GetMoney(tradePacket.Mithril, tradePacket.Platinum, tradePacket.Gold, tradePacket.Silver, tradePacket.Copper);
				trade.TradeMoney = money;
				
				trade.TradeUpdate();
			}
			else if (isok == 2)
			{
				trade.AcceptTrade();
			}
		}
	}
}

