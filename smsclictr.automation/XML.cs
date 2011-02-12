//SCCM Client Center Automation Library (SMSCliCtr.automation)
//Copyright (c) 2008 by Roger Zander

//This program is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or any later version. 
//This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details. 
//GNU General Public License: http://www.gnu.org/licenses/lgpl.html

using System;
using System.Collections.Generic;
using System.Text;
using System.Management;
using System.Xml;

namespace smsclictr.automation
{
    /// <summary>
    /// XML Functions
    /// </summary>
    public class XML
    {
        /// <summary>
        /// Serialize all Properties of a ManagementObject
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <param name="MO">ManagementObject to serialize</param>
        /// <returns>An XMLNode with all ManagementObject Properties</returns>
        public static XmlNode MO2XML(XmlDocument xmlDoc, ManagementObject MO)
        {
            return MO2XML(xmlDoc, MO, "__RELPATH");
        }

        /// <summary>
        /// Serialize all Properties of a ManagementObject
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <param name="MO">ManagementObject to serialize</param>
        /// <param name="instanceDisplayProperty">Property to show as Instance Name (Default= __RELPATH)</param>
        /// <returns>An XMLNode with all ManagementObject Properties</returns>
        public static XmlNode MO2XML(XmlDocument xmlDoc, ManagementObject MO, string instanceDisplayProperty)
        {
            XmlNode oRoot = xmlDoc.DocumentElement;
            XmlNodeList XNL_NS = xmlDoc.GetElementsByTagName("Namespace");
            XmlNodeList XNL_Class = xmlDoc.GetElementsByTagName("Class");

            XmlElement oNameSpace = xmlDoc.CreateElement("Namespace");
            string sNamespace = MO.SystemProperties["__NAMESPACE"].Value as string;
            if (!string.IsNullOrEmpty(sNamespace))
            {
                sNamespace = sNamespace.ToUpper();
            }
            else
            {
                sNamespace = "None";
            }
            oNameSpace.SetAttribute("Value", sNamespace);
            XmlElement oClass = xmlDoc.CreateElement("Class");
            oClass.SetAttribute("Value", MO.SystemProperties["__CLASS"].Value.ToString().ToUpper());
            XmlElement oRelpath = xmlDoc.CreateElement("Path");

            string sProperty = "__RELPATH";

            //Check if the MO contains the display property...
            foreach (PropertyData PD in MO.Properties)
            {
                if (string.Compare(PD.Name, instanceDisplayProperty, true) == 0)
                {
                    //found...
                    sProperty = instanceDisplayProperty;
                    break;
                }
            }

            string sDispValue = MO.GetPropertyValue(sProperty) as string;
            if (!string.IsNullOrEmpty(sDispValue))
            {
                oRelpath.SetAttribute("Value", sDispValue.ToUpper());
            }
            else
            {
                oRelpath.SetAttribute("Value", MO.SystemProperties["__RELPATH"].Value.ToString().ToUpper());
            }
            
            XmlElement oProperties = xmlDoc.CreateElement("Properties");

            //Check if Namespace already exists...
            XmlNode xNodeNS = xmlDoc.SelectSingleNode("/ManagementObjects/Namespace[@Value='" + sNamespace + "']");
            if (xNodeNS != null)
            {
                oNameSpace = xNodeNS as XmlElement;
            }

            //Check if Class already exists...
            XmlNode xNodeClass = xmlDoc.SelectSingleNode("/ManagementObjects/Namespace/Class[@Value='" + MO.SystemProperties["__CLASS"].Value.ToString().ToUpper() + "']");
            if (xNodeClass != null)
            {
                oClass = xNodeClass as XmlElement;
            }

            oRoot.AppendChild(oNameSpace);
            oNameSpace.AppendChild(oClass);
            oClass.AppendChild(oRelpath);
            oRelpath.AppendChild(oProperties);

            foreach (PropertyData PD in MO.Properties)
            {
                if (PD.Value != null)
                {

                    if (!PD.IsArray & (PD.Value != null))
                    {
                        if (PD.Value.ToString() != "")
                        {
                            XmlElement oProp = xmlDoc.CreateElement(PD.Name);
                            XmlText TextValue = xmlDoc.CreateTextNode(PD.Value.ToString());
                            oProp.AppendChild(TextValue);
                            oProperties.AppendChild(oProp);
                        }
                    }
                    else
                    {
                        XmlElement oProp = xmlDoc.CreateElement(PD.Name);
                        switch (PD.Type)
                        {
                            case CimType.Object:
                                oProp.SetAttribute("Type", PD.Type.ToString());
                                object[] oObjects = PD.Value as Object[];
                                foreach (object oObj in oObjects)
                                {
                                    XmlNode oMBO = MBO2XML(xmlDoc, oObj as ManagementBaseObject);
                                    oProp.AppendChild(oMBO);
                                }
                                oProperties.AppendChild(oProp);
                                break;

                            case CimType.UInt8:
                                oProp.SetAttribute("Type", PD.Type.ToString());
                                byte[] bVal = PD.Value as byte[];
                                string[] sArray = new string[bVal.Length];
                                for (int i = 0; i <= bVal.Length - 1; i++)
                                {
                                    sArray[i] = bVal[i].ToString();
                                }
                                string sByteArray = String.Join(",", sArray);
                                XmlText oByteArray = xmlDoc.CreateTextNode(sByteArray);
                                oProp.AppendChild(oByteArray);
                                oProperties.AppendChild(oProp);
                                break;

                            case CimType.String:
                                oProp.SetAttribute("Type", PD.Type.ToString());
                                string[] oVal = PD.Value as string[];
                                foreach (string sValue in oVal)
                                {
                                    XmlElement oStringArray = xmlDoc.CreateElement(PD.Type.ToString());
                                    XmlText StringValue = xmlDoc.CreateTextNode(sValue);
                                    oStringArray.AppendChild(StringValue);
                                    oProp.AppendChild(oStringArray);
                                }
                                oProperties.AppendChild(oProp);
                                break;
                        }
                    }
                }
            }
            return oRoot;
        }

