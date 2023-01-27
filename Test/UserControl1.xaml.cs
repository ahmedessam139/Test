﻿using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Activities.Presentation;
using System.Activities.Presentation.Debug;
using System.Activities.Core.Presentation;
using System.Activities;
using System.Activities.Debugger;
using System.Activities.Presentation.Services;
using System.Activities.Tracking;
using System.Windows.Threading;
using System.Threading;
using System.Windows;
using System.Activities.XamlIntegration;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.View;
using System.Windows.Input;
using System.Activities.Presentation.Toolbox;
using System.Activities.Statements;
using System.Reflection;
using System.Linq;
using System.ServiceModel.Activities;
using System.ServiceModel.Activities.Presentation.Factories;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;

namespace Test
{
    
    public partial class UserControl1 : UserControl
    {


        public WorkflowDesigner WorkflowDesigner;
        public IDesignerDebugView DebuggerService;


        const String DefultWorkflowFilePath = @"DefaultWorkflows\defaultWorkflow.xaml";
        public string WorkflowFilePath = DefultWorkflowFilePath;

        TextBox logsTxtbox;

        Dictionary<int, SourceLocation> textLineToSourceLocationMap;
        Dictionary<object, SourceLocation> designerSourceLocationMapping = new Dictionary<object, SourceLocation>();
        Dictionary<object, SourceLocation> wfElementToSourceLocationMap = null;

        AutoResetEvent resumeRuntimeFromHost = null;
        List<SourceLocation> breakpointList = new List<SourceLocation>();
        

