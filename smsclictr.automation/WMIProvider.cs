//SCCM Client Center Automation Library (SMSCliCtr.automation)
//Copyright (c) 2008 by Roger Zander

//This program is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or any later version. 
//This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details. 
//GNU General Public License: http://www.gnu.org/licenses/lgpl.html

using System;
using System.Collections.Generic;
using System.Text;
using System.Management;


namespace smsclictr.automation
{
    /// <summary>
    /// WMI Provider Class
    /// </summary>
    public class WMIProvider
    {
        internal Boolean cisx86 = false;
        internal Boolean cachedisx86 = false;

        /// <summary>
        /// Management Scope of the current WMIProvider
        /// </summary>
        private ManagementScope mscope;

        /// <summary>
        /// Management Scope of the current WMIProvider
        /// </summary>
        public ManagementScope mScope
        {
            get
            {
                return mscope;
            }
            set
            {
                mscope = value;
            }
        }

        /// <summary>
        /// WMI Provider Constructor
        /// </summary>
        /// <param name="sNamespace"></param>
        public WMIProvider(string sNamespace)
        {
            Connect(sNamespace);
        }

        /// <summary>
        /// WMI Provider Constructor
        /// </summary>
        /// <param name="mScope"></param>
        public WMIProvider(ManagementScope mScope)
        {
            mscope = mScope.Clone();
            //mscope.Connect();
        }

        /// <summary>
        /// WMI Provider Constructor
        /// </summary>
        /// <param name="sNamespace"></param>
        /// <param name="WMIUser"></param>
        /// <param name="WMIPassword"></param>
        public WMIProvider(string sNamespace, string WMIUser, string WMIPassword)
        {
            Connect(sNamespace, WMIUser, WMIPassword);
        }

        /// <summary>
        /// Get a Management Object
        /// </summary>
        /// <param name="sPath"></param>
        /// <param name="oOptions"></param>
        /// <returns></returns>
        public ManagementObject GetObject(string sPath, ObjectGetOptions oOptions)
        {
            // Get the object
            ManagementObject oRet = new ManagementObject(mScope, new ManagementPath(sPath), oOptions);
            oRet.Get();
            return oRet;
        }

        /// <summary>
        /// Get a Management Object
        /// </summary>
        /// <param name="sPath"></param>
        /// <returns></returns>
        public ManagementObject GetObject(string sPath)
        {
            // Get the object
            return GetObject(sPath, new ObjectGetOptions());
        }

        /// <summary>
        /// Get a Management Class
        /// </summary>
        /// <param name="sClassName"></param>
        /// <param name="oOptions"></param>
        /// <returns></returns>
        public ManagementClass GetClass(string sClassName, ObjectGetOptions oOptions)
        {
            // Get the class
            ManagementClass oRet = new ManagementClass(mScope, new ManagementPath(sClassName), oOptions);
            return oRet;
        }

        /// <summary>
        /// Get a Management Class
        /// </summary>
        /// <param name="sClassName"></param>
        /// <returns></returns>
        public ManagementClass GetClass(string sClassName)
        {
            // Get the class
            return GetClass(sClassName, new ObjectGetOptions());
        }

        /// <summary>
        /// Execute a WMI Query
        /// </summary>
        /// <param name="sQuery"></param>
        /// <returns></returns>
        public ManagementObjectCollection ExecuteQuery(string sQuery)
        {
            // Create the query object
            ManagementObjectSearcher oRet = new ManagementObjectSearcher(mscope, new ObjectQuery(sQuery));
            return oRet.Get();
        }

        /// <summary>
        /// Execute a WMI Query
        /// </summary>
        /// <param name="sQuery"></param>
        /// <param name="oEnumOptions"></param>
        /// <returns></returns>
        public ManagementObjectCollection ExecuteQuery(string sQuery, EnumerationOptions oEnumOptions)
        {
            ManagementObjectSearcher oRet = new ManagementObjectSearcher(mscope, new ObjectQuery(sQuery), oEnumOptions);
            return oRet.Get();
        }

        /// <summary>
        /// Delete all instances from a Query Result
        /// </summary>
        /// <param name="sQuery"></param>
        public void DeleteQueryResults(string sQuery)
        {
            ManagementObjectCollection oResults = ExecuteQuery(sQuery);
            foreach (ManagementObject oInst in oResults)
            {
                oInst.Delete();
            }
        }

        /// <summary>
        /// Delete all instances from a Query Result
        /// </summary>
        /// <param name="sNamespace"></param>
        /// <param name="sQuery"></param>
        public void DeleteQueryResults(string sNamespace, string sQuery)
        {
            try
            {
                WMIProvider oProv = new WMIProvider(this.mscope.Clone());
                oProv.mScope.Path.NamespacePath = sNamespace;
                ManagementObjectCollection oResults = oProv.ExecuteQuery(sQuery);
                foreach (ManagementObject oInst in oResults)
                {
                    oInst.Delete();
                }
            }
            catch { }
        }

