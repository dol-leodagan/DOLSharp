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
    /// Client Packet Sent When using an In-Game Command.
    /// </summary>
    public class CommandPacket : AbstractClientPacket, IClientPacketSessionId
    {
        public virtual ushort SessionId { get; set; }
        public virtual ushort Unknown1 { get; set; }
        public virtual ushort Unknown2 { get; set; }
        public virtual ushort Unknown3 { get; set; }
        public virtual string Command { get; set; }
        public virtual byte Flag { get; set; }
        
        /// <summary>
        /// Create a new Instance of <see cref="CommandPacket"/> using Packet.
        /// </summary>
        /// <param name="Packet">GameServer Packet to Parse.</param>
        public CommandPacket(GSPacketIn Packet)
        {
            SessionId = Packet.ReadShort();
            Unknown1 = Packet.ReadShort();
            Unknown2 = Packet.ReadShort();
            Unknown3 = Packet.ReadShort();
            Command = Packet.ReadString(255);
            Flag = (byte)(Packet.Position < Packet.Length ? Packet.ReadByte() : 0);
        }
        
        protected CommandPacket() { }
    }
}
