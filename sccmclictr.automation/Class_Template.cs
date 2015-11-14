//SCCM Client Center Automation Library (SCCMCliCtr.automation)
//Copyright (c) 2011 by Roger Zander

//This program is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or any later version. 
//This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details. 
//GNU General Public License: http://www.gnu.org/licenses/lgpl.html

#define CM2012
#define CM2007

using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Diagnostics;

namespace sccmclictr.automation.functions
{
    /// <summary>
    /// Template for an empty Class
    /// </summary>
    public class Class_Template : baseInit
    {
        internal Runspace remoteRunspace;
        internal TraceSource pSCode;
        internal ccm baseClient;

        //Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Class_Template"/> class.
        /// </summary>
        /// <param name="RemoteRunspace">The remote runspace.</param>
        /// <param name="PSCode">The PowerShell code.</param>
        /// <param name="oClient">A ccm client object.</param>
        public Class_Template(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
            : base(RemoteRunspace, PSCode)
        {
            remoteRunspace = RemoteRunspace;
            pSCode = PSCode;
            baseClient = oClient;
        }


        /// <summary>
        /// Gets the public list.
        /// </summary>
        /// <value>The public list.</value>
        public List<_PublicClass> PublicList
        {
            get
            {
                List<_PublicClass> lCache = new List<_PublicClass>();
                List<PSObject> oObj = GetObjects(@"ROOT\ccm\SoftMgmtAgent", "SELECT * FROM CacheInfoEx");
                foreach (PSObject PSObj in oObj)
                {
                    //Get AppDTs sub Objects
                    _PublicClass oCIEx = new _PublicClass(PSObj, remoteRunspace, pSCode);

                    oCIEx.remoteRunspace = remoteRunspace;
                    oCIEx.pSCode = pSCode;
                    lCache.Add(oCIEx);
                }
                return lCache; 
            } 
        }
    }

    /// <summary>
    /// Class _PublicClass.
    /// </summary>
    public class _PublicClass
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="_PublicClass"/> class.
        /// </summary>
        /// <param name="WMIObject">A WMI object.</param>
        /// <param name="RemoteRunspace">The remote runspace.</param>
        /// <param name="PSCode">The PowerShell code.</param>
        public _PublicClass(PSObject WMIObject, Runspace RemoteRunspace, TraceSource PSCode)
        {
            remoteRunspace = RemoteRunspace;
            pSCode = PSCode;

            this.__CLASS = WMIObject.Properties["__CLASS"].Value as string;
            this.__NAMESPACE = WMIObject.Properties["__NAMESPACE"].Value as string;
            this.__RELPATH = WMIObject.Properties["__RELPATH"].Value as string;
            this.__INSTANCE = true;
            this.WMIObject = WMIObject;

            //this.CacheId = WMIObject.Properties["CacheId"].Value as string;
        }

        #region Properties
        internal string __CLASS { get; set; }
        internal string __NAMESPACE { get; set; }
        internal bool __INSTANCE { get; set; }
        internal string __RELPATH { get; set; }
        internal PSObject WMIObject { get; set; }
        internal Runspace remoteRunspace;
        internal TraceSource pSCode;

        //public String CacheId { get; set; }

        #endregion

        #region Methods

        #endregion
    }


}
