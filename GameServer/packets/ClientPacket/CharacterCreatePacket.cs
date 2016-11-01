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
using System.Linq;
using System.Collections.Generic;

namespace DOL.GS.ClientPacket
{
    public struct CharacterCreateData
    {
        public string CharacterName;
        public byte CustomMode;
        public byte EyeSize;
        public byte LipSize;
        public byte EyeColor;
        public byte HairColor;
        public byte FaceType;
        public byte HairStyle;
        public byte ExtensionData1;
        public byte ExtensionData2;
        public byte CustomizationStep;
        public byte MoodType;
        public byte NewGuildEmblem;
        public byte[] Unknown1;
        public uint Operation;
        public byte Unknown2;
        
        public string ZoneDescription;
        public string ClassName;
        public string RaceName;
        
        public byte Level;
        public byte Class;
        public byte Realm;
        public byte Data;
        public int Gender { get { return (Data >> 4) & 0x01; } }
        public int Race { get { return (Data & 0x0F) + ((Data & 0x40) >> 2); } }
        public bool SiStartLocation { get { return (Data >> 7) != 0; } }
        
        public ushort Model;
        public byte RegionID;
        public byte RegionID2;
        public uint DatabaseId;
        
        public byte StatStr;
        public byte StatDex;
        public byte StatCon;
        public byte StatQui;
        public byte StatInt;
        public byte StatPie;
        public byte StatEmp;
        public byte StatChr;
        
        public ushort[] ArmorModels;
        public ushort[] ArmorColors;
        public ushort[] WeaponModels;
        
        public byte ActiveRightSlot;
        public byte ActiveLeftSlot;
        public byte SiZone;

        public byte NewConstitution;
        
        public uint Unknown3;
    }
    
    /// <summary>
    /// Client Packet sent when creating a new character.
    /// </summary>
    public class CharacterCreatePacket : AbstractClientPacket
    {
        public virtual string AccountName { get; set; }
        public virtual CharacterCreateData[] Characters { get; set; }
        
        /// <summary>
        /// Create a new Instance of <see cref="CharacterCreatePacket"/> using Packet.
        /// </summary>
        /// <param name="Packet">GameServer Packet to Parse.</param>
        public CharacterCreatePacket(GSPacketIn Packet)
        {
            AccountName = Packet.ReadString(24);
            
            var CharacterData = new List<CharacterCreateData>();
            while (Packet.Position < Packet.Length)
            {
                var charData = new CharacterCreateData {
                    CharacterName = Packet.ReadString(24),
                    CustomMode = (byte)Packet.ReadByte(),
                    EyeSize = (byte)Packet.ReadByte(),
                    LipSize = (byte)Packet.ReadByte(),
                    EyeColor = (byte)Packet.ReadByte(),
                    HairColor = (byte)Packet.ReadByte(),
                    FaceType = (byte)Packet.ReadByte(),
                    HairStyle = (byte)Packet.ReadByte(),
                    ExtensionData1 = (byte)Packet.ReadByte(),
                    ExtensionData2 = (byte)Packet.ReadByte(),
                    CustomizationStep = (byte)Packet.ReadByte(),
                    MoodType = (byte)Packet.ReadByte(),
                    NewGuildEmblem = (byte)Packet.ReadByte(),
                    Unknown1 = Enumerable.Range(0, 7).Select(i => (byte)Packet.ReadByte()).ToArray(),
                    Operation = Packet.ReadInt(),
                    Unknown2 = (byte)Packet.ReadByte(),
                    
                    ZoneDescription = Packet.ReadString(24),
                    ClassName = Packet.ReadString(24),
                    RaceName = Packet.ReadString(24),
                    
                    Level = (byte)Packet.ReadByte(),
                    Class = (byte)Packet.ReadByte(),
                    Realm = (byte)Packet.ReadByte(),
                    Data = (byte)Packet.ReadByte(),
                    
                    Model = Packet.ReadShortLowEndian(),
                    RegionID = (byte)Packet.ReadByte(),
                    RegionID2 = (byte)Packet.ReadByte(),
                    DatabaseId = Packet.ReadInt(),

                    StatStr = (byte)Packet.ReadByte(),
                    StatDex = (byte)Packet.ReadByte(),
                    StatCon = (byte)Packet.ReadByte(),
                    StatQui = (byte)Packet.ReadByte(),
                    StatInt = (byte)Packet.ReadByte(),
                    StatPie = (byte)Packet.ReadByte(),
                    StatEmp = (byte)Packet.ReadByte(),
                    StatChr = (byte)Packet.ReadByte(),
                    
                    ArmorModels = Enumerable.Range(0, 8).Select(i => Packet.ReadShortLowEndian()).ToArray(),
                    ArmorColors = Enumerable.Range(0, 8).Select(i => Packet.ReadShortLowEndian()).ToArray(),
                    WeaponModels = Enumerable.Range(0, 4).Select(i => Packet.ReadShortLowEndian()).ToArray(),
                    
                    ActiveRightSlot = (byte)Packet.ReadByte(),
                    ActiveLeftSlot = (byte)Packet.ReadByte(),
                    SiZone = (byte)Packet.ReadByte(),
                    
                    NewConstitution = (byte)Packet.ReadByte(),
                };
                
                CharacterData.Add(charData);
            }
            
            Characters = CharacterData.ToArray();
        }
        
        protected CharacterCreatePacket() { }
    }
}
