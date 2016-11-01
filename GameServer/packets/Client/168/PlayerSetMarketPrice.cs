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

using DOL.GS.ClientPacket;

namespace DOL.GS.PacketHandler.Client.v168
{
    [PacketHandlerAttribute(PacketHandlerType.TCP, eClientPackets.SetMarketPrice, "Set Market/Consignment Merchant Price.", eClientStatus.PlayerInGame)]
    public class PlayerSetMarketPriceHandler : IPacketHandler
    {
        public void HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client == null || client.Player == null)
                return;
            
            var marketPrice = new SetMarketPricePacket(packet);

			int slot = marketPrice.Slot;
			uint price = marketPrice.Price;

			// ChatUtil.SendDebugMessage(client.Player, "PlayerSetMarketPriceHandler");

			// only IGameInventoryObjects can handle set price commands
			if (client.Player.TargetObject == null || (client.Player.TargetObject is IGameInventoryObject) == false)
				return;

			(client.Player.TargetObject as IGameInventoryObject).SetSellPrice(client.Player, (ushort)slot, price);
        }
    }
}