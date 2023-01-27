using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnRunLoadedWorkflow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UserControl1.RunWorkflow();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Errorr", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void fileMenuExit_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        private void fileMenuItem_Click_LoadWorkflow(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            if (openFileDialog.ShowDialog(this).Value)
            {
                UserControl1.WorkflowFilePath = openFileDialog.FileName;
                UserControl1.AddWorkflowDesigner();

            }
        }

        private void WFHost_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void btnSaveWorkflow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UserControl1.SaveWorkflow();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnNewWorkflow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UserControl1.NewWorkflow();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnOpenWorkflow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UserControl1.OpenWorkflow();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnFastRunWorkflow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UserControl1.FastRunWorkflow();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btnStopWorkflow_Click(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {

            }
        }

        private void btnBreakpointToggle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UserControl1.BreakPointToggle();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UserControl1._continue();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnStopDep_Click(object sender, RoutedEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
