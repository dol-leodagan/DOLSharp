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
    /// Client Packet sent when running a market search request.
    /// </summary>
    public class MarketSearchPacket : AbstractClientPacket
    {
        public virtual string Filter { get; set; }
        public virtual uint Slot { get; set; }
        public virtual uint Skill { get; set; }
        public virtual uint Resist { get; set; }
        public virtual uint Bonus { get; set; }
        public virtual uint HealthPoint { get; set; }
        public virtual uint Power { get; set; }
        public virtual uint Proc { get; set; }
        public virtual uint QuantityMin { get; set; }
        public virtual uint QuantityMax { get; set; }
        public virtual uint LevelMin { get; set; }
        public virtual uint LevelMax { get; set; }
        public virtual uint PriceMin { get; set; }
        public virtual uint PriceMax { get; set; }
        public virtual uint Visual { get; set; }
        public virtual byte Page { get; set; }
        public virtual byte Unknown1 { get; set; }
        public virtual ushort Unknown2 { get; set; }
        
        /// <summary>
        /// Create a new Instance of <see cref="MarketSearchPacket"/> using Packet.
        /// </summary>
        /// <param name="Packet">GameServer Packet to Parse.</param>
        public MarketSearchPacket(GSPacketIn Packet)
        {
            Filter = Packet.ReadString(64);
            Slot = Packet.ReadInt();
            Skill = Packet.ReadInt();
            Resist = Packet.ReadInt();
            Bonus = Packet.ReadInt();
            HealthPoint = Packet.ReadInt();
            Power = Packet.ReadInt();
            Proc = Packet.ReadInt();
            QuantityMin = Packet.ReadInt();
            QuantityMax = Packet.ReadInt();
            LevelMin = Packet.ReadInt();
            LevelMax = Packet.ReadInt();
            PriceMin = Packet.ReadInt();
            PriceMax = Packet.ReadInt();
            Visual = Packet.ReadInt();
            Page = (byte)Packet.ReadByte();
            Unknown1 = (byte)Packet.ReadByte();
            Unknown2 = Packet.ReadShort();
        }
        
        protected MarketSearchPacket() { }
    }
}
