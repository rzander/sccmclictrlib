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
    /// SCCM Agent Properties
    /// </summary>
    public class agentProperties
    {
        private Runspace remoteRunspace { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="remoteRunspace"></param>
        public agentProperties(Runspace RemoteRunspace)
        {
            remoteRunspace = RemoteRunspace;
        }

        /// <summary>
        /// Return the full SCCM Agent ClientVersion
        /// </summary>
        public string ClientVersion
        {
            get 
            {
                return WSMan.RunPSScript("(Get-Wmiobject -class SMS_Client -namespace 'ROOT\\CCM').ClientVersion", remoteRunspace).Trim();
            }
        }

        /// <summary>
        /// Get/Set the option if an Administrator can Override Agent Settings from the ControlPanel Applet
        /// </summary>
        public Boolean AllowLocalAdminOverride
        {
            get 
            {
                return Boolean.Parse(WSMan.RunPSScript("(Get-Wmiobject -class SMS_Client -namespace 'ROOT\\CCM').AllowLocalAdminOverride", remoteRunspace));
            }
            set
            {
                WSMan.RunPSScript("$a = (Get-Wmiobject -class SMS_Client -namespace 'ROOT\\CCM');" +
                "$a.AllowLocalAdminOverride = $" + value.ToString() + ";" +
                "$a.Put()", remoteRunspace);
            }
        }

        /// <summary>
        /// Enable Site Code Auto Assignment on next Agent Restart
        /// </summary>
        public Boolean EnableAutoAssignment
        {
            get 
            {
                return Boolean.Parse(WSMan.RunPSScript("(Get-Wmiobject -class SMS_Client -namespace 'ROOT\\CCM').EnableAutoAssignment", remoteRunspace));
            }
            set
            {
                WSMan.RunPSScript("$a = (Get-Wmiobject -class SMS_Client -namespace 'ROOT\\CCM');" +
                "$a.EnableAutoAssignment = $" + value.ToString() + ";" +
                "$a.Put()", remoteRunspace);
            }
        }
        
        /// <summary>
        /// Get the Agent GUID
        /// </summary>
        public string ClientId
        {
            get 
            {
                return WSMan.RunPSScript("(Get-Wmiobject -class CCM_Client -namespace 'ROOT\\CCM').ClientId", remoteRunspace).Trim();
            }
        }

        /// <summary>
        /// Get the previous Agent GUID
        /// </summary>
        public string PreviousClientId
        {
            get
            {
                return WSMan.RunPSScript("(Get-Wmiobject -class CCM_Client -namespace 'ROOT\\CCM').PreviousClientId", remoteRunspace).Trim();
            }
        }

        /// <summary>
        /// Get last Agent GUID change date as string
        /// </summary>
        public string ClientIdChangeDate
        {
            get
            {
                return WSMan.RunPSScript("(Get-Wmiobject -class CCM_Client -namespace 'ROOT\\CCM').ClientIdChangeDate", remoteRunspace).Trim();
            }
        }

        /// <summary>
        /// Get the Client Version (SCCM2007 = 2.50); This function seems to be obsolete!!!
        /// </summary>
        public string ClientVersionEx
        {
            get
            {
                return WSMan.RunPSScript("(Get-Wmiobject -class CCM_Client -namespace 'ROOT\\CCM').ClientVersion", remoteRunspace).Trim();
            }
        }
        
        /// <summary>
        /// Get the Client Type
        /// </summary>
        public UInt32 ClientType
        {
            get
            {
                return UInt32.Parse(WSMan.RunPSScript("(Get-Wmiobject -class SMS_Client -namespace 'ROOT\\CCM').ClientType", remoteRunspace));
            }
        }
        
        /// <summary>
        /// Return the number of days where the system is up and running
        /// </summary>
        public string LastRebootDays
        {
            get
            {
                return WSMan.RunPSScript("$wmi = Get-WmiObject -Class Win32_OperatingSystem \n$a = New-TimeSpan $wmi.ConvertToDateTime($wmi.LastBootUpTime) $(Get-Date) \n$a.Days", remoteRunspace).Trim();

            }
        }
    }
}
