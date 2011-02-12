//SCCM Client Center Automation Library (SMSCliCtr.automation)
//Copyright (c) 2008 by Roger Zander

//This program is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or any later version. 
//This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details. 
//GNU General Public License: http://www.gnu.org/licenses/lgpl.html

using System;
using System.Collections;
using System.Collections.Generic;
using System.Management;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Security;

namespace smsclictr.automation
{
    /// <summary>
    /// WMI Functions to access the Registry
    /// </summary>
    public class WMIRegistry
    {
        private WMIProvider oWMIProvider;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="oProvider"></param>
        public WMIRegistry(WMIProvider oProvider)
        {
            oWMIProvider = oProvider;
        }

        /// <summary>
        /// Get a DWORD Registry Value as string
        /// </summary>
        /// <param name="hDefKey">HKLM = 2147483650</param>
        /// <param name="sSubKeyName"></param>
        /// <param name="sValueName"></param>
        /// <returns></returns>
        public string GetDWord(UInt32 hDefKey, string sSubKeyName, string sValueName)
        {
            return GetDWord(hDefKey, sSubKeyName, sValueName, "");
        }

        /// <summary>
        /// Get a DWORD Registry Value as string
        /// </summary>
        /// <param name="hDefKey">HKLM = 2147483650</param>
        /// <param name="sSubKeyName"></param>
        /// <param name="sValueName"></param>
        /// <param name="DefaultValue">return string if key or value does not exist</param>
        /// <returns></returns>
        public string GetDWord(UInt32 hDefKey, string sSubKeyName, string sValueName, string DefaultValue)
        {
            {
                String result = "";
                try
                {
                    WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                    oProv.mScope.Path.NamespacePath = @"ROOT\default";

                    ManagementBaseObject inParams = oProv.GetClass("StdRegProv").GetMethodParameters("GetDWORDValue");
                    inParams["hDefKey"] = hDefKey;
                    inParams["sSubKeyName"] = sSubKeyName;
                    inParams["sValueName"] = sValueName;
                    ManagementBaseObject outParams = oProv.ExecuteMethod("StdRegProv", "GetDWORDValue", inParams);

                    if (outParams.GetPropertyValue("ReturnValue").ToString() == "0")
                    {
                        if (outParams.GetPropertyValue("uValue") != null)
                        {
                            result = outParams.GetPropertyValue("uValue").ToString();
                        }
                    }
                    return result;
                }
                catch
                {
                    return DefaultValue;
                }
            }
        }

        /// <summary>
        /// Get a String Registry Value
        /// </summary>
        /// <param name="hDefKey">HKLM = 2147483650</param>
        /// <param name="sSubKeyName"></param>
        /// <param name="sValueName"></param>
        /// <returns></returns>
        public string GetString(UInt32 hDefKey, string sSubKeyName, string sValueName)
        {
            return GetString(hDefKey, sSubKeyName, sValueName, "");
        }

        /// <summary>
        /// Get a String Registry Value
        /// </summary>
        /// <param name="hDefKey">HKLM = 2147483650</param>
        /// <param name="sSubKeyName"></param>
        /// <param name="sValueName"></param>
        /// <param name="DefaultValue">return string if key or value does not exist</param>
        /// <returns></returns>
        public string GetString(UInt32 hDefKey, string sSubKeyName, string sValueName, string DefaultValue)
        {
            {
                String result = "";
                try
                {
                    WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                    oProv.mScope.Path.NamespacePath = @"ROOT\default";
                    ManagementBaseObject inParams = oProv.GetClass("StdRegProv").GetMethodParameters("GetStringValue");
                    inParams["hDefKey"] = hDefKey;
                    inParams["sSubKeyName"] = sSubKeyName;
                    inParams["sValueName"] = sValueName;
                    ManagementBaseObject outParams = oProv.ExecuteMethod("StdRegProv", "GetStringValue", inParams);

                    if (outParams.GetPropertyValue("ReturnValue").ToString() == "0")
                    {
                        if (outParams.GetPropertyValue("sValue") != null)
                        {
                            result = outParams.GetPropertyValue("sValue").ToString();
                        }
                    }
                    return result;
                }
                catch
                {
                    return DefaultValue;
                }
            }
        }

