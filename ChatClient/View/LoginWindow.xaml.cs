using ChatClient.ApplicationService;
using ChatClient.Model;
using ChatClient.ViewModel;
using System;
using System.Windows;
using System.Windows.Input;

namespace ChatClient.View
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window, ILoginWindowServices
    {
        public LoginWindow()
        {
            InitializeComponent();

            // Associa a instância do ViewModel ao DataContext da janela.
            LoginViewModel viewModel = LoginViewModel.Current;
            DataContext = viewModel;

            // Atribui o WindowServices ao ViewModel (isso permite que o ViewModel se comunique com a janela sem conhecer a view).
            viewModel.WindowServices = this;
        }

        // Permite que apenas caracteres númericos sejam digitados como valor para a porta do servidor.
        private void TxtPort_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0)
            {
                if (!char.IsDigit(e.Text, e.Text.Length - 1))
                {
                    e.Handled = true;
                }
            }
        }

        // Mostra uma caixa de diálogo de erro de conexão.
        public void ShowErrorConnectionDialog(string errorMessage)
        {
            MessageBox.Show(this, errorMessage, "Connection error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        // Abre a janela do chat.
        public void OpenChatWindow(Client client)
        {
            ChatWindow window = new ChatWindow();

            // Inicializa o ViewModel da janela do chat.
            ChatViewModel.Current.Initialize(client);

            window.Show();
            Close();
        }

        // Executa o código usando a thread da interface gráfica.
        public void RunOnUIThread(Action action)
        {
            Dispatcher.Invoke(action);
        }
    }
}
