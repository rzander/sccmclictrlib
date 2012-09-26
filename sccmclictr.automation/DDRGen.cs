//SCCM Client Center Automation Library (SMSCliCtr.automation)
//Copyright (c) 2008 by Roger Zander

//This program is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or any later version. 
//This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details. 
//GNU General Public License: http://www.gnu.org/licenses/lgpl.html

using System;
using System.Collections.Generic;
using System.Web;
using System.Text;
using System.IO;
using System.Threading;
using System.Globalization;

namespace sccmclictr.automation
{
    /// <summary>
    /// Create a DataDiscoveryRecord (DDR) File
    /// </summary>
    public class DDRFile
    {
        internal StringBuilder sDDR = new StringBuilder();
        internal static string sArchitecture;
        internal static string sAgentName;
        internal static string sSiteCode;

        /// <summary>
        /// DDR Property Flags
        /// </summary>
        [Flags]
        public enum DDRPropertyFlagsEnum : int
        {
            /// <summary>
            /// Reserved.
            /// </summary>
            ADDPROP_AGENT = 32,
            /// <summary>
            /// Value is an Array value
            /// </summary>
            ADDPROP_ARRAY = 16,
            /// <summary>
            /// Reserved.
            /// </summary>
            ADDPROP_GROUPING = 4,
            /// <summary>
            /// Defines this property as being a GUID.
            /// </summary>
            ADDPROP_GUID = 2,
            /// <summary>
            /// Defines this property as being a Key value that must be unique.
            /// </summary>
            ADDPROP_KEY = 8,
            /// <summary>
            /// Specifies this property as the actual Name property in the resource.
            /// </summary>
            ADDPROP_NAME = 68,
            /// <summary>
            /// Specifies this property as the actual Comment property in the resource.
            /// </summary>
            ADDPROP_NAME2 = 132,
            /// <summary>
            /// replace existing values
            /// </summary>
            ADDPROP_REPLACE = 1,
            /// <summary>
            /// No special properties.
            /// </summary>
            ADDPROP_NONE = 0
        }

        /// <summary>
        /// Create a new DDR File Structure
        /// </summary>
        /// <param name="Architecture">Name of the Archtecture (like "System")</param>
        /// <param name="AgentName">Name of the Discovery Agent</param>
        /// <param name="SiteCode">3 Digit SMS Site Code</param>
        public DDRFile(string Architecture, string AgentName, string SiteCode)
        {
            sArchitecture = Architecture;
            sAgentName = AgentName;
            sSiteCode = SiteCode;
            sDDR.AppendLine(string.Format("<{0}>", sArchitecture));
            //sDDR.AppendLine(string.Format("AGENTINFO<{0}><{1}>", sAgentName, sSiteCode) + "<" + DateTime.Now.ToString() + ">");
        }

        /// <summary>
        /// Create the DDR File
        /// </summary>
        /// <param name="FileName">Full Path and Filname</param>
        public void DDRWrite(string FileName)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            sDDR.AppendLine(string.Format("AGENTINFO<{0}><{1}>", sAgentName, sSiteCode) + "<" + DateTime.Now.ToString("M/d/yyyy H:m:s") + ">");
            sDDR.Append("FEOF");
            int length = sDDR.Length + 17;
            //Binary DDR Header (mostly unknown)
            byte[] bHeader = { 1, 0, 216, 4, 0, 0, 70, 86, 160, 0, 0, 0, BitConverter.GetBytes(length)[0], BitConverter.GetBytes(length)[1], 0, 0 };
            //Binary DDR Footer (mostly unknown)
            byte[] bFooter = { 13, 0, 0, 70, 86 };
            FileStream FS = new FileStream(FileName, FileMode.Create);
            StreamWriter SW = new StreamWriter(FS, Encoding.Default);
            FS.Write(bHeader, 0, bHeader.Length);
            SW.Write(sDDR);
            SW.Flush();
            FS.Write(bFooter, 0, bFooter.Length);
            FS.Close();
            FS.Dispose();

        }

        internal void AddBegin()
        {
            sDDR.AppendLine("BEGIN_PROPERTY");
        }

        internal void AddEnd()
        {
            sDDR.AppendLine("END_PROPERTY");
        }

