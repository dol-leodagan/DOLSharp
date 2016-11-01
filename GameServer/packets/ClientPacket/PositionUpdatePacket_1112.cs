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
    /// Client Packet sent when player update its position.
    /// </summary>
    public class PositionUpdatePacket_1112 : PositionUpdatePacket_190
    {        
        /// <summary>
        /// Create a new Instance of <see cref="PositionUpdatePacket_1112"/> using Packet.
        /// </summary>
        /// <param name="Packet">GameServer Packet to Parse.</param>
        public PositionUpdatePacket_1112(GSPacketIn Packet)
        {
            SessionId = Packet.ReadShort();
            Status = Packet.ReadShort();
            CurrentZoneZ = Packet.ReadShort();
            CurrentZoneX = Packet.ReadShort();
            CurrentZoneY = Packet.ReadShort();
            CurrentZoneId = Packet.ReadShort();
            Heading = Packet.ReadShort();
            Speed = Packet.ReadShort();
            Flag = (byte)Packet.ReadByte();
            Health = (byte)Packet.ReadByte();
            Unknown2 = Packet.ReadShort();
            Unknown3 = new uint[0];
            Unknown4 = (byte)Packet.ReadByte();
            Unknown5 = (byte)Packet.ReadByte();
        }
        
        protected PositionUpdatePacket_1112() { }
    }
}
