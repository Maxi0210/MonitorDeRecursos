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


namespace MonitorDeRecursos
{
    public partial class CPU : UserControl 
    {
        private ObservableCollection<double> C_Values;
        private PerformanceCounter cpu; //variable que contiene los datos del CPU
        private Timer T; //variable de tiempo

        public ISeries[] CpuSeries { get; set; }
        public Axis[] YAxes { get; set; }
        public Axis[] XAxes { get; set; }

        private MainWindow main;

        private int C;

        public event Action<string>? D;
        public CPU()
        {
            InitializeComponent();
            C_Values = new ObservableCollection<double>();
            cpu = new PerformanceCounter("Processor", "% Processor Time", "_Total"); //le pido que de la info del procesador me de el uso, y la suma de todos los nucleos

            var NVioletBrush = (SolidColorBrush)Application.Current.Resources["NeonVioletBrush"];
            var SNVioletBrush = ConvertToSKColor(NVioletBrush.Color);
            CpuSeries = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = C_Values,
                    Fill = new SolidColorPaint(SNVioletBrush.WithAlpha(20)),
                    LineSmoothness = 1,
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
                    MaxLimit = 100,
                    MinStep = 50,
                    SeparatorsPaint = new SolidColorPaint
                    {
                        Color = new SkiaSharp.SKColor(255, 255, 255, 30),
                        StrokeThickness = 1
                    }
                }
            };
            XAxes = new Axis[] { new Axis { IsVisible = false } };

            T = new Timer(1000); //tiempo de actualizacion (en  milisegundos)
            T.Elapsed += act_c;
            T.Start();

            DataContext = this;
        }

        private void act_c(object sender, ElapsedEventArgs e)
        {
            C++;
            var usage = Math.Round(cpu.NextValue(), 3); //recive el valor actual del cpu y lo redondeo a 3 decimales
            
            Dispatcher.Invoke(() =>  //basicamente lo que hace esto es llamar y actualizar la UI desde el hilo principal
            {
                
                cpu_t.Text = $"CPU: {usage:0.00}%";  //llamo a cpu_v y le cambio el texto
                C_Values.Add(usage);
                if (C == 5)
                {
                    C = 0;
                    D?.Invoke($" CPU\n{usage:0.00}%");
                }
                if (C_Values.Count > 50) C_Values.RemoveAt(0);
            });
        }

        private SKColor ConvertToSKColor(System.Windows.Media.Color mediaColor) //convierto un win media color a un sk color
        {
            return new SKColor(mediaColor.R, mediaColor.G, mediaColor.B, mediaColor.A);
        }

        private void Ch_Border_L(object sender, RoutedEventArgs e) //funcion que redimenciona el chart
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