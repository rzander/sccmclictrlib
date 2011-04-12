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

        public string AgentVersion
        {
            get 
            {
                return WSMan.RunPSScript("(Get-Wmiobject -class SMS_Client -namespace 'ROOT\\CCM').ClientVersion", remoteRunspace);
            }
        }

        public string LastBootTime
        {
            get
            {
                return WSMan.RunPSScript("$wmi = Get-WmiObject -Class Win32_OperatingSystem \n$a = New-TimeSpan $wmi.ConvertToDateTime($wmi.LastBootUpTime) $(Get-Date) \n$a.Days", remoteRunspace);

            }
        }
    }
}