        /// <summary>
        /// Serialize all Properties of a ManagementBaseObject
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <param name="MO">ManagementBaseObject to serialize</param>
        /// <returns>An XMLNode with all ManagementBaseObject Properties</returns>
        private static XmlNode MBO2XML(XmlDocument xmlDoc, ManagementBaseObject MO)
        {
            XmlNode oRoot = xmlDoc.CreateElement(MO.SystemProperties["__CLASS"].Value.ToString().ToUpper());
            XmlElement oProperties = xmlDoc.CreateElement("Properties");

            oRoot.AppendChild(oProperties);

            foreach (PropertyData PD in MO.Properties)
            {
                if (PD.Value != null)
                {

                    if (!PD.IsArray & (PD.Value != null))
                    {
                        XmlElement oProp = xmlDoc.CreateElement(PD.Name);
                        XmlText TextValue = xmlDoc.CreateTextNode(PD.Value.ToString());
                        oProp.AppendChild(TextValue);
                        oProperties.AppendChild(oProp);
                    }
                    else
                    {
                        XmlElement oProp = xmlDoc.CreateElement(PD.Name);
                        switch (PD.Type)
                        {
                            case CimType.Object:
                                oProp.SetAttribute("Type", PD.Type.ToString());
                                object[] oObjects = PD.Value as Object[];
                                foreach (object oObj in oObjects)
                                {
                                    XmlNode oMBO = MBO2XML(xmlDoc, oObj as ManagementBaseObject);
                                    oProp.AppendChild(oMBO);
                                }
                                oProperties.AppendChild(oProp);
                                break;

                            case CimType.UInt8:
                                oProp.SetAttribute("Type", PD.Type.ToString());
                                byte[] bVal = PD.Value as byte[];
                                string[] sArray = new string[bVal.Length];
                                for (int i = 0; i <= bVal.Length - 1; i++)
                                {
                                    sArray[i] = bVal[i].ToString();
                                }
                                string sByteArray = String.Join(",", sArray);
                                XmlText oByteArray = xmlDoc.CreateTextNode(sByteArray);
                                oProp.AppendChild(oByteArray);
                                oProperties.AppendChild(oProp);
                                break;

                            case CimType.String:
                                oProp.SetAttribute("Type", PD.Type.ToString());
                                string[] oVal = PD.Value as string[];
                                foreach (string sValue in oVal)
                                {
                                    XmlElement oStringArray = xmlDoc.CreateElement(PD.Type.ToString());
                                    XmlText StringValue = xmlDoc.CreateTextNode(sValue);
                                    oStringArray.AppendChild(StringValue);
                                    oProp.AppendChild(oStringArray);
                                }
                                oProperties.AppendChild(oProp);
                                break;
                        }
                    }
                }
            }
            return oRoot;
        }

        /// <summary>
        /// Fillup XML Properties to a ManagementObject
        /// </summary>
        /// <param name="xPropNode">XML Property Node</param>
        /// <param name="EmptyMO">A empty ManagementObject of the required WMI Class</param>
        /// <returns>ManagementObject with all Properties set</returns>
        public static ManagementObject XML2MO(XmlNode xPropNode, ManagementObject EmptyMO)
        {
            foreach (XmlNode xProp in xPropNode.ChildNodes)
            {
                try
                {
                    if (xProp.Attributes.Count == 0)
                    {
                        EmptyMO.SetPropertyValue(xProp.Name, xProp.FirstChild.Value);
                    }
                    else
                    {
                        switch (xProp.Attributes["Type"].Value)
                        {
                            case "UInt8":
                                string sByteArray = xProp.FirstChild.Value;
                                string[] aSArray = sByteArray.Split(',');
                                EmptyMO.SetPropertyValue(xProp.Name, aSArray);
                                try
                                {
                                    EmptyMO.SetPropertyValue(xProp.Name + "Size", aSArray.Length);
                                }
                                catch { }
                                break;

                            case "Object":
                                int iCount2 = xProp.ChildNodes.Count;
                                Object[] oArray = new Object[iCount2];

                                ManagementScope oScope = EmptyMO.Scope;

                                for (int i = 0; i < iCount2; i++)
                                {
                                    XmlNode oClassNode = xProp.ChildNodes[i];
                                    ManagementClass MC = new ManagementClass(oScope, new ManagementPath(oClassNode.Name), new ObjectGetOptions());
                                    ManagementObject MO = XML2MO(oClassNode.FirstChild, MC.CreateInstance());
                                    oArray[i] = MO;
                                }

                                EmptyMO.SetPropertyValue(xProp.Name, oArray);
                                break;

                            case "String":
                                int iCount = xProp.ChildNodes.Count;
                                string[] aString = new string[iCount];

                                //Fill the String Array
                                for (int i = 0; i < iCount; i++)
                                {
                                    aString[i] = xProp.ChildNodes[i].FirstChild.Value;
                                }

                                EmptyMO.SetPropertyValue(xProp.Name, aString);
                                break;
                        }
                    }
                }
                catch { }
            }
            return EmptyMO;
        }
    }
}
