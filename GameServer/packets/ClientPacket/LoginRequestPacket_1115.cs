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
    public class LoginRequestPacket_1115 : LoginRequestPacket_1104
    {
        public virtual char ClientVersionRevision { get; set; }
        public virtual ushort ClientVersionSerial { get; set; }

        /// <summary>
        /// Create a new Instance of <see cref="LoginRequestPacket_1115"/> using Packet.
        /// </summary>
        /// <param name="Packet">GameServer Packet to Parse.</param>
        public LoginRequestPacket_1115(GSPacketIn Packet)
        {
            ClientTypeAndAddons = (byte)Packet.ReadByte();
            ClientVersionMajor = (byte)Packet.ReadByte();
            ClientVersionMinor = (byte)Packet.ReadByte();
            ClientVersionBuild = (byte)Packet.ReadByte();
            ClientVersionRevision = (char)Packet.ReadByte();
            ClientVersionSerial = Packet.ReadShort();
            AccountName = Packet.ReadLowEndianShortPascalString();
            Password = Packet.ReadLowEndianShortPascalString();
        }
        
        protected LoginRequestPacket_1115() { }
    }
}
