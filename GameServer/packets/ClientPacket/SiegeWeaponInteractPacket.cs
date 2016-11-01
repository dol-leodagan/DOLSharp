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
    /// Client Packet sent when player interact with Siege Weapon.
    /// </summary>
    public class SiegeWeaponInteractPacket : AbstractClientPacket
    {
        public virtual ushort Unknown1 { get; set; }
        public virtual byte Action { get; set; }
        public virtual byte AmmoIndex { get; set; }
        public virtual ushort Unknown2 { get; set; }
        public virtual ushort Unknown3 { get; set; }
        public virtual ushort Unknown4 { get; set; }
        public virtual ushort Unknown5 { get; set; }
        public virtual ushort Unknown6 { get; set; }
        public virtual ushort Unknown7 { get; set; }

        
        /// <summary>
        /// Create a new Instance of <see cref="SiegeWeaponInteractPacket"/> using Packet.
        /// </summary>
        /// <param name="Packet">GameServer Packet to Parse.</param>
        public SiegeWeaponInteractPacket(GSPacketIn Packet)
        {
            Unknown1 = Packet.ReadShort();
            Action = (byte)Packet.ReadByte();
            AmmoIndex = (byte)Packet.ReadByte();
            Unknown2 = Packet.ReadShort(); // unused
            Unknown3 = Packet.ReadShort(); // unused
            Unknown4 = Packet.ReadShort(); // unused
            Unknown5 = Packet.ReadShort(); // unused
            Unknown6 = Packet.ReadShort(); // unused
            Unknown7 = Packet.ReadShort(); // unused
        }
        
        protected SiegeWeaponInteractPacket() { }
    }
}
