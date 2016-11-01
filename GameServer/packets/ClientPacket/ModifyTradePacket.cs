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
using System.Linq;

namespace DOL.GS.ClientPacket
{
    /// <summary>
    /// Client Packet sent when.
    /// </summary>
    public class ModifyTradePacket : AbstractClientPacket
    {
        public virtual byte Code { get; set; }
        public virtual byte Repair { get; set; }
        public virtual byte Combine { get; set; }
        public virtual byte Unknown1 { get; set; }
        public virtual byte[] Slots { get; set; }
        public virtual ushort Unknown2 { get; set; }
        public virtual ushort Mithril { get; set; }
        public virtual ushort Platinum { get; set; }
        public virtual ushort Gold { get; set; }
        public virtual ushort Silver { get; set; }
        public virtual ushort Copper { get; set; }
        public virtual ushort Unknown3 { get; set; }
        
        /// <summary>
        /// Create a new Instance of <see cref="ModifyTradePacket"/> using Packet.
        /// </summary>
        /// <param name="Packet">GameServer Packet to Parse.</param>
        public ModifyTradePacket(GSPacketIn Packet)
        {
            Code = (byte)Packet.ReadByte();
            Repair = (byte)Packet.ReadByte();
            Combine = (byte)Packet.ReadByte();
            Unknown1 = (byte)Packet.ReadByte();
            Slots = Enumerable.Range(0, 10).Select(i => (byte)Packet.ReadByte()).ToArray();
            Unknown2 = Packet.ReadShort();
            Mithril = Packet.ReadShort();
            Platinum = Packet.ReadShort();
            Gold = Packet.ReadShort();
            Silver = Packet.ReadShort();
            Copper = Packet.ReadShort();
            Unknown3 = Packet.ReadShort();
        }
        
        protected ModifyTradePacket() { }
    }
}
