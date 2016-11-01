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
    /// Client Packet sent when replying to a UI Pop Up Dialog.
    /// </summary>
    public class DialogResponsePacket : AbstractClientPacket
    {
        public virtual ushort Data1 { get; set; }
        public virtual ushort Data2 { get; set; }
        public virtual ushort Data3 { get; set; }
        public virtual byte DialogCode { get; set; }
        public virtual byte Response { get; set; }
        
        /// <summary>
        /// Create a new Instance of <see cref="DialogResponsePacket"/> using Packet.
        /// </summary>
        /// <param name="Packet">GameServer Packet to Parse.</param>
        public DialogResponsePacket(GSPacketIn Packet)
        {
            Data1 = Packet.ReadShort();
            Data2 = Packet.ReadShort();
            Data3 = Packet.ReadShort();
            DialogCode = (byte)Packet.ReadByte();
            Response = (byte)Packet.ReadByte();
        }
        
        protected DialogResponsePacket() { }
    }
}
