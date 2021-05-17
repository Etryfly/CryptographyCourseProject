using System.Windows;

namespace Client
{

    public partial class ClientWindow : Window
    {
        public ClientWindow()
        {
            InitializeComponent();
            this.DataContext = new ClientViewModel();
        }
    }
}
