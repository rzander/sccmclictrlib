//SCCM Client Center Automation Library (SMSCliCtr.automation)
//Copyright (c) 2008 by Roger Zander

//This program is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or any later version. 
//This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details. 
//GNU General Public License: http://www.gnu.org/licenses/lgpl.html

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Diagnostics;


namespace smsclictr.automation
{
    /// <summary>
    /// AppV functions
    /// </summary>
    public static class AppV
    {
        /// <summary>
        /// Check if current process is running in AppV
        /// </summary>
        /// <returns>true = running in App-V</returns>
        public static bool isAppV()
        {
            try
            {
                string sParent = Process.GetCurrentProcess().Parent().ProcessName.ToString();
                if (string.Compare(sParent, "sfttray.exe", true) == 0)
                {
                    return true;
                }
            }
            catch { }

            return false;
        }
    }

    /// <summary>
    /// Code from: http://stackoverflow.com/questions/394816/how-to-get-parent-process-in-net-in-managed-way
    /// </summary>
    internal static class ProcessExtensions
    {
        private static string FindIndexedProcessName(int pid)
        {
            var processName = Process.GetProcessById(pid).ProcessName;
            var processesByName = Process.GetProcessesByName(processName);
            string processIndexdName = null;

            for (var index = 0; index < processesByName.Length; index++)
            {
                processIndexdName = index == 0 ? processName : processName + "#" + index;
                var processId = new PerformanceCounter("Process", "ID Process", processIndexdName);
                if ((int)processId.NextValue() == pid)
                {
                    return processIndexdName;
                }
            }

            return processIndexdName;
        }

        private static Process FindPidFromIndexedProcessName(string indexedProcessName)
        {
            var parentId = new PerformanceCounter("Process", "Creating Process ID", indexedProcessName);
            return Process.GetProcessById((int)parentId.NextValue());
        }

        public static Process Parent(this Process process)
        {
            return FindPidFromIndexedProcessName(FindIndexedProcessName(process.Id));
        }
    }
}
