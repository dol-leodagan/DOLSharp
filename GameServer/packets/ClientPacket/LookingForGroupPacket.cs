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
    /// Client Packet sent when changing Looking for Group Flag.
    /// </summary>
    public class LookingForGroupPacket : AbstractClientPacket
    {
        public virtual byte LFGFlag { get; set; }
        public virtual byte Unknown1 { get; set; }
        public virtual byte[] Filters { get; set; }
        public virtual byte LevelMin { get; set; }
        public virtual byte LevelMax { get; set; }
        public virtual byte Unknown2 { get; set; }
        public virtual ushort Unknown3 { get; set; }

        /// <summary>
        /// Create a new Instance of <see cref="LookingForGroupPacket"/> using Packet.
        /// </summary>
        /// <param name="Packet">GameServer Packet to Parse.</param>
        public LookingForGroupPacket(GSPacketIn Packet)
        {
            LFGFlag = (byte)Packet.ReadByte();
            Unknown1 = (byte)Packet.ReadByte();
            Filters = Enumerable.Range(0, 117).Select(i => (byte)Packet.ReadByte()).ToArray();
            LevelMin = (byte)Packet.ReadByte();
            LevelMax = (byte)Packet.ReadByte();
            Unknown2 = (byte)Packet.ReadByte();
            Unknown3 = Packet.ReadShort();
        }
        
        protected LookingForGroupPacket() { }
    }
}
