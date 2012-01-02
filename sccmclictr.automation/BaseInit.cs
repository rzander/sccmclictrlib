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
using System.Runtime.Caching;

namespace sccmclictr.automation
{
    public class baseInit
    {
        private Runspace remoteRunspace { get; set; }

        internal string CreateHash(string str)
        {
            // First we need to convert the string into bytes, which
            // means using a text encoder.
            Encoder enc = System.Text.Encoding.Unicode.GetEncoder();

            // Create a buffer large enough to hold the string
            byte[] unicodeText = new byte[str.Length * 2];
            enc.GetBytes(str.ToCharArray(), 0, str.Length, unicodeText, 0, true);

            // Now that we have a byte array we can ask the CSP to hash it
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(unicodeText);

            // Build the final string by converting each byte
            // into hex and appending it to a StringBuilder
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                sb.Append(result[i].ToString("X2"));
            }

            // And return it
            return sb.ToString();
        }

        internal MemoryCache Cache = new MemoryCache("baseInit", new System.Collections.Specialized.NameValueCollection(99));
        
        internal bool bShowPSCodeOnly = false;
        internal TimeSpan cacheTime = new TimeSpan(0, 0, 30);

        #region Initializing

        /// <summary>
        /// Define the DebugLevel
        /// </summary>
        private TraceSwitch debugLevel = new TraceSwitch("DebugLevel", "DebugLevel from ConfigFile", "Verbose");

        /// <summary>
        /// Trace Source for PowerShell Commands
        /// </summary>
        private TraceSource tsPSCode { get; set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="RemoteRunspace">PowerShell RunSpace</param>
        /// <param name="PSCode">TraceSource for PowerShell Commands</param>
        public baseInit(Runspace RemoteRunspace, TraceSource PSCode) : base()
        {
            remoteRunspace = RemoteRunspace;
            tsPSCode = PSCode;
        }

        public string GetStringFromClassMethod(string WMIPath, string WMIMethod, string ResultProperty)
        {
            if (!ResultProperty.StartsWith("."))
                ResultProperty = "." + ResultProperty;

            string sResult = "";
            string sPSCode = string.Format("$a=[wmiclass]\"{0}\";$a.{1}{2}", WMIPath, WMIMethod, ResultProperty);

            if (!bShowPSCodeOnly)
            {
                string sHash = CreateHash(WMIPath + WMIMethod + ResultProperty);

                if (Cache.Get(sHash) != null)
                {
                    sResult = Cache.Get(sHash) as string;
                }
                else
                {
                    foreach (PSObject obj in WSMan.RunPSScript(sPSCode, remoteRunspace))
                    {
                        try
                        {
                            sResult = obj.BaseObject.ToString();
                            Cache.Add(sHash, sResult, DateTime.Now + cacheTime);
                            break;
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceError, ex.Message);
                        }
                    }
                }
            }

            //Trace the PowerShell Command
            tsPSCode.TraceInformation(sPSCode);
            return sResult;
        }

        public string GetStringFromMethod(string WMIPath, string WMIMethod, string ResultProperty)
        {
            if (!ResultProperty.StartsWith("("))
                ResultProperty = "(" + ResultProperty + ")";

            string sResult = "";
            string sPSCode = string.Format("$a=[wmi]\"{0}\";$a.{1}{2}", WMIPath, WMIMethod, ResultProperty);

            if (!bShowPSCodeOnly)
            {
                string sHash = CreateHash(WMIPath + WMIMethod + ResultProperty);

                if (Cache.Get(sHash) != null)
                {
                    sResult = Cache.Get(sHash) as string;
                }
                else
                {
                    foreach (PSObject obj in WSMan.RunPSScript(sPSCode, remoteRunspace))
                    {
                        try
                        {
                            sResult = obj.BaseObject.ToString();
                            Cache.Add(sHash, sResult, DateTime.Now + cacheTime);
                            break;
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceError, ex.Message);
                        }
                    }
                }
            }

            //Trace the PowerShell Command
            tsPSCode.TraceInformation(sPSCode);

