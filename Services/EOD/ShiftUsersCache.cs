/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;

namespace Microsoft.Dynamics.Retail.Pos.EOD
{
    /// <summary>
    /// Class encapsulates opened shift users cache.
    /// </summary>
    internal static class ShiftUsersCache
    {

        /// <summary>
        ///  Dictionary of Collection maintains the non-owener shift users assigned to a currenlty opened shift.
        ///  Shift ID => List of Non-owner shifts users
        /// </summary>
        private static IDictionary<string, ICollection<string>> shiftUsers = new Dictionary<string, ICollection<string>>();

        /// <summary>
        /// Adds the specified user to the shift.
        /// </summary>
        /// <param name="shift">The shift.</param>
        /// <param name="operatorId">The operator id.</param>
        public static void Add(IPosBatchStaging shift, string operatorId)
        {
            string shiftId = GetShiftId(shift);
            ICollection<string> userList = null;

            if (!shiftUsers.TryGetValue(shiftId, out userList))
            {
                userList = new Collection<string>();
                shiftUsers[shiftId] = userList;
            }

            if (!userList.Contains(operatorId))
                userList.Add(operatorId);
        }

        /// <summary>
        /// Determines whether [contains] [the specified shift].
        /// </summary>
        /// <param name="shift">The shift.</param>
        /// <param name="operatorId">The operator id.</param>
        /// <returns>
        ///   <c>true</c> if [contains] [the specified shift]; otherwise, <c>false</c>.
        /// </returns>
        public static bool Contains(IPosBatchStaging shift, string operatorId)
        {
            bool result = false;
            ICollection<string> userList = null;

            if (shiftUsers.TryGetValue(GetShiftId(shift), out userList)
                && userList.Contains(operatorId))
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Remove the specified shift from cache.
        /// </summary>
        /// <param name="shift">The shift.</param>
        public static void Remove(IPosBatchStaging shift)
        {
            shiftUsers.Remove(GetShiftId(shift));
        }

        /// <summary>
        /// Gets the shift id.
        /// </summary>
        /// <param name="shift">The shift.</param>
        /// <returns></returns>
        private static string GetShiftId(IPosBatchStaging shift)
        {
            return string.Format("{0}:{1}", shift.TerminalId, shift.BatchId);
        }
    }
}
