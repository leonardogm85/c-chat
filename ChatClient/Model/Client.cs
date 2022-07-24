using ChatCommon;
using System.Net.Sockets;

namespace ChatClient.Model
{
    public class Client
    {
        // Cliente do socket.
        private TcpClient tcpClient;

        // InputHandler e OutputHandler, que gerenciam os dados da stream.
        public InputHandler InputHandler { get; private set; }
        public OutputHandler OutputHandler { get; private set; }

        // Nome usado pelo cliente no chat.
        public string Name { get; set; }

        // Faz a conexão no servidor.
        public bool Connect(string server, int port, string name)
        {
            Name = name;

            try
            {
                tcpClient = new TcpClient(server, port);
                InputHandler = new InputHandler(tcpClient.GetStream());
                OutputHandler = new OutputHandler(tcpClient.GetStream());

                // Inicia a thread do InputHandler, que fica aguardando a chegada de dados na stream.
                InputHandler.Start();

                // Se deu tudo certo, retorna true, indicando que a conexão foi realizada.
                return true;
            }
            catch (SocketException)
            {
                // Se gerar exceção, a conexão não pode ser estabelecida com o servidor.
                return false;
            }
        }

        public void Disconnect()
        {
            // Desconecta o socket.
            tcpClient.Close();
        }
    }
}
