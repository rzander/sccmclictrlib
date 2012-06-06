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
#if CM2012

    /// <summary>
    /// Template for an empty Class
    /// </summary>
    public class softwaredistribution : baseInit
    {
        internal Runspace remoteRunspace;
        internal TraceSource pSCode;
        internal ccm baseClient;

        //Constructor
        public softwaredistribution(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
            : base(RemoteRunspace, PSCode)
        {
            remoteRunspace = RemoteRunspace;
            pSCode = PSCode;
            baseClient = oClient;
        }

        public List<CCM_Application> Applications
        {
            get
            {
                List<CCM_Application> lApps = new List<CCM_Application>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\ClientSDK", "SELECT * FROM CCM_Application");
                foreach (PSObject PSObj in oObj)
                {
                    //Get AppDTs sub Objects
                    CCM_Application oApp = new CCM_Application(PSObj, remoteRunspace, pSCode);

                    oApp.remoteRunspace = remoteRunspace;
                    oApp.pSCode = pSCode;
                    lApps.Add(oApp);
                }
                return lApps;
            }
        }

        public List<REG_ExecutionHistory> ExecutionHistory
        {
            get
            {
                
                List<REG_ExecutionHistory> lExec = new List<REG_ExecutionHistory>();
                List<PSObject> oObj = new List<PSObject>();
                Boolean bisSCCM2012 = baseClient.AgentProperties.isSCCM2012;
                Boolean bisx64OS = true;
                
                //Only Get Architecture if SCCM < 2012
                if(!bisSCCM2012)
                    bisx64OS = baseClient.Inventory.isx64OS;
                
                if(bisSCCM2012)
                    oObj = GetObjectsFromPS("Get-ChildItem -path \"HKLM:\\SOFTWARE\\Microsoft\\SMS\\Mobile Client\\Software Distribution\\Execution History\\System\" -Recurse | % { get-itemproperty -path  $_.PsPath }", false, new TimeSpan(0, 0, 10));
                if (!bisSCCM2012 & bisx64OS)
                    oObj = GetObjectsFromPS("Get-ChildItem -path \"HKLM:\\SOFTWARE\\Wow6432Node\\Microsoft\\SMS\\Mobile Client\\Software Distribution\\Execution History\\System\" -Recurse | % { get-itemproperty -path  $_.PsPath }", false, new TimeSpan(0, 0, 10));
                if (!bisSCCM2012 & !bisx64OS)
                    oObj = GetObjectsFromPS("Get-ChildItem -path \"HKLM:\\SOFTWARE\\Microsoft\\SMS\\Mobile Client\\Software Distribution\\Execution History\\System\" -Recurse | % { get-itemproperty -path  $_.PsPath }", false, new TimeSpan(0, 0, 10));

                foreach (PSObject PSObj in oObj)
                {
                    //Get AppDTs sub Objects
                    REG_ExecutionHistory oReg = new REG_ExecutionHistory(PSObj, remoteRunspace, pSCode);

                    oReg.remoteRunspace = remoteRunspace;
                    oReg.pSCode = pSCode;
                    lExec.Add(oReg);
                }


                return lExec;
            }
        }

        public class CCM_SoftwareBase
        {
            #region Properties

            internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode;

            public UInt32? ContentSize { get; set; }
            public DateTime? Deadline { get; set; }
            public String Description { get; set; }
            public UInt32? ErrorCode { get; set; }
            public UInt32? EstimatedInstallTime { get; set; }
            public UInt32? EvaluationState { get; set; }
            public String FullName { get; set; }
            public String Name { get; set; }
            public DateTime? NextUserScheduledTime { get; set; }
            public UInt32? PercentComplete { get; set; }
            public String Publisher { get; set; }
            public UInt32? Type { get; set; }
            #endregion

            #region Methods

            #endregion

            public CCM_SoftwareBase(PSObject WMIObject)
            {
                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;

                this.ContentSize = WMIObject.Properties["ContentSize"].Value as uint?;

                string sDeadline = WMIObject.Properties["Deadline"].Value as string;
                if (string.IsNullOrEmpty(sDeadline))
                    this.Deadline = null;
                else
                    this.Deadline = ManagementDateTimeConverter.ToDateTime(sDeadline) as DateTime?;

                this.Description = WMIObject.Properties["Description"].Value as string;
                this.ErrorCode = WMIObject.Properties["ErrorCode"].Value as uint?;
                this.EstimatedInstallTime = WMIObject.Properties["EstimatedInstallTime"].Value as uint?;
                this.EvaluationState = WMIObject.Properties["EvaluationState"].Value as uint?;
                this.FullName = WMIObject.Properties["FullName"].Value as string;
                this.Name = WMIObject.Properties["Name"].Value as string;
                string sNextUserScheduledTime = WMIObject.Properties["NextUserScheduledTime"].Value as string;
                if (string.IsNullOrEmpty(sNextUserScheduledTime))
                    this.NextUserScheduledTime = null;
                else
                    this.NextUserScheduledTime = ManagementDateTimeConverter.ToDateTime(sNextUserScheduledTime) as DateTime?;
                this.PercentComplete = WMIObject.Properties["PercentComplete"].Value as uint?;
                this.Publisher = WMIObject.Properties["Publisher"].Value as string;
                this.Type = WMIObject.Properties["Type"].Value as uint?;
            }
        }

        public class CCM_AppDeploymentType : CCM_SoftwareBase
        {
            #region Properties

            public String[] AllowedActions { get; set; }
            public String ApplicabilityState { get; set; }
            public String CurrentState { get; set; }

            public CCM_AppDeploymentType[] Dependencies { get; set; }
            /*{
                get
                {
                    //Get Dependencies sub Objects
                    List<CCM_AppDeploymentType> lTypes = new List<CCM_AppDeploymentType>();

                    baseInit oNewBase = new baseInit(remoteRunspace, pSCode);

                    List<PSObject> lPSAppDts = oNewBase.GetProperties(@"ROOT\ccm\clientsdk:" + this.__RELPATH, "Dependencies");
                    List<CCM_AppDeploymentType> lAppDTs = new List<CCM_AppDeploymentType>();
                    foreach (PSObject pAppDT in lPSAppDts)
                    {
                        lAppDTs.Add(new CCM_AppDeploymentType(pAppDT));
                    }
                    return lAppDTs.ToArray();
                }
                set
                {
                    Dependencies = value;
                }
            } */

            public String DeploymentReport { get; set; }
            public String Id { get; set; }
            public DateTime? LastEvalTime { get; set; }
            public String PostInstallAction { get; set; }
            public String ProgressState { get; set; }
            public String ResolvedState { get; set; }
            public UInt32? RetriesRemaining { get; set; }
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

            public CCM_AppDeploymentType(PSObject WMIObject)
                : base(WMIObject)
            {

                this.WMIObject = WMIObject;

                this.AllowedActions = WMIObject.Properties["AllowedActions"].Value as string[];
                this.ApplicabilityState = WMIObject.Properties["ApplicabilityState"].Value as string;
                this.CurrentState = WMIObject.Properties["CurrentState"].Value as string;
                this.Dependencies = WMIObject.Properties["Dependencies"].Value as CCM_AppDeploymentType[];
                this.DeploymentReport = WMIObject.Properties["DeploymentReport"].Value as string;
                this.Id = WMIObject.Properties["Id"].Value as string;

                string sLastEvalTime = WMIObject.Properties["LastEvalTime"].Value as string;
                if (string.IsNullOrEmpty(sLastEvalTime))
                    this.LastEvalTime = null;
                else
                    this.LastEvalTime = ManagementDateTimeConverter.ToDateTime(sLastEvalTime) as DateTime?;

                this.PostInstallAction = WMIObject.Properties["PostInstallAction"].Value as string;
                this.ProgressState = WMIObject.Properties["ProgressState"].Value as string;
                this.ResolvedState = WMIObject.Properties["ResolvedState"].Value as string;
                this.RetriesRemaining = WMIObject.Properties["RetriesRemaining"].Value as uint?;
                this.Revision = WMIObject.Properties["Revision"].Value as string;

            }

        }

        public class CCM_Application : CCM_SoftwareBase
        {
            internal baseInit oNewBase;

            #region Properties
            public String[] AllowedActions { get; set; }
            public String ApplicabilityState { get; set; }
            public String CurrentState { get; set; }
            public String DeploymentReport { get; set; }
            public UInt32? EnforcePreference { get; set; }
            public String FileTypes { get; set; }
            public String Icon { get; set; }
            public String Id { get; set; }
            public String InformativeUrl { get; set; }
            public Boolean? IsMachineTarget { get; set; }
            public DateTime? LastEvalTime { get; set; }
            public DateTime? LastInstallTime { get; set; }
            public Boolean? NotifyUser { get; set; }
            public String ProgressState { get; set; }
            public DateTime? ReleaseDate { get; set; }
            public String ResolvedState { get; set; }
            public String Revision { get; set; }
            public String SoftwareVersion { get; set; }
            public Boolean? UserUIExperience { get; set; }

            public CCM_AppDeploymentType[] AppDTs
            {
                get
                {
                    //Get AppDTs sub Objects
                    List<CCM_AppDeploymentType> lAppDTs = new List<CCM_AppDeploymentType>();

                    if (remoteRunspace != null)
                    {
                        List<PSObject> lPSAppDts = oNewBase.GetProperties(@"ROOT\ccm\clientsdk:" + this.__RELPATH, "AppDTs");

                        foreach (PSObject pAppDT in lPSAppDts)
                        {
                            lAppDTs.Add(new CCM_AppDeploymentType(pAppDT));
                        }
                    }
                    return lAppDTs.ToArray();
                }
                set
                {
                    AppDTs = value;
                }
            }
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

            public string Install(string Priority, bool isRebootIfNeeded)
            {
                string sJobID = "";
                PSObject oResult = oNewBase.CallClassMethod("ROOT\\ccm\\ClientSdk:CCM_Application", "Install", "'" + Id + "', " + Revision + ", $" + IsMachineTarget.ToString() + ", " + EnforcePreference.ToString() + ", " + Priority + ", $" + isRebootIfNeeded.ToString());
                //sJobID = oResult.Properties["JobID"].Value.ToString();
                return sJobID;
            }

            public string Uninstall(string Priority, bool isRebootIfNeeded)
            {
                string sJobID = "";
                PSObject oResult = oNewBase.CallClassMethod("ROOT\\ccm\\ClientSdk:CCM_Application", "Uninstall", "'" + Id + "', " + Revision + ", $" + IsMachineTarget.ToString() + ", " + EnforcePreference.ToString() + ", " + Priority + ", $" + isRebootIfNeeded.ToString());
                //sJobID = oResult.Properties["JobID"].Value.ToString();
                return sJobID;
            }

            #endregion

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="WMIObject"></param>
            public CCM_Application(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
                : base(WMIObject)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;
                oNewBase = new baseInit(remoteRunspace, pSCode);

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;

                List<string> lActions = new List<string>();
                foreach (string sAction in ((WMIObject.Properties["AllowedActions"].Value as PSObject).BaseObject as System.Collections.ArrayList))
                {
                    lActions.Add(sAction);
                }

                this.AllowedActions = lActions.ToArray();
                this.ApplicabilityState = WMIObject.Properties["ApplicabilityState"].Value as string;
                this.CurrentState = WMIObject.Properties["CurrentState"].Value as string;
                this.DeploymentReport = WMIObject.Properties["DeploymentReport"].Value as string;
                this.EnforcePreference = WMIObject.Properties["EnforcePreference"].Value as uint?;
                this.FileTypes = WMIObject.Properties["FileTypes"].Value as string;
                this.Icon = WMIObject.Properties["Icon"].Value as string;
                this.Id = WMIObject.Properties["Id"].Value as string;
                this.InformativeUrl = WMIObject.Properties["InformativeUrl"].Value as string;
                this.IsMachineTarget = WMIObject.Properties["IsMachineTarget"].Value as bool?;

                string sLastEvalTime = WMIObject.Properties["LastEvalTime"].Value as string;
                if (string.IsNullOrEmpty(sLastEvalTime))
                    this.LastEvalTime = null;
                else
                    this.LastEvalTime = ManagementDateTimeConverter.ToDateTime(sLastEvalTime) as DateTime?;

                string sLastInstallTime = WMIObject.Properties["LastInstallTime"].Value as string;
                if (string.IsNullOrEmpty(sLastInstallTime))
                    this.LastInstallTime = null;
                else
                    this.LastInstallTime = ManagementDateTimeConverter.ToDateTime(sLastInstallTime) as DateTime?;

                this.NotifyUser = WMIObject.Properties["NotifyUser"].Value as bool?;
                this.ProgressState = WMIObject.Properties["ProgressState"].Value as string;

                string sReleaseDate = WMIObject.Properties["ReleaseDate"].Value as string;
                if (string.IsNullOrEmpty(sReleaseDate))
                    this.ReleaseDate = null;
                else
                    this.ReleaseDate = ManagementDateTimeConverter.ToDateTime(sReleaseDate) as DateTime?;

                this.ResolvedState = WMIObject.Properties["ResolvedState"].Value as string;
                this.Revision = WMIObject.Properties["Revision"].Value as string;
                this.SoftwareVersion = WMIObject.Properties["SoftwareVersion"].Value as string;
                this.UserUIExperience = WMIObject.Properties["UserUIExperience"].Value as bool?;
            }
        }

        public class REG_ExecutionHistory
        {
            internal baseInit oNewBase;

            internal string __RegPATH { get; set; }
            internal PSObject RegObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode;

            public String _ProgramID { get; set; }
            public String _State { get; set; }
            public DateTime? _RunStartTime { get; set; }
            public int? SuccessOrFailureCode { get; set; }
            public string SuccessOrFailureReason { get; set; }
            public string UserID { get; set; }
            public string PackageID { get; set; }

            public REG_ExecutionHistory(PSObject RegObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;
                oNewBase = new baseInit(remoteRunspace, pSCode);

                this.__RegPATH = (RegObject.Properties["PSPath"].Value as string).Replace("Microsoft.PowerShell.Core\\Registry::", "");
                this._ProgramID = RegObject.Properties["_ProgramID"].Value as string;
                this._State = RegObject.Properties["_State"].Value as string;
                this._RunStartTime = DateTime.Parse(RegObject.Properties["_RunStartTime"].Value as string);

                string sSuccessOrFailureCode = RegObject.Properties["SuccessOrFailureCode"].Value as string;
                if (!string.IsNullOrEmpty(sSuccessOrFailureCode))
                {
                    this.SuccessOrFailureCode = int.Parse(RegObject.Properties["SuccessOrFailureCode"].Value as string);
                }

                string sSuccessOrFailureReason = RegObject.Properties["SuccessOrFailureReason"].Value as string;
                if (!string.IsNullOrEmpty(sSuccessOrFailureReason))
                {
                    this.SuccessOrFailureReason = RegObject.Properties["SuccessOrFailureReason"].Value as string;
                }

                this.UserID = __RegPATH.Substring(__RegPATH.IndexOf("Execution History", StringComparison.CurrentCultureIgnoreCase)).Split('\\')[1];
                this.PackageID = __RegPATH.Substring(__RegPATH.IndexOf(UserID, StringComparison.CurrentCultureIgnoreCase)).Split('\\')[1];

            }

            /// <summary>
            /// Delete the Execution-History Item
            /// </summary>
            public void Delete()
            {
                try
                {
                    string sReg = __RegPATH.Replace(@"HKEY_LOCAL_MACHINE\", @"HKLM:\");
                    oNewBase.GetObjectsFromPS("Remove-Item \"" + sReg + "\" -Recurse", true, new TimeSpan(0, 0, 1));
                }
                catch { }
            }

        }
    }



#endif
}
