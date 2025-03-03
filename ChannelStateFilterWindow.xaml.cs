using System.Collections.Generic;
using System.Windows;

namespace TvPourTous
{
    public partial class ChannelStateFilterWindow : Window
    {
        // Propriété pour récupérer la liste des états sélectionnés
        public List<string> SelectedStates { get; private set; } = new List<string>();

        public ChannelStateFilterWindow()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // Vider la liste et ajouter les états sélectionnés
            SelectedStates.Clear();

            if (checkBoxActif.IsChecked == true)
                SelectedStates.Add("v");

            if (checkBoxInactif.IsChecked == true)
                SelectedStates.Add("x");

            // Pour les chaînes sans état défini, on pourra par exemple utiliser "u"
            if (checkBoxSansEtat.IsChecked == true)
                SelectedStates.Add("u");

            // On s'assure qu'au moins une case est cochée
            if (SelectedStates.Count == 0)
            {
                MessageBox.Show("Veuillez sélectionner au moins un état.", "Avertissement", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
