using System.Windows;

namespace TvPourTous
{
    public partial class EditM3USourceWindow : Window
    {
        public string SourceName { get; private set; }
        public string SourceUrl { get; private set; }

        public EditM3USourceWindow(string currentName, string currentUrl)
        {
            InitializeComponent();
            txtSourceName.Text = currentName;
            txtSourceUrl.Text = currentUrl;
        }

        private void btnModify_Click(object sender, RoutedEventArgs e)
        {
            SourceName = txtSourceName.Text;
            SourceUrl = txtSourceUrl.Text;
            DialogResult = true;
            Close();
        }
    }
}
