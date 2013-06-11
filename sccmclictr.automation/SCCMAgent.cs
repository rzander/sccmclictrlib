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
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace sccmclictr.automation
{
    /// <summary>
    /// SCCMAgent Main Class
    /// </summary>
    public class SCCMAgent : IDisposable
    {
        private WSManConnectionInfo connectionInfo;
        private string Username { get; set; }
        private string Password { get; set; }
        private string Hostname { get; set; }
        private int WSManPort { get; set; }
        private Runspace remoteRunspace { get; set;}
        private bool ipcconnected { get; set; }

        public void Dispose()
        {
            try
            {
                if (isConnected)
                {
                    disconnect();
                }
            }
            catch { }

            connectionInfo = null;
            Username = null;
            Password = null;
            Hostname = null;

            if(Client != null)
                Client.Dispose();

            if (remoteRunspace != null)
                remoteRunspace.Dispose();
            


        }

        //private agentProperties oAgentProperties;
        //private softwareDistribution oSoftwareDistribution;
        
        public WSManConnectionInfo ConnectionInfo { get { return connectionInfo; } }

        /// <summary>
        /// Get the connected computername.
        /// </summary>
        public string TargetHostname
        {
            get 
            {
                try
                {
                    return ConnectionInfo.ComputerName;
                }
                catch
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// TraceSource for all PowerShell Command
        /// </summary>
        public TraceSource PSCode {get; set; }

        /// <summary>
        /// True = Session to remote Host is Open
        /// False = Session to remote Host is not Open
        /// </summary>
        public Boolean isConnected
        {
            get
            {
                try
                {
                    if (remoteRunspace.RunspaceStateInfo.State == RunspaceState.Opened)
                    {
                        return true;
                    }
                }
                catch { }

                return false;

            }
        }

        /// <summary>
        /// Connect to a remote SCCM Agent by using WSMan
        /// </summary>
        /// <param name="hostname">target computername</param>
        public SCCMAgent(string hostname)
        {
            initialize(hostname, null, null, 5985, true, false);
        }

        /// <summary>
        /// Connect to a remote SCCM Agent by using WSMan
        /// </summary>
        /// <param name="hostname">target computername</param>
        /// <param name="username">username for the connection</param>
        /// <param name="password">password for the connection</param>
        public SCCMAgent(string hostname, string username, string password)
        {
            initialize(hostname, username, password, 5985, true, false);
        }

        /// <summary>
        /// Connect to a remote SCCM Agent by using WSMan
        /// </summary>
        /// <param name="hostname">target computername</param>
        /// <param name="username">username for the connection</param>
        /// <param name="password">password for the connection</param>
        /// <param name="wsManPort">WSManagement Port (Default = 5985)</param>
        /// <param name="Connect">automatically connect after initializing</param>
        public SCCMAgent(string hostname, string username, string password, int wsManPort, bool Connect)
        {
            initialize(hostname, username, password, wsManPort, Connect, false);
        }

        public SCCMAgent(string hostname, string username, string password, int wsManPort, bool Connect, bool encryption)
        {
            initialize(hostname, username, password, wsManPort, Connect, encryption);
        }

        /// <summary>
        /// Connect to a remote SCCM Agent by using WSMan
        /// </summary>
        /// <param name="hostname">target computername</param>
        /// <param name="username">username for the connection</param>
        /// <param name="password">password for the connection</param>
        /// <param name="wsManPort">WSManagement Port (Default = 5985)</param>
        public SCCMAgent(string hostname, string username, string password, int wsManPort)
        {
            initialize(hostname, username, password, wsManPort, true, false);
        }

        /// <summary>
        /// Connect to a remote computer by using WSMan
        /// </summary>
        /// <param name="hostname">target computername</param>
        /// <param name="username">username for the connectio</param>
        /// <param name="password">password for the connection</param>
        /// <param name="wsManPort">WSManagement Port (Default = 5985)</param>
        /// <param name="doNotConnect">Only prepare the connection, connection must be initialized with 'reconnect'</param>
        protected void initialize(string hostname, string username, string password, int wsManPort, bool bConnect, bool Encryption)
        {
            Hostname = hostname;
            Username = username;
            Password = password;
            WSManPort = wsManPort;

            ipcconnected = false;

            PSCode = new TraceSource("PSCode");
            PSCode.Switch.Level = SourceLevels.All;

            if (string.IsNullOrEmpty(username))
            {
                if (!Encryption)
                    connectionInfo = new WSManConnectionInfo(new Uri(string.Format("http://{0}:{1}/wsman", hostname, wsManPort)));
                else
                    connectionInfo = new WSManConnectionInfo(new Uri(string.Format("https://{0}:{1}/wsman", hostname, wsManPort)));
                ipcconnected = true;
            }
            else
            {
                System.Security.SecureString secpassword = new System.Security.SecureString();
                foreach (char c in password.ToCharArray())
                {
                    secpassword.AppendChar(c);
                }
                PSCredential psc = new PSCredential(username, secpassword);
                if (!Encryption)
                    connectionInfo = new WSManConnectionInfo(new Uri(string.Format("http://{0}:{1}/wsman", hostname, wsManPort)), "http://schemas.microsoft.com/powershell/Microsoft.PowerShell", psc);
                else
                    connectionInfo = new WSManConnectionInfo(new Uri(string.Format("https://{0}:{1}/wsman", hostname, wsManPort)), "http://schemas.microsoft.com/powershell/Microsoft.PowerShell", psc);

            }

            //Default Settings
            connectionInfo.AuthenticationMechanism = AuthenticationMechanism.Default;
            connectionInfo.ProxyAuthentication = AuthenticationMechanism.Negotiate;

            if(bConnect)
                connect();

            //Initialzie connection
            //Client = new ccm(remoteRunspace, PSCode);
        }

        /// <summary>
        /// Re-Connect to a predefined connection
        /// </summary>
        /// <param name="ConnectionInfo">WSManConnectionInfo</param>
        public void connect()
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

                Client = new ccm(remoteRunspace, PSCode);
                //oAgentProperties = new agentProperties(remoteRunspace, PSCode);
                //oSoftwareDistribution = new softwareDistribution(remoteRunspace, PSCode);

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
        public void disconnect()
        {
            remoteRunspace.Close();
        }

        #region IPCConnect
        struct NETRESOURCE
        {
            internal int dwScope;
            internal int dwType;
            internal int dwDisplayType;
            internal int dwUsage;
            internal string LocalName;
            internal string RemoteName;
            internal string Comment;
            internal string Provider;
        }

        internal const int RESOURCETYPE_ANY = 0x0;

        /// <summary>
        /// Connect the IPC$ share on a remote computer to preauthenticate.
        /// </summary>
        /// <param name="Hostname"></param>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        internal int connectIPC(string Hostname, string UserName, string Password)
        {
            NETRESOURCE ConnInf = new NETRESOURCE();
            ConnInf.dwType = RESOURCETYPE_ANY;
            ConnInf.RemoteName = @"\\" + Hostname;
            IntPtr hWnd = IntPtr.Zero;
            return WNetAddConnection3(hWnd, ref ConnInf, Password, UserName, 0);
        }

        /// <summary>
        /// Connect the IPC$ Share if no integrated authentication is used
        /// </summary>
        public bool ConnectIPC
        {
            get { return ipcconnected; }
            set
            {
                if (value)
                {
                    try
                    {
                        //Check if a Username is defined
                        if (!string.IsNullOrEmpty(Username))
                        {
                            //Connect IPC$ share with username and password
                            connectIPC(Hostname, Username, Password);
                        }

                        ipcconnected = true;

                    }
                    catch
                    {
                        ipcconnected = false;
                    }
                }
                else
                {
                    //No option to disconnect at the moment
                }

            }
        

        }

        [DllImport("mpr.dll")]
        private static extern int WNetAddConnection3(IntPtr hWndOwner,
            ref NETRESOURCE lpNetResource, string lpPassword,
            string lpUserName, int dwFlags);

        #endregion

        public ccm Client;
        
    }
}
