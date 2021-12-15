using System.Threading;
using System.Windows;

namespace WorkerThread
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Declare le delegate qui va permettre
        // d'encapsuler la methode UpdateStatus
        private delegate void UpdateStatusDelegate(int val, int max);
        private UpdateStatusDelegate updateStatusDelegate = null;

        // Declare le worker thread
        private Thread WorkerThread = null;
        // Boolean pour stopper le thread 
        private bool StopProcess = false;

        public MainWindow()
        {
            InitializeComponent();

            // Initialise le delegate
            updateStatusDelegate = new UpdateStatusDelegate(UpdateStatus);
        }
        private void HeavyOperation()
        {
            int max = 100;
            // Incremente de 1 toutes les 0.5 sec
            for (int i = 0; i <= max; i++)
            {
                // Check if Stop button was clicked
                if (!StopProcess)
                {
                    // Show progress
                    updateStatusDelegate.Invoke(i, max);
                }
                else
                {
                    // Pour les petits malins, Abort()
                    // releve une exception
                    //
                    // Il faut donc trouver une autre
                    // moyen pour finir le thread

                    updateStatusDelegate.Invoke(0, max);
                    // Stop heavy operation
                    break;
                }
                Thread.Sleep(500);
            }
        }
        private void UpdateStatus(int val, int max)
        {
            // Verifie si le thread a le droit d'acceder
            // a ProgressBar1
            //
            // Pour une raison que j'ignore, retoure
            // toujours faux
            if (ProgressBar1.Dispatcher.CheckAccess())
            {
                ProgressBar1.Value = val;
                ProgressBar1.Maximum = max;
            }
            else
            {
                // C'est donc cette partie qui change
                // vraiment la progression
                ProgressBar1.Dispatcher.Invoke(() =>
                {
                    ProgressBar1.Value = val;
                    ProgressBar1.Maximum = max;
                });
            }
        }
        /// <summary>
        /// Lance le Thread quand on clique.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtStart_Click(object sender, RoutedEventArgs e)
        {
            StopProcess = false;

            // Initialise et debute le worker thread
            WorkerThread = new Thread(new ThreadStart(HeavyOperation));
            WorkerThread.Start();
        }
        /// <summary>
        /// Stop le Thread quand on clique.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtStop_Click(object sender, RoutedEventArgs e)
        {
            StopProcess = true;
        }
    }
}
