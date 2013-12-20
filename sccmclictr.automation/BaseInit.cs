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
using System.Threading;


namespace sccmclictr.automation
{
    public class baseInit : IDisposable
    {
        public void Dispose()
        {
            try
            {
                if(remoteRunspace != null)
                    remoteRunspace.Dispose();
                if (tsPSCode != null)
                {
                    try
                    {
                        tsPSCode.Close();
                        Cache.Dispose();
                    }
                    catch { }

                    tsPSCode = null;
                }

                if(Cache != null)
                    Cache.Dispose();
            }
            catch { }

        }

        private Runspace remoteRunspace { get; set; }

        internal string CreateHash(string str)
        {
            // First we need to convert the string into bytes, which
            // means using a text encoder.
            Encoder enc = System.Text.Encoding.Unicode.GetEncoder();

            // Create a buffer large enough to hold the string
            byte[] unicodeText = new byte[str.Length * 2];
            enc.GetBytes(str.ToCharArray(), 0, str.Length, unicodeText, 0, true);

            //Change to be FIPS compliant 
            System.Security.Cryptography.SHA1 sha1 = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            byte[] result = sha1.ComputeHash(unicodeText);


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

        //This initialization is required in a multithreaded environment (e.g. Collection commander and orchestrator) !
        //internal MemoryCache Cache = new MemoryCache("baseInit", new System.Collections.Specialized.NameValueCollection(99));
        internal MemoryCache Cache;

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

            Cache = new MemoryCache(RemoteRunspace.ConnectionInfo.ComputerName, new System.Collections.Specialized.NameValueCollection(99));
        }

        public string GetStringFromClassMethod(string WMIPath, string WMIMethod, string ResultProperty)
        {
            return GetStringFromClassMethod(WMIPath, WMIMethod, ResultProperty, false);
        }

