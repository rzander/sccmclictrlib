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
using System.Management;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Diagnostics;
using System.Web;

namespace sccmclictr.automation.functions
{
    public class health : baseInit
    {
        internal Runspace remoteRunspace;
        internal TraceSource pSCode;
        internal ccm baseClient;

        //Constructor
        public health(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
            : base(RemoteRunspace, PSCode)
        {
            remoteRunspace = RemoteRunspace;
            pSCode = PSCode;
            baseClient = oClient;
        }

        /// <summary>
        /// Verify WMI Repository (winmgmt /verifyrepository)
        /// </summary>
        /// <returns>Command results as string</returns>
        public string WMIVerifyRepository()
        {
            TimeSpan toldCacheTime = base.cacheTime;

            //Cache for 30 seconds
            base.cacheTime = new TimeSpan(0, 0, 30);
            string sResult = base.GetStringFromPS("winmgmt /verifyrepository");
            base.cacheTime = toldCacheTime;

            return sResult;
        }

        /// <summary>
        /// Performs a consistency check on the WMI repository (winmgmt /salvagerepository)
        /// </summary>
        /// <returns>Command results as string</returns>
        public string WMISalvageRepository()
        {
            string sResult = base.GetStringFromPS("winmgmt /salvagerepository", true);

            return sResult;
        }

        /// <summary>
        /// The repository is reset to the initial state when the operating system is first installed (winmgmt /resetrepository)
        /// </summary>
        /// <returns>Command results as string</returns>
        public string WMIResetRepository()
        {
            string sResult = base.GetStringFromPS("winmgmt /resetrepository", true);

            return sResult;
        }
    }
}
