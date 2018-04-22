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
using Microsoft.Kinect;
using System.IO;
using System.Windows.Threading;
using System.Globalization;

namespace ExercisesPerformanceControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Adding exercises data from files to listBoxItem
            string[] filePaths = Directory.GetFiles(@"ExercisesData\", "*.xrs", SearchOption.TopDirectoryOnly);
            foreach (var filePath in filePaths)
            {
                ListBoxItem itm = new ListBoxItem();
                itm.Content = System.IO.Path.GetFileNameWithoutExtension(filePath);
                this.ListOfExrcs.Items.Add(itm);
            }
        }

        private void ChooseExBtn_Click(object sender, RoutedEventArgs e)
        {
            ExerciseControl page = new ExerciseControl();
            dynamic tmp = this.ListOfExrcs.SelectedItem;
            page.ExName = tmp.Content;
            page.ShowDialog();
        }

        private void RecExBtn_Click(object sender, RoutedEventArgs e)
        {
            ExerciseRecording page = new ExerciseRecording();
            page.ShowDialog();
        }
    }
}
