using System.Windows;

namespace BassPlayer.Controls
{
    /// <summary>
    /// Interaction logic for AddUrlDialog.xaml
    /// </summary>
    public partial class AddUrlDialog : Window
    {
        public AddUrlDialog()
        {
            InitializeComponent();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public string Url
        {
            get { return TbUrl.Text; }
            set { TbUrl.Text = value; }
        }
    }
}