        /// <summary>
        /// Get a MultiString Registry Value
        /// </summary>
        /// <param name="hDefKey">HKLM = 2147483650</param>
        /// <param name="sSubKeyName"></param>
        /// <param name="sValueName"></param>
        /// <param name="DefaultValue"></param>
        /// <returns></returns>
        public string[] GetMultiString(UInt32 hDefKey, string sSubKeyName, string sValueName, string DefaultValue)
        {
            {
                try
                {
                    WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                    oProv.mScope.Path.NamespacePath = @"ROOT\default";
                    ManagementBaseObject inParams = oProv.GetClass("StdRegProv").GetMethodParameters("GetMultiStringValue");
                    inParams["hDefKey"] = hDefKey;
                    inParams["sSubKeyName"] = sSubKeyName;
                    inParams["sValueName"] = sValueName;
                    ManagementBaseObject outParams = oProv.ExecuteMethod("StdRegProv", "GetMultiStringValue", inParams);

                    if (outParams.GetPropertyValue("ReturnValue").ToString() == "0")
                    {
                        if (outParams.GetPropertyValue("sValue") != null)
                        {
                            return outParams.GetPropertyValue("sValue") as string[];
                        }
                    }
                    return new string[] { DefaultValue };
                }
                catch
                {
                    return new string[] { DefaultValue };
                }
            }
        }

        /// <summary>
        /// Set a DWORD Registry Value from a string
        /// </summary>
        /// <param name="hDefKey"></param>
        /// <param name="sSubKeyName"></param>
        /// <param name="sValueName"></param>
        /// <param name="sValue"></param>
        public void SetDWord(UInt32 hDefKey, string sSubKeyName, string sValueName, string sValue)
        {
            SetDWord(hDefKey, sSubKeyName, sValueName, System.Convert.ToUInt32(sValue));
        }

