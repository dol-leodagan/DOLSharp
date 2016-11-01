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
	[PacketHandlerAttribute(PacketHandlerType.TCP, eClientPackets.CryptKeyRequest, "Handles crypt key requests", eClientStatus.None)]
	public class CryptKeyRequestHandler : IPacketHandler
	{
		public void HandlePacket(GameClient client, GSPacketIn packet)
		{
		    CryptKeyClientPacket cryptPacket;
		    if (client.Version >= GameClient.eClientVersion.Version1115)
		        cryptPacket = new CryptKeyClientPacket_1115(packet);
		    else if (client.Version >= GameClient.eClientVersion.Version186)
		        cryptPacket = new CryptKeyClientPacket_186(packet);
		    else
		        cryptPacket = new CryptKeyClientPacket(packet);
		    
		    if (cryptPacket.RC4Enabled == 1)
		    {
		        client.PacketProcessor.Encoding.SBox = cryptPacket.EncryptionKey;
		        ((PacketEncoding168)client.PacketProcessor.Encoding).EncryptionState = PacketEncoding168.eEncryptionState.PseudoRC4Encrypted;
		    }
		    else
		    {
		      client.Out.SendVersionAndCryptKey();
		    }
		}
	}
}