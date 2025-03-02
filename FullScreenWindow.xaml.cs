using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using LibVLCSharp.Shared;

namespace TvPourTous
{
    public partial class FullScreenWindow : Window
    {
        private MediaPlayer _fullScreenMediaPlayer;
        private readonly LibVLC _libVLC;
        private string _currentUrl;
        private readonly Window _mainWindow;

        public FullScreenWindow(LibVLC libVLC, string url, Window mainWindow)
        {
            InitializeComponent();

            _libVLC = libVLC;
            _currentUrl = url;
            _mainWindow = mainWindow;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialisation du MediaPlayer si non initialisé
            if (_fullScreenMediaPlayer == null)
                _fullScreenMediaPlayer = new MediaPlayer(_libVLC);

            // Associer le MediaPlayer au contrôle VideoView
            videoViewFullScreen.MediaPlayer = _fullScreenMediaPlayer;

            // Charger et jouer le flux initial
            ChangeChannel(_currentUrl);

            // Déplacer la fenêtre sur l'écran actif
            MoveToActiveScreen();
        }

        public void ChangeChannel(string url)
        {
            if (_fullScreenMediaPlayer != null)
            {
                // Arrêter l'ancien flux avant d'en charger un nouveau
                _fullScreenMediaPlayer.Stop();

                var media = new Media(_libVLC, new Uri(url));
                _fullScreenMediaPlayer.Media = media;
                _fullScreenMediaPlayer.Play();

                // Mettre à jour l'URL courante
                _currentUrl = url;
            }
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                // Fermer la fenêtre plein écran proprement
                CloseFullScreen();
            }
        }

        private void MoveToActiveScreen()
        {
            var mainWindowHandle = new WindowInteropHelper(_mainWindow).Handle;
            var screen = Screen.FromHandle(mainWindowHandle);

            // Ajuster la fenêtre plein écran à la taille de l'écran actif
            WindowStyle = WindowStyle.None;
            WindowState = WindowState.Normal;
            Left = screen.Bounds.Left;
            Top = screen.Bounds.Top;
            Width = screen.Bounds.Width;
            Height = screen.Bounds.Height;
            WindowState = WindowState.Maximized;
        }

        protected override void OnClosed(EventArgs e)
        {
            // Arrêter et libérer le MediaPlayer proprement
            CloseFullScreen();
            base.OnClosed(e);
        }

        private void CloseFullScreen()
        {
            if (_fullScreenMediaPlayer != null)
            {
                _fullScreenMediaPlayer.Stop();
                _fullScreenMediaPlayer.Dispose();
                _fullScreenMediaPlayer = null;
            }
            Close();
        }

        private DateTime _lastClickTime;
        private readonly TimeSpan _doubleClickThreshold = TimeSpan.FromMilliseconds(300);

        private void VideoViewFullScreen_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DateTime now = DateTime.Now;
                TimeSpan interval = now - _lastClickTime;

                if (interval < _doubleClickThreshold)
                {
                    // Double-clic détecté => Quitter le plein écran
                    CloseFullScreen();
                }

                _lastClickTime = now;
                e.Handled = true;
            }
        }
    }
}