        public UserControl1()
        {
            InitializeComponent();
            this.RegisterMetadata();
            this.AddWorkflowDesigner();
            this.AddToolBox();
        }
        #region Adding Workflow - Toolbox
        private void RegisterMetadata()
        {
            (new DesignerMetadata()).Register();
        }
        public void AddWorkflowDesigner()
        {
            this.WorkflowDesigner = new WorkflowDesigner();
            this.DebuggerService = this.WorkflowDesigner.DebugManagerView;



            if (!string.IsNullOrEmpty(WorkflowFilePath))
            {

                this.WorkflowDesigner.Load(WorkflowFilePath);

            }
           
            
            this.workflowDesignerPanel.Content = this.WorkflowDesigner.View;
            this.AddPropertyInspector();


        }
        private ToolboxControl GetToolboxControl()
        {


            ToolboxControl toolboxControl = new ToolboxControl();



            //AppDomain.CurrentDomain.Load("System.Activities");
            //AppDomain.CurrentDomain.Load("System.ServiceModel.Activities");
            //AppDomain.CurrentDomain.Load("System.Activities.Core.Presentation");




            // get all loaded assemblies
            /* IEnumerable<Assembly> appAssemblies = AppDomain.CurrentDomain.GetAssemblies().OrderBy(a => a.GetName().Name);

             // check if assemblies contain activities
             int activitiesCount = 0;
             foreach (Assembly activityLibrary in appAssemblies)
             {
                 var wfToolboxCategory = new ToolboxCategory(activityLibrary.GetName().Name);
                 var actvities = from
                                     activityType in activityLibrary.GetExportedTypes()
                                 where
                                     (activityType.IsSubclassOf(typeof(Activity))
                                     || activityType.IsSubclassOf(typeof(NativeActivity))
                                     || activityType.IsSubclassOf(typeof(DynamicActivity))
                                     || activityType.IsSubclassOf(typeof(ActivityWithResult))
                                     || activityType.IsSubclassOf(typeof(AsyncCodeActivity))
                                     || activityType.IsSubclassOf(typeof(CodeActivity))
                                     || activityType == typeof(System.Activities.Core.Presentation.Factories.ForEachWithBodyFactory<Type>)
                                     || activityType == typeof(System.Activities.Statements.FlowNode)
                                     || activityType == typeof(System.Activities.Statements.State)
                                     || activityType == typeof(System.Activities.Core.Presentation.FinalState)
                                     || activityType == typeof(System.Activities.Statements.FlowDecision)
                                     || activityType == typeof(System.Activities.Statements.FlowNode)
                                     || activityType == typeof(System.Activities.Statements.FlowStep)
                                     || activityType == typeof(System.Activities.Statements.FlowSwitch<Type>)
                                     || activityType == typeof(System.Activities.Statements.ForEach<Type>)
                                     || activityType == typeof(System.Activities.Statements.Switch<Type>)
                                     || activityType == typeof(System.Activities.Statements.TryCatch)
                                     || activityType == typeof(System.Activities.Statements.While))
                                     && activityType.IsVisible
                                     && activityType.IsPublic
                                     && !activityType.IsNested
                                     && !activityType.IsAbstract
                                     && (activityType.GetConstructor(Type.EmptyTypes) != null)
                                     && !activityType.Name.Contains('`')                                 //optional, for extra cleanup
                                 orderby
                                     activityType.Name
                                 select
                                     new ToolboxItemWrapper(activityType);

                 actvities.ToList().ForEach(wfToolboxCategory.Add);

                 if (wfToolboxCategory.Tools.Count > 0)
                 {
                     toolboxControl.Categories.Add(wfToolboxCategory);
                     activitiesCount += wfToolboxCategory.Tools.Count;
                 }
             }*/
            //fixed ForEach


            //fixid activites




            toolboxControl.Categories.Add(new ToolboxCategory("Basic Activities")
            {
                new ToolboxItemWrapper(typeof(Sequence)),
                new ToolboxItemWrapper(typeof(WriteLine)),
                new ToolboxItemWrapper(typeof(Assign)),
            });

            toolboxControl.Categories.Add(new ToolboxCategory("Control Flow Activities")
            {
                new ToolboxItemWrapper(typeof(Flowchart)),
                new ToolboxItemWrapper(typeof(FlowSwitch<>)),
                new ToolboxItemWrapper(typeof(FlowDecision)),
                new ToolboxItemWrapper(typeof(Parallel)),
                new ToolboxItemWrapper(typeof(TransactionScope)),
                new ToolboxItemWrapper(typeof(While)),
                new ToolboxItemWrapper(typeof(DoWhile)),
                new ToolboxItemWrapper(typeof(ForEach<>)),
                new ToolboxItemWrapper(typeof(ParallelForEach<>)),
                new ToolboxItemWrapper(typeof(TryCatch)),
                new ToolboxItemWrapper(typeof(Rethrow)),
                new ToolboxItemWrapper(typeof(Delay)),
                new ToolboxItemWrapper(typeof(If)),
                new ToolboxItemWrapper(typeof(Throw))
            });

            toolboxControl.Categories.Add(new ToolboxCategory("Collection Activities")
            {
                new ToolboxItemWrapper(typeof(AddToCollection<>)),
                new ToolboxItemWrapper(typeof(ClearCollection<>)),
                new ToolboxItemWrapper(typeof(RemoveFromCollection<>)),
                new ToolboxItemWrapper(typeof(ExistsInCollection<>))
            });


            toolboxControl.Categories.Add(new ToolboxCategory("Error Handling Activities")
            {
                new ToolboxItemWrapper(typeof(TransactionScope)),
                new ToolboxItemWrapper(typeof(TryCatch)),
                new ToolboxItemWrapper(typeof(Rethrow)),
                new ToolboxItemWrapper(typeof(Throw))
            });


            return toolboxControl;
        }
        private void AddToolBox()
        {
            ToolboxControl tc = GetToolboxControl();
            this.toolboxPanel.Content = tc;
        }
        private void AddPropertyInspector()
        {
            if (this.WorkflowDesigner == null)
                return;

            this.WorkflowPropertyPanel.Content = this.WorkflowDesigner.PropertyInspectorView;
        }

