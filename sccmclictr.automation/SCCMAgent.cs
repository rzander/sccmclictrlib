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

namespace sccmclictr.automation
{
    /// <summary>
    /// SCCMAgent Main Class
    /// </summary>
    public class SCCMAgent
    {
        private WSManConnectionInfo connectionInfo;
        private string Username { get; set; }
        private string Password { get; set; }
        private string Hostname { get; set; }
        private int WSManPort { get; set; }
        private Runspace remoteRunspace { get; set;}

        private agentProperties oAgentProperties;
        
        public WSManConnectionInfo ConnectionInfo { get { return connectionInfo; } }

        /// <summary>
        /// True = Session to remote Host is Open
        /// False = Session to remote Host is not Open
        /// </summary>
        public Boolean isConnected
        {
            get
            {
                if (remoteRunspace.RunspaceStateInfo.State == RunspaceState.Opened)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        /// <summary>
        /// Connect to a remote SCCM Agent by using WSMan
        /// </summary>
        /// <param name="hostname">target computername</param>
        public SCCMAgent(string hostname)
        {
            connect(hostname, null, null, 5985, false);
        }

        /// <summary>
        /// Connect to a remote SCCM Agent by using WSMan
        /// </summary>
        /// <param name="hostname">target computername</param>
        public SCCMAgent(string hostname, Boolean doNotConnect)
        {
            connect(hostname, null, null, 5985, doNotConnect);
        }

        /// <summary>
        /// Connect to a remote SCCM Agent by using WSMan
        /// </summary>
        /// <param name="hostname">target computername</param>
        /// <param name="username">username for the connection</param>
        /// <param name="password">password for the connection</param>
        public SCCMAgent(string hostname, string username, string password)
        {
            connect(hostname, username, password, 5985, false);
        }

        /// <summary>
        /// Connect to a remote SCCM Agent by using WSMan
        /// </summary>
        /// <param name="hostname">target computername</param>
        /// <param name="username">username for the connection</param>
        /// <param name="password">password for the connection</param>
        /// <param name="wsManPort">WSManagement Port (Default = 5985)</param>
        /// <param name="doNotConnect">Only prepare the connection, connection must be initialized with 'reconnect'</param>
        public SCCMAgent(string hostname, string username, string password, int wsManPort, Boolean doNotConnect)
        {
            connect(hostname, username, password, wsManPort, doNotConnect);
        }

        /// <summary>
        /// Connect to a remote computer by using WSMan
        /// </summary>
        /// <param name="hostname">target computername</param>
        /// <param name="username">username for the connectio</param>
        /// <param name="password">password for the connection</param>
        /// <param name="wsManPort">WSManagement Port (Default = 5985)</param>
        /// <param name="doNotConnect">Only prepare the connection, connection must be initialized with 'reconnect'</param>
        protected void connect(string hostname, string username, string password, int wsManPort, Boolean doNotConnect)
        {
            Hostname = hostname;
            Username = username;
            Password = password;
            WSManPort = wsManPort;

            if (string.IsNullOrEmpty(username))
            {
                connectionInfo = new WSManConnectionInfo(new Uri(string.Format("http://{0}:{1}/wsman", hostname, wsManPort)));
            }
            else
            {
                System.Security.SecureString secpassword = new System.Security.SecureString();
                foreach (char c in password.ToCharArray())
                {
                    secpassword.AppendChar(c);
                }
                PSCredential psc = new PSCredential(username, secpassword);
                connectionInfo = new WSManConnectionInfo(new Uri(string.Format("http://{0}:{1}/wsman", hostname, wsManPort)), "http://schemas.microsoft.com/powershell/Microsoft.PowerShell", psc);
            }

            connectionInfo.AuthenticationMechanism = AuthenticationMechanism.Default;
            connectionInfo.ProxyAuthentication = AuthenticationMechanism.Negotiate;

            //Do not connect if the doNotConnect flag is set...
            if (!doNotConnect)
            {
                //...otherweise connect.
                Runspace RemoteRunspace = null;
                
                //suppress exceptions
                try
                {
                    WSMan.openRunspace(connectionInfo, ref RemoteRunspace);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                //Check if connection was successful...
                if (RemoteRunspace.RunspaceStateInfo.State == RunspaceState.Opened)
                {
                    //Yes, return Runspace
                    remoteRunspace = RemoteRunspace;
                }
                else
                {
                    //No, throw an execption...
                    throw new Exception("Unable to connect");
                }

            }

            oAgentProperties = new agentProperties(remoteRunspace);

        }

        /// <summary>
        /// Re-Connect to a predefined connection
        /// </summary>
        /// <param name="ConnectionInfo">WSManConnectionInfo</param>
        public void reconnect()
        {
            Runspace RemoteRunspace = null;

            //suppress exceptions
            try
            {
                WSMan.openRunspace(connectionInfo, ref RemoteRunspace);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            //Check if connection was successful...
            if (RemoteRunspace.RunspaceStateInfo.State == RunspaceState.Opened)
            {
                //Yes, return Runspace
                remoteRunspace = RemoteRunspace;
            }
            else
            {
                //No, throw an execption...
                throw new Exception("Unable to connect");
            }
        }

        /// <summary>
        /// Disconnect an open connection
        /// </summary>
        public void diconnect()
        {
            remoteRunspace.Close();
        }


        public agentProperties AgentProperties { get { return oAgentProperties; } }
    }
}
