using System.Windows;

namespace football_project
{
    public partial class DialogWindow : Window
    {
        public bool Result { get; private set; } = false;

        //basic info dialog with a single OK button
        public DialogWindow(Window owner, string message)
        {
            InitializeComponent();
            Owner = owner;
            txtMessage.Text = message;
        }

        //confirm dialog - YES and NO buttons
        public DialogWindow(Window owner, string message, bool isConfirm)
        {
            InitializeComponent();
            Owner = owner;
            txtMessage.Text = message;
            if (isConfirm)
            {
                btnNo.Visibility = Visibility.Visible;
                btnYes.Content = "YES";
            }
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            Result = true;
            Close();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            Result = false;
            Close();
        }
    }
}