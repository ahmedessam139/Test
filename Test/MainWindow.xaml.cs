using System;
using System.Collections.Generic;
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

        }

        private void fileMenuExit_Click(object sender, RoutedEventArgs e)
        {
        }

        private void fileMenuItem_Click_LoadWorkflow(object sender, RoutedEventArgs e)
        {

        }


        private void btnSaveWorkflow_Click(object sender, RoutedEventArgs e)
        {
            UserControl1.SaveWorkflow();
        }

        private void btnNewWorkflow_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnOpenWorkflow_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnFastRunWorkflow_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnBreakpointToggle_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnStopDep_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
