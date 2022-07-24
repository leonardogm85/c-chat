using ChatClient.ApplicationService;
using ChatClient.ViewModel;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace ChatClient.View
{
    /// <summary>
    /// Interaction logic for ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow : Window, IChatWindowServices
    {
        // Indica se a janela deve ser fechada.
        private bool closeWindow = false;

        public ChatWindow()
        {
            InitializeComponent();

            // Atribui a instância do ViewModel ao DataContext da janela.
            ChatViewModel viewModel = ChatViewModel.Current;
            DataContext = viewModel;
            viewModel.WindowServices = this;
        }

        private void TxtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            // Faz um tratamento especial caso um ENTER seja digitado na caixa de texto da mensagem.
            if (e.Key != Key.Enter)
            {
                return;
            }

            // Indica que o evento foi tratado.
            e.Handled = true;

            // Coloca o foco no botão "Enviar" (isso é necessário para que a mensagem seja atualizada no binding do ViewModel).
            FocusManager.SetFocusedElement(this, btnSend);

            // Envia a mensagem.
            ChatViewModel.Current.Send();

            // Coloca o foco novamente na caixa de texto da mensagem.
            txtMessage.Focus();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            // Só fecha a janela realmente se closeWindow for true.
            if (!closeWindow)
            {
                // Se closeWindow for false, primeiro faz o processo de saída da sala.
                ChatViewModel.Current.LeaveRoom();
                e.Cancel = true;
            }
        }

        public void RunOnUIThread(Action action)
        {
            // Executa o código usando a thread da interface gráfica.
            Dispatcher.Invoke(action);
        }

        public void ScrollMessagesToEnd()
        {
            // Faz o scroll para o final do texto na janela de mensagens.
            txtMessages.ScrollToEnd();
        }

        public void CloseWindow()
        {
            // Fecha a janela.
            closeWindow = true;
            Close();
        }
    }
}
