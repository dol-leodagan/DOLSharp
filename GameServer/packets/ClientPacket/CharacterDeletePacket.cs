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
    /// Client Packet sent for requesting Character Deletion.
    /// </summary>
    public class CharacterDeletePacket: AbstractClientPacket
    {
        public virtual string CharacterName { get; set; }
        public virtual string AccountName { get; set; }

        /// <summary>
        /// Create a new Instance of <see cref="CharacterDeletePacket"/> using Packet.
        /// </summary>
        /// <param name="Packet">GameServer Packet to Parse.</param>
        public CharacterDeletePacket(GSPacketIn Packet)
        {
            CharacterName = Packet.ReadString(30);
            AccountName = Packet.ReadString(24);
        }
        
        protected CharacterDeletePacket() { }
    }
}