        public string GetStringFromClassMethod(string WMIPath, string WMIMethod, string ResultProperty, bool Reload)
        {
            if (!ResultProperty.StartsWith("."))
                ResultProperty = "." + ResultProperty;

            string sResult = "";
            string sPSCode = string.Format("([wmiclass]\"{0}\").{1}{2}", WMIPath, WMIMethod, ResultProperty);

            if (!bShowPSCodeOnly)
            {
                string sHash = CreateHash(WMIPath + WMIMethod + ResultProperty);

                if ((Cache.Get(sHash) != null) & !Reload)
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
            return GetStringFromMethod(WMIPath, WMIMethod, ResultProperty, false);
        }

        public string GetStringFromMethod(string WMIPath, string WMIMethod, string ResultProperty, bool Reload)
        {
            if (!ResultProperty.StartsWith("("))
                ResultProperty = "(" + ResultProperty + ")";

            string sResult = "";
            string sPSCode = string.Format("([wmi]\"{0}\").{1}{2}", WMIPath, WMIMethod, ResultProperty);

            if (!bShowPSCodeOnly)
            {
                string sHash = CreateHash(WMIPath + WMIMethod + ResultProperty);

                if ((Cache.Get(sHash) != null) & !Reload)
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
            //do not cache per default.
            return CallClassMethod(WMIPath, WMIMethod, MethodParams, true);
        }

        public PSObject CallClassMethod(string WMIPath, string WMIMethod, string MethodParams, bool Reload)
        {
            PSObject pResult = null;
            if (!MethodParams.StartsWith("("))
                MethodParams = "(" + MethodParams + ")";
            string sPSCode = string.Format("([wmiclass]'{0}').{1}{2}", WMIPath, WMIMethod, MethodParams);

            if (!bShowPSCodeOnly)
            {
                string sHash = CreateHash(WMIPath + WMIMethod + MethodParams);

                if ((Cache.Get(sHash) != null) & !Reload)
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
            //Do not cache per default
            return CallInstanceMethod(WMIPath, WMIMethod, MethodParams, true);
        }

        public PSObject CallInstanceMethod(string WMIPath, string WMIMethod, string MethodParams, bool Reload)
        {
            PSObject pResult = null;
            string sPSCode = string.Format("([wmi]'{0}').{1}({2})", WMIPath, WMIMethod, MethodParams);

            if (!bShowPSCodeOnly)
            {
                string sHash = CreateHash(WMIPath + WMIMethod + MethodParams);

                if ((Cache.Get(sHash) != null) & !Reload)
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
            return GetStringFromPS(PSCode, false);
        }

        public string GetStringFromPS(string PSCode, bool Reload)
        {
            string sResult = "";

            if (!bShowPSCodeOnly)
            {
                string sHash = CreateHash(PSCode);

                if ((Cache.Get(sHash) != null) & !Reload)
                {
                    sResult = Cache.Get(sHash) as string;
                }
                else
                {
                    foreach (PSObject obj in WSMan.RunPSScript(PSCode, remoteRunspace))
                    {
                        try
                        {
                            //sResult = obj.BaseObject.ToString();
                            sResult = obj.ToString();
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
            return GetProperty(WMIPath, ResultProperty, false);
        }

        public string GetProperty(string WMIPath, string ResultProperty, bool Reload)
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



                if ((Cache.Get(sHash) != null) & !Reload)
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
            return GetProperties(WMIPath, ResultProperty, false);
        }

        public List<PSObject> GetProperties(string WMIPath, string ResultProperty, bool Reload)
        {
            //$a=([wmi]"ROOT\ccm:SMS_Client=@").ClientVersion
            if (!ResultProperty.StartsWith("."))
                ResultProperty = "." + ResultProperty;

            List<PSObject> lResult = new List<PSObject>();

            string sPSCode = string.Format("([wmi]'{0}'){1}", WMIPath, ResultProperty);

            if (!bShowPSCodeOnly)
            {
                string sHash = CreateHash(WMIPath + ResultProperty);



                if ((Cache.Get(sHash) != null) & !Reload)
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
            //return cached Items
            return GetObjects(WMINamespace, WQLQuery, false);
        }

        public List<PSObject> GetObjects(string WMINamespace, string WQLQuery, bool Reload)
        {
            return GetObjects(WMINamespace, WQLQuery, Reload, cacheTime);
        }

        public List<PSObject> GetObjects(string WMINamespace, string WQLQuery, bool Reload, TimeSpan tCacheTime)
        {
            //get-wmiobject -query "SELECT * FROM CacheInfoEx" -namespace "root\ccm\SoftMgmtAgent"
            List<PSObject> lResult = new List<PSObject>();
            string sPSCode = string.Format("get-wmiobject -query \"{0}\" -namespace \"{1}\"", WQLQuery, WMINamespace);

            if (!bShowPSCodeOnly)
            {
                string sHash = CreateHash(WMINamespace + WQLQuery);
                if ((Cache.Get(sHash) != null) & !Reload)
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

        /// <summary>
        /// Get Object from Powershell Command
        /// </summary>
        /// <param name="PSCode"></param>
        /// <returns></returns>
        public List<PSObject> GetObjectsFromPS(string PSCode)
        {
            return GetObjectsFromPS(PSCode, false, cacheTime);
        }

        /// <summary>
        /// Get Object from Powershell Command
        /// </summary>
        /// <param name="PSCode"></param>
        /// <param name="Reload">Ignore cached results, always reload Objects</param>
        /// <returns></returns>
        public List<PSObject> GetObjectsFromPS(string PSCode, bool Reload)
        {
            return GetObjectsFromPS(PSCode, Reload, cacheTime);
        }

        /// <summary>
        /// Get Object from Powershell Command
        /// </summary>
        /// <param name="PSCode">Powershell code</param>
        /// <param name="Reload">enforce reload</param>
        /// <param name="tCacheTime">custom cache time</param>
        /// <returns></returns>
        public List<PSObject> GetObjectsFromPS(string PSCode, bool Reload, TimeSpan tCacheTime)
        {
            List<PSObject> lResult = new List<PSObject>();

            if (!bShowPSCodeOnly)
            {
                string sHash = CreateHash(PSCode);

                if ((Cache.Get(sHash) != null) & !Reload)
                {
                    lResult = Cache.Get(sHash) as List<PSObject>;
                }
                else
                {
                    lResult = WSMan.RunPSScript(PSCode, remoteRunspace).ToList<PSObject>();
                    Cache.Add(sHash, lResult, DateTime.Now + tCacheTime);
                }
            }

            //Trace the PowerShell Command
            tsPSCode.TraceInformation(PSCode);

            return lResult;
        }

   }

    public class ccm : baseInit
    {
        public functions.agentproperties AgentProperties;
        public functions.agentactions AgentActions;
        public functions.softwaredistribution SoftwareDistribution;
        public functions.swcache SWCache;
        public functions.softwareupdates SoftwareUpdates;
        public functions.inventory Inventory;
        public functions.components Components;
        public functions.services Services;
        public functions.processes Process;
        public functions.dcm DCM;
        public sccmclictr.automation.policy.requestedConfig RequestedConfig;
        public sccmclictr.automation.policy.actualConfig ActualConfig;
        public functions.monitoring Monitoring;
        public functions.health Health;

        public void Dispose()
        {
            AgentProperties = null;
            AgentActions = null;
            Health = null;
            Monitoring = null;
            ActualConfig = null;
            RequestedConfig = null;
            Process = null;
            Services = null;
            Components = null;
            Inventory = null;
            SoftwareUpdates = null;
            SWCache = null;
            SoftwareDistribution = null;
            DCM = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="RemoteRunspace"></param>
        /// <param name="PSCode"></param>
        internal ccm(Runspace RemoteRunspace, TraceSource PSCode) : base(RemoteRunspace, PSCode)
        {
            AgentProperties = new functions.agentproperties(RemoteRunspace, PSCode, this);
            AgentActions = new functions.agentactions(RemoteRunspace, PSCode, this);
            SoftwareDistribution = new functions.softwaredistribution(RemoteRunspace, PSCode, this);
            SWCache = new functions.swcache(RemoteRunspace, PSCode, this);
            SoftwareUpdates = new functions.softwareupdates(RemoteRunspace, PSCode, this);
            Inventory = new functions.inventory(RemoteRunspace, PSCode, this);
            Components = new functions.components(RemoteRunspace, PSCode, this);
            RequestedConfig = new sccmclictr.automation.policy.requestedConfig(RemoteRunspace, PSCode, this);
            ActualConfig = new sccmclictr.automation.policy.actualConfig(RemoteRunspace, PSCode, this);
            Services = new functions.services(RemoteRunspace, PSCode, this);
            Process = new functions.processes(RemoteRunspace, PSCode, this);
            Monitoring = new functions.monitoring(RemoteRunspace, PSCode, this);
            Health = new functions.health(RemoteRunspace, PSCode, this);
            DCM = new functions.dcm(RemoteRunspace, PSCode, this);
        }
    }
}
