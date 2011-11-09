//SCCM Client Center Automation Library (SCCMCliCtr.automation)
//Copyright (c) 2011 by Roger Zander

//This program is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or any later version. 
//This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details. 
//GNU General Public License: http://www.gnu.org/licenses/lgpl.html

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
    public class agentproperties : baseInit
    {
        public agentproperties(Runspace RemoteRunspace, TraceSource PSCode)
            : base(RemoteRunspace, PSCode)
        {
        }

        /// <summary>
        /// Return the ActiveDirectory Site-Name (if exist).
        /// </summary>
        public string ADSiteName
        {
            get
            {
                return base.GetStringFromPS("$a=New-Object -comObject 'CPAPPLET.CPAppletMgr';($a.GetClientProperties() | Where-Object { $_.Name -eq 'ADSiteName' }).Value");
            }
        }

        /// <summary>
        /// Get/Set the option if an Administrator can Override Agent Settings from the ControlPanel Applet
        /// </summary>
        public Boolean AllowLocalAdminOverride
        {
            get
            {
                return bool.Parse(base.GetProperty(@"ROOT\ccm:SMS_Client=@", "AllowLocalAdminOverride"));
            }
            set
            {
                base.SetProperty(@"ROOT\ccm:SMS_Client=@", "AllowLocalAdminOverride", "$" + value.ToString());
            }
        }

        /// <summary>
        /// Return the SCCM Agent GUID
        /// </summary>
        public string ClientId
        {
            get
            {
                return base.GetProperty(@"ROOT\ccm:CCM_Client=@", "ClientId");
            }
        }

        /// <summary>
        /// Return the previous SCCM Agent GUID
        /// </summary>
        public string PreviousClientId
        {
            get
            {
                return base.GetProperty(@"ROOT\ccm:CCM_Client=@", "PreviousClientId");
            }
        }

        /// <summary>
        /// Return the full SCCM Agent ClientVersion
        /// </summary>
        public string ClientVersion
        {
            get
            {
                return base.GetProperty(@"ROOT\ccm:SMS_Client=@", "ClientVersion");
            }
        }

        /// <summary>
        /// Enable Site Code Auto Assignment on next Agent Restart
        /// </summary>
        public Boolean EnableAutoAssignment
        {
            get
            {
                return bool.Parse(base.GetProperty(@"ROOT\ccm:SMS_Client=@", "EnableAutoAssignment"));
            }
            set
            {
                base.SetProperty(@"ROOT\ccm:SMS_Client=@", "EnableAutoAssignment", "$" + value.ToString());
            }
        }


    }
}
