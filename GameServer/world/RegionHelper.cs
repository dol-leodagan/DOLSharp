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
using System.Reflection;

using log4net;

namespace DOL.GS
{
    /// <summary>
    /// Static Class for Region Helper Methods and Extensions
    /// </summary>
    public static class RegionHelper
    {
        /// <summary>
        /// Defines a logger for this class.
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Factory method to create regions.  Will create a region of data.ClassType, or default to Region if 
        /// an error occurs or ClassType is not specified
        /// </summary>
        /// <param name="time"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Region Create(GameTimer.TimeManager time, RegionData data)
        {
            try
            {
                Type t = typeof(Region);

                if (string.IsNullOrEmpty(data.ClassType) == false)
                {
                    t = Type.GetType(data.ClassType);

                    if (t == null)
                    {
                        t = ScriptMgr.GetType(data.ClassType);
                    }

                    if (t != null)
                    {
                        ConstructorInfo info = t.GetConstructor(new Type[] { typeof(GameTimer.TimeManager), typeof(RegionData) });

                        Region r = (Region)info.Invoke(new object[] { time, data });

                        if (r != null)
                        {
                            // Success with requested classtype
                            log.InfoFormat("Created Region {0} using ClassType '{1}'", r.ID, data.ClassType);
                            return r;
                        }

                        log.ErrorFormat("Failed to Invoke Region {0} using ClassType '{1}'", r.ID, data.ClassType);
                    }
                    else
                    {
                        log.ErrorFormat("Failed to find ClassType '{0}' for region {1}!", data.ClassType, data.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Failed to start region {0} with requested classtype: {1}.  Exception: {2}!", data.Id, data.ClassType, ex.Message);
            }

            // Create region using default type
            return new Region(time, data);
        }
    }
}
