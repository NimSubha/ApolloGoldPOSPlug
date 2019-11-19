using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices.AccountManagement;

namespace Microsoft.Dynamics.Retail.Pos.CreateDatabaseService
{
    /// <summary>
    /// Utility class to create/verify the db access user group.
    /// </summary>
    internal static class UserUtility
    {
        /// <summary>
        /// Creates a local user group.
        /// </summary>
        /// <param name="groupName">The user group name.</param>
        /// <returns>Fully qualified group name that was created.</returns>
        public static string CreateUserGroup(string groupName, string groupDescription)
        {
            using (PrincipalContext context = new PrincipalContext(ContextType.Machine))
            {
                // create a local group if it doesn't already exist
                if (GroupPrincipal.FindByIdentity(context,IdentityType.Name, groupName) == null)
                {
                    using (GroupPrincipal group = new GroupPrincipal(context))
                    {
                        group.Name = groupName;
                        group.SamAccountName = groupName;

                        if (!string.IsNullOrWhiteSpace(groupDescription))
                        {
                            // setting this property is not supported. Need to figure out what is the right place to set this.
                            // group.Description = groupDescription;
                        }

                        group.Save();
                    }
                }
            }

            // if the group is not fully qualified, return as a fully qualified group name for the local machine.
            if (!groupName.Contains("\\"))
            {
                return string.Format("{0}\\{1}", Environment.MachineName, groupName);
            }
            else
            {
                return groupName;
            }
        }
    }
}
