using System.Collections.Generic;
using System.Windows;

namespace TvPourTous
{
    public partial class M3USourcesManager : Window
    {
        // Propriété publique pour récupérer la liste modifiée
        public Dictionary<string, string> M3USources { get; private set; }

        public M3USourcesManager(Dictionary<string, string> initialSources)
        {
            InitializeComponent();
            // Cloner la liste initiale pour éviter toute modification directe
            M3USources = new Dictionary<string, string>(initialSources);
            UpdateListBox();
        }

        private void UpdateListBox()
        {
            listBoxSources.Items.Clear();
            foreach (var item in M3USources)
            {
                listBoxSources.Items.Add(item);
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            // Ouvre la fenêtre d'ajout d'une source
            var addWindow = new AddM3USourceWindow();
            if (addWindow.ShowDialog() == true)
            {
                if (!string.IsNullOrWhiteSpace(addWindow.SourceName) && !string.IsNullOrWhiteSpace(addWindow.SourceUrl))
                {
                    M3USources[addWindow.SourceName] = addWindow.SourceUrl;
                    UpdateListBox();
                }
            }
        }

        private void btnModify_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxSources.SelectedItem != null)
            {
                var selected = (KeyValuePair<string, string>)listBoxSources.SelectedItem;
                var editWindow = new EditM3USourceWindow(selected.Key, selected.Value);
                if (editWindow.ShowDialog() == true)
                {
                    if (editWindow.SourceName != selected.Key)
                    {
                        M3USources.Remove(selected.Key);
                    }
                    M3USources[editWindow.SourceName] = editWindow.SourceUrl;
                    UpdateListBox();
                }
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxSources.SelectedItem != null)
            {
                var selected = (KeyValuePair<string, string>)listBoxSources.SelectedItem;
                M3USources.Remove(selected.Key);
                UpdateListBox();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
