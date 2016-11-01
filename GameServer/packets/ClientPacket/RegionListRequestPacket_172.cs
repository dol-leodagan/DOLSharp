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
    /// Client Packet sent when requesting region list.
    /// </summary>
    public class RegionListRequestPacket_172 : RegionListRequestPacket
    {
        public virtual byte Slot { get; set; }
        
        public virtual ushort Resolution { get; set; }
        public virtual ushort Options { get; set; }
        public virtual byte Memory { get; set; }
        public virtual ushort Unknown1 { get; set; }
        public virtual byte Unknown2 { get; set; }
        public virtual uint FigureVersion { get; set; }
        public virtual byte FigureVersionMinor { get; set; }
        public virtual byte Skin { get; set; }
        public virtual byte GenderRace { get; set; }
        public virtual byte RegionExpantions { get; set; }
        public virtual byte Zero { get; set; }
        
        /// <summary>
        /// Create a new Instance of <see cref="RegionListRequestPacket_172"/> using Packet.
        /// </summary>
        /// <param name="Packet">GameServer Packet to Parse.</param>
        public RegionListRequestPacket_172(GSPacketIn Packet)
        {
            Slot = (byte)Packet.ReadByte();
            Flag = (byte)Packet.ReadByte();
            if (Flag > 0)
            {
                Resolution = Packet.ReadShort();
                Options = Packet.ReadShort();
                Memory = (byte)Packet.ReadByte();
                Unknown1 = Packet.ReadShort();
                Unknown2 = (byte)Packet.ReadByte();
                FigureVersion = Packet.ReadInt();
                FigureVersionMinor = (byte)Packet.ReadByte();
                Skin = (byte)Packet.ReadByte();
                GenderRace = (byte)Packet.ReadByte();
                RegionExpantions = (byte)Packet.ReadByte();
                Zero = (byte)Packet.ReadByte();
            }
        }
        
        protected RegionListRequestPacket_172() { }
    }
}
