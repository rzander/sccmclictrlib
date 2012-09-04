//SCCM Client Center Automation Library (SCCMCliCtr.automation)
//Copyright (c) 2011 by Roger Zander

//This program is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; either version 3 of the License, or any later version. 
//This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details. 
//GNU General Public License: http://www.gnu.org/licenses/lgpl.html

#define CM2012
#define CM2007

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sccmclictr.automation;
using System.Management;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Diagnostics;
using System.Web;
using System.Threading;

using System.Collections.ObjectModel;


namespace sccmclictr.automation.functions
{
    public class monitoring : baseInit
    {
        internal Runspace remoteRunspace;
        internal TraceSource pSCode;
        internal ccm baseClient;

        //Constructor
        public monitoring(Runspace RemoteRunspace, TraceSource PSCode, ccm oClient)
            : base(RemoteRunspace, PSCode)
        {
            remoteRunspace = RemoteRunspace;
            pSCode = PSCode;
            baseClient = oClient;
            AsynchronousScript = new runScriptAsync(RemoteRunspace);
        }

        public runScriptAsync AsynchronousScript;

        public class runScriptAsync
        {
            private AutoResetEvent _autoResetEvent = new AutoResetEvent(false);
            internal Runspace _remoteRunspace;
            internal Pipeline pipeline;
            internal RunspaceConnectionInfo _connectionInfo;

            public runScriptAsync(Runspace remoteRunspace)
            {
                if (_remoteRunspace == null)
                {
                    _connectionInfo = remoteRunspace.ConnectionInfo;
                    _remoteRunspace = RunspaceFactory.CreateRunspace(_connectionInfo);
                }
                else
                {
                    _remoteRunspace = RunspaceFactory.CreateRunspace(remoteRunspace.ConnectionInfo);
                }
            }

            public void Connect()
            {
                if (_remoteRunspace.RunspaceStateInfo.State != RunspaceState.Opened)
                {
                    _remoteRunspace = RunspaceFactory.CreateRunspace(_connectionInfo);
                    _remoteRunspace.Open();
                }

                pipeline = _remoteRunspace.CreatePipeline();

                pipeline.Output.DataReady += new EventHandler(Output_DataReady);
                pipeline.StateChanged += new EventHandler<PipelineStateEventArgs>(pipeline_StateChanged);
            }

            void pipeline_StateChanged(object sender, PipelineStateEventArgs e)
            {
                if (e.PipelineStateInfo.State != PipelineState.Running)
                {
                    pipeline.Output.Close();
                    pipeline.Input.Close();
                }

                if (e.PipelineStateInfo.State == PipelineState.Completed)
                {
                    if (this.Finished != null)
                        this.Finished.Invoke(pipeline.Output, new EventArgs());
                }

                _autoResetEvent.Set();
            }

            /// <summary>
            ///  Output data arrived
            /// </summary>
            /// <param name="sender">contains the result as List of strings</string></param>
            /// <param name="e"></param>
            internal void Output_DataReady(object sender, EventArgs e)
            {
                PipelineReader<PSObject> output = sender as PipelineReader<PSObject>;
                List<string> lStringOutput = new List<string>();
                List<object> lOutput = new List<object>();
                if (output != null)
                {
                    Collection<PSObject> pso = output.NonBlockingRead();
                    if (pso.Count > 0)
                    {
                        //Forward the Raw data...
                        if (this.RawOutput != null)
                            this.RawOutput.Invoke(pso, e);

                        foreach (PSObject PO in pso)
                        {
                            if (PO != null)
                            {
                                lStringOutput.Add(PO.ToString());


                                foreach (string sType in PO.TypeNames)
                                {
                                    ConvertThroughString cts = new ConvertThroughString();
                                    Type objectType = Type.GetType(sType.Replace("Deserialized.", ""));

                                    if (cts.CanConvertFrom(PO, objectType))
                                    {
                                        try
                                        {
                                            lOutput.Add(cts.ConvertFrom(PO, objectType, null, true));
                                            break;
                                        }
                                        catch (Exception ex)
                                        {
                                            try
                                            {
                                                System.Collections.Hashtable HT = new System.Collections.Hashtable();
                                                foreach (PSPropertyInfo PI in PO.Properties)
                                                {
                                                    try
                                                    {
                                                        HT.Add(PI.Name, PI.Value.ToString());
                                                    }
                                                    catch { }
                                                }
                                                lOutput.Add(HT);
                                                break;
                                            }
                                            catch { }
                                            //break;
                                        }
                                    }
                                    else
                                    {
                                    }

                                }
                            }
                        }
                    }

                    if (output.EndOfPipeline & output.IsOpen)
                    {
                        output.Close();
                    }
                }
                else
                {
                    PipelineReader<object> error = sender as PipelineReader<object>;

                    if (error != null)
                    {
                        while (error.Count > 0)
                        {
                            lStringOutput.Add(error.Read().ToString());
                            lOutput.Add(error.Read());
                        }

                        if (error.EndOfPipeline)
                        {
                            error.Close();
                        }

                        if (this.ErrorOccured != null)
                            this.ErrorOccured.Invoke(sender, e);

                        _autoResetEvent.Set();
                    }
                }

                //Forward output as ListOfStrings
                if (this.StringOutput != null)
                    this.StringOutput.Invoke(lStringOutput, e);
                if (this.TypedOutput != null)
                    this.TypedOutput.Invoke(lOutput, e);
                _autoResetEvent.Set();
            }

            /// <summary>
            /// Powershell Script to execute
            /// </summary>
            public string Command
            {
                get
                {
                    if (pipeline == null)
                        this.Connect();
                    return pipeline.Commands.ToString();
                }
                set
                {
                    if (pipeline == null)
                        this.Connect();
                    pipeline.Commands.Clear();
                    pipeline.Commands.AddScript(value);
                }

            }

            public void Run()
            {
                if (pipeline == null)
                    this.Connect();
                pipeline.InvokeAsync();
                pipeline.Input.Close();
            }

            public void RunWait()
            {
                if (pipeline == null)
                    this.Connect();
                pipeline.InvokeAsync();
                pipeline.Input.Close();
                do
                {
                    _autoResetEvent.WaitOne(500);
                }
                while (pipeline.Output.IsOpen);

            }

            /// <summary>
            /// Stop the pieline reader
            /// </summary>
            public void Stop()
            {
                if (pipeline != null)
                {
                    if (pipeline.Output.IsOpen)
                        pipeline.Output.Close();
                }
            }

            /// <summary>
            /// Check if PS output is open..
            /// </summary>
            public bool isRunning
            {
                get
                {
                    if (pipeline != null)
                        if (pipeline.Output != null)
                            return pipeline.Output.IsOpen;
                    return false;
                }
            }

            /// <summary>
            /// Close the pipeline
            /// </summary>
            public void Close()
            {
                try
                {
                    if (pipeline != null)
                    {
                        if (pipeline.Output.IsOpen)
                            pipeline.Output.Close();

                        pipeline.Output.DataReady -= Output_DataReady;
                        pipeline.StateChanged -= pipeline_StateChanged;

                        _remoteRunspace.Close();
                    }
                }
                catch { }
            }

            public event EventHandler StringOutput;
            public event EventHandler RawOutput;
            public event EventHandler TypedOutput;
            public event EventHandler Finished;
            public event EventHandler ErrorOccured;


        }
    }


}