        private void TabItem_GotFocus_RefreshXamlBox(object sender, RoutedEventArgs e)
        {

            this.WorkflowDesigner.Flush(); 
            xamlTextBox.Text = this.WorkflowDesigner.Text;

        }
        #endregion
        
        #region New-Open-save - Clear

        public void SaveWorkflow()
        {
            if (WorkflowDesigner != null && (WorkflowFilePath !=DefultWorkflowFilePath))
            {
                WorkflowDesigner.Flush();
                WorkflowDesigner.Save(WorkflowFilePath);
            }
            else
            {
                string dummyFileName = "New_WF";
                SaveFileDialog sf = new SaveFileDialog();
                // Feed the dummy name to the save dialog
                sf.Filter = "XAML files(.xaml)|*.xaml";
                sf.FilterIndex = 2;
                sf.FileName = dummyFileName;

                if (sf.ShowDialog() == true)
                {
                    // Now here's our save folder
                    string savePath = System.IO.Path.GetDirectoryName(sf.FileName);
                    
                    // Do whatever
                    WorkflowFilePath = sf.FileName;
                    WorkflowDesigner.Flush();
                    WorkflowDesigner.Save(WorkflowFilePath);


                }
            }
        }

        public void NewWorkflow()
        {
            ClearWorkflow();

            WorkflowFilePath =DefultWorkflowFilePath;

            AddWorkflowDesigner();
        }

        public void OpenWorkflow()
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "XAML files(.xaml)|*.xaml";

            if (dlg.ShowDialog() == true)
            {
                WorkflowFilePath = dlg.FileName;
                ClearWorkflow();
                AddWorkflowDesigner();
            }
        }

