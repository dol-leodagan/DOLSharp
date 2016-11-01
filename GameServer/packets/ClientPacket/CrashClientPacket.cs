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
    /// Client Packet sent when getting a game crash, containing debug info.
    /// </summary>
    public class CrashClientPacket : AbstractClientPacket
    {
        public virtual string Module { get; set; }
        public virtual string Version { get; set; }
        public virtual uint ErrorCode { get; set; }
        public virtual uint CS { get; set; }
        public virtual uint EIP { get; set; }
        public virtual uint Options { get; set; }
        public virtual byte[] GSXM { get; set; }
        public virtual byte ClientRegionExpantions { get; set; }
        public virtual byte ClientType { get; set; }
        public virtual byte OSType { get; set; }
        public virtual byte TerrainOptions { get; set; }
        public virtual ushort Region { get; set; }
        public virtual ushort Unknown1 { get; set; }
        public virtual uint Uptime { get; set; }
        public virtual uint Stack1 { get; set; }
        public virtual uint Stack2 { get; set; }
        public virtual uint Stack3 { get; set; }
        public virtual uint Stack4 { get; set; }
        
        /// <summary>
        /// Create a new Instance of <see cref="CrashClientPacket"/> using Packet.
        /// </summary>
        /// <param name="Packet">GameServer Packet to Parse.</param>
        public CrashClientPacket(GSPacketIn Packet)
        {
            Module = Packet.ReadString(32);
            Version = Packet.ReadString(8);
            ErrorCode = Packet.ReadInt();
            CS = Packet.ReadInt();
            EIP = Packet.ReadInt();
            Options = Packet.ReadInt();
            GSXM = new byte[16];
            for (int i = 0; i < 16; i++)
                GSXM[i] = (byte)Packet.ReadByte();
            ClientRegionExpantions = (byte)Packet.ReadByte();
            ClientType = (byte)Packet.ReadByte();
            OSType = (byte)Packet.ReadByte();
            TerrainOptions = (byte)Packet.ReadByte();
            Region = Packet.ReadShort();
            Unknown1 = Packet.ReadShort();
            Uptime = Packet.ReadInt();
            Stack1 = Packet.ReadIntLowEndian();
            Stack2 = Packet.ReadIntLowEndian();
            Stack3 = Packet.ReadIntLowEndian();
            Stack4 = Packet.ReadIntLowEndian();
        }
        
        protected CrashClientPacket() { }
    }
}
