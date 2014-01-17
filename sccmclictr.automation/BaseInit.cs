//SCCM Client Center Automation Library (SCCMCliCtr.automation)
//Copyright (c) 2011 by Roger Zander

//This program is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or any later version. 
//This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details. 
//GNU General Public License: http://www.gnu.org/licenses/lgpl.html

using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Diagnostics;
using System.Runtime.Caching;

namespace sccmclictr.automation
{
    /// <summary>
    /// 
    /// </summary>
    public class baseInit : IDisposable
    {
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
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
            Encoder enc = Encoding.Unicode.GetEncoder();

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

//        // Alternate Create Hash method for testing and consideration
//        internal string CreateHash(string stringToHash)
//        {
//            // This method is used for generating a hash for caching not security, so an int32 precision will suffice and is a little faster than a 160 byte digest.
//            System.Security.Cryptography.SHA1 sha1 = new System.Security.Cryptography.SHA1CryptoServiceProvider();
//            return BitConverter.ToInt32(sha1.ComputeHash(Encoding.Default.GetBytes(stringToHash)), 0).ToString(("X"));
//        }

        //This initialization is required in a multi threaded environment (e.g. Collection commander and orchestrator) !
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

        /// <summary>
        /// Gets a string from cache or from a WMI class method.
        /// </summary>
        /// <param name="WMIPath">The WMI path.</param>
        /// <param name="WMIMethod">The WMI method.</param>
        /// <param name="ResultProperty">The name of the property you are trying to retrieve.</param>
        /// <returns>Command results as a string.</returns>
        /// <example><code>string siteCode = base.GetStringFromClassMethod(@"ROOT\ccm:SMS_Client", "GetAssignedSite()", "sSiteCode");</code></example>
        public string GetStringFromClassMethod(string WMIPath, string WMIMethod, string ResultProperty)
        {
            return GetStringFromClassMethod(WMIPath, WMIMethod, ResultProperty, false);
        }

