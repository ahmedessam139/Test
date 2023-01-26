using System;
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

        const String DefultWorkflowFilePath = @"DefaultWorkflows\defaultWorkflow.xaml";
        public string WorkflowFilePath = @"DefaultWorkflows\defaultWorkflow.xaml";
        

        public UserControl1()
        {
            InitializeComponent();
            this.RegisterMetadata();
            this.AddWorkflowDesigner();
            this.AddToolBox();
        }
        private void RegisterMetadata()
        {
            (new DesignerMetadata()).Register();
        }
        public void AddWorkflowDesigner()
        {
            this.WorkflowDesigner = new WorkflowDesigner();



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
        
        #region New-Open-save - Clear

        public void SaveWorkflow()
        {
            if (WorkflowDesigner != null && (WorkflowFilePath != @"DefaultWorkflows\defaultWorkflow.xaml"))
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

            WorkflowFilePath = @"DefaultWorkflows\defaultWorkflow.xaml";

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
                    var writer = new StringWriter();
                    var workflow = ActivityXamlServices.Load(WorkflowFilePath);
                    var wa = new WorkflowApplication(workflow);
                    wa.Extensions.Add(writer);
                    wa.Run();


                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        #endregion

        public void ClearWorkflow()
        {
            if (WorkflowDesigner != null)
            {
                grid1.Children.Remove(WorkflowDesigner.View);
                grid1.Children.Remove(WorkflowDesigner.PropertyInspectorView);
                WorkflowDesigner = null;
            }
        }

    }
}


