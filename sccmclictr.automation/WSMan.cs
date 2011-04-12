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

        internal static string RunPSScript(string scriptText, Runspace remoteRunspace)
        {
            StringBuilder stringBuilder = new StringBuilder();

            using (PowerShell powershell = PowerShell.Create())
            {
                powershell.Runspace = remoteRunspace;
                powershell.AddScript(scriptText);
                powershell.Invoke();
                Collection<PSObject> results = powershell.Invoke();

                //remoteRunspace.Close();

                foreach (PSObject obj in results)
                {
                    stringBuilder.AppendLine(obj.ToString());
                }
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

        private static string RunScript(string scriptText, string servername, string username, string password)
        {
            return RunScript(scriptText, servername, username, password, 5985);
        }

        private static string RunScript(string scriptText, string servername, string username, string password, int port)
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
                //powershell.AddCommand("get-process");
                powershell.AddScript(scriptText);
                powershell.Invoke();
                Collection<PSObject> results = powershell.Invoke();

                remoteRunspace.Close();

                foreach (PSObject obj in results)
                {
                    stringBuilder.AppendLine(obj.ToString());
                }
            }

            return stringBuilder.ToString();
        }

    }
}
