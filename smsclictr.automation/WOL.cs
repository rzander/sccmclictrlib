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


namespace smsclictr.automation
{
    /// <summary>
    /// WakeOnLan (WOL) Functions
    /// </summary>
    public class WOL
    {
        internal class WOLClass : UdpClient
        {
            internal WOLClass()
                : base()
            { }
            //this is needed to send broadcast packet
            internal void SetClientToBrodcastMode()
            {
                if (this.Active)
                    this.Client.SetSocketOption(SocketOptionLevel.Socket,
                                              SocketOptionName.Broadcast, 0);
            }
        }

        /// <summary>
        /// Send a WakeOnLan command 
        /// <a href="#" onclick='javascript: window.open("http://www.codeproject.com/cs/internet/cswol.asp" );'>Found at: www.codeproject.com</a>
        /// </summary>
        /// <param name="MAC_ADDRESS">MAC Address</param>
        /// <remarks>remove all delimiters or other special characters from the MAC_ADDRESS string</remarks>
        /// <example>
        /// C#:
        /// <code>
        /// using System;
        /// using System.Collections.Generic;
        /// using System.Text;
        /// using smsclictr.automation;
        /// using System.Management;
        /// 
        /// namespace ConsoleApplication1
        /// {
        ///    class Program
        ///      {
        ///         static void Main(string[] args)
        ///             {
        ///                WOL.WakeUp("000BCDBFA1DE");
        ///             }
        ///       }
        /// }
        /// </code>
        /// </example>
        static public void WakeUp(string MAC_ADDRESS)
        {
            WakeUp(new IPAddress(0xffffffff), 0x2fff,  MAC_ADDRESS);
        }

        /// <summary>
        /// Send a WakeOnLan command 
        /// </summary>
        /// <param name="IPAddr">Destination IP Address (e.g. 255.255.255.255 = broadcast)</param>
        /// <param name="Port">UDP Port</param>
        /// <param name="MAC_ADDRESS">MAC Address to wakeup</param>
        static public void WakeUp(IPAddress IPAddr, int Port, string MAC_ADDRESS)
        {
            try
            {
                WOLClass client = new WOLClass();
                Regex oRegex = new Regex("[^a-fA-F0-9]");
                MAC_ADDRESS = oRegex.Replace(MAC_ADDRESS, "");
                client.Connect(IPAddr,  //255.255.255.255  i.e broadcast
                   Port); // port=12287 let's use this one 
                client.SetClientToBrodcastMode();
                //set sending bites
                int counter = 0;
                //buffer to be send
                byte[] bytes = new byte[1024];   // more than enough :-)
                //first 6 bytes should be 0xFF
                for (int y = 0; y < 6; y++)
                    bytes[counter++] = 0xFF;
                //now repeate MAC 16 times
                for (int y = 0; y < 16; y++)
                {
                    int i = 0;
                    for (int z = 0; z < 6; z++)
                    {
                        bytes[counter++] =
                            byte.Parse(MAC_ADDRESS.Substring(i, 2),
                            NumberStyles.HexNumber);
                        i += 2;
                    }
                }

                //now send wake up packet
                int reterned_value = client.Send(bytes, 1024);
            }
            catch { }
        }
    }
}