        /// <summary>
        /// Set a DWORD Registry Value from a UInt32 Value
        /// </summary>
        /// <param name="hDefKey"></param>
        /// <param name="sSubKeyName"></param>
        /// <param name="sValueName"></param>
        /// <param name="uValue"></param>
        public void SetDWord(UInt32 hDefKey, string sSubKeyName, string sValueName, UInt32 uValue)
        {
            try
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"ROOT\default";
                ManagementBaseObject inParams = oProv.GetClass("StdRegProv").GetMethodParameters("SetDWORDValue");
                inParams["hDefKey"] = hDefKey;
                inParams["sSubKeyName"] = sSubKeyName;
                inParams["sValueName"] = sValueName;
                inParams["uValue"] = uValue;
                ManagementBaseObject outParams = oProv.ExecuteMethod("StdRegProv", "SetDWORDValue", inParams);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Delete a Registry Key and all Subkeys
        /// </summary>
        /// <param name="hDefKey"></param>
        /// <param name="sSubKeyName"></param>
        public void DeleteKey(UInt32 hDefKey, String sSubKeyName)
        {
            try
            {
                //Delete all subkeys
                ArrayList Subkeys = RegKeys(hDefKey, sSubKeyName);
                foreach (string skey in Subkeys)
                {
                    DeleteKey(hDefKey, sSubKeyName + @"\" + skey);
                }

                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"ROOT\default";
                ManagementBaseObject inParams = oProv.GetClass("StdRegProv").GetMethodParameters("DeleteKey");
                inParams["hDefKey"] = hDefKey;
                inParams["sSubKeyName"] = sSubKeyName;
                oProv.ExecuteMethod("StdRegProv", "DeleteKey", inParams);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Get an ArrayList of all Registry SubKeys
        /// </summary>
        /// <param name="hDefKey">2147483650 = HKLM</param>
        /// <param name="sSubKeyName"></param>
        /// <returns>RegistryKeys (string)</returns>
        public ArrayList RegKeys(UInt32 hDefKey, string sSubKeyName)
        {
            try
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"ROOT\default";
                ManagementBaseObject inParams = oProv.GetClass("StdRegProv").GetMethodParameters("EnumKey");
                inParams["hDefKey"] = hDefKey;
                inParams["sSubKeyName"] = sSubKeyName;
                ManagementBaseObject outParams = oProv.ExecuteMethod("StdRegProv", "EnumKey", inParams);
                ArrayList result = new ArrayList();
                if (outParams.GetPropertyValue("ReturnValue").ToString() == "0")
                {
                    if (outParams.GetPropertyValue("sNames") != null)
                    {
                        result.AddRange(outParams.GetPropertyValue("sNames") as String[]);
                    }
                }
                return result;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Get an ArrayList of all Regisry Values
        /// </summary>
        /// <param name="hDefKey">2147483650 = HKLM</param>
        /// <param name="sSubKeyName"></param>
        /// <returns>RegistryValues (string)</returns>
        public ArrayList RegValues(UInt32 hDefKey, string sSubKeyName)
        {
            try
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"ROOT\default";
                ManagementBaseObject inParams = oProv.GetClass("StdRegProv").GetMethodParameters("EnumValues");
                inParams["hDefKey"] = hDefKey;
                inParams["sSubKeyName"] = sSubKeyName;
                ManagementBaseObject outParams = oProv.ExecuteMethod("StdRegProv", "EnumValues", inParams);
                ArrayList result = new ArrayList();
                if (outParams.GetPropertyValue("ReturnValue").ToString() == "0")
                {
                    if (outParams.GetPropertyValue("sNames") != null)
                    {
                        result.AddRange(outParams.GetPropertyValue("sNames") as String[]);
                    }
                }
                return result;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Get an ArrayList of all Regisry Values
        /// </summary>
        /// <param name="hDefKey">2147483650 = HKLM</param>
        /// <param name="sSubKeyName"></param>
        /// <returns>RegistryValues</returns>
        public List<string> RegValuesList(UInt32 hDefKey, string sSubKeyName)
        {
            try
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"ROOT\default";
                ManagementBaseObject inParams = oProv.GetClass("StdRegProv").GetMethodParameters("EnumValues");
                inParams["hDefKey"] = hDefKey;
                inParams["sSubKeyName"] = sSubKeyName;
                ManagementBaseObject outParams = oProv.ExecuteMethod("StdRegProv", "EnumValues", inParams);
                List<string> result = new List<string>();
                if (outParams.GetPropertyValue("ReturnValue").ToString() == "0")
                {
                    if (outParams.GetPropertyValue("sNames") != null)
                    {
                        result.AddRange(outParams.GetPropertyValue("sNames") as String[]);
                    }
                }
                return result;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Get an extended String value (REG_EXPAND_SZ)
        /// </summary>
        /// <param name="hDefKey">HKLM = 2147483650</param>
        /// <param name="sSubKeyName"></param>
        /// <param name="sValueName"></param>
        /// <returns></returns>
        public String GetExStringValue(UInt32 hDefKey, String sSubKeyName, String sValueName)
        {
            return GetExStringValue(hDefKey, sSubKeyName, sValueName, "");
        }

        /// <summary>
        /// Get an extended String value (REG_EXPAND_SZ)
        /// </summary>
        /// <param name="hDefKey">HKLM = 2147483650</param>
        /// <param name="sSubKeyName"></param>
        /// <param name="sValueName"></param>
        /// <param name="DefaultValue">return string if key or value does not exist</param>
        /// <returns></returns>
        public String GetExStringValue(UInt32 hDefKey, String sSubKeyName, String sValueName, String DefaultValue)
        {
            try
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"ROOT\default";
                ManagementBaseObject inParams = oProv.GetClass("StdRegProv").GetMethodParameters("GetExpandedStringValue");
                inParams["hDefKey"] = hDefKey;
                inParams["sSubKeyName"] = sSubKeyName;
                inParams["sValueName"] = sValueName;
                ManagementBaseObject outParams = oProv.ExecuteMethod("StdRegProv", "GetExpandedStringValue", inParams);
                String result = "";
                if (outParams.GetPropertyValue("ReturnValue").ToString() == "0")
                {
                    if (outParams.GetPropertyValue("sValue") != null)
                    {
                        result = outParams.GetPropertyValue("sValue").ToString();
                    }
                }
                return result;
            }
            catch
            {
                return DefaultValue;
            }
        }

        /// <summary>
        /// Set Registry Expanded String value
        /// </summary>
        /// <param name="hDefKey"></param>
        /// <param name="sSubKeyName"></param>
        /// <param name="sValueName"></param>
        /// <param name="sValue"></param>
        public void SetExStringValue(UInt32 hDefKey, String sSubKeyName, String sValueName, String sValue)
        {
            try
            {
                WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
                oProv.mScope.Path.NamespacePath = @"ROOT\default";
                ManagementBaseObject inParams = oProv.GetClass("StdRegProv").GetMethodParameters("SetExpandedStringValue");
                inParams["hDefKey"] = hDefKey;
                inParams["sSubKeyName"] = sSubKeyName;
                inParams["sValueName"] = sValueName;
                inParams["sValue"] = sValue;
                oProv.ExecuteMethod("StdRegProv", "SetExpandedStringValue", inParams);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Set Registry String Value
        /// </summary>
        /// <param name="hDefKey">HKLM = 2147483650</param>
        /// <param name="sSubKeyName"></param>
        /// <param name="sValueName"></param>
        /// <param name="sValue"></param>
        public void SetStringValue(UInt32 hDefKey, String sSubKeyName, String sValueName, String sValue)
        {
            WMIProvider oProv = new WMIProvider(oWMIProvider.mScope.Clone());
            oProv.mScope.Path.NamespacePath = @"ROOT\default";
            ManagementBaseObject inParams = oProv.GetClass("StdRegProv").GetMethodParameters("SetStringValue");
            inParams["hDefKey"] = hDefKey;
            inParams["sSubKeyName"] = sSubKeyName;
            inParams["sValueName"] = sValueName;
            inParams["sValue"] = sValue;
            oProv.ExecuteMethod("StdRegProv", "SetStringValue", inParams);
        }
    }
}
