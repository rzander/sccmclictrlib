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
    /// Template for an empty Class
    /// </summary>
    public class services : baseInit
    {
        internal Runspace remoteRunspace;
        internal TraceSource pSCode;
        internal ccm baseClient;

        //Constructor
        public services(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
            : base(RemoteRunspace, PSCode)
        {
            remoteRunspace = RemoteRunspace;
            pSCode = PSCode;
            baseClient = oClient;
        }

        internal List<Win32_Service> win32_Services
        {
            get
            {
                List<Win32_Service> lCache = new List<Win32_Service>();
                List<PSObject> oObj = GetObjects(@"ROOT\cimv2", "SELECT * FROM Win32_Service", false);
                foreach (PSObject PSObj in oObj)
                {
                    //Get AppDTs sub Objects
                    Win32_Service oCIEx = new Win32_Service(PSObj, remoteRunspace, pSCode);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                return lCache;
            }

            set { }

        }

        /// <summary>
        /// Get a List of all Services
        /// </summary>
        public List<Win32_Service> Win32_Services
        {
            get
            {
                return win32_Services;
            }
        }

        public void Reload()
        {
            List<Win32_Service> lCache = new List<Win32_Service>();
            List<PSObject> oObj = GetObjects(@"ROOT\cimv2", "SELECT * FROM Win32_Service", true);
            foreach (PSObject PSObj in oObj)
            {
                Win32_Service oCIEx = new Win32_Service(PSObj, remoteRunspace, pSCode);

                oCIEx.remoteRunspace = remoteRunspace;
                oCIEx.pSCode = pSCode;
                lCache.Add(oCIEx);
            }
            win32_Services = lCache;
        }

        /// <summary>
        /// Get a single Service Instance
        /// </summary>
        /// <param name="ServiceName"></param>
        /// <returns></returns>
        public Win32_Service GetService(string ServiceName)
        {
            //Remove cached results
            string sHash1 = base.CreateHash(@"ROOT\cimv2" + string.Format("SELECT * FROM Win32_Service WHERE Name ='{0}'", ServiceName));
            base.Cache.Remove(sHash1);

            List<PSObject> oObj = GetObjects(@"ROOT\cimv2", string.Format("SELECT * FROM Win32_Service WHERE Name ='{0}'", ServiceName));
            foreach (PSObject PSObj in oObj)
            {
                Win32_Service oCIEx = new Win32_Service(PSObj, remoteRunspace, pSCode);

                oCIEx.remoteRunspace = remoteRunspace;
                oCIEx.pSCode = pSCode;
                return oCIEx;
            }

            return null;
        }

    }



    /// <summary>
    /// Source:ROOT\CIMV2
    /// </summary>
    public class Win32_Service : Win32_BaseService
    {
        //Constructor
        public Win32_Service(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            : base(WMIObject, RemoteRunspace, PSCode)
        {
            remoteRunspace = RemoteRunspace;
            pSCode = PSCode;

            oNewBase = new baseInit(remoteRunspace, pSCode);

            this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
            this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
            this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
            this.__INSTANCE = true;
            this.WMIObject = WMIObject;
            this.CheckPoint = WMIObject.Properties["CheckPoint"].Value as UInt32?;
            this.ProcessId = WMIObject.Properties["ProcessId"].Value as UInt32?;
            this.WaitHint = WMIObject.Properties["WaitHint"].Value as UInt32?;
        }

        internal baseInit oNewBase;

        #region Properties

        //internal string __CLASS { get; set; }
        //internal string __NAMESPACE { get; set; }
        //internal bool __INSTANCE { get; set; }
        //internal string __RELPATH { get; set; }
        //internal PSObject WMIObject { get; set; }
        //internal Runspace remoteRunspace;
        //internal TraceSource pSCode;
        public UInt32? CheckPoint { get; set; }
        public UInt32? ProcessId { get; set; }
        public UInt32? WaitHint { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// Start the Service and wait until it's started
        /// </summary>
        /// <returns>0</returns>
        public UInt32 StartService()
        {
            //Remove cached results
            string sHash1 = oNewBase.CreateHash(string.Format("(Get-Service '{0}').Start()", base.Name));
            oNewBase.Cache.Remove(sHash1);
            string sHash2 = oNewBase.CreateHash(string.Format("(Get-Service '{0}').Status", base.Name));
            oNewBase.Cache.Remove(sHash2);

            oNewBase.GetStringFromPS(string.Format("(Get-Service '{0}').Start()", base.Name));
            base.State = oNewBase.GetStringFromPS(string.Format("(Get-Service '{0}').Status", base.Name));

            //Remove cached results
            oNewBase.Cache.Remove(sHash1);
            oNewBase.Cache.Remove(sHash2);
            
            return 0;
        }
        public UInt32 StopService()
        {
            //Remove cached results
            string sHash1 = oNewBase.CreateHash(string.Format("(Get-Service '{0}').Stop()", base.Name));
            oNewBase.Cache.Remove(sHash1);
            string sHash2 = oNewBase.CreateHash(string.Format("(Get-Service '{0}').Status", base.Name));
            oNewBase.Cache.Remove(sHash2);

            oNewBase.GetStringFromPS(string.Format("(Get-Service '{0}').Stop()", base.Name));
            base.State = oNewBase.GetStringFromPS(string.Format("(Get-Service '{0}').Status", base.Name));

            //Remove cached results
            oNewBase.Cache.Remove(sHash1);
            oNewBase.Cache.Remove(sHash2);

            return 0;
        }
        public UInt32 RestartService()
        {
            //Remove cached results
            string sHash1 = oNewBase.CreateHash(string.Format("Restart-Service '{0}'", base.Name));
            oNewBase.Cache.Remove(sHash1);
            string sHash2 = oNewBase.CreateHash(string.Format("(Get-Service '{0}').Status", base.Name));
            oNewBase.Cache.Remove(sHash2);

            oNewBase.GetStringFromPS(string.Format("Restart-Service '{0}'", base.Name));
            base.State = oNewBase.GetStringFromPS(string.Format("(Get-Service '{0}').Status", base.Name));

            //Remove cached results
            oNewBase.Cache.Remove(sHash1);
            oNewBase.Cache.Remove(sHash2);

            return 0;
        }
        public UInt32 PauseService()
        {
            //Remove cached results
            string sHash1 = oNewBase.CreateHash(string.Format("(Get-Service '{0}').Pause()", base.Name));
            oNewBase.Cache.Remove(sHash1);
            string sHash2 = oNewBase.CreateHash(string.Format("(Get-Service '{0}').Status", base.Name));
            oNewBase.Cache.Remove(sHash2);

            oNewBase.GetStringFromPS(string.Format("(Get-Service '{0}').Pause()", base.Name));
            base.State = oNewBase.GetStringFromPS(string.Format("(Get-Service '{0}').Status", base.Name));

            //Remove cached results
            oNewBase.Cache.Remove(sHash1);
            oNewBase.Cache.Remove(sHash2);

            return 0;
        }
        public UInt32 ResumeService()
        {
            //Remove cached results
            string sHash1 = oNewBase.CreateHash(string.Format("(Get-Service '{0}').Start()", base.Name));
            oNewBase.Cache.Remove(sHash1);
            string sHash2 = oNewBase.CreateHash(string.Format("(Get-Service '{0}').Status", base.Name));
            oNewBase.Cache.Remove(sHash2);

            oNewBase.GetStringFromPS(string.Format("(Get-Service '{0}').Start()", base.Name));
            base.State = oNewBase.GetStringFromPS(string.Format("(Get-Service '{0}').Status", base.Name));

            //Remove cached results
            oNewBase.Cache.Remove(sHash1);
            oNewBase.Cache.Remove(sHash2);

            return 0;
        }
        /*
        public UInt32 InterrogateService()
        {
            return 0;
        }
        public UInt32 UserControlService(int ControlCode)
        {
            return 0;
        }
        public UInt32 Create(Boolean DesktopInteract, String DisplayName, int ErrorControl, String LoadOrderGroup, String LoadOrderGroupDependencies, String Name, String PathName, String ServiceDependencies, int ServiceType, String StartMode, String StartName, String StartPassword)
        {
            return 0;
        }
        public UInt32 Change(Boolean DesktopInteract, String DisplayName, int ErrorControl, String LoadOrderGroup, String LoadOrderGroupDependencies, String PathName, String ServiceDependencies, int ServiceType, String StartMode, String StartName, String StartPassword)
        {
            return 0;
        }
        public UInt32 ChangeStartMode(String StartMode)
        {
            return 0;
        }
        public UInt32 Delete()
        {
            return 0;
        }
        public UInt32 GetSecurityDescriptor(out Object Descriptor)
        {
            Descriptor = new object();
            return 0;
        }
        public UInt32 SetSecurityDescriptor(Object Descriptor)
        {
            return 0;
        }
         **/
        #endregion
    }

    /// <summary>
    /// Source:ROOT\CIMV2
    /// </summary>
    public class Win32_BaseService : CIM_Service
    {
        //Constructor
        public Win32_BaseService(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            : base(WMIObject, RemoteRunspace, PSCode)
        {
            remoteRunspace = RemoteRunspace;
            pSCode = PSCode;

            this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
            this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
            this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
            this.__INSTANCE = true;
            this.WMIObject = WMIObject;
            this.AcceptPause = WMIObject.Properties["AcceptPause"].Value as Boolean?;
            this.AcceptStop = WMIObject.Properties["AcceptStop"].Value as Boolean?;
            this.DesktopInteract = WMIObject.Properties["DesktopInteract"].Value as Boolean?;
            this.DisplayName = WMIObject.Properties["DisplayName"].Value as String;
            this.ErrorControl = WMIObject.Properties["ErrorControl"].Value as String;
            this.ExitCode = WMIObject.Properties["ExitCode"].Value as UInt32?;
            this.PathName = WMIObject.Properties["PathName"].Value as String;
            this.ServiceSpecificExitCode = WMIObject.Properties["ServiceSpecificExitCode"].Value as UInt32?;
            this.ServiceType = WMIObject.Properties["ServiceType"].Value as String;
            this.StartName = WMIObject.Properties["StartName"].Value as String;
            this.State = WMIObject.Properties["State"].Value as String;
            this.TagId = WMIObject.Properties["TagId"].Value as UInt32?;
        }

        #region Properties

        //internal string __CLASS { get; set; }
        //internal string __NAMESPACE { get; set; }
        //internal bool __INSTANCE { get; set; }
        //internal string __RELPATH { get; set; }
        //internal PSObject WMIObject { get; set; }
        //internal Runspace remoteRunspace;
        //internal TraceSource pSCode;
        public Boolean? AcceptPause { get; set; }
        public Boolean? AcceptStop { get; set; }
        public Boolean? DesktopInteract { get; set; }
        public String DisplayName { get; set; }
        public String ErrorControl { get; set; }
        public UInt32? ExitCode { get; set; }
        public String PathName { get; set; }
        public UInt32? ServiceSpecificExitCode { get; set; }
        public String ServiceType { get; set; }
        public String StartName { get; set; }
        public String State { get; set; }
        public UInt32? TagId { get; set; }
        #endregion

        #region Methods

        /*public UInt32 StartService()
        {
            return 0;
        }
        public UInt32 StopService()
        {
            return 0;
        }
        public UInt32 PauseService()
        {
            return 0;
        }
        public UInt32 ResumeService()
        {
            return 0;
        }
        public UInt32 InterrogateService()
        {
            return 0;
        }
        public UInt32 UserControlService(int ControlCode)
        {
            return 0;
        }
        public UInt32 Create(Boolean DesktopInteract, String DisplayName, int ErrorControl, String LoadOrderGroup, String LoadOrderGroupDependencies, String Name, String PathName, String ServiceDependencies, int ServiceType, String StartMode, String StartName, String StartPassword)
        {
            return 0;
        }
        public UInt32 Change(Boolean DesktopInteract, String DisplayName, int ErrorControl, String LoadOrderGroup, String LoadOrderGroupDependencies, String PathName, String ServiceDependencies, int ServiceType, String StartMode, String StartName, String StartPassword)
        {
            return 0;
        }
        public UInt32 ChangeStartMode(String StartMode)
        {
            return 0;
        }
        public UInt32 Delete()
        {
            return 0;
        } */
        #endregion
    }

    /// <summary>
    /// Source:ROOT\CIMV2
    /// </summary>
    public class CIM_Service : CIM_LogicalElement
    {
        //Constructor
        public CIM_Service(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            : base(WMIObject, RemoteRunspace, PSCode)
        {
            remoteRunspace = RemoteRunspace;
            pSCode = PSCode;

            this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
            this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
            this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
            this.__INSTANCE = true;
            this.WMIObject = WMIObject;
            this.CreationClassName = WMIObject.Properties["CreationClassName"].Value as String;
            this.Started = WMIObject.Properties["Started"].Value as Boolean?;
            this.StartMode = WMIObject.Properties["StartMode"].Value as String;
            this.SystemCreationClassName = WMIObject.Properties["SystemCreationClassName"].Value as String;
            this.SystemName = WMIObject.Properties["SystemName"].Value as String;
        }

        #region Properties

        //internal string __CLASS { get; set; }
        //internal string __NAMESPACE { get; set; }
        //internal bool __INSTANCE { get; set; }
        //internal string __RELPATH { get; set; }
        //internal PSObject WMIObject { get; set; }
        //internal Runspace remoteRunspace;
        //internal TraceSource pSCode;
        public String CreationClassName { get; set; }
        public Boolean? Started { get; set; }
        public String StartMode { get; set; }
        public String SystemCreationClassName { get; set; }
        public String SystemName { get; set; }
        #endregion

        #region Methods

        /*
        public UInt32 StartService()
        {
            return 0;
        }
        public UInt32 StopService()
        {
            return 0;
        } */
        #endregion
    }

    /// <summary>
    /// Source:ROOT\CIMV2
    /// </summary>
    public class CIM_LogicalElement : CIM_ManagedSystemElement
    {
        //Constructor
        public CIM_LogicalElement(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            : base(WMIObject, RemoteRunspace, PSCode)
        {
            remoteRunspace = RemoteRunspace;
            pSCode = PSCode;

            this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
            this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
            this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
            this.__INSTANCE = true;
            this.WMIObject = WMIObject;
        }

        #region Properties

        //internal string __CLASS { get; set; }
        //internal string __NAMESPACE { get; set; }
        //internal bool __INSTANCE { get; set; }
        //internal string __RELPATH { get; set; }
        //internal PSObject WMIObject { get; set; }
        //internal Runspace remoteRunspace;
        //internal TraceSource pSCode;
        #endregion

    }

    /// <summary>
    /// Source:ROOT\CIMV2
    /// </summary>
    public class CIM_ManagedSystemElement
    {
        //Constructor
        public CIM_ManagedSystemElement(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
        {
            remoteRunspace = RemoteRunspace;
            pSCode = PSCode;

            this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
            this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
            this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
            this.__INSTANCE = true;
            this.WMIObject = WMIObject;
            this.Caption = WMIObject.Properties["Caption"].Value as String;
            this.Description = WMIObject.Properties["Description"].Value as String;
            string sInstallDate = WMIObject.Properties["InstallDate"].Value as string;
            if (string.IsNullOrEmpty(sInstallDate))
                this.InstallDate = null;
            else
                this.InstallDate = ManagementDateTimeConverter.ToDateTime(sInstallDate) as DateTime?;
            this.Name = WMIObject.Properties["Name"].Value as String;
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
        public String Caption { get; set; }
        public String Description { get; set; }
        public DateTime? InstallDate { get; set; }
        public String Name { get; set; }
        public String Status { get; set; }
        #endregion

    }


}
