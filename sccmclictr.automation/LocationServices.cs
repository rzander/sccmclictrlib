using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace sccmclictr.automation.functions
{
    public class locationservices : baseInit
    {
        internal Runspace remoteRunspace;
        internal TraceSource pSCode;
        internal ccm baseClient;

        public locationservices(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
            : base(RemoteRunspace, PSCode)
        {
            remoteRunspace = RemoteRunspace;
            pSCode = PSCode;
            baseClient = oClient;
        }

        public List<BoundaryGroupCache> BoundaryGroupCacheList
        {
            get
            {
                List<BoundaryGroupCache> lCache = new List<BoundaryGroupCache>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\LocationServices", "SELECT * FROM BoundaryGroupCache", true);
                foreach (PSObject PSObj in oObj)
                {
                    //Get AppDTs sub Objects
                    BoundaryGroupCache oCIEx = new BoundaryGroupCache(PSObj, remoteRunspace, pSCode);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }

                return lCache;
            }
        }

        /// <summary>
        /// Source:ROOT\ccm\LocationServices
        /// </summary>
        public class BoundaryGroupCache
        {
            //Constructor
            public BoundaryGroupCache(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
            {
                remoteRunspace = RemoteRunspace;
                pSCode = PSCode;

                __CLASS = WMIObject.Properties["__CLASS"].Value as string;
                __NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
                __RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
                __INSTANCE = true;
                this.WMIObject = WMIObject;
                BoundaryGroupIDs = WMIObject.Properties["BoundaryGroupIDs"].Value as String[];
                CacheToken = WMIObject.Properties["CacheToken"].Value as String;
            }

            #region Properties

            internal string __CLASS { get; set; }
            internal string __NAMESPACE { get; set; }
            internal bool __INSTANCE { get; set; }
            internal string __RELPATH { get; set; }
            internal PSObject WMIObject { get; set; }
            internal Runspace remoteRunspace;
            internal TraceSource pSCode;
            public String[] BoundaryGroupIDs { get; set; }
            public String CacheToken { get; set; }
            #endregion

        }

    }
}
