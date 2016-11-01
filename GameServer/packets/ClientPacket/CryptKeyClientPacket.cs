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
    /// Client Packet Sent when Initializing Encryption.
    /// </summary>
    public class CryptKeyClientPacket : AbstractClientPacket
    {
        public virtual byte RC4Enabled { get; set; }
        public virtual byte ClientTypeAndAddons { get; set; }
        public virtual byte ClientVersionMajor { get; set; }
        public virtual byte ClientVersionMinor { get; set; }
        public virtual byte ClientVersionBuild { get; set; }
        public virtual byte[] EncryptionKey { get; set; }
        
        /// <summary>
        /// Create a new Instance of <see cref="CryptKeyClientPacket"/> using Packet.
        /// </summary>
        /// <param name="Packet">GameServer Packet to Parse.</param>
        public CryptKeyClientPacket(GSPacketIn Packet)
        {
            RC4Enabled = (byte)Packet.ReadByte();
            ClientTypeAndAddons = (byte)Packet.ReadByte();
            ClientVersionMajor = (byte)Packet.ReadByte();
            ClientVersionMinor = (byte)Packet.ReadByte();
            ClientVersionBuild = (byte)Packet.ReadByte();
            
            if (RC4Enabled == 1)
            {
                EncryptionKey = new byte[256];
                for (int i = 0 ; i < 256 ; i++)
                    EncryptionKey[i] = (byte)Packet.ReadByte();
            }
            else
            {
                EncryptionKey = new byte[0];
            }
        }
        
        protected CryptKeyClientPacket() { }
    }
}
