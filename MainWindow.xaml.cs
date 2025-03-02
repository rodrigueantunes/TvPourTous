using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LibVLCSharp.Shared;
using LibVLCSharp.WPF;
using System.IO;
using System.Text.Json;
using System.Windows.Input;

namespace TvPourTous
{
    public partial class MainWindow : Window
    {
        private Dictionary<string, string> _channels = new Dictionary<string, string>();
        private const string M3UFilePath = "m3u.json";

        // Liste par défaut des fichiers M3U disponibles (valeurs par variables)
        private Dictionary<string, string> _m3uSources = new Dictionary<string, string>
        {
            { "M3U Général FR", "https://iptv-org.github.io/iptv/countries/fr.m3u" },
            { "Free-TV M3U", "https://raw.githubusercontent.com/Free-TV/IPTV/master/playlist.m3u8" },
            { "French IPTV", "https://github.com/GSIGuy/guytestinghusham.com/blob/master/Lists/FrenchIPTV.m3u8" }
        };

        private LibVLC _libVLC;
        private MediaPlayer _mediaPlayer;
        private string _currentURL;

        public MainWindow()
        {
            InitializeComponent();
            Core.Initialize();

            _libVLC = new LibVLC();
            _mediaPlayer = new MediaPlayer(_libVLC);
            videoView.MediaPlayer = _mediaPlayer;

            // Charger (ou créer) le fichier m3u.json contenant les sources M3U
            LoadM3USources();

            // Une fois la fenêtre chargée, tester les URL et remplir le ComboBox avec celles valides
            this.Loaded += async (s, e) => await PopulateValidM3USourcesAsync();
        }

        private void LoadM3USources()
        {
            if (!File.Exists(M3UFilePath))
            {
                // Si le fichier n'existe pas, créer une liste par défaut et sauvegarder
                _m3uSources = new Dictionary<string, string>
        {
            { "M3U Général FR", "https://iptv-org.github.io/iptv/countries/fr.m3u" },
            { "Free-TV M3U", "https://raw.githubusercontent.com/Free-TV/IPTV/master/playlist.m3u8" },
            { "French IPTV", "https://github.com/GSIGuy/guytestinghusham.com/blob/master/Lists/FrenchIPTV.m3u8" }
        };
                SaveM3USources();
            }
            else
            {
                // Lire le fichier JSON et vérifier qu'il est bien chargé
                string json = File.ReadAllText(M3UFilePath);
                _m3uSources = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
                // Affichage du debug via notification auto-fermant
                var notif = new NotificationWindow($"Nombre de sources M3U chargées : {_m3uSources.Count}");
                notif.ShowAndAutoClose(TimeSpan.FromSeconds(2));
            }
        }
        

        private void ParseAndMergeM3U(string content)
        {
            string[] lines = content.Split('\n');
            string currentChannel = "";

            foreach (string line in lines)
            {
                if (line.StartsWith("#EXTINF"))
                {
                    // Récupérer le nom de la chaîne (la partie après la virgule)
                    currentChannel = line.Split(',').Last().Trim();
                }
                else if (line.StartsWith("http"))
                {
                    if (!string.IsNullOrEmpty(currentChannel))
                    {
                        // Vérifier si le nom de la chaîne existe déjà
                        if (!_channels.ContainsKey(currentChannel))
                        {
                            _channels[currentChannel] = line.Trim();
                        }
                        else
                        {
                            // S'il existe déjà, générer un nouveau nom avec un préfixe incrémenté
                            int count = 1;
                            string newChannelName = $"{count}-{currentChannel}";
                            // Incrémente tant que le nom existe déjà
                            while (_channels.ContainsKey(newChannelName))
                            {
                                count++;
                                newChannelName = $"{count}-{currentChannel}";
                            }
                            _channels[newChannelName] = line.Trim();
                        }
                    }
                }
            }
        }



        private void SaveM3USources()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(_m3uSources, options);
            File.WriteAllText(M3UFilePath, json);
        }

        /// <summary>
        /// Teste toutes les URL de _m3uSources et remplit comboBoxM3U uniquement avec celles qui répondent correctement.
        /// </summary>
        // Ajoutez une variable membre dans MainWindow.cs
        private List<string> _validSources = new List<string>();


