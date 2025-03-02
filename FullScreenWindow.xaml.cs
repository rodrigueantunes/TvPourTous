using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using LibVLCSharp.Shared;

namespace TvPourTous
{
    public partial class FullScreenWindow : Window
    {
        private MediaPlayer _fullScreenMediaPlayer;
        private readonly LibVLC _libVLC;
        private readonly string _url;
        private readonly Window _mainWindow;

        public FullScreenWindow(LibVLC libVLC, string url, Window mainWindow)
        {
            InitializeComponent();

            _libVLC = libVLC;
            _url = url;
            _mainWindow = mainWindow;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Créer un nouveau MediaPlayer
            _fullScreenMediaPlayer = new MediaPlayer(_libVLC);

            // Associer le MediaPlayer au contrôle VideoView de la fenêtre plein écran
            videoViewFullScreen.MediaPlayer = _fullScreenMediaPlayer;

            // Charger et jouer le flux (même URL)
            var media = new Media(_libVLC, new Uri(_url));
            _fullScreenMediaPlayer.Media = media;
            _fullScreenMediaPlayer.Play();

            // Déplacer la fenêtre sur le même écran que la fenêtre principale
            MoveToActiveScreen();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                // Arrêter et libérer le MediaPlayer avant de fermer la fenêtre
                if (_fullScreenMediaPlayer != null)
                {
                    _fullScreenMediaPlayer.Stop();
                    _fullScreenMediaPlayer.Dispose();
                    _fullScreenMediaPlayer = null;
                }
                this.Close();
            }
        }

        private void MoveToActiveScreen()
        {
            // Récupérer l'écran où se trouve la fenêtre principale
            var mainWindowHandle = new WindowInteropHelper(_mainWindow).Handle;
            var screen = Screen.FromHandle(mainWindowHandle);

            // Appliquer la taille et la position de l'écran détecté
            this.WindowStyle = WindowStyle.None;
            this.WindowState = WindowState.Normal; // repasse brièvement en mode normal
            this.Left = screen.Bounds.Left;
            this.Top = screen.Bounds.Top;
            this.Width = screen.Bounds.Width;
            this.Height = screen.Bounds.Height;

            // Puis maximiser pour être en plein écran
            this.WindowState = WindowState.Maximized;
        }

        protected override void OnClosed(EventArgs e)
        {
            // Assurer que le MediaPlayer est arrêté et libéré quand la fenêtre se ferme
            if (_fullScreenMediaPlayer != null)
            {
                _fullScreenMediaPlayer.Stop();
                _fullScreenMediaPlayer.Dispose();
                _fullScreenMediaPlayer = null;
            }
            base.OnClosed(e);
        }
    }
}