        /// <summary>
        /// Gets a string from cache(if Reload==False) or from a WMI class method.
        /// </summary>
        /// <param name="WMIPath">The WMI path.</param>
        /// <param name="WMIMethod">The WMI method.</param>
        /// <param name="ResultProperty">The name of the property you are trying to retrieve.</param>
        /// <param name="Reload">Enforce reload. i.e. don't use cached results.</param>
        /// <returns>Command results as a string.</returns>
        /// <example><code>string siteCode = base.GetStringFromClassMethod(@"ROOT\ccm:SMS_Client", "GetAssignedSite()", "sSiteCode", True);</code></example>
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
                            Trace.WriteLineIf(debugLevel.TraceError, ex.Message);
                        }
                    }
                }
            }

            //Trace the PowerShell Command
            tsPSCode.TraceInformation(sPSCode);
            return sResult;
        }

        /// <summary>
        /// Gets a string from cache or from a WMI method.
        /// </summary>
        /// <param name="WMIPath">The WMI path.</param>
        /// <param name="WMIMethod">The WMI method.</param>
        /// <param name="ResultProperty">The name of the property you are trying to retrieve.</param>
        /// <returns>Command results a as string.</returns>
        /// <example><code>bool multiUser = Boolean.Parse(GetStringFromMethod(@"ROOT\ccm\ClientSDK:CCM_ClientInternalUtilities=@", "AreMultiUsersLoggedOn", "MultiUsersLoggedOn"));</code></example>
        public string GetStringFromMethod(string WMIPath, string WMIMethod, string ResultProperty)
        {
            return GetStringFromMethod(WMIPath, WMIMethod, ResultProperty, false);
        }

        /// <summary>
        /// Gets a string from cache(if Reload==False) or from a WMI method.
        /// </summary>
        /// <param name="WMIPath">The WMI path.</param>
        /// <param name="WMIMethod">The WMI method.</param>
        /// <param name="ResultProperty">The name of the property you are trying to retrieve.</param>
        /// <param name="Reload">Enforce reload. i.e. don't use cached results.</param>
        /// <returns>Command results as a string.</returns>
        /// <example><code>bool multiUser = Boolean.Parse(GetStringFromMethod(@"ROOT\ccm\ClientSDK:CCM_ClientInternalUtilities=@", "AreMultiUsersLoggedOn", "MultiUsersLoggedOn", True));</code></example>
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
                            Trace.WriteLineIf(debugLevel.TraceError, ex.Message);
                        }
                    }
                }
            }

            //Trace the PowerShell Command
            tsPSCode.TraceInformation(sPSCode);

            return sResult;
        }

        /// <summary>
        /// Gets a PSObject from a WMI class method.
        /// </summary>
        /// <param name="WMIPath">The WMI path.</param>
        /// <param name="WMIMethod">The WMI method.</param>
        /// <param name="MethodParams">The method parameters.</param>
        /// <returns>Command results as a PSObject.</returns>
        /// <example><code>base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000001}'");</code></example>
        public PSObject CallClassMethod(string WMIPath, string WMIMethod, string MethodParams)
        {
            //do not cache per default.
            return CallClassMethod(WMIPath, WMIMethod, MethodParams, true);
        }

        /// <summary>
        /// Gets a PSObject from cache(if Reload==False) or from a WMI class method.
        /// </summary>
        /// <param name="WMIPath">The WMI path.</param>
        /// <param name="WMIMethod">The WMI method.</param>
        /// <param name="MethodParams">The method parameters.</param>
        /// <param name="Reload">Enforce reload. i.e. don't use cached results.</param>
        /// <returns>Command results as a PSObject.</returns>
        /// <example><code>base.CallClassMethod(@"ROOT\ccm:SMS_Client", "TriggerSchedule", "'{00000000-0000-0000-0000-000000000001}'", True);</code></example>
        public PSObject CallClassMethod(string WMIPath, string WMIMethod, string MethodParams, bool Reload)
        {
            if (!MethodParams.StartsWith("("))
                MethodParams = "(" + MethodParams + ")";

            PSObject pResult = null; 
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
                            Trace.WriteLineIf(debugLevel.TraceError, ex.Message);
                        }
                    }
                }
            }

            //Trace the PowerShell Command
            tsPSCode.TraceInformation(sPSCode);

            return pResult;
        }

        /// <summary>
        /// Gets a PSObject from a WMI instance method.
        /// </summary>
        /// <param name="WMIPath">The WMI path.</param>
        /// <param name="WMIMethod">The WMI method.</param>
        /// <param name="MethodParams">The method parameters.</param>
        /// <returns>Command results as a PSObject.</returns>
        public PSObject CallInstanceMethod(string WMIPath, string WMIMethod, string MethodParams)
        {
            //Do not cache per default
            return CallInstanceMethod(WMIPath, WMIMethod, MethodParams, true);
        }

        /// <summary>
        /// Gets a PSObject from cache(if Reload==False) of from a WMI instance method.
        /// </summary>
        /// <param name="WMIPath">The WMI path.</param>
        /// <param name="WMIMethod">The WMI method.</param>
        /// <param name="MethodParams">The method parameters.</param>
        /// <param name="Reload">Enforce reload. i.e. don't use cached results.</param>
        /// <returns>Command results as a PSObject.</returns>
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
                            Trace.WriteLineIf(debugLevel.TraceError, ex.Message);
                        }
                    }
                }
            }

            //Trace the PowerShell Command
            tsPSCode.TraceInformation(sPSCode);

            return pResult;
        }

        /// <summary>
        /// Gets a string from cache or from a PowerShell command.
        /// </summary>
        /// <param name="PSCode">The ps code.</param>
        /// <returns>Command results as a string.</returns>
        /// <example><code>string sPort = base.GetStringFromPS("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Microsoft\\CCM\")).$(\"HttpPort\")");</code></example>
        public string GetStringFromPS(string PSCode)
        {
            return GetStringFromPS(PSCode, false);
        }

        /// <summary>
        /// Gets a string from cache(if Reload==False) or from a PowerShell command.
        /// </summary>
        /// <param name="PSCode">The ps code.</param>
        /// <param name="Reload">Enforce reload. i.e. don't use cached results.</param>
        /// <returns>Command results as a string.</returns>
        /// <example><code>string sPort = base.GetStringFromPS("(Get-ItemProperty(\"HKLM:\\SOFTWARE\\Microsoft\\CCM\")).$(\"HttpPort\")", True);</code></example>
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
                            Trace.WriteLineIf(debugLevel.TraceError, ex.Message);
                        }
                    }
                }
            }

            //Trace the PowerShell Command
            tsPSCode.TraceInformation(PSCode);

            return sResult;
        }

        /// <summary>
        /// Gets a string from cache or from a WMI property.
        /// </summary>
        /// <param name="WMIPath">The WMI path.</param>
        /// <param name="ResultProperty">The name of the property you are trying to retrieve.</param>
        /// <returns>Command results as a string.</returns>
        /// <example><code>string siteCode = base.GetStringFromClassMethod(@"ROOT\ccm:CCM_Client=@", "ClientVersion");</code></example>
        public string GetProperty(string WMIPath, string ResultProperty)
        {
            return GetProperty(WMIPath, ResultProperty, false);
        }

        /// <summary>
        /// Gets a string from cache(if Reload==False) or from a WMI property.
        /// </summary>
        /// <param name="WMIPath">The WMI path.</param>
        /// <param name="ResultProperty">The name of the property you are trying to retrieve.</param>
        /// <param name="Reload">Enforce reload. i.e. don't use cached results.</param>
        /// <returns>Command results as a string.</returns>
        /// <example><code>string siteCode = base.GetStringFromClassMethod(@"ROOT\ccm:CCM_Client=@", "ClientVersion", True);</code></example>
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
                            Trace.WriteLineIf(debugLevel.TraceError, ex.Message);
                        }
                    }
                }
            }

            //Trace the PowerShell Command
            tsPSCode.TraceInformation(sPSCode);

            return sResult;
        }

        /// <summary>
        /// Gets a list of PSObjects from cache or from a WMI property.
        /// </summary>
        /// <param name="WMIPath">The WMI path.</param>
        /// <param name="ResultProperty">The name of the property you are trying to retrieve.</param>
        /// <returns>Command results as a list of PSObjects.</returns>
        /// <example><code>List&lt;PSObject&gt; lPSAppDts = base.GetProperties(@"ROOT\ccm\clientsdk:CCM_Application", "AppDTs");</code></example>
        public List<PSObject> GetProperties(string WMIPath, string ResultProperty)
        {
            return GetProperties(WMIPath, ResultProperty, false);
        }

        /// <summary>
        /// Gets a list of PSObjects from cache(if Reload==False) or from a WMI property.
        /// </summary>
        /// <param name="WMIPath">The WMI path.</param>
        /// <param name="ResultProperty">The name of the property you are trying to retrieve.</param>
        /// <param name="Reload">Enforce reload. i.e. don't use cached results.</param>
        /// <returns>Command results as a list of PSObjects.</returns>
        /// <example><code>List&lt;PSObject&gt; lPSAppDts = base.GetProperties(@"ROOT\ccm\clientsdk:CCM_Application", "AppDTs", True);</code></example>
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
                            Trace.WriteLineIf(debugLevel.TraceError, ex.Message);
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
        /// Sets a WMI property.
        /// </summary>
        /// <param name="WMIPath">The WMI path.</param>
        /// <param name="Property">The property.</param>
        /// <param name="Value">The value.</param>
        /// <example><code>base.SetProperty(@"ROOT\ccm:SMS_Client=@", "EnableAutoAssignment", "$True");</code></example>
        public void SetProperty(string WMIPath, string Property, string Value)
        {
            //$a=([wmi]"ROOT\ccm:SMS_Client=@");$a.AllowLocalAdminOverride=$false;$a.Put()

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
                        string sResult = obj.BaseObject.ToString();
                        break;
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLineIf(debugLevel.TraceError, ex.Message);
                    }
                }

            }

            //Trace the PowerShell Command
            tsPSCode.TraceInformation(sPSCode);
        }

        /// <summary>
        /// Gets a list of PSObjects from cache or from a given WMI namespace using a given WQL query
        /// </summary>
        /// <param name="WMINamespace">The WMI namespace.</param>
        /// <param name="WQLQuery">The WQL query.</param>
        /// <returns>Command results as list of PSObjects.</returns>
        /// <example><code>List&lt;PSObject&gt; lResult = base.GetObjects(@"ROOT\CCM", "SELECT * FROM SMS_MPProxyInformation Where State = 'Active'");</code></example>
        public List<PSObject> GetObjects(string WMINamespace, string WQLQuery)
        {
            //return cached Items
            return GetObjects(WMINamespace, WQLQuery, false);
        }

        /// <summary>
        /// Gets a list of PSObjects from cache(if Reload==False) or from a given WMI namespace using a given WQL query
        /// </summary>
        /// <param name="WMINamespace">The WMI namespace.</param>
        /// <param name="WQLQuery">The WQL query.</param>
        /// <param name="Reload">Enforce reload. i.e. don't use cached results.</param>
        /// <returns>Command results as a list of PSObjects.</returns>
        /// <example><code>List&lt;PSObject&gt; lResult = base.GetObjects(@"ROOT\CCM", "SELECT * FROM SMS_MPProxyInformation Where State = 'Active'", True);</code></example>
        public List<PSObject> GetObjects(string WMINamespace, string WQLQuery, bool Reload)
        {
            return GetObjects(WMINamespace, WQLQuery, Reload, cacheTime);
        }

        /// <summary>
        /// Gets a list of PSObjects from cache(if Reload==False) or from a given WMI namespace using a given WQL query
        /// </summary>
        /// <param name="WMINamespace">The WMI namespace.</param>
        /// <param name="WQLQuery">The WQL query.</param>
        /// <param name="Reload">Enforce reload. i.e. don't use cached results.</param>
        /// <param name="tCacheTime">Custom cache time.</param>
        /// <returns>Command results as a list of PSObjects.</returns>
        /// <example><code>List&lt;PSObject&gt; lResult = base.GetObjects(@"ROOT\CCM", "SELECT * FROM SMS_MPProxyInformation Where State = 'Active'", True, new TimeSpan(0,0,30));</code></example>
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
                            Trace.WriteLineIf(debugLevel.TraceError, ex.Message);
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
        /// Get Object from PowerShell Command
        /// </summary>
        /// <param name="PSCode">PowerShell code</param>
        /// <returns>Command results as a list of PSObjects.</returns>
        /// <example><code>List&lt;PSObject&gt; lResult = base.GetObjectsFromPS("(Get-ItemProperty(\"HKLM:\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\")).$(\"PendingFileRenameOperations\")");</code></example>
        public List<PSObject> GetObjectsFromPS(string PSCode)
        {
            return GetObjectsFromPS(PSCode, false, cacheTime);
        }

        /// <summary>
        /// Get Object from PowerShell Command
        /// </summary>
        /// <param name="PSCode">PowerShell code</param>
        /// <param name="Reload">Ignore cached results, always reload Objects</param>
        /// <returns>Command results as a list of PSObjects.</returns>
        /// <example><code>List&lt;PSObject&gt; lResult = base.GetObjectsFromPS("(Get-ItemProperty(\"HKLM:\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\")).$(\"PendingFileRenameOperations\")", True);</code></example>
        public List<PSObject> GetObjectsFromPS(string PSCode, bool Reload)
        {
            return GetObjectsFromPS(PSCode, Reload, cacheTime);
        }

        /// <summary>
        /// Get Object from PowerShell Command
        /// </summary>
        /// <param name="PSCode">PowerShell code</param>
        /// <param name="Reload">enforce reload</param>
        /// <param name="tCacheTime">custom cache time</param>
        /// <returns>Command results as list of PSObjects.</returns>
        /// <example><code>List&lt;PSObject&gt; lResult = base.GetObjectsFromPS("(Get-ItemProperty(\"HKLM:\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\")).$(\"PendingFileRenameOperations\")", True, new TimeSpan(0,0,30));</code></example>
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
                    foreach (PSObject obj in WSMan.RunPSScript(PSCode, remoteRunspace))
                    {
                        try
                        {
                            lResult.Add(obj);
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLineIf(debugLevel.TraceError, ex.Message);
                        }
                    }
                    Cache.Add(sHash, lResult, DateTime.Now + tCacheTime);
                }
            }

            //Trace the PowerShell Command
            tsPSCode.TraceInformation(PSCode);

            return lResult;
        }

   }

    /// <summary>
    /// 
    /// </summary>
    public class ccm : baseInit
    {
        #pragma warning disable 1591 // Disable warnings about missing XML comments

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
        public policy.requestedConfig RequestedConfig;
        public policy.actualConfig ActualConfig;
        public functions.monitoring Monitoring;
        public functions.health Health;
        public functions.appv5 AppV5;
        public functions.appv4 AppV4;

        #pragma warning restore 1591 // Enable warnings about missing XML comments

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public new void Dispose()
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
            RequestedConfig = new policy.requestedConfig(RemoteRunspace, PSCode, this);
            ActualConfig = new policy.actualConfig(RemoteRunspace, PSCode, this);
            Services = new functions.services(RemoteRunspace, PSCode, this);
            Process = new functions.processes(RemoteRunspace, PSCode, this);
            Monitoring = new functions.monitoring(RemoteRunspace, PSCode, this);
            Health = new functions.health(RemoteRunspace, PSCode, this);
            DCM = new functions.dcm(RemoteRunspace, PSCode, this);
            AppV5 = new functions.appv5(RemoteRunspace, PSCode, this);
            AppV4 = new functions.appv4(RemoteRunspace, PSCode, this);
        }
    }
}
