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
    /// Client Packet sent when choosing an Emblem.
    /// </summary>
    public class EmblemDialogPacket : AbstractClientPacket
    {
        public virtual byte Color1 { get; set; }
        public virtual byte Color2 { get; set; }
        public virtual byte Pattern { get; set; }
        public virtual byte Logo { get; set; }
        
        /// <summary>
        /// Create a new Instance of <see cref="EmblemDialogPacket"/> using Packet.
        /// </summary>
        /// <param name="Packet">GameServer Packet to Parse.</param>
        public EmblemDialogPacket(GSPacketIn Packet)
        {
            Color1 = (byte)Packet.ReadByte();
            Color2 = (byte)Packet.ReadByte();
            Pattern = (byte)Packet.ReadByte();
            Logo = (byte)Packet.ReadByte();
        }
        
        protected EmblemDialogPacket() { }
    }
}
