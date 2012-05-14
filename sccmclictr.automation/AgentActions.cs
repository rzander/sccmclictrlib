//SCCM Client Center Automation Library (SCCMCliCtr.automation)
//Copyright (c) 2011 by Roger Zander

//This program is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or any later version. 
//This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details. 
//GNU General Public License: http://www.gnu.org/licenses/lgpl.html

#define CM2012
#define CM2007

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sccmclictr.automation;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Diagnostics;
using System.Web;


namespace sccmclictr.automation.functions
{

    public class agentactions: baseInit
    {
        internal Runspace remoteRunspace;
        internal TraceSource pSCode;
        internal ccm baseClient;
        
        public agentactions(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
            : base(RemoteRunspace, PSCode)
        {
            remoteRunspace = RemoteRunspace;
            pSCode = PSCode;
            baseClient = oClient;
        }

        /// <summary>
        /// Trigger Hardware-Inventory
        /// </summary>
        /// <param name="Full">Enforce a Full inventory (Default=Delta)</param>
        /// <returns>false=Error</returns>
        public bool HardwareInventory(Boolean Full)
        {
            try
            {
                base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000001}'");
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
