using System.Windows;

namespace TvPourTous
{
    public partial class AddM3USourceWindow : Window
    {
        // Ces propriétés seront accessibles depuis M3USourcesManager
        public string SourceName { get; private set; }
        public string SourceUrl { get; private set; }

        public AddM3USourceWindow()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            // Récupérer le nom et l'URL entrés dans les TextBox
            SourceName = txtSourceName.Text;
            SourceUrl = txtSourceUrl.Text;
            DialogResult = true;
            Close();
        }
    }
}
