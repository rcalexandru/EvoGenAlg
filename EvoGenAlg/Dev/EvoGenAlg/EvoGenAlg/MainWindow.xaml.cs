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
using System.Windows.Threading;

namespace EvoGenAlg
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        EvolutionAlgImpl _evoAlg = null;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            _evoAlg = new EvolutionAlgImpl(new MatrixEvolution());
            _evoAlg.OnUpdateStatus += _evoAlg_OnUpdateStatus;
            _evoAlg.Evolve();
        }

        private void _evoAlg_OnUpdateStatus(object sender, ProgressEventArgs e)
        {
            try
            {
                rcDisplay.Children.Clear();

                List<List<int>> mat = e.Data as List<List<int>>;
                for (int i = 0; i < mat.Count; i++)
                {
                    for (int j = 0; j < mat[i].Count; j++)
                    {
                        var myRect = new System.Windows.Shapes.Rectangle();
                        myRect.Stroke = System.Windows.Media.Brushes.Black;
                        if (mat[i][j] == 0)
                        {
                            myRect.Fill = System.Windows.Media.Brushes.White;
                        }
                        else
                        {
                            myRect.Fill = System.Windows.Media.Brushes.Black;
                        }

                        myRect.HorizontalAlignment = HorizontalAlignment.Left;
                        myRect.VerticalAlignment = VerticalAlignment.Center;
                        myRect.Height = 20;
                        myRect.Width = 20;

                        rcDisplay.Children.Add(myRect);
                        Canvas.SetTop(myRect, i * 20);
                        Canvas.SetLeft(myRect, j * 20);
                    }
                }

                tbFitness.Text = e.Fitness.ToString();
                rcDisplay.UpdateLayout();

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() => { })).Wait();
            }
            catch (Exception) { }

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
        
    }
}
