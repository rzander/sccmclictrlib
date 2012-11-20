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
    /// <summary>
    /// Software Update Class
    /// </summary>
    public class softwareupdates : baseInit
    {
        internal Runspace remoteRunspace;
        internal TraceSource pSCode;
        internal ccm baseClient;

        //Constructor
        public softwareupdates(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
            : base(RemoteRunspace, PSCode)
        {
            remoteRunspace = RemoteRunspace;
            pSCode = PSCode;
            baseClient = oClient;
        }

        /// <summary>
        /// Get all known Software Updates with status
        /// </summary>
        public List<CCM_UpdateStatus> UpdateStatus
        {
            get
            {
                List<CCM_UpdateStatus> lCache = new List<CCM_UpdateStatus>();
                List<PSObject> oObj = GetObjects(@"root\ccm\SoftwareUpdates\UpdatesStore", "SELECT * FROM CCM_UpdateStatus");
                foreach (PSObject PSObj in oObj)
                {
                    CCM_UpdateStatus oUpdStat = new CCM_UpdateStatus(PSObj, remoteRunspace, pSCode);

                    oUpdStat.remoteRunspace = remoteRunspace;
                    oUpdStat.pSCode = pSCode;
                    lCache.Add(oUpdStat);
                }
                return lCache;
            }
        }

        /// <summary>
        /// Get all mandatory Updates
        /// </summary>
        public List<CCM_TargetedUpdateEx1> TargetUpdates
        {
            get
            {
                List<CCM_TargetedUpdateEx1> lCache = new List<CCM_TargetedUpdateEx1>();
                List<PSObject> oObj = GetObjects(@"root\ccm\SoftwareUpdates\DeploymentAgent", "SELECT * FROM CCM_TargetedUpdateEx1");
                foreach (PSObject PSObj in oObj)
                {
                    //Get AppDTs sub Objects
                    CCM_TargetedUpdateEx1 oUpdStat = new CCM_TargetedUpdateEx1(PSObj, remoteRunspace, pSCode);

                    oUpdStat.remoteRunspace = remoteRunspace;
                    oUpdStat.pSCode = pSCode;
                    lCache.Add(oUpdStat);
                }
                return lCache;
            }
        }

        /// <summary>
        /// Get the Content Locations for Software Updates
        /// </summary>
        public List<CCM_UpdateSource> UpdateSource
        {
            get
            {
                List<CCM_UpdateSource> lCache = new List<CCM_UpdateSource>();
                List<PSObject> oObj = GetObjects(@"root\ccm\SoftwareUpdates\WUAHandler", "SELECT * FROM CCM_UpdateSource");
                foreach (PSObject PSObj in oObj)
                {
                    //Get AppDTs sub Objects
                    CCM_UpdateSource oUpdStat = new CCM_UpdateSource(PSObj, remoteRunspace, pSCode);

                    oUpdStat.remoteRunspace = remoteRunspace;
                    oUpdStat.pSCode = pSCode;
                    lCache.Add(oUpdStat);
                }
                return lCache;
            }
        }

        /// <summary>
        /// Show required Software Updates and the current state (from cache)
        /// </summary>
        public List<CCM_SoftwareUpdate> SoftwareUpdate
        {
            get
            {
                List<CCM_SoftwareUpdate> lCache = new List<CCM_SoftwareUpdate>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\ClientSDK", "SELECT * FROM CCM_SoftwareUpdate", false, new TimeSpan(0,0,10));
                foreach (PSObject PSObj in oObj)
                {
                    //Get AppDTs sub Objects
                    CCM_SoftwareUpdate oUpdStat = new CCM_SoftwareUpdate(PSObj, remoteRunspace, pSCode);

                    oUpdStat.remoteRunspace = remoteRunspace;
                    oUpdStat.pSCode = pSCode;
                    lCache.Add(oUpdStat);
                }
                return lCache;
            }
        }

        /// <summary>
        ///  Show required Software Updates and the current state (reload the cache)
        /// </summary>
        public List<CCM_SoftwareUpdate> SoftwareUpdateReload
        {
            get
            {
                List<CCM_SoftwareUpdate> lCache = new List<CCM_SoftwareUpdate>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\ClientSDK", "SELECT * FROM CCM_SoftwareUpdate", true);
                foreach (PSObject PSObj in oObj)
                {
                    //Get AppDTs sub Objects
                    CCM_SoftwareUpdate oUpdStat = new CCM_SoftwareUpdate(PSObj, remoteRunspace, pSCode);

                    oUpdStat.remoteRunspace = remoteRunspace;
                    oUpdStat.pSCode = pSCode;
                    lCache.Add(oUpdStat);
                }
                return lCache;
            }
        }

        /// <summary>
        /// Source:root\ccm\SoftwareUpdates\UpdatesStore
        /// </summary>
        public class CCM_UpdateStatus
        {
            //Constructor
            public CCM_UpdateStatus(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;

                this.Article = WMIObject.Properties["Article"].Value as string;
                this.Bulletin = WMIObject.Properties["Bulletin"].Value as string;
                this.Language = WMIObject.Properties["Language"].Value as string;
                this.ProductID = WMIObject.Properties["ProductID"].Value as string;
                this.RevisionNumber = WMIObject.Properties["RevisionNumber"].Value as UInt32?;
                string sScanTime = WMIObject.Properties["ScanTime"].Value as string;
                if (string.IsNullOrEmpty(sScanTime))
                    this.ScanTime = null;
                else
                    this.ScanTime = ManagementDateTimeConverter.ToDateTime(sScanTime) as DateTime?;

                //Not implemented
                this.Sources = null;

                this.SourceType = WMIObject.Properties["SourceType"].Value as UInt32?;
                this.SourceUniqueId = WMIObject.Properties["SourceUniqueId"].Value as string;
                this.SourceVersion = WMIObject.Properties["SourceVersion"].Value as UInt32?;
                this.Status = WMIObject.Properties["Status"].Value as string;
                this.Title = WMIObject.Properties["Title"].Value as string;
                this.UniqueId = WMIObject.Properties["UniqueId"].Value as string;

            }

            #region Properties
            internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode;

            public String Article { get; set; }
            public String Bulletin { get; set; }
            public String Language { get; set; }
            public String ProductID { get; set; }
            public UInt32? RevisionNumber { get; set; }
            public DateTime? ScanTime { get; set; }
            public CCM_SourceStatus[] Sources { get; set; }
            public UInt32? SourceType { get; set; }
            public String SourceUniqueId { get; set; }
            public UInt32? SourceVersion { get; set; }
            public String Status { get; set; }
            public String Title { get; set; }
            public String UniqueId { get; set; }

            #endregion

        }

        /// <summary>
        /// Source:root\ccm\SoftwareUpdates\UpdatesStore
        /// </summary>
        public class CCM_SourceStatus
        {
            //Constructor
            public CCM_SourceStatus(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.RevisionNumber = WMIObject.Properties["RevisionNumber"].Value as UInt32?;
                this.ScanTime = WMIObject.Properties["ScanTime"].Value as DateTime?;
                this.SourceType = WMIObject.Properties["SourceType"].Value as UInt32?;
                this.SourceUniqueId = WMIObject.Properties["SourceUniqueId"].Value as String;
                this.SourceVersion = WMIObject.Properties["SourceVersion"].Value as UInt32?;
                this.Status = WMIObject.Properties["Status"].Value as String;
            }

            #region Properties

            internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode;
            public UInt32? RevisionNumber { get; set; }
            public DateTime? ScanTime { get; set; }
            public UInt32? SourceType { get; set; }
            public String SourceUniqueId { get; set; }
            public UInt32? SourceVersion { get; set; }
            public String Status { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\SoftwareUpdates\DeploymentAgent
        /// </summary>
        public class CCM_AssignmentCompliance
        {
            //Constructor
            public CCM_AssignmentCompliance(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.AssignmentId = WMIObject.Properties["AssignmentId"].Value as String;
                this.IsCompliant = WMIObject.Properties["IsCompliant"].Value as Boolean?;
                this.Reserved = WMIObject.Properties["Reserved"].Value as String;
                this.Signature = WMIObject.Properties["Signature"].Value as String;
            }

            #region Properties

            internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode;
            public String AssignmentId { get; set; }
            public Boolean? IsCompliant { get; set; }
            public String Reserved { get; set; }
            public String Signature { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\SoftwareUpdates\DeploymentAgent
        /// </summary>
        public class CCM_AssignmentJobEx1
        {
            //Constructor
            public CCM_AssignmentJobEx1(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.AssignmentId = WMIObject.Properties["AssignmentId"].Value as String;
                this.AssignmentState = WMIObject.Properties["AssignmentState"].Value as UInt32?;
                this.AssignmentSubState = WMIObject.Properties["AssignmentSubState"].Value as UInt32?;
                this.AssignmentTrigger = WMIObject.Properties["AssignmentTrigger"].Value as UInt32?;
                this.JobId = WMIObject.Properties["JobId"].Value as String;
                this.ReEvaluateTrigger = WMIObject.Properties["ReEvaluateTrigger"].Value as UInt32?;
                this.Reserved = WMIObject.Properties["Reserved"].Value as String;
            }

            #region Properties

            internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode;
            public String AssignmentId { get; set; }
            public UInt32? AssignmentState { get; set; }
            public UInt32? AssignmentSubState { get; set; }
            public UInt32? AssignmentTrigger { get; set; }
            public String JobId { get; set; }
            public UInt32? ReEvaluateTrigger { get; set; }
            public String Reserved { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\SoftwareUpdates\DeploymentAgent
        /// </summary>
        public class CCM_ComponentState
        {
            //Constructor
            public CCM_ComponentState(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.ComponentName = WMIObject.Properties["ComponentName"].Value as String;
                this.MaxPauseDuration = WMIObject.Properties["MaxPauseDuration"].Value as UInt32?;
                this.PauseCookie = WMIObject.Properties["PauseCookie"].Value as UInt32?;
                string sPauseStartTime = WMIObject.Properties["PauseStartTime"].Value as string;
                if (string.IsNullOrEmpty(sPauseStartTime))
                    this.PauseStartTime = null;
                else
                    this.PauseStartTime = ManagementDateTimeConverter.ToDateTime(sPauseStartTime) as DateTime?;
                this.Reserved = WMIObject.Properties["Reserved"].Value as String;
            }

            #region Properties

            internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode;
            public String ComponentName { get; set; }
            public UInt32? MaxPauseDuration { get; set; }
            public UInt32? PauseCookie { get; set; }
            public DateTime? PauseStartTime { get; set; }
            public String Reserved { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\SoftwareUpdates\DeploymentAgent
        /// </summary>
        public class CCM_DeploymentTaskEx1
        {
            //Constructor
            public CCM_DeploymentTaskEx1(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.AssignmentId = WMIObject.Properties["AssignmentId"].Value as String;
                this.DetectJobId = WMIObject.Properties["DetectJobId"].Value as String;
                this.IsEnforcedInstall = WMIObject.Properties["IsEnforcedInstall"].Value as Boolean?;
                this.JobId = WMIObject.Properties["JobId"].Value as String;
                this.nKey = WMIObject.Properties["nKey"].Value as UInt32?;
                this.Reserved = WMIObject.Properties["Reserved"].Value as String;
            }

            #region Properties

            internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode;
            public String AssignmentId { get; set; }
            public String DetectJobId { get; set; }
            public Boolean? IsEnforcedInstall { get; set; }
            public String JobId { get; set; }
            public UInt32? nKey { get; set; }
            public String Reserved { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\SoftwareUpdates\DeploymentAgent
        /// </summary>
        public class CCM_SUMLocalSettings
        {
            //Constructor
            public CCM_SUMLocalSettings(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.nKey = WMIObject.Properties["nKey"].Value as UInt32?;
                this.Reserved = WMIObject.Properties["Reserved"].Value as String;
                this.UserExperience = WMIObject.Properties["UserExperience"].Value as UInt32?;
            }

            #region Properties

            internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode;
            public UInt32? nKey { get; set; }
            public String Reserved { get; set; }
            public UInt32? UserExperience { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\SoftwareUpdates\DeploymentAgent
        /// </summary>
        public class CCM_TargetedUpdateEx1
        {
            //Constructor
            public CCM_TargetedUpdateEx1(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                string sDeadline = WMIObject.Properties["Deadline"].Value as string;
                if (string.IsNullOrEmpty(sDeadline))
                    this.Deadline = null;
                else
                    this.Deadline = ManagementDateTimeConverter.ToDateTime(sDeadline) as DateTime?;
                this.DisableMomAlerts = WMIObject.Properties["DisableMomAlerts"].Value as Boolean?;
                this.DownloadSize = WMIObject.Properties["DownloadSize"].Value as UInt32?;
                this.DPLocality = WMIObject.Properties["DPLocality"].Value as UInt32?;
                this.IsDeleted = WMIObject.Properties["IsDeleted"].Value as Boolean?;
                this.KeepHidden = WMIObject.Properties["KeepHidden"].Value as Boolean?;
                this.LimitStateMessageVerbosity = WMIObject.Properties["LimitStateMessageVerbosity"].Value as Boolean?;
                this.OverrideServiceWindows = WMIObject.Properties["OverrideServiceWindows"].Value as Boolean?;
                this.PercentComplete = WMIObject.Properties["PercentComplete"].Value as UInt32?;
                this.RaiseMomAlertsOnFailure = WMIObject.Properties["RaiseMomAlertsOnFailure"].Value as Boolean?;
                string sRebootDeadline = WMIObject.Properties["RebootDeadline"].Value as string;
                if (string.IsNullOrEmpty(sRebootDeadline))
                    this.RebootDeadline = null;
                else
                    this.RebootDeadline = ManagementDateTimeConverter.ToDateTime(sRebootDeadline) as DateTime?;
                this.RebootOutsideOfServiceWindows = WMIObject.Properties["RebootOutsideOfServiceWindows"].Value as Boolean?;
                this.RefAssignments = WMIObject.Properties["RefAssignments"].Value as String;
                this.Reserved = WMIObject.Properties["Reserved"].Value as String;
                this.ScheduledUpdateOptions = WMIObject.Properties["ScheduledUpdateOptions"].Value as UInt32?;
                string sStartTime = WMIObject.Properties["StartTime"].Value as string;
                if (string.IsNullOrEmpty(sStartTime))
                    this.StartTime = null;
                else
                    this.StartTime = ManagementDateTimeConverter.ToDateTime(sStartTime) as DateTime?;
                this.UpdateAction = WMIObject.Properties["UpdateAction"].Value as UInt32?;
                this.UpdateApplicability = WMIObject.Properties["UpdateApplicability"].Value as UInt32?;
                this.UpdateId = WMIObject.Properties["UpdateId"].Value as String;
                this.UpdateState = WMIObject.Properties["UpdateState"].Value as UInt32?;
                this.UpdateStatus = WMIObject.Properties["UpdateStatus"].Value as UInt32?;
                this.UpdateVersion = WMIObject.Properties["UpdateVersion"].Value as String;
                this.UserUIExperience = WMIObject.Properties["UserUIExperience"].Value as Boolean?;
            }

            #region Properties

            internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode;

            public DateTime? Deadline { get; set; }
            public Boolean? DisableMomAlerts { get; set; }
            public UInt32? DownloadSize { get; set; }
            public UInt32? DPLocality { get; set; }
            public Boolean? IsDeleted { get; set; }
            public Boolean? KeepHidden { get; set; }
            public Boolean? LimitStateMessageVerbosity { get; set; }
            public Boolean? OverrideServiceWindows { get; set; }
            public UInt32? PercentComplete { get; set; }
            public Boolean? RaiseMomAlertsOnFailure { get; set; }
            public DateTime? RebootDeadline { get; set; }
            public Boolean? RebootOutsideOfServiceWindows { get; set; }
            public String RefAssignments { get; set; }
            public String Reserved { get; set; }
            public UInt32? ScheduledUpdateOptions { get; set; }
            public DateTime? StartTime { get; set; }
            public UInt32? UpdateAction { get; set; }
            public UInt32? UpdateApplicability { get; set; }
            public String UpdateId { get; set; }
            public UInt32? UpdateState { get; set; }
            public UInt32? UpdateStatus { get; set; }
            public String UpdateVersion { get; set; }
            public Boolean? UserUIExperience { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\SoftwareUpdates\Handler
        /// </summary>
        public class CCM_AtomicUpdateEx1
        {
            //Constructor
            public CCM_AtomicUpdateEx1(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.CachedContentInUse = WMIObject.Properties["CachedContentInUse"].Value as UInt32?;
                this.ContentPriority = WMIObject.Properties["ContentPriority"].Value as UInt32?;
                this.ContentRequestId = WMIObject.Properties["ContentRequestId"].Value as String;
                this.ExecutionRequestId = WMIObject.Properties["ExecutionRequestId"].Value as String;
                this.FailureDetail = WMIObject.Properties["FailureDetail"].Value as String;
                this.Reserved = WMIObject.Properties["Reserved"].Value as String;
                this.UpdateID = WMIObject.Properties["UpdateID"].Value as String;
                this.UpdateResult = WMIObject.Properties["UpdateResult"].Value as UInt32?;
                this.UpdateState = WMIObject.Properties["UpdateState"].Value as UInt32?;
                this.UpdateVersion = WMIObject.Properties["UpdateVersion"].Value as String;
            }

            #region Properties

            internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode;
            public UInt32? CachedContentInUse { get; set; }
            public UInt32? ContentPriority { get; set; }
            public String ContentRequestId { get; set; }
            public String ExecutionRequestId { get; set; }
            public String FailureDetail { get; set; }
            public String Reserved { get; set; }
            public String UpdateID { get; set; }
            public UInt32? UpdateResult { get; set; }
            public UInt32? UpdateState { get; set; }
            public String UpdateVersion { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\SoftwareUpdates\Handler
        /// </summary>
        public class CCM_BundledUpdateEx1
        {
            //Constructor
            public CCM_BundledUpdateEx1(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.CachedContentInUse = WMIObject.Properties["CachedContentInUse"].Value as UInt32?;
                this.ContentPriority = WMIObject.Properties["ContentPriority"].Value as UInt32?;
                this.ContentRequestId = WMIObject.Properties["ContentRequestId"].Value as String;
                this.ExecutionRequestId = WMIObject.Properties["ExecutionRequestId"].Value as String;
                this.FailureDetail = WMIObject.Properties["FailureDetail"].Value as String;
                this.IsSelfDownloadComplete = WMIObject.Properties["IsSelfDownloadComplete"].Value as Boolean?;
                this.IsSelfInstalling = WMIObject.Properties["IsSelfInstalling"].Value as Boolean?;
                this.RelatedUpdates = WMIObject.Properties["RelatedUpdates"].Value as String;
                this.Reserved = WMIObject.Properties["Reserved"].Value as String;
                this.UpdateID = WMIObject.Properties["UpdateID"].Value as String;
                this.UpdateResult = WMIObject.Properties["UpdateResult"].Value as UInt32?;
                this.UpdateState = WMIObject.Properties["UpdateState"].Value as UInt32?;
            }

            #region Properties

            internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode;
            public UInt32? CachedContentInUse { get; set; }
            public UInt32? ContentPriority { get; set; }
            public String ContentRequestId { get; set; }
            public String ExecutionRequestId { get; set; }
            public String FailureDetail { get; set; }
            public Boolean? IsSelfDownloadComplete { get; set; }
            public Boolean? IsSelfInstalling { get; set; }
            public String RelatedUpdates { get; set; }
            public String Reserved { get; set; }
            public String UpdateID { get; set; }
            public UInt32? UpdateResult { get; set; }
            public UInt32? UpdateState { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\SoftwareUpdates\Handler
        /// </summary>
        public class CCM_UpdatesDeploymentJobEx1
        {
            //Constructor
            public CCM_UpdatesDeploymentJobEx1(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.HandleForMTC = WMIObject.Properties["HandleForMTC"].Value as String;
                this.IsCompleted = WMIObject.Properties["IsCompleted"].Value as Boolean?;
                this.IsOwnerOfMTCTask = WMIObject.Properties["IsOwnerOfMTCTask"].Value as Boolean?;
                this.IsRequestSumbittedToMTC = WMIObject.Properties["IsRequestSumbittedToMTC"].Value as Boolean?;
                this.JobID = WMIObject.Properties["JobID"].Value as String;
                this.JobState = WMIObject.Properties["JobState"].Value as UInt32?;
                this.JobType = WMIObject.Properties["JobType"].Value as UInt32?;
                this.JobUpdates = WMIObject.Properties["JobUpdates"].Value as String;
                this.NotifyDownloadComplete = WMIObject.Properties["NotifyDownloadComplete"].Value as String;
                this.Reserved = WMIObject.Properties["Reserved"].Value as String;
                this.TaskPriorityForMTC = WMIObject.Properties["TaskPriorityForMTC"].Value as UInt32?;
            }

            #region Properties

            internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode;
            public String HandleForMTC { get; set; }
            public Boolean? IsCompleted { get; set; }
            public Boolean? IsOwnerOfMTCTask { get; set; }
            public Boolean? IsRequestSumbittedToMTC { get; set; }
            public String JobID { get; set; }
            public UInt32? JobState { get; set; }
            public UInt32? JobType { get; set; }
            public String JobUpdates { get; set; }
            public String NotifyDownloadComplete { get; set; }
            public String Reserved { get; set; }
            public UInt32? TaskPriorityForMTC { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\SoftwareUpdates\WUAHandler
        /// </summary>
        public class CCM_UpdateSource
        {
            //Constructor
            public CCM_UpdateSource(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;
                this.ContentLocation = WMIObject.Properties["ContentLocation"].Value as String;
                this.ContentType = WMIObject.Properties["ContentType"].Value as UInt32?;
                this.ContentVersion = WMIObject.Properties["ContentVersion"].Value as UInt32?;
                this.ServiceId = WMIObject.Properties["ServiceId"].Value as String;
                this.UniqueId = WMIObject.Properties["UniqueId"].Value as String;
            }

            #region Properties

            internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode;
            public String ContentLocation { get; set; }
            public UInt32? ContentType { get; set; }
            public UInt32? ContentVersion { get; set; }
            public String ServiceId { get; set; }
            public String UniqueId { get; set; }
            #endregion

        }

        /// <summary>
        /// Source:ROOT\ccm\ClientSDK
        /// </summary>
        public class CCM_SoftwareUpdate : softwaredistribution.CCM_SoftwareBase
        {
            internal baseInit oNewBase;

            //Constructor
            public CCM_SoftwareUpdate(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
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
                this.ArticleID = WMIObject.Properties["ArticleID"].Value as String;
                this.BulletinID = WMIObject.Properties["BulletinID"].Value as String;
                this.ComplianceState = WMIObject.Properties["ComplianceState"].Value as UInt32?;
                this.ExclusiveUpdate = WMIObject.Properties["ExclusiveUpdate"].Value as Boolean?;
                this.OverrideServiceWindows = WMIObject.Properties["OverrideServiceWindows"].Value as Boolean?;
                this.RebootOutsideServiceWindows = WMIObject.Properties["RebootOutsideServiceWindows"].Value as Boolean?;
                string sRestartDeadline = WMIObject.Properties["RestartDeadline"].Value as string;
                if (string.IsNullOrEmpty(sRestartDeadline))
                    this.RestartDeadline = null;
                else
                    this.RestartDeadline = ManagementDateTimeConverter.ToDateTime(sRestartDeadline) as DateTime?;
                this.UpdateID = WMIObject.Properties["UpdateID"].Value as String;
                this.URL = WMIObject.Properties["URL"].Value as String;
                this.UserUIExperience = WMIObject.Properties["UserUIExperience"].Value as Boolean?;
            }

            #region Properties

            /*internal string __CLASS { get; set; }
        internal string __NAMESPACE { get; set; }
        internal bool __INSTANCE { get; set; }
        internal string __RELPATH { get; set; }
        internal PSObject WMIObject { get; set; }
        internal Runspace remoteRunspace;
        internal TraceSource pSCode; */
            public String ArticleID { get; set; }
            public String BulletinID { get; set; }
            public UInt32? ComplianceState { get; set; }
            public Boolean? ExclusiveUpdate { get; set; }
            public Boolean? OverrideServiceWindows { get; set; }
            public Boolean? RebootOutsideServiceWindows { get; set; }
            public DateTime? RestartDeadline { get; set; }
            public String UpdateID { get; set; }
            public String URL { get; set; }
            public Boolean? UserUIExperience { get; set; }
            #endregion

            public void Install()
            {
                string sCode = string.Format("$a = get-wmiobject -query \"SELECT * FROM CCM_SoftwareUpdate WHERE UpdateID='{0}'\" -namespace \"ROOT\\ccm\\ClientSDK\";([wmiclass]'ROOT\\ccm\\ClientSDK:CCM_SoftwareUpdatesManager').InstallUpdates($a)", this.UpdateID);
                oNewBase.GetObjectsFromPS(sCode, true);
            }

        }

        /// <summary>
        /// Install all required updates with a deadline (mandatory).
        /// </summary>
        public void InstallAllRequiredUpdates()
        {
            string sCode = string.Format("([wmiclass]'ROOT\\ccm\\ClientSDK:CCM_SoftwareUpdatesManager').InstallUpdates()");
            baseClient.GetObjectsFromPS(sCode, true);
        }

        /// <summary>
        /// Install all approved updates even if they do not have a deadline.
        /// </summary>
        public void InstallAllApprovedUpdates()
        {
            string sCode = string.Format(@"([wmiclass]'ROOT\ccm\ClientSDK:CCM_SoftwareUpdatesManager').InstallUpdates([System.Management.ManagementObject[]] (get-wmiobject -query 'SELECT * FROM CCM_SoftwareUpdate' -namespace 'ROOT\ccm\ClientSDK'))");
            baseClient.GetObjectsFromPS(sCode, true);
        }

        //Install List of Updates
        public void InstallUpdates(List<CCM_SoftwareUpdate> Updates)
        {
            List<string> sUpdateIDs = new List<string>();
            sUpdateIDs = Updates.Select(t => t.UpdateID).ToList();

            string sIDs = string.Join("' OR UpdateID='", sUpdateIDs);

            string sCode = string.Format("[System.Management.ManagementObject[]] $a = get-wmiobject -query \"SELECT * FROM CCM_SoftwareUpdate WHERE UpdateID like '{0}'\" -namespace \"ROOT\\ccm\\ClientSDK\";([wmiclass]'ROOT\\ccm\\ClientSDK:CCM_SoftwareUpdatesManager').InstallUpdates($a)", sIDs);
            baseClient.GetObjectsFromPS(sCode, true);
        }

    }



}
