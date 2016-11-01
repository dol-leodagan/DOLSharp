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
    /// Client Packet sent when selecting a Player and hitting the Play Button.
    /// </summary>
    public class CharacterSelectPacket_174 : CharacterSelectPacket
    {
        public virtual byte ServerId { get; set; }
        public virtual ushort Unknown12 { get; set; }
        public virtual byte Unknown13 { get; set; }
        
        /// <summary>
        /// Create a new Instance of <see cref="CharacterSelectPacket_174"/> using Packet.
        /// </summary>
        /// <param name="Packet">GameServer Packet to Parse.</param>
        public CharacterSelectPacket_174(GSPacketIn Packet)
        {
            SessionId = Packet.ReadShort();
            RegionIndex = (byte)Packet.ReadByte();
            Unknown1 = (byte)Packet.ReadByte();
            ServerId = (byte)Packet.ReadByte();
            CharacterName = Packet.ReadString(24);
            Unknown12 = Packet.ReadShort();
            Unknown13 = (byte)Packet.ReadByte();
            Unknown2 = Packet.ReadIntLowEndian();
            AccountName = Packet.ReadString(20);
            Unknown3 = Packet.ReadIntLowEndian();
            Unknown4 = Packet.ReadIntLowEndian();
            Unknown5 = Packet.ReadIntLowEndian();
            Unknown6 = Packet.ReadIntLowEndian();
            Unknown7 = Packet.ReadIntLowEndian();
            Unknown8 = Packet.ReadIntLowEndian();
            Unknown9 = Packet.ReadIntLowEndian();
            Unknown10 = Packet.ReadIntLowEndian();
            Port = Packet.ReadShort();
            Unknown11 = Packet.ReadShort();
        }
        
        protected CharacterSelectPacket_174() { }
    }
}
