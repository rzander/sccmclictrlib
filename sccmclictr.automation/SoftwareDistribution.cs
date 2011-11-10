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
    /// <summary>
    /// Template for an empty Class
    /// </summary>
    public class softwaredistribution : baseInit
    {
        //Constructor
        public softwaredistribution(Runspace RemoteRunspace, TraceSource PSCode) : base(RemoteRunspace, PSCode)
        {
        }

        public List<CCM_Application> Applications
        {
            get 
            {
                List<CCM_Application> lApps = new List<CCM_Application>();
                List<Object> oObj = GetObjects(@"ROOT\ccm\ClientSDK", "SELECT * FROM CCM_Application");

                return lApps;
            }
        }
    }

    public class CCM_SoftwareBase
    {
        #region Properties

        internal string __CLASS { get; set; }
        internal string __NAMESPACE { get; set; }
        internal bool __INSTANCE { get; set; }
        internal PSObject WMIObject { get; set; }

        public UInt32 ContentSize { get; set; }
        public DateTime Deadline { get; set; }
        public String Description { get; set; }
        public UInt32 ErrorCode { get; set; }
        public UInt32 EstimatedInstallTime { get; set; }
        public UInt32 EvaluationState { get; set; }
        public String FullName { get; set; }
        public String Name { get; set; }
        public DateTime NextUserScheduledTime { get; set; }
        public UInt32 PercentComplete { get; set; }
        public String Publisher { get; set; }
        public UInt32 Type { get; set; }
        #endregion

        #region Methods

        #endregion

        public CCM_SoftwareBase(PSObject WMIObject)
        {
            __INSTANCE = true;
        }
    }

    public class CCM_AppDeploymentType : CCM_SoftwareBase
    {
        #region Properties

        public String[] AllowedActions { get; set; }
        public String ApplicabilityState { get; set; }
        public String CurrentState { get; set; }
        public CCM_AppDeploymentType[] Dependencies { get; set; }
        public String DeploymentReport { get; set; }
        public String Id { get; set; }
        public DateTime LastEvalTime { get; set; }
        public String PostInstallAction { get; set; }
        public String ProgressState { get; set; }
        public String ResolvedState { get; set; }
        public UInt32 RetriesRemaining { get; set; }
        public String Revision { get; set; }
        #endregion

        #region Methods
        /*
        public UInt32 GetProperty(UInt32 LanguageId, String PropertyName, out String PropertyValue)
        {
            PropertyValue = "";
            return 0;
        }*/
        #endregion

        public CCM_AppDeploymentType(PSObject WMIObject) : base(WMIObject)
        { }
    }

    public class CCM_Application : CCM_SoftwareBase
    {
        #region Properties

        public String[] AllowedActions { get; set; }
        public CCM_AppDeploymentType[] AppDTs { get; set; }
        public String ApplicabilityState { get; set; }
        public String CurrentState { get; set; }
        public String DeploymentReport { get; set; }
        public UInt32 EnforcePreference { get; set; }
        public String FileTypes { get; set; }
        public String Icon { get; set; }
        public String Id { get; set; }
        public String InformativeUrl { get; set; }
        public Boolean IsMachineTarget { get; set; }
        public DateTime LastEvalTime { get; set; }
        public DateTime LastInstallTime { get; set; }
        public Boolean NotifyUser { get; set; }
        public String ProgressState { get; set; }
        public DateTime ReleaseDate { get; set; }
        public String ResolvedState { get; set; }
        public String Revision { get; set; }
        public String SoftwareVersion { get; set; }
        public Boolean UserUIExperience { get; set; }
        #endregion

        #region Methods
        /*
        public UInt32 GetProperty(UInt32 LanguageId, String PropertyName, out String PropertyValue)
        {
            return 0;
        }
        public UInt32 DownloadContents(String Id, Boolean IsMachineTarget, String Priority, String Revision, out String JobId)
        {
            return 0;
        }
        public UInt32 Install(UInt32 EnforcePreference, String Id, Boolean IsMachineTarget, Boolean IsRebootIfNeeded, String Priority, String Revision, out String JobId)
        {
            return 0;
        }
        public UInt32 Cancel(String Id, Boolean IsMachineTarget, String Revision)
        {
            return 0;
        }
        public UInt32 Uninstall(UInt32 EnforcePreference, String Id, Boolean IsMachineTarget, Boolean IsRebootIfNeeded, String Priority, String Revision, out String JobId)
        {
            return 0;
        }
        public UInt32 GetPendingComponentList(String AppDeliveryTypeId, UInt32 Revision, out String PendingComponentList)
        {
            return 0;
        } */
        #endregion

        public CCM_Application(PSObject WMIObject):base(WMIObject)
        {
            __INSTANCE = true;
        }
    }


}
