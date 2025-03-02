using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace TvPourTous
{
    public class NotificationWindow : Window
    {
        public NotificationWindow(string message)
        {
            // Configuration de la fenêtre
            Width = 300;
            Height = 100;
            WindowStyle = WindowStyle.None;
            AllowsTransparency = true;
            Background = new SolidColorBrush(Color.FromArgb(220, 0, 0, 0));
            Topmost = true;
            ShowInTaskbar = false;
            // Positionnez la fenêtre en haut à droite (exemple)
            Left = 10;
            Top = 10;

            // Création du contenu
            var textBlock = new TextBlock
            {
                Text = message,
                Foreground = Brushes.White,
                FontSize = 14,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap
            };

            Content = textBlock;
        }

        public void ShowAndAutoClose(TimeSpan duration)
        {
            Show();
            var timer = new DispatcherTimer { Interval = duration };
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                Close();
            };
            timer.Start();
        }
    }
}
