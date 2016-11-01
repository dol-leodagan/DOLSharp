﻿/*
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
    /// Client Packet sent when Interacting with a Ship Hook Point.
    /// </summary>
    public class ShipHookPointInteractPacket : AbstractClientPacket, IClientPacketObjectOid
    {
        public virtual ushort Unknown1 { get; set; }
        public virtual ushort ObjectOid { get; set; }
        public virtual ushort Unknown2 { get; set; }
        public virtual byte Slot { get; set; }
        public virtual byte Flag { get; set; }
        public virtual byte Currency { get; set; }
        public virtual byte Unknown3 { get; set; }
        public virtual ushort Unknown4 { get; set; }
        public virtual uint Type { get; set; }
        /// <summary>
        /// Create a new Instance of <see cref="ShipHookPointInteractPacket"/> using Packet.
        /// </summary>
        /// <param name="Packet">GameServer Packet to Parse.</param>
        public ShipHookPointInteractPacket(GSPacketIn Packet)
        {
            Unknown1 = Packet.ReadShort();
            ObjectOid = Packet.ReadShort();
            Unknown2 = Packet.ReadShort();
            Slot = (byte)Packet.ReadByte();
            Flag = (byte)Packet.ReadByte();
            Currency = (byte)Packet.ReadByte();
            Unknown3 = (byte)Packet.ReadByte();
            Unknown4 = Packet.ReadShort();
            Type = Packet.ReadIntLowEndian();
        }
        
        protected ShipHookPointInteractPacket() { }
    }
}
