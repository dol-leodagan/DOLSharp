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
	[PacketHandlerAttribute(PacketHandlerType.TCP, eClientPackets.RegionListRequest, "Handles sending the region overview", eClientStatus.None)]
	public class RegionListRequestHandler : IPacketHandler
	{
		public void HandlePacket(GameClient client, GSPacketIn packet)
		{
		    /*
		    RegionListRequestPacket regionRequest;
            if (client.Version >= GameClient.eClientVersion.Version183)
                regionRequest = new RegionListRequestPacket_183(packet);
            else if (client.Version >= GameClient.eClientVersion.Version180)
                regionRequest = new RegionListRequestPacket_180(packet);
            else if (client.Version >= GameClient.eClientVersion.Version174)
                regionRequest = new RegionListRequestPacket_174(packet);
            else if (client.Version >= GameClient.eClientVersion.Version172)
                regionRequest = new RegionListRequestPacket_172(packet);
            else
                regionRequest = new RegionListRequestPacket(packet);
            */
		    
			client.Out.SendRegions();
		}
	}
}
