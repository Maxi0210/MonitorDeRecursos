using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;
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
using LiveChartsCore.SkiaSharpView.Painting;
using System.Runtime.CompilerServices;
using System.Drawing;
using SkiaSharp;
using System.Security.Cryptography.X509Certificates;
using System.Printing.IndexedProperties;

namespace MonitorDeRecursos
{
    public partial class MainWindow : Window
    {
        public string c_btn;
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.Topmost = true;
#endif

            Main.Content = new CPU();

            var cpu = new CPU();
            cpu.D += cpubtn;
        }

        private void cpu_click(object sender, RoutedEventArgs e)
        {
            Main.Content = new CPU();
        }
        private void cpubtn(string cont)
        {
            cpu_btn.Content = cont;
        }

        private void ram_click(object sender, RoutedEventArgs e)
        {
            Main.Content = new RAM();
        }

        private void disk_click(object sender, RoutedEventArgs e) 
        {
        
        }

    }
}
