using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations
{
    public class VersionHandler
    {
        public List<String> vers;
        public String dllName;
        public String strDllVersion;
        public Version dllVersion;


        public VersionHandler()
        {

        }

        public VersionHandler(String verFullName)
        {
            vers = new List<String>();
            String[] arName = verFullName.Split(',');

            for (int i = 0; i < arName.Length; i++)
            {
                vers.Add(arName[i]);
            }
            if (arName.Length > 1)
            {
                dllName = arName[0].Trim();
                String dllFileVersion = arName[1].Trim();
                String[] dllver = dllFileVersion.Split('=');
                if (dllver.Length > 1)
                {
                    strDllVersion = dllver[1];
                    dllVersion = new Version(strDllVersion);
                }
            }
        }
    }

}
