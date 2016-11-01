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
    /// Client Packet sent when Initializing a new House User Permission List.
    /// </summary>
    public class HouseUserPermissionSetPacket : AbstractClientPacket, IClientPacketHouseOid
    {
        public virtual byte Index { get; set; }
        public virtual byte Level { get; set; }
        public virtual ushort HouseOid { get; set; }
        
        /// <summary>
        /// Create a new Instance of <see cref="HouseUserPermissionSetPacket"/> using Packet.
        /// </summary>
        /// <param name="Packet">GameServer Packet to Parse.</param>
        public HouseUserPermissionSetPacket(GSPacketIn Packet)
        {
            Index = (byte)Packet.ReadByte();
            Level = (byte)Packet.ReadByte();
            HouseOid = Packet.ReadShort();
        }
        
        protected HouseUserPermissionSetPacket() { }
    }
}
