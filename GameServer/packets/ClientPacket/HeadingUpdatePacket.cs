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
    /// Client Packet sent when changing heading also known as Short State for small player broadcast update.
    /// </summary>
    public class HeadingUpdatePacket : AbstractClientPacket, IClientPacketSessionId
    {
        public virtual ushort SessionId { get; set; }
        public virtual ushort Heading { get; set; }
        public virtual byte Unknown1 { get; set; }
        public virtual byte Flag { get; set; }
        public virtual byte Counter { get; set; }
        public virtual byte RideSlot { get; set; }
        public virtual byte Health { get; set; }
        public virtual byte State { get; set; }
        
        /// <summary>
        /// Create a new Instance of <see cref="HeadingUpdatePacket"/> using Packet.
        /// </summary>
        /// <param name="Packet">GameServer Packet to Parse.</param>
        public HeadingUpdatePacket(GSPacketIn Packet)
        {
            SessionId = Packet.ReadShort();
            Heading = Packet.ReadShort();
            Unknown1 = (byte)Packet.ReadByte();
            Flag = (byte)Packet.ReadByte();
            Counter = (byte)Packet.ReadByte();
            RideSlot = (byte)Packet.ReadByte();
            Health = (byte)Packet.ReadByte();
            State = (byte)Packet.ReadByte();
        }
        
        protected HeadingUpdatePacket() { }
    }
}
