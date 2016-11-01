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

namespace DOL.GS.ClientPacket
{
    /// <summary>
    /// Client Packet sent when trying to Log In.
    /// </summary>
    public class LoginRequestPacket_174 : LoginRequestPacket
    {
        /// <summary>
        /// Create a new Instance of <see cref="LoginRequestPacket_174"/> using Packet.
        /// </summary>
        /// <param name="Packet">GameServer Packet to Parse.</param>
        public LoginRequestPacket_174(GSPacketIn Packet)
        {
            Unknown1 = (byte)Packet.ReadByte();
            ClientTypeAndAddons = (byte)Packet.ReadByte();
            ClientVersionMajor = (byte)Packet.ReadByte();
            ClientVersionMinor = (byte)Packet.ReadByte();
            ClientVersionBuild = (byte)Packet.ReadByte();
            Password = Packet.ReadString(19);
            
            Unknown7 = Packet.ReadIntLowEndian();
            
            Unknown12 = Packet.ReadIntLowEndian();
            
            Unknown3 = Packet.ReadIntLowEndian();
            Unknown4 = Packet.ReadIntLowEndian();
            Unknown5 = Packet.ReadIntLowEndian();
            Unknown6 = Packet.ReadIntLowEndian();
            
            Unknown2 = Packet.ReadIntLowEndian();
            
            EDI = Packet.ReadIntLowEndian();
            Unknown8 = Packet.ReadIntLowEndian();

            Unknown9 = Packet.ReadIntLowEndian();
            Unknown10 = Packet.ReadIntLowEndian();
            Unknown11 = Packet.ReadIntLowEndian();
            
            Unknown13 = (byte)Packet.ReadByte();
            Unknown14 = Packet.ReadShort();

            AccountName = Packet.ReadString(20);
        }
        
        protected LoginRequestPacket_174() { }
    }
}