            return sResult;
        }

        public PSObject CallClassMethod(string WMIPath, string WMIMethod, string MethodParams)
        {
            PSObject pResult = null;
            if (!MethodParams.StartsWith("("))
                MethodParams = "(" + MethodParams + ")";
            string sPSCode = string.Format("$a=[wmiclass]'{0}';$a.{1}{2}", WMIPath, WMIMethod, MethodParams);

            if (!bShowPSCodeOnly)
            {
                string sHash = CreateHash(WMIPath + WMIMethod + MethodParams);

                if (Cache.Get(sHash) != null)
                {
                    pResult = Cache.Get(sHash) as PSObject;
                }
                else
                {
                    foreach (PSObject obj in WSMan.RunPSScript(sPSCode, remoteRunspace))
                    {
                        try
                        {
                            pResult = obj;
                            Cache.Add(sHash, pResult, DateTime.Now + cacheTime);
                            break;
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceError, ex.Message);
                        }
                    }
                }
            }

            //Trace the PowerShell Command
            tsPSCode.TraceInformation(sPSCode);

            return pResult;
        }

        public PSObject CallInstanceMethod(string WMIPath, string WMIMethod, string MethodParams)
        {
            //$a=([wmi]"ROOT\ccm:SMS_Client=@").ClientVersion()
            PSObject pResult = null;
            string sPSCode = string.Format("$a=[wmi]'{0}';$a.{1}({2})", WMIPath, WMIMethod, MethodParams);

            if (!bShowPSCodeOnly)
            {
                string sHash = CreateHash(WMIPath + WMIMethod + MethodParams);

                if (Cache.Get(sHash) != null)
                {
                    pResult = Cache.Get(sHash) as PSObject;
                }
                else
                {
                    foreach (PSObject obj in WSMan.RunPSScript(sPSCode, remoteRunspace))
                    {
                        try
                        {
                            pResult = obj;
                            Cache.Add(sHash, pResult, DateTime.Now + cacheTime);
                            break;
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceError, ex.Message);
                        }
                    }
                }
            }

            //Trace the PowerShell Command
            tsPSCode.TraceInformation(sPSCode);

            return pResult;
        }

        public string GetStringFromPS(string PSCode)
        {
            string sResult = "";

            if (!bShowPSCodeOnly)
            {
                string sHash = CreateHash(PSCode);

                if (Cache.Get(sHash) != null)
                {
                    sResult = Cache.Get(sHash) as string;
                }
                else
                {
                    foreach (PSObject obj in WSMan.RunPSScript(PSCode, remoteRunspace))
                    {
                        try
                        {
                            sResult = obj.BaseObject.ToString();
                            Cache.Add(sHash, sResult, DateTime.Now + cacheTime);
                            break;
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceError, ex.Message);
                        }
                    }
                }
            }

            //Trace the PowerShell Command
            tsPSCode.TraceInformation(PSCode);

            return sResult;
        }

        public string GetProperty(string WMIPath, string ResultProperty)
        {
            //(Get-Wmiobject -class CCM_Client -namespace 'ROOT\CCM').ClientIDChangeDate
            //$a=([wmi]"ROOT\ccm:SMS_Client=@").ClientVersion
            if (!ResultProperty.StartsWith("."))
                ResultProperty = "." + ResultProperty;

            string sResult = "";
            string sPSCode = string.Format("([wmi]\"{0}\"){1}", WMIPath, ResultProperty);

            if (!bShowPSCodeOnly)
            {
                string sHash = CreateHash(WMIPath + ResultProperty);



                if (Cache.Get(sHash) != null)
                {
                    sResult = Cache.Get(sHash) as string;
                }
                else
                {
                    foreach (PSObject obj in WSMan.RunPSScript(sPSCode, remoteRunspace))
                    {
                        try
                        {
                            sResult = obj.BaseObject.ToString();
                            Cache.Add(sHash, sResult, DateTime.Now + cacheTime);
                            break;
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceError, ex.Message);
                        }
                    }
                }
            }

            //Trace the PowerShell Command
            tsPSCode.TraceInformation(sPSCode);

            return sResult;
        }

        public List<PSObject> GetProperties(string WMIPath, string ResultProperty)
        {
            //$a=([wmi]"ROOT\ccm:SMS_Client=@").ClientVersion
            if (!ResultProperty.StartsWith("."))
                ResultProperty = "." + ResultProperty;

            List<PSObject> lResult = new List<PSObject>();

            string sPSCode = string.Format("([wmi]'{0}'){1}", WMIPath, ResultProperty);

            if (!bShowPSCodeOnly)
            {
                string sHash = CreateHash(WMIPath + ResultProperty);



                if (Cache.Get(sHash) != null)
                {
                    lResult = Cache.Get(sHash) as List<PSObject>;
                }
                else
                {
                    foreach (PSObject obj in WSMan.RunPSScript(sPSCode, remoteRunspace))
                    {
                        try
                        {
                            lResult.Add(obj);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceError, ex.Message);
                        }
                    }
                 Cache.Add(sHash, lResult, DateTime.Now + cacheTime);
                }
            }

            //Trace the PowerShell Command
            tsPSCode.TraceInformation(sPSCode);

            return lResult;
        }

        public void SetProperty(string WMIPath, string Property, string Value)
        {
            //$a=([wmi]"ROOT\ccm:SMS_Client=@");$a.AllowLocalAdminOverride=$false;$a.Put()

            string sResult = "";
            string sPSCode = string.Format("$a=([wmi]\"{0}\");$a.{1}={2};$a.Put()", WMIPath, Property, Value);
            if (!bShowPSCodeOnly)
            {
                string sHash = CreateHash(WMIPath + "." + Property);

                if (Value.StartsWith("$"))
                    Value = Value.Remove(0, 1);
                
                Cache.Add(sHash, Value, DateTime.Now + cacheTime);

                foreach (PSObject obj in WSMan.RunPSScript(sPSCode, remoteRunspace))
                {
                    try
                    {
                        sResult = obj.BaseObject.ToString();
                        break;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceError, ex.Message);
                    }
                }

            }

            //Trace the PowerShell Command
            tsPSCode.TraceInformation(sPSCode);
        }

        public List<PSObject> GetObjects(string WMINamespace, string WQLQuery)
        {
            //get-wmiobject -query "SELECT * FROM CacheInfoEx" -namespace "root\ccm\SoftMgmtAgent"
            List<PSObject> lResult = new List<PSObject>();
            string sPSCode = string.Format("get-wmiobject -query \"{0}\" -namespace \"{1}\"", WQLQuery, WMINamespace);

            if (!bShowPSCodeOnly)
            {
                string sHash = CreateHash(WMINamespace + WQLQuery);

                if (Cache.Get(sHash) != null)
                {
                    lResult = Cache.Get(sHash) as List<PSObject>;
                }
                else
                {
                    foreach (PSObject obj in WSMan.RunPSScript(sPSCode, remoteRunspace))
                    {
                        try
                        {
                            lResult.Add(obj);

                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Trace.WriteLineIf(debugLevel.TraceError, ex.Message);
                        }
                    }

                    Cache.Add(sHash, lResult, DateTime.Now + cacheTime);
                }
            }

            //Trace the PowerShell Command
            tsPSCode.TraceInformation(sPSCode);

            return lResult;
        }

        public List<PSObject> GetObjectsFromPS(string PSCode)
        {
            List<PSObject> lResult = new List<PSObject>();

            if (!bShowPSCodeOnly)
            {
                string sHash = CreateHash(PSCode);

                if (Cache.Get(sHash) != null)
                {
                    lResult = Cache.Get(sHash) as List<PSObject>;
                }
                else
                {
                    lResult = WSMan.RunPSScript(PSCode, remoteRunspace).ToList<PSObject>();
                }
            }

            //Trace the PowerShell Command
            tsPSCode.TraceInformation(PSCode);

            return lResult;
        }
    }

    public class ccm : baseInit
    {
        //SCCM2007 Agent related properties 
        public functions.agentproperties AgentProperties;
        public functions.softwaredistribution SoftwareDistribution;
        public functions.swcache SWCache;
        public functions.softwareupdates SoftwareUpdates;

        internal ccm(Runspace RemoteRunspace, TraceSource PSCode) : base(RemoteRunspace, PSCode)
        {
            AgentProperties = new functions.agentproperties(RemoteRunspace, PSCode);
            SoftwareDistribution = new functions.softwaredistribution(RemoteRunspace, PSCode);
            SWCache = new functions.swcache(RemoteRunspace, PSCode);
            SoftwareUpdates = new functions.softwareupdates(RemoteRunspace, PSCode);
        }
    }
}