        public void FastRunWorkflow()
        {
            if (!string.IsNullOrEmpty(WorkflowFilePath))
            {
                try
                {
                    SaveWorkflow();
                    var workflow = ActivityXamlServices.Load(WorkflowFilePath);
                    var wa = new WorkflowApplication(workflow);
                    wa.Run();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        #endregion

        #region debuging Runner 

        public void RunWorkflow()
        {

            WorkflowDesigner.Flush();
            SaveWorkflow();
            logsTxtbox.Text = String.Empty;
            AddWorkflowDesigner();
            wfElementToSourceLocationMap = UpdateSourceLocationMappingInDebuggerService();


            WorkflowInvoker instance = new WorkflowInvoker(GetRuntimeExecutionRoot());
            resumeRuntimeFromHost = new AutoResetEvent(false);

            Dictionary<string, Activity> activityIdToWfElementMap = BuildActivityIdToWfElementMap(wfElementToSourceLocationMap);

            #region Set up the Custom Tracking
            const String all = "*";
            SimulatorTrackingParticipant simTracker = new SimulatorTrackingParticipant()
            {
                TrackingProfile = new TrackingProfile()
                {
                    Name = "CustomTrackingProfile",
                    Queries =
                    {
                        new CustomTrackingQuery()
                        {
                            Name = all,
                            ActivityName = all
                        },
                        new WorkflowInstanceQuery()
                        {
                            // Limit workflow instance tracking records for started and completed workflow states
                            States = { WorkflowInstanceStates.Started, WorkflowInstanceStates.Completed, WorkflowInstanceStates.Idle },
                        },
                        new ActivityStateQuery()
                        {
                            // Subscribe for track records from all activities for all states
                            ActivityName = all,
                            States = { all },

                            // Extract workflow variables and arguments as a part of the activity tracking record
                            // VariableName = "*" allows for extraction of all variables in the scope
                            // of the activity
                            Variables =
                            {
                                { all }
                            }
                        }
                    }
                }
            };

            simTracker.ActivityIdToWorkflowElementMap = activityIdToWfElementMap;

            #endregion

            //As the tracking events are received
            simTracker.TrackingRecordReceived += (trackingParticpant, trackingEventArgs) =>
            {
                if (trackingEventArgs.Activity != null)
                {
                    System.Diagnostics.Debug.WriteLine(
                        String.Format("<+=+=+=+> Activity Tracking Record Received for ActivityId: {0}, record: {1} ",
                        trackingEventArgs.Activity.Id,
                        trackingEventArgs.Record
                        )
                    );

                    ShowDebug(wfElementToSourceLocationMap[trackingEventArgs.Activity]);

                    this.Dispatcher.Invoke(DispatcherPriority.SystemIdle, (Action)(() =>
                    {
                        //Textbox Updates
                        logsTxtbox.AppendText(trackingEventArgs.Activity.Id + " " + trackingEventArgs.Activity.DisplayName + " " + ((ActivityStateRecord)trackingEventArgs.Record).State + "\n");
                        logsTxtbox.AppendText("Instance ID: " + ((ActivityStateRecord)trackingEventArgs.Record).InstanceId + "\n");
                        logsTxtbox.AppendText("Activity: " + ((ActivityStateRecord)trackingEventArgs.Record).Activity.Name + "\n");
                        logsTxtbox.AppendText("Level: " + ((ActivityStateRecord)trackingEventArgs.Record).Level + "\n");
                        logsTxtbox.AppendText("Time: " + ((ActivityStateRecord)trackingEventArgs.Record).EventTime + "\n");
                        logsTxtbox.AppendText("************************\n");


                        //Add a sleep so that the debug adornments are visible to the user
                        System.Threading.Thread.Sleep(500);
                    }));
                }
            };

            instance.Extensions.Add(simTracker);
            ThreadPool.QueueUserWorkItem(new WaitCallback((context) =>
            {
                //Start the Runtime
                instance.Invoke();// new TimeSpan(1,0,0));

                //This is to remove the final debug adornment
                this.Dispatcher.Invoke(DispatcherPriority.Render
                    , (Action)(() =>
                    {
                        this.WorkflowDesigner.DebugManagerView.CurrentLocation = new SourceLocation(WorkflowFilePath, 1, 1, 1, 10);
                    }));

            }));


        }


        #endregion

        #region Helper Methods

        public void ClearWorkflow()
        {
            if (WorkflowDesigner != null)
            {
                grid1.Children.Remove(WorkflowDesigner.View);
                grid1.Children.Remove(WorkflowDesigner.PropertyInspectorView);
                WorkflowDesigner = null;
            }
        }
        void ShowDebug(SourceLocation srcLoc)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Render
                , (Action)(() =>
                {

                    this.WorkflowDesigner.DebugManagerView.CurrentLocation = srcLoc;

                }));

            //Check if this is where any Breakpoint is set
            bool isBreakpointHit = false;
            foreach (SourceLocation src in breakpointList)
            {
                if (src.StartLine == srcLoc.StartLine && src.EndLine == srcLoc.EndLine)
                {
                    isBreakpointHit = true;
                }
            }

            if (isBreakpointHit == true)
            {
                resumeRuntimeFromHost.WaitOne();
            }

        }



        private Dictionary<string, Activity> BuildActivityIdToWfElementMap(Dictionary<object, SourceLocation> wfElementToSourceLocationMap)
        {
            Dictionary<string, Activity> map = new Dictionary<string, Activity>();

            Activity wfElement;
            foreach (object instance in wfElementToSourceLocationMap.Keys)
            {
                wfElement = instance as Activity;
                if (wfElement != null)
                {
                    map.Add(wfElement.Id, wfElement);
                }
            }

            return map;

        }


        Dictionary<object, SourceLocation> UpdateSourceLocationMappingInDebuggerService()
        {
            object rootInstance = GetRootInstance();
            Dictionary<object, SourceLocation> sourceLocationMapping = new Dictionary<object, SourceLocation>();


            if (rootInstance != null)
            {
                Activity documentRootElement = GetRootWorkflowElement(rootInstance);

                SourceLocationProvider.CollectMapping(GetRootRuntimeWorkflowElement(), documentRootElement, sourceLocationMapping,
                    this.WorkflowDesigner.Context.Items.GetValue<WorkflowFileItem>().LoadedFile);

                //Collect the mapping between the Model Item tree and its underlying source location
                SourceLocationProvider.CollectMapping(documentRootElement, documentRootElement, designerSourceLocationMapping,
                   this.WorkflowDesigner.Context.Items.GetValue<WorkflowFileItem>().LoadedFile);

            }

            // Notify the DebuggerService of the new sourceLocationMapping.
            // When rootInstance == null, it'll just reset the mapping.
            //DebuggerService debuggerService = debuggerService as DebuggerService;
            if (this.DebuggerService != null)
            {
                ((DebuggerService)this.DebuggerService).UpdateSourceLocations(designerSourceLocationMapping);
            }

            return sourceLocationMapping;
        }


        object GetRootInstance()
        {
            ModelService modelService = this.WorkflowDesigner.Context.Services.GetService<ModelService>();
            if (modelService != null)
            {
                return modelService.Root.GetCurrentValue();
            }
            else
            {
                return null;
            }
        }


        // Get root WorkflowElement.  Currently only handle when the object is ActivitySchemaType or WorkflowElement.
        // May return null if it does not know how to get the root activity.
        Activity GetRootWorkflowElement(object rootModelObject)
        {
            System.Diagnostics.Debug.Assert(rootModelObject != null, "Cannot pass null as rootModelObject");

            Activity rootWorkflowElement;
            IDebuggableWorkflowTree debuggableWorkflowTree = rootModelObject as IDebuggableWorkflowTree;
            if (debuggableWorkflowTree != null)
            {
                rootWorkflowElement = debuggableWorkflowTree.GetWorkflowRoot();
            }
            else // Loose xaml case.
            {
                rootWorkflowElement = rootModelObject as Activity;
            }
            return rootWorkflowElement;
        }

        Activity GetRuntimeExecutionRoot()
        {

            Activity root = ActivityXamlServices.Load(WorkflowFilePath);
            WorkflowInspectionServices.CacheMetadata(root);

            return root;
        }


        Activity GetRootRuntimeWorkflowElement()
        {

            Activity root = ActivityXamlServices.Load(WorkflowFilePath);
            WorkflowInspectionServices.CacheMetadata(root);


            IEnumerator<Activity> enumerator1 = WorkflowInspectionServices.GetActivities(root).GetEnumerator();
            //Get the first child of the x:class
            enumerator1.MoveNext();
            root = enumerator1.Current;
            return root;
        }
        void AddTrackingTextbox()
        {
            Grid.SetRow(logsTxtbox, 1);
            this.TrackingRecord.Children.Add(logsTxtbox);
        }

        #endregion

        #region Debugging controls


        public void BreakPointToggle()
        {
            ModelItem mi = this.WorkflowDesigner.Context.Items.GetValue<Selection>().PrimarySelection;
            Activity a = mi.GetCurrentValue() as Activity;

            if (a != null)
            {
                SourceLocation srcLoc = designerSourceLocationMapping[a];
                if (!breakpointList.Contains(srcLoc))
                {
                    this.WorkflowDesigner.Context.Services.GetService<IDesignerDebugView>().UpdateBreakpoint(srcLoc, BreakpointTypes.Bounded | BreakpointTypes.Enabled);
                    breakpointList.Add(srcLoc);
                }
                else
                {
                    this.WorkflowDesigner.Context.Services.GetService<IDesignerDebugView>().UpdateBreakpoint(srcLoc, BreakpointTypes.None);
                    breakpointList.Remove(srcLoc);
                }
            }


        }

        public void _continue()
        {
            resumeRuntimeFromHost.Set();
        }

        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {

            if (e.Key == Key.F9)
            {
                BreakPointToggle();
            }
            else if (e.Key == Key.F5)
            {
                _continue();
            }
        }




        #endregion

    }
}


