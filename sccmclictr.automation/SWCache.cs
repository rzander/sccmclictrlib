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
using System.Management;

namespace sccmclictr.automation.functions
{
    /// <summary>
    /// Class to manage SCCM Agent Cache (SW Packages, Updates etc.)
    /// </summary>
    public class swcache : baseInit
    {
        internal Runspace remoteRunspace;
        internal TraceSource pSCode;
        internal ccm baseClient;

        //Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="swcache"/> class.
        /// </summary>
        /// <param name="RemoteRunspace">The remote runspace.</param>
        /// <param name="PSCode">The PowerShell code.</param>
        /// <param name="oClient">A CCM Client object.</param>
        public swcache(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
            : base(RemoteRunspace, PSCode)
        {
            remoteRunspace = RemoteRunspace;
            pSCode = PSCode;
            baseClient = oClient;
        }

        /// <summary>
        /// Gets the content of the cached.
        /// </summary>
        /// <value>The content of the cached.</value>
        public List<CacheInfoEx> CachedContent
        {
            get
            {
                //Backup original Cache timout value
                TimeSpan oldTime = base.cacheTime;

                //Set new CacheTimeout for the AssignedSite Code to 30 seconds
                base.cacheTime = new TimeSpan(0, 0, 30);

                List<CacheInfoEx> lCache = new List<CacheInfoEx>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\SoftMgmtAgent", "SELECT * FROM CacheInfoEx");
                foreach (PSObject PSObj in oObj)
                {
                    //Get AppDTs sub Objects
                    CacheInfoEx oCIEx = new CacheInfoEx(PSObj, remoteRunspace, pSCode);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }

                //Reset to original Cache timeout value
                base.cacheTime = oldTime;

                return lCache;
            }
        }

        /// <summary>
        /// Gets all Directories in the SCCM Agent Cache Folder
        /// </summary>
        /// <returns>List{System.String}.</returns>
        public List<string> GetAllCacheDirs()
        {
            List<string> lResult = new List<string>();
            List<PSObject> lPSO = base.GetObjectsFromPS(@"dir '" + CachePath +"' | WHERE {$_.PsIsContainer} | select Name");
            foreach (PSObject pso in lPSO)
            {
                lResult.Add(pso.Members["Name"].Value.ToString());
            }
            return lResult;
        }

        /// <summary>
        /// Get all Package Directories in the SCCM Agent Cache Folder
        /// </summary>
        /// <returns></returns>
        public List<string> GetPkgCacheDirs()
        {
            string sSiteCode = base.GetStringFromClassMethod(@"ROOT\ccm:SMS_Client", "GetAssignedSite()", "sSiteCode");
            
            List<string> lResult = new List<string>();
            List<PSObject> lPSO = base.GetObjectsFromPS(@"dir '" + CachePath + "' | WHERE {$_.PsIsContainer -and $_.Name[8] -eq '.'} | select Name");
            foreach (PSObject pso in lPSO)
            {
                lResult.Add(pso.Members["Name"].Value.ToString());
            }
            return lResult;
        }

        #region Properties

        /// <summary>
        /// Gets or sets the path where SCCM Client stores software packages and updates.
        /// </summary>
        /// <value>The cache path.</value>
        public string CachePath
        {
            get
            {
                return base.GetProperty(@"ROOT\ccm\SoftMgmtAgent:CacheConfig.ConfigKey='Cache'", "Location");
            }
            set
            {
                base.SetProperty(@"ROOT\ccm\SoftMgmtAgent:CacheConfig.ConfigKey='Cache'", "Location", "'" + value + "'");
            }
        }

        /// <summary>
        /// Gets or sets the size of the cache.
        /// </summary>
        /// <value>The size of the cache.</value>
        public UInt32? CacheSize
        {
            get
            {
                string sSize = base.GetProperty(@"ROOT\ccm\SoftMgmtAgent:CacheConfig.ConfigKey='Cache'", "Size");
                if (string.IsNullOrEmpty(sSize))
                    return null;
                else
                    return UInt32.Parse(sSize);
            }
            set
            {
                base.SetProperty(@"ROOT\ccm\SoftMgmtAgent:CacheConfig.ConfigKey='Cache'", "Size", value.ToString());
            }
        }

        /// <summary>
        /// Gets the inUse property.
        /// </summary>
        /// <value>The in use.</value>
        public Boolean? InUse
        {
            get
            {
                string sUse = base.GetProperty(@"ROOT\ccm\SoftMgmtAgent:CacheConfig.ConfigKey='Cache'", "InUse");
                if (string.IsNullOrEmpty(sUse))
                    return null;
                else
                    return Boolean.Parse(sUse);
            }
        }

        #endregion

        /// <summary>
        /// Cleanups the orphaned cache items  (Clean from WMI and Disk).
        /// </summary>
        public string CleanupOrphanedCacheItems()
        {
            try
            {
                return base.GetStringFromPS(Properties.Resources.CacheCleanup);
            }
            catch(Exception ex) 
            {
                return ex.Message;
            }
            /*
            //Cleanup Orphaned Database Entries
            foreach (CacheInfoEx CIX in CachedContent)
            {
                if (!CIX.FolderExists())
                {
                    CIX.DeleteFromDatabase();
                }
            }

            //Cleanup Orphaned Package-Folder Entries
            foreach (string sDir in GetPkgCacheDirs())
            {
                List<CacheInfoEx> lItems = CachedContent.FindAll(p => p.Location.EndsWith(sDir));
                if (lItems.Count == 0)
                {
                    base.GetStringFromPS("Remove-Item \"" + System.IO.Path.Combine(CachePath, sDir) + "\" -recurse");
                }
            } */
        }

        /// <summary>
        /// Class CacheInfoEx.
        /// </summary>
        public class CacheInfoEx
        {
            internal baseInit oNewBase;

            /// <summary>
            /// Initializes a new instance of the <see cref="CacheInfoEx"/> class.
            /// </summary>
            /// <param name="WMIObject">The WMI object.</param>
            /// <param name="RemoteRunspace">The remote runspace.</param>
            /// <param name="PSCode">The PowerShell code.</param>
            public CacheInfoEx(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;
                oNewBase = new baseInit(remoteRunspace, pSCode);

                this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
                this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                this.__INSTANCE = true;
                this.WMIObject = WMIObject;

                this.CacheId = WMIObject.Properties["CacheId"].Value as string;
                this.ContentId = WMIObject.Properties["ContentId"].Value as string;
                this.ContentSize = WMIObject.Properties["ContentSize"].Value as UInt32?;
                this.ContentType = WMIObject.Properties["ContentType"].Value as string;
                this.ContentVer = WMIObject.Properties["ContentVer"].Value as string;

                string sLastEvalTime = WMIObject.Properties["LastReferenced"].Value as string;
                if (string.IsNullOrEmpty(sLastEvalTime))
                    this.LastReferenced = null;
                else
                    this.LastReferenced = ManagementDateTimeConverter.ToDateTime(sLastEvalTime) as DateTime?;

                this.Location = WMIObject.Properties["Location"].Value as string;
                this.PersistInCache = WMIObject.Properties["PersistInCache"].Value as UInt32?;
                this.ReferenceCount = WMIObject.Properties["ReferenceCount"].Value as UInt32?;
            }

            #region Properties
            internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode;

            /// <summary>
            /// Gets or sets the cache identifier.
            /// </summary>
            /// <value>The cache identifier.</value>
            public String CacheId { get; set; }

            /// <summary>
            /// Gets or sets the content identifier.
            /// </summary>
            /// <value>The content identifier.</value>
            public String ContentId { get; set; }

            /// <summary>
            /// Gets or sets the size of the content.
            /// </summary>
            /// <value>The size of the content.</value>
            public UInt32? ContentSize { get; set; }

            /// <summary>
            /// Gets or sets the type of the content.
            /// </summary>
            /// <value>The type of the content.</value>
            public String ContentType { get; set; }

            /// <summary>
            /// Gets or sets the content ver.
            /// </summary>
            /// <value>The content ver.</value>
            public String ContentVer { get; set; }

            /// <summary>
            /// Gets or sets the last referenced.
            /// </summary>
            /// <value>The last referenced.</value>
            public DateTime? LastReferenced { get; set; }

            /// <summary>
            /// Gets or sets the location.
            /// </summary>
            /// <value>The location.</value>
            public String Location { get; set; }

            /// <summary>
            /// Gets or sets the persist in cache.
            /// </summary>
            /// <value>The persist in cache.</value>
            public UInt32? PersistInCache { get; set; }

            /// <summary>
            /// Gets or sets the reference count.
            /// </summary>
            /// <value>The reference count.</value>
            public UInt32? ReferenceCount { get; set; }
            #endregion

            #region Methods


            /// <summary>
            /// Check if the Folders exists.
            /// </summary>
            /// <returns>Boolean.</returns>
            public Boolean FolderExists()
            {
                string sResult = oNewBase.GetStringFromPS("Test-Path \"" + Location + "\"");
                if (string.IsNullOrEmpty(sResult))
                    return false;
                else
                {
                    return Boolean.Parse(sResult);
                }
            }

            /// <summary>
            /// Deletes the cached items from the disk.
            /// </summary>
            public void DeleteFolder()
            {
                //Prevent deletion of all Files
                if (Location.Length > 3)
                {
                    oNewBase.GetStringFromPS("Remove-Item \"" + Location + "\" -recurse");
                }
            }

            /// <summary>
            /// Deletes the cached items from  the database (WMI).
            /// </summary>
            public void DeleteFromDatabase()
            {
                oNewBase.GetStringFromPS("[wmi]'" + __NAMESPACE + ":" + __RELPATH + "' | remove-wmiobject");
            }

            /// <summary>
            /// Delete Cached Item from the Database (WMI) and from the Disk
            /// </summary>
            public void Delete()
            {
                DeleteFolder();
                DeleteFromDatabase();
            }

            #endregion
        }

    }


}
