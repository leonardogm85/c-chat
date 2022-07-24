using ChatCommon;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace ChatServer.Model
{
    // Representa um cliente conectado no chat.
    public class Member
    {
        private Server server;
        private TcpClient tcpClient;

        // Nome do membro.
        private string name;

        // Handler para gerenciar os comandos que chegam ao membro e são enviados ao cliente do chat.
        private InputHandler inputHandler;
        private OutputHandler outputHandler;

        public Member(Server server, TcpClient tcpClient)
        {
            this.server = server;
            this.tcpClient = tcpClient;

            inputHandler = new InputHandler(tcpClient.GetStream());
            outputHandler = new OutputHandler(tcpClient.GetStream());
        }

        public void HandleMemberInteraction()
        {
            // Registra os eventos necessários.
            inputHandler.EnterRoom += OnEnterRoom;
            inputHandler.GetMembers += OnGetMembers;
            inputHandler.SendMessage += OnSendMessage;
            inputHandler.MemberLeaving += OnMemberLeaving;

            // Inicia o input handler, que fica aguardando a chegada de comandos do cliente na stream.
            inputHandler.Start();
        }

        // Um cliente quer entrar na sala.
        private void OnEnterRoom(object? sender, MemberEventArgs e)
        {
            // Define o seu nome.
            name = e.Name;

            bool valid = true;
            string error = null;

            lock (server.Members)
            {
                // Verifica se já não existe um membro com o mesmo nome.
                foreach (Member member in server.Members)
                {
                    if (member != this && member.name == name)
                    {
                        valid = false;
                        error = $"The name {name} already exists in chat.";
                        break;
                    }
                }
            }

            // Envia uma resposta ao cliente, dizendo se ele pode ou não entrar. Em caso negativo, envia o erro.
            outputHandler.SendEnterRoomResponseCommand(valid, error);

            if (valid)
            {
                // Se o cliente pode se conectar, avisa todos os membros existentes que um novo membro está entrando.
                lock (server.Members)
                {
                    foreach (Member member in server.Members)
                    {
                        member.outputHandler.SendMemberEnteredCommand(name);
                    }

                    // Adiciona o cliente na lista de membros.
                    server.Members.Add(this);
                }
            }
            else
            {
                // Se o cliente não pode se conectar, indica que a thread de leitura da stream deve ser interrompida.
                e.StopInputHandler = true;
            }
        }

        private void OnGetMembers(object? sender, BaseEventArgs e)
        {
            // Um novo membro solicita a lista de membros conectados.
            List<string> names = new List<string>();

            lock (server.Members)
            {
                // Cria uma lista com os nomes dos membros.
                foreach (Member member in server.Members)
                {
                    names.Add(member.name);
                }
            }

            // Envia a lista para o membro que pediu.
            outputHandler.SendGetMembersResponseCommand(names);
        }

        private void OnSendMessage(object? sender, MessageEventArgs e)
        {
            // Um membro está mandando uma mensagem.

            // Decora a mensagem com a hora atual e o nome do membro.
            string time = DateTime.Now.ToString("HH:mm:ss");
            string message = string.Format("[{0}] {1} - {2}", time, name, e.Message);

            lock (server.Members)
            {
                // Envia a mensagem para todos os membros conectados.
                foreach (Member member in server.Members)
                {
                    member.outputHandler.SendMessageReceivedCommand(message);
                }
            }
        }

        private void OnMemberLeaving(object? sender, MemberEventArgs e)
        {
            // Um membro está saindo do chat.
            lock (server.Members)
            {
                // Remove o membro da lista de membros.
                server.Members.Remove(this);

                // Avisa os outros membros a respeito da saída.
                foreach (Member member in server.Members)
                {
                    member.outputHandler.SendMemberLeftCommand(name);
                }
            }

            // Avisa o membro que agora ele está autorizado a se desconectar.
            outputHandler.SendMemberCanLeaveCommand(name);

            // Indica que a thread de leitura da stream deve ser interrompida.
            e.StopInputHandler = true;
        }

        public void SendServerDisconnectingCommand()
        {
            // O servidor está sendo desconectado, então avisa o membro.
            outputHandler.SendServerDisconnectingCommand();
        }
    }
}