        internal void AddBeginArray()
        {
            sDDR.AppendLine("BEGIN_ARRAY_VALUES");
        }

        internal void AddEndArray()
        {
            sDDR.AppendLine("END_ARRAY_VALUES");
        }

        /// <summary>
        /// Add a string property
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        /// <param name="SQLWidth"></param>
        /// <param name="DDRPropertyFlag"></param>
        public void DDRAddString(string Name, string Value, int SQLWidth, DDRPropertyFlagsEnum DDRPropertyFlag)
        {
            if (Value.Length > SQLWidth)
                Value = Value.Substring(0, SQLWidth);
            AddBegin();
            object[] args = new object[5];
            args[0] = Name;
            args[1] = Value;
            args[2] = SQLWidth.ToString();
            args[3] = ((int)DDRPropertyFlag).ToString();
            args[4] = "11";
            sDDR.AppendLine(string.Format("<{3}><{0}><{4}><{2}><{1}>", args));
            AddEnd();
        }

        /// <summary>
        /// Add a string array property
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        /// <param name="SQLWidth"></param>
        /// <param name="DDRPropertyFlag"></param>
        public void DDRAddStringArray(string Name, object Value, int SQLWidth, DDRPropertyFlagsEnum DDRPropertyFlag)
        {
            AddBegin();
            object[] args = new object[5];
            args[0] = Name;
            args[1] = Value;
            args[2] = SQLWidth.ToString();
            args[3] = ((int)(DDRPropertyFlag | DDRPropertyFlagsEnum.ADDPROP_ARRAY)).ToString();
            args[4] = "11";

            object[] aValues = Value as object[];

            sDDR.AppendLine(string.Format("<{3}><{0}><{4}><{2}>", args));
            AddBeginArray();

            foreach (object obj in aValues)
            {
                sDDR.AppendLine(string.Format("<{0}>", obj.ToString()));
            }

            AddEndArray();
            AddEnd();
        }

        /// <summary>
        /// Add an Integer Property
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        /// <param name="DDRPropertyFlag"></param>
        public void DDRAddInteger(string Name, int Value, DDRPropertyFlagsEnum DDRPropertyFlag)
        {
            AddBegin();
            object[] args = new object[5];
            args[0] = Name;
            if (Value != null)
                args[1] = Value;
            else
                args[1] = "(null)";
            args[2] = 4; //Length
            args[3] = ((int)DDRPropertyFlag).ToString();
            args[4] = "8";

            sDDR.AppendLine(string.Format("<{3}><{0}><{4}><{2}><{1}>", args));
            AddEnd();
        }

        /// <summary>
        /// Add an Integer Array Property
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        /// <param name="DDRPropertyFlag"></param>
        public void DDRAddIntegerArray(string Name, object Value, DDRPropertyFlagsEnum DDRPropertyFlag)
        {
            AddBegin();
            object[] args = new object[5];
            args[0] = Name;
            args[1] = Value;
            args[2] = 4;
            args[3] = ((int)(DDRPropertyFlag | DDRPropertyFlagsEnum.ADDPROP_ARRAY)).ToString();
            args[4] = "8";

            object[] aValues = Value as object[];

            sDDR.AppendLine(string.Format("<{3}><{0}><{4}><{2}>", args));
            AddBeginArray();

            foreach (object obj in aValues)
            {
                sDDR.AppendLine(string.Format("<{0}>", obj.ToString()));
            }

            AddEndArray();
            AddEnd();
        }

        /// <summary>
        /// Add DateTime value (MM/DD/YY HH:MM:SS) 
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        /// <param name="DDRPropertyFlag"></param>
        public void DDRAddDateTime(string Name, DateTime Value, DDRPropertyFlagsEnum DDRPropertyFlag)
        {
            AddBegin();
            object[] args = new object[5];
            args[0] = Name;
            if (Value != null)
                args[1] = Value.ToString("MM/dd/yy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            else
                args[1] = "(null)";
            args[2] = 4; //Length
            args[3] = ((int)DDRPropertyFlag).ToString();
            args[4] = "12";

            sDDR.AppendLine(string.Format("<{3}><{0}><{4}><{2}><{1}>", args));
            AddEnd();
        }
    }
}
