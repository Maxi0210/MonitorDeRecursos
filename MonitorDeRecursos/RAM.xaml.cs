using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Management;
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

namespace MonitorDeRecursos
{
    /// <summary>
    /// Lógica de interacción para UserControl1.xaml
    /// </summary>
    public partial class RAM : UserControl
    {
        private ObservableCollection<double> R_Values;

        public Timer T;

        public double P_ram;

        public ISeries[] R_Series { get; set; }
        public Axis[] YAxes { get; set; }
        public Axis[] XAxes { get; set; }
        public RAM()
        {
            InitializeComponent();
            R_Values = new ObservableCollection<double>();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {

                var NVioletBrush = (SolidColorBrush)Application.Current.Resources["NeonVioletBrush"];
                var SNVioletBrush = ConvertToSKColor(NVioletBrush.Color);

                var searcher = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize FROM Win32_OperatingSystem");
                UInt64 total = 1;
                foreach (ManagementObject obj in searcher.Get())
                {
                    total = Convert.ToUInt64(obj["TotalVisibleMemorySize"]) / 1024;
                }

                R_Series = new ISeries[]
                {
                    new LineSeries<double>
                    {
                        Values = R_Values,
                        Fill = new SolidColorPaint(SNVioletBrush.WithAlpha(20)),
                        LineSmoothness = 0,
                        GeometrySize = 0,
                        Stroke = new SolidColorPaint
                        {
                            Color = SkiaSharp.SKColor.Parse("#9D00FF"),
                            StrokeThickness = 2
                        }
                    }
                };

                YAxes = new Axis[]
                {
                    new Axis
                    {
                        MinLimit = 0,
                        MaxLimit = total,
                        MinStep = total/2,
                        SeparatorsPaint = new SolidColorPaint
                        {
                            Color = new SkiaSharp.SKColor(255, 255, 255, 30),
                            StrokeThickness = 2
                        }
                    }
                };
                XAxes = new Axis[] { new Axis { IsVisible = false } };

                T = new Timer(1000); //tiempo de actualizacion (en  milisegundos)
                T.Elapsed += act_r;
                T.Start();

                DataContext = this;
            }
        }

        public void act_r (object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                UInt64 T_ram = 1;
                UInt64 F_ram = 1;

                var searcher = new ManagementObjectSearcher("SELECT FreePhysicalMemory, TotalVisibleMemorySize FROM Win32_OperatingSystem"); //le digo "che dame estos 2 datos desde win32_OS"
                foreach (ManagementObject obj in searcher.Get())
                {
                    F_ram = Convert.ToUInt64(obj["FreePhysicalMemory"]) / 1024;       // estos son los valores de la ram libre y
                    T_ram = Convert.ToUInt64(obj["TotalVisibleMemorySize"]) / 1024;  // la total, como vienen en kb los convierto en mb
                }
                double used = T_ram - F_ram;
                double P_ram = (used / T_ram ) * 100 ; //saco el porcentaje de uso
                P_ram = Math.Round(P_ram, 3); //lo redondeo a 3 decimales
                    
                ram_t.Text = $"RAM: {P_ram:0.00}% \n {used:0}MB / {T_ram:0}MB";
                R_Values.Add(used);
                if (R_Values.Count > 50) R_Values.RemoveAt(0);
            });
        }

        

        private SKColor ConvertToSKColor(System.Windows.Media.Color mediaColor) //convierto un win media color a un sk color
        {
            return new SKColor(mediaColor.R, mediaColor.G, mediaColor.B, mediaColor.A);
        }

        private void Ch_Border_L(object sender, EventArgs e) //esta func rezisea el tamaño del chart
        {
            var width = ChartContent.ActualWidth;
            var height = ChartContent.ActualHeight;
            ChartClip.Rect = new Rect(0, 0, width, height);

            ChartContent.SizeChanged += (s, ev) =>
            {
                ChartClip.Rect = new Rect(0, 0, ev.NewSize.Width, ev.NewSize.Height);
            };
        }
    }
}
