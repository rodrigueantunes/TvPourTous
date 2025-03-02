using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace TvPourTous
{
    public class ProgressNotificationWindow : Window
    {
        private TextBlock _textBlock;
        private ProgressBar _progressBar;

        public ProgressNotificationWindow()
        {
            Width = 300;
            Height = 120;
            WindowStyle = WindowStyle.None;
            AllowsTransparency = true;
            Background = new SolidColorBrush(Color.FromArgb(220, 0, 0, 0));
            Topmost = true;
            ShowInTaskbar = false;
            // Positionner la fenêtre en haut à droite
            Left = SystemParameters.WorkArea.Width - Width - 10;
            Top = 10;

            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(10)
            };

            _textBlock = new TextBlock
            {
                Text = "Chargement des chaînes : 0%",
                Foreground = Brushes.White,
                FontSize = 14,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 10)
            };

            _progressBar = new ProgressBar
            {
                Minimum = 0,
                Maximum = 100,
                Height = 20,
                Value = 0
            };

            stackPanel.Children.Add(_textBlock);
            stackPanel.Children.Add(_progressBar);

            Content = stackPanel;
        }

        public void UpdateProgress(double percentage)
        {
            // Mise à jour de la barre et du texte en s'assurant que l'appel se fait sur le thread UI
            Dispatcher.Invoke(() =>
            {
                _progressBar.Value = percentage;
                _textBlock.Text = $"Chargement des chaînes : {percentage:0}%";
            });
        }
    }
}