        /// <summary>
        /// Execute a WMI Method
        /// </summary>
        /// <param name="WMIPath"></param>
        /// <param name="Method"></param>
        /// <param name="inParams"></param>
        /// <returns></returns>
        public ManagementBaseObject ExecuteMethod(string WMIPath, string Method, ManagementBaseObject inParams)
        {
            ManagementClass SMSClass = GetClass(WMIPath);
            SMSClass.Get();
            return SMSClass.InvokeMethod(Method, inParams, new InvokeMethodOptions());
        }

        /// <summary>
        /// Execute a WMI Method
        /// </summary>
        /// <param name="WMIPath"></param>
        /// <param name="Method"></param>
        /// <returns></returns>
        public ManagementBaseObject ExecuteMethod(string WMIPath, string Method)
        {
            //ManagementPath Path = new ManagementPath(WMIPath);
            ManagementClass WMIClass = GetClass(WMIPath);
            WMIClass.Get();
            ManagementBaseObject Params = WMIClass.GetMethodParameters(Method);
            return WMIClass.InvokeMethod(Method, Params, new InvokeMethodOptions());
        }

        /// <summary>
        /// Execute a WMI Method
        /// </summary>
        /// <param name="WMIClass"></param>
        /// <param name="Method"></param>
        /// <returns></returns>
        static public ManagementBaseObject ExecuteMethod(ManagementObject WMIClass, string Method)
        {
            if (WMIClass != null)
            {
                WMIClass.Get();
                ManagementBaseObject Params = WMIClass.GetMethodParameters(Method);
                return WMIClass.InvokeMethod(Method, Params, new InvokeMethodOptions());
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Execute a WMI Method
        /// </summary>
        /// <param name="WMIClass"></param>
        /// <param name="Method"></param>
        /// <param name="inParams"></param>
        /// <returns></returns>
        static public ManagementBaseObject ExecuteMethod(ManagementObject WMIClass, string Method, ManagementBaseObject inParams)
        {
            if (WMIClass != null)
            {
                WMIClass.Get();
                return WMIClass.InvokeMethod(Method, inParams, new InvokeMethodOptions());
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Connect to a WMI NameSpace
        /// </summary>
        /// <param name="sNameSpace"></param>
        public void Connect(string sNameSpace)
        {
            mscope = new ManagementScope(sNameSpace);
            mscope.Connect();
        }

        /// <summary>
        /// Detect if remote system is a x64 OS
        /// This attribute is cached.
        /// </summary>
        /// <returns></returns>
        public bool isX86
        {
            get
            {
                //Check if status is cached
                if (!cachedisx86)
                {
                    try
                    {
                        WMIComputerSystem oSys = new WMIComputerSystem(this);
                        if (oSys.Architecture == "x86")
                            cisx86 = true;
                        else
                            cisx86 = false;
                        cachedisx86 = true;
                    }
                    catch { }
                }

                return cisx86;
            }
        }

        /*
        /// <summary>
        /// Connect to a ManagementScope
        /// </summary>
        /// <param name="mScope"></param>
        static public void Connect(ManagementScope mScope)
        {
            if (mScope != null)
            {
                mScope = mScope.Clone();
                mScope.Connect();
            }
        }*/

        /// <summary>
        /// Connect to a WMI NameSpace
        /// </summary>
        /// <param name="sNameSpace"></param>
        /// <param name="sUser"></param>
        /// <param name="sPassword"></param>
        public void Connect(string sNameSpace, string sUser, string sPassword)
        {
            if (!string.IsNullOrEmpty(sNameSpace))
            {
                // Setup the connection options
                ConnectionOptions cOptions = new ConnectionOptions();
                // Only specify login credentials on remote connections
                if (sNameSpace.ToUpper().StartsWith("\\\\" + System.Environment.MachineName.ToUpper() + "\\") == false)
                {
                    cOptions.Username = sUser;
                    cOptions.Password = sPassword;
                    /*
                    cOptions.Impersonation = ImpersonationLevel.Impersonate;
                    cOptions.Authentication = AuthenticationLevel.Packet;
                
                    if (sUser.Contains(@"\"))
                    {
                        //Authority = Kerberos:<Domain>\<Computername>
                        cOptions.Authority = "Kerberos:" + sUser.Split('\\')[0] + @"\" + sNameSpace.Split('\\')[2];
                    }*/
                }

                mscope = new ManagementScope(sNameSpace, cOptions);
                mscope.Connect();
            }
        }

        /// <summary>
        /// Reconnect the existing connection
        /// </summary>
        public void Connect()
        {
            mscope.Connect();
        }

        /// <summary>
        /// Copy a ManagementObject to a new Path
        /// </summary>
        /// <param name="MO"></param>
        /// <param name="Scope"></param>
        /// <param name="Dest"></param>
        static public void ManagementObjectCopy(ManagementBaseObject MO, ManagementScope Scope, ManagementPath Dest)
        {
            if (MO != null)
            {
                try
                {
                    ManagementObject MORemote = new ManagementObject();
                    ManagementClass RemoteClas = new ManagementClass(Scope, Dest, new ObjectGetOptions());
                    MORemote = RemoteClas.CreateInstance();

                    foreach (PropertyData PD in MO.Properties)
                    {
                        try
                        {
                            MORemote.Properties[PD.Name].Value = PD.Value;
                        }
                        catch { }
                    }
                    MORemote.Put();
                }
                catch { }
            }
        }



    }
}