        private async Task PopulateValidM3USourcesAsync()
        {
            comboBoxM3U.Items.Clear();
            _validSources.Clear();

            // Tester chaque source
            var tasks = _m3uSources.Select(async kvp =>
            {
                bool valid = await IsUrlValidAsync(kvp.Value);
                return new { Key = kvp.Key, Url = kvp.Value, Valid = valid };
            }).ToArray();

            var results = await Task.WhenAll(tasks);
            foreach (var result in results)
            {
                if (result.Valid)
                {
                    _validSources.Add(result.Key);
                }
            }

            // Ajouter l'option "Tout"
            comboBoxM3U.Items.Add("Tout");

            // Ajouter les sources valides
            foreach (var source in _validSources)
            {
                comboBoxM3U.Items.Add(source);
            }

            // Affichage du debug via notification auto-fermant
            var notif = new NotificationWindow($"Sources valides trouvées : {_validSources.Count}");
            notif.ShowAndAutoClose(TimeSpan.FromSeconds(2));


            if (comboBoxM3U.Items.Count > 0)
            {
                comboBoxM3U.SelectedIndex = 0;
            }
        }




        /// <summary>
        /// Vérifie si une URL est valide en envoyant une requête HTTP HEAD.
        /// </summary>
        private async Task<bool> IsUrlValidAsync(string url)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Head, url);
                    var response = await client.SendAsync(request);
                    return response.IsSuccessStatusCode;
                }
            }
            catch
            {
                return false;
            }
        }

        private async void LoadM3U(string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string m3uContent = await client.GetStringAsync(url);
                    ParseM3U(m3uContent);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement du fichier M3U : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ParseM3U(string content)
        {
            _channels.Clear();
            listBoxChannels.Items.Clear();

            string[] lines = content.Split('\n');
            string currentChannel = "";

            foreach (string line in lines)
            {
                if (line.StartsWith("#EXTINF"))
                {
                    currentChannel = line.Split(',').Last().Trim();
                }
                else if (line.StartsWith("http"))
                {
                    if (!string.IsNullOrEmpty(currentChannel))
                    {
                        _channels[currentChannel] = line.Trim();
                        listBoxChannels.Items.Add(currentChannel);
                    }
                }
            }
        }

        private void listBoxChannels_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBoxChannels.SelectedItem != null)
            {
                string channelName = listBoxChannels.SelectedItem.ToString();
                if (_channels.TryGetValue(channelName, out string url))
                {
                    PlayStream(url);
                }
            }
        }

        private void PlayStream(string url)
        {
            _currentURL = url;

            // S'assurer qu'aucun média précédent ne persiste
            StopAndDisposeMedia(_mediaPlayer);

            try
            {
                var media = new Media(_libVLC, new Uri(url));
                _mediaPlayer.Media = media;
                _mediaPlayer.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la lecture du flux : {ex.Message}");
            }
        }

        private async void comboBoxM3U_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxM3U.SelectedItem != null)
            {
                string selectedSource = comboBoxM3U.SelectedItem.ToString();
                if (selectedSource == "Tout")
                {
                    await LoadAllM3UsAsync();
                }
                else if (_m3uSources.TryGetValue(selectedSource, out string url))
                {
                    LoadM3U(url);
                }
            }
        }

        private async Task LoadAllM3UsAsync()
        {
            // Afficher le popup de progression
            var progressWindow = new ProgressNotificationWindow();
            progressWindow.Show();

            // Liste temporaire pour stocker toutes les chaînes (nom original et URL)
            List<(string Name, string Url)> allChannels = new List<(string, string)>();

            int totalSources = _validSources.Count;
            int sourceIndex = 0;

            // Télécharger et parser chaque source M3U valide
            foreach (var key in _validSources)
            {
                if (_m3uSources.TryGetValue(key, out string url))
                {
                    try
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            string m3uContent = await client.GetStringAsync(url);
                            var channels = ParseM3UContent(m3uContent);
                            allChannels.AddRange(channels);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Erreur pour la source {key}: {ex.Message}");
                    }
                }
                sourceIndex++;
                double progressPercentage = (sourceIndex / (double)totalSources) * 100;
                progressWindow.UpdateProgress(progressPercentage);
            }

            // Réinitialiser la collection des chaînes et la ListBox
            _channels.Clear();
            listBoxChannels.Items.Clear();

            // Grouper par nom de chaîne d'origine
            var groups = allChannels.GroupBy(c => c.Name);
            foreach (var group in groups)
            {
                string originalName = group.Key;
                var items = group.ToList();
                if (items.Count == 1)
                {
                    _channels[originalName] = items[0].Url;
                }
                else
                {
                    for (int i = 0; i < items.Count; i++)
                    {
                        string newName = $"{i + 1}-{originalName}";
                        _channels[newName] = items[i].Url;
                    }
                }
            }

            // Mettre à jour la ListBox avec toutes les chaînes fusionnées
            foreach (var channel in _channels.Keys)
            {
                listBoxChannels.Items.Add(channel);
            }

            // Fermer le popup de progression une fois terminé
            progressWindow.Close();
        }


        // Méthode de parsing d'un contenu M3U pour retourner une liste de (nom, url)
        private List<(string Name, string Url)> ParseM3UContent(string content)
        {
            List<(string Name, string Url)> channels = new List<(string, string)>();
            string[] lines = content.Split('\n');
            string currentChannel = "";
            foreach (string line in lines)
            {
                if (line.StartsWith("#EXTINF"))
                {
                    // Extraire le nom de la chaîne (la partie après la virgule)
                    currentChannel = line.Split(',').Last().Trim();
                }
                else if (line.StartsWith("http"))
                {
                    if (!string.IsNullOrEmpty(currentChannel))
                    {
                        channels.Add((currentChannel, line.Trim()));
                    }
                }
            }
            return channels;
        }



        private void mediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            _mediaPlayer.Stop();
        }

        private void BtnFullScreen_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_currentURL))
            {
                // Arrêter et libérer le média dans la fenêtre principale
                StopAndDisposeMedia(_mediaPlayer);

                // Créer et afficher la fenêtre plein écran
                var fullScreen = new FullScreenWindow(_libVLC, _currentURL, this);

                // Lorsqu'on quitte le plein écran (par Échap ou fermeture), arrêter le flux plein écran et relancer le flux dans la fenêtre principale
                fullScreen.Closed += FullScreen_Closed;
                fullScreen.Show();
            }
        }




        private void btnManageSources_Click(object sender, RoutedEventArgs e)
        {
            // Ouvre la fenêtre de gestion en passant la liste actuelle
            var manager = new M3USourcesManager(_m3uSources);
            if (manager.ShowDialog() == true)
            {
                _m3uSources = manager.M3USources;

                // Actualiser le ComboBox avec les clés de _m3uSources (sans tester ici)
                comboBoxM3U.Items.Clear();
                foreach (var source in _m3uSources.Keys)
                {
                    comboBoxM3U.Items.Add(source);
                }
                if (comboBoxM3U.Items.Count > 0)
                    comboBoxM3U.SelectedIndex = 0;

                SaveM3USources();
            }
        }

        private void FullScreen_Closed(object sender, EventArgs e)
        {
            // À la fermeture du plein écran, arrêter et libérer le média courant dans le MediaPlayer principal
            StopAndDisposeMedia(_mediaPlayer);

            // Relancer le flux dans la fenêtre principale
            PlayStream(_currentURL);
        }

        /// <summary>
        /// Arrête le MediaPlayer et libère le média courant.
        /// </summary>
        private void StopAndDisposeMedia(MediaPlayer player)
        {
            if (player.IsPlaying)
            {
                player.Stop();
            }
            if (player.Media != null)
            {
                player.Media.Dispose();
                player.Media = null;
            }
        }

        private void EnterFullScreen()
        {
            MainGrid.Visibility = Visibility.Collapsed;
            this.WindowState = WindowState.Maximized;
            this.WindowStyle = WindowStyle.None;
            this.Topmost = true;
            this.Focus();

            if (_mediaPlayer != null)
            {
                _mediaPlayer.Fullscreen = true;
                Dispatcher.Invoke(() =>
                {
                    _mediaPlayer.SetPause(true);
                    _mediaPlayer.SetPause(false);
                }, System.Windows.Threading.DispatcherPriority.Render);
            }
        }

        private void ExitFullScreen()
        {
            MainGrid.Visibility = Visibility.Visible;
            this.WindowState = WindowState.Normal;
            this.WindowStyle = WindowStyle.SingleBorderWindow;
            this.Topmost = false;

            if (_mediaPlayer != null)
            {
                _mediaPlayer.Fullscreen = false;
            }
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
                ExitFullScreen();
        }

        private void textBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filterText = textBoxSearch.Text;
            FilterChannels(filterText);
        }

        private void FilterChannels(string filterText)
        {
            listBoxChannels.Items.Clear();
            foreach (var channel in _channels)
            {
                if (channel.Key.ToLower().Contains(filterText.ToLower()))
                    listBoxChannels.Items.Add(channel.Key);
            }
        }

        private DateTime _lastClickTime;
        private readonly TimeSpan _doubleClickThreshold = TimeSpan.FromMilliseconds(300); // Temps max entre 2 clics

        private void VideoView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DateTime now = DateTime.Now;
                TimeSpan interval = now - _lastClickTime;

                if (interval < _doubleClickThreshold)
                {
                    // Double-clic détecté
                    if (this.WindowState == WindowState.Normal)
                    {
                        BtnFullScreen_Click(sender, e); // Passer en plein écran
                    }
                    else
                    {
                        ExitFullScreen(); // Quitter le plein écran
                    }
                }

                _lastClickTime = now;
                e.Handled = true; // Empêcher l'événement d'être bloqué par un autre élément
            }
        }
    }
}
