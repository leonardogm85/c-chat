using ChatClient.ApplicationService;
using ChatClient.Model;
using ChatCommon;
using System.Collections.ObjectModel;

namespace ChatClient.ViewModel
{
    public class ChatViewModel : Bindable
    {
        // Cliente do chat.
        public Client client;

        // Serviços de janela.
        public IChatWindowServices WindowServices { get; set; }

        // Lista de nomes de membros conectados.
        // É do tipo ObservableCollection para que as mudanças nos elementos sejam refletidas na interface gráfica.
        private ObservableCollection<string> names;
        public ObservableCollection<string> Names
        {
            get { return names; }
            set
            {
                SetValue(ref names, value);
            }
        }

        // Mensagem a ser enviada.
        private string message;
        public string Message
        {
            get { return message; }
            set
            {
                SetValue(ref message, value);
            }
        }

        // Mensagens do chat.
        private string messages;
        public string Messages
        {
            get { return messages; }
            set
            {
                SetValue(ref messages, value);
            }
        }

        // Título da janela.
        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                SetValue(ref title, value);
            }
        }

        // Comando de envio de mensagem.
        public Command SendCommand { get; set; }

        // ViewModel implementa o padrão singleton.
        private static ChatViewModel current;
        public static ChatViewModel Current
        {
            get
            {
                if (current == null)
                {
                    current = new ChatViewModel();
                }
                return current;
            }
        }

        // Construtor privado.
        private ChatViewModel()
        {
            Names = new ObservableCollection<string>();
            SendCommand = new Command(Send);
        }

        // Inicializa o ViewModel.
        public void Initialize(Client client)
        {
            this.client = client;

            // Customiza o título com o nome de quem se conectou.
            Title = $"Chat - {client.Name}";

            // Registro nos eventos necessários.
            client.InputHandler.GetMembersResponse += OnGetMembersResponse;
            client.InputHandler.MessageReceived += OnMessageReceived;
            client.InputHandler.MemberEntered += OnMemberEntered;
            client.InputHandler.MemberLeft += OnMemberLeft;
            client.InputHandler.MemberCanLeave += OnMemberCanLeave;
            client.InputHandler.ServerDisconnecting += OnServerDisconnecting;

            // Solicita a lista fr membros.
            client.OutputHandler.SendGetMembersCommand();
        }

        // O servidor enviou a lista de membros.
        private void OnGetMembersResponse(object? sender, MembersEventArgs e)
        {
            if (WindowServices != null)
            {
                WindowServices.RunOnUIThread(() =>
                {
                    // Atribui a lista à coleção 'Nome'.
                    Names = new ObservableCollection<string>(e.Members);
                });
            }
        }

        // Algum membro mandou uma mensagem.
        private void OnMessageReceived(object? sender, MessageEventArgs e)
        {
            if (WindowServices != null)
            {
                WindowServices.RunOnUIThread(() =>
                {
                    // Mostra a mensagem na tela.
                    ShowMessage(e.Message);
                });
            }
        }

        // Um novo membro entrou.
        private void OnMemberEntered(object? sender, MemberEventArgs e)
        {
            if (WindowServices != null)
            {
                WindowServices.RunOnUIThread(() =>
                {
                    // Adiciona o nome na lista de nomes.
                    Names.Add(e.Name);

                    // Mostra mensagem avisando.
                    ShowMessage($"{e.Name} entered the chat.");
                });
            }
        }

        // Um membro saiu.
        private void OnMemberLeft(object? sender, MemberEventArgs e)
        {
            if (WindowServices != null)
            {
                WindowServices.RunOnUIThread(() =>
                {
                    // Remove o nome na lista de nomes.
                    Names.Remove(e.Name);

                    // Mostra mensagem avisando.
                    ShowMessage($"{e.Name} left the chat.");
                });
            }
        }

        // Inidica que o servidor autorizou este cliente a sair do chat.
        private void OnMemberCanLeave(object? sender, MemberEventArgs e)
        {
            // Para a thread que lê dadis da stream.
            e.StopInputHandler = true;

            // Desconecta o cliente.
            client.Disconnect();

            if (WindowServices != null)
            {
                // Fecha a janela do chat.
                WindowServices.RunOnUIThread(WindowServices.CloseWindow);
            }
        }

        // O servidor está se desconectando.
        private void OnServerDisconnecting(object? sender, BaseEventArgs e)
        {
            // Inicia o processo de desconexão do cliente.
            // Quando o servidor avisa que vai se desconectar, todos os clientes se desconectam antes.
            LeaveRoom();
        }

        private void ShowMessage(string message)
        {
            if (string.IsNullOrEmpty(Messages))
            {
                // Se for a primeira mensagem, apenas mostra.
                Messages = message;
            }
            else
            {
                // Se não for a primeira mensagem, coloca uma quebra de linha no início.
                Messages += $"\n{message}";
            }

            // Faz o scroll até o final das mensagens.
            WindowServices.ScrollMessagesToEnd();
        }

        // O cliente está iniciando o processo de sair do chat.
        public void LeaveRoom()
        {
            // Avisa o servidor que o cliente está saindo.
            client.OutputHandler.SendMemberLeavingCommand(client.Name);
        }

        // Envia uma mensagem.
        public void Send()
        {
            if (!string.IsNullOrEmpty(Message))
            {
                // Envia a mensagem ao servidor.
                client.OutputHandler.SendMessageCommand(Message);

                // Limpa o texto depois de enviado.
                Message = "";
            }
        }
    }
}
