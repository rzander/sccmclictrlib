//SCCM Client Center Automation Library (SCCMCliCtr.automation)
//Copyright (c) 2011 by Roger Zander

//This program is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or any later version. 
//This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details. 
//GNU General Public License: http://www.gnu.org/licenses/lgpl.html


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Collections.ObjectModel;
using System.Threading;

namespace sccmclictr.automation
{
    static class WSMan
    {
        /// <summary>
        /// Connect a remote Runspace
        /// </summary>
        /// <param name="connectionInfo">WSManConnectionInfo</param>
        /// <param name="remoteRunspace">Reference to a Runspace</param>
        internal static void openRunspace(WSManConnectionInfo connectionInfo, ref Runspace remoteRunspace)
        {
            remoteRunspace = RunspaceFactory.CreateRunspace(connectionInfo);
            remoteRunspace.Open();
        }

        /// <summary>
        /// Run a PSScript
        /// </summary>
        /// <param name="scriptText"></param>
        /// <param name="remoteRunspace"></param>
        /// <returns></returns>
        internal static Collection<PSObject> RunPSScript(string scriptText, Runspace remoteRunspace)
        {
            try
            {
                using (PowerShell powershell = PowerShell.Create())
                {
                    powershell.Runspace = remoteRunspace;
                    powershell.AddScript(scriptText);
                    Collection<PSObject> PSresults = powershell.Invoke();
                    List<PSObject> loRes = PSresults.Where(t => t != null).ToList();
                    Collection<PSObject> results = new Collection<PSObject>();
                    foreach (PSObject po in loRes)
                    {
                        if (po != null)
                            results.Add(po);
                    }
                    return results;
                }
            }
            catch { }

            return null;
        }

        /// <summary>
        /// Run a PSScript and return the result as string
        /// </summary>
        /// <param name="scriptText"></param>
        /// <param name="remoteRunspace"></param>
        /// <returns></returns>
        internal static string RunPSScriptAsString(string scriptText, Runspace remoteRunspace)
        {
            StringBuilder stringBuilder = new StringBuilder();

            Collection<PSObject> results = RunPSScript(scriptText, remoteRunspace);

            foreach (PSObject obj in results)
            {
                try
                {
                    if (obj != null)
                    {
                        stringBuilder.AppendLine(obj.ToString());
                    }
                }
                catch { }
            }


            return stringBuilder.ToString();
        }

        private static void openRunspace(string uri, string schema, string username, string livePass, ref Runspace remoteRunspace)
        {
            System.Security.SecureString password = new System.Security.SecureString();
            foreach (char c in livePass.ToCharArray())
            {
                password.AppendChar(c);
            }

            PSCredential psc = new PSCredential(username, password);
            WSManConnectionInfo rri = new WSManConnectionInfo(new Uri(uri), schema, psc);
            rri.AuthenticationMechanism = AuthenticationMechanism.Kerberos;
            rri.ProxyAuthentication = AuthenticationMechanism.Negotiate;
            remoteRunspace = RunspaceFactory.CreateRunspace(rri);
            remoteRunspace.Open();
        }

        private static void openRunspace(string uri, ref Runspace remoteRunspace)
        {
            WSManConnectionInfo rri = new WSManConnectionInfo(new Uri(uri));
            rri.AuthenticationMechanism = AuthenticationMechanism.Kerberos;
            rri.ProxyAuthentication = AuthenticationMechanism.Negotiate;
            
            remoteRunspace = RunspaceFactory.CreateRunspace(rri);
            remoteRunspace.Open();
        }

        private static string RunScriptAsString(string scriptText, string servername, string username, string password)
        {
            return RunScriptAsString(scriptText, servername, username, password, 5985);
        }

        private static string RunScriptAsString(string scriptText, string servername, string username, string password, int port)
        {
            Runspace remoteRunspace = null;

            if (!string.IsNullOrEmpty(username))
            {
                openRunspace(string.Format("http://{0}:{1}/wsman", servername, port),
                    "http://schemas.microsoft.com/powershell/Microsoft.PowerShell",
                    @username, password, ref remoteRunspace);
            }
            else
            {
                openRunspace(string.Format("http://{0}:{1}/wsman", servername, port), ref remoteRunspace);
            }

            StringBuilder stringBuilder = new StringBuilder();

            using (PowerShell powershell = PowerShell.Create())
            {
                powershell.Runspace = remoteRunspace;
                powershell.AddScript(scriptText);
                powershell.Invoke();
                Collection<PSObject> results = powershell.Invoke();

                remoteRunspace.Close();

                foreach (PSObject obj in results)
                {
                    try
                    {
                        stringBuilder.AppendLine(obj.ToString());
                    }
                    catch { }
                }
            }

            return stringBuilder.ToString();
        }



    }
}
