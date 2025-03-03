using System.Windows;

namespace TvPourTous
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Empêche la fermeture automatique de l'application
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            ChannelStateFilterWindow filterWindow = new ChannelStateFilterWindow();
            bool? result = filterWindow.ShowDialog();

            if (result == true)
            {
                var selectedStates = filterWindow.SelectedStates;
                MainWindow mainWindow = new MainWindow(selectedStates);

                // Définit MainWindow comme fenêtre principale
                Current.MainWindow = mainWindow;
                // Réactive le comportement de fermeture lors de la fermeture de MainWindow
                this.ShutdownMode = ShutdownMode.OnMainWindowClose;
                mainWindow.Show();
            }
            else
            {
                Shutdown();
            }
        }
    }
}
