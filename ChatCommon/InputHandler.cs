﻿namespace ChatCommon
{
    // Executa em uma thread a parte e fica lendo o que chega na stream de entrada.
    public class InputHandler
    {
        // Eventos disparados em decorrência da chegada de comandos.
        public event EventHandler<MemberEventArgs> EnterRoom;
        public event EventHandler<EnterRoomResponseEventArgs> EnterRoomResponse;
        public event EventHandler<BaseEventArgs> GetMembers;
        public event EventHandler<MembersEventArgs> GetMembersResponse;
        public event EventHandler<MessageEventArgs> SendMessage;
        public event EventHandler<MessageEventArgs> MessageReceived;
        public event EventHandler<MemberEventArgs> MemberEntered;
        public event EventHandler<MemberEventArgs> MemberLeft;
        public event EventHandler<MemberEventArgs> MemberLeaving;
        public event EventHandler<MemberEventArgs> MemberCanLeave;
        public event EventHandler<BaseEventArgs> ServerDisconnecting;

        // Stream de entrada.
        private StreamReader input;

        // Indica se o loop de leitura deve parar.
        private bool stop;

        public InputHandler(Stream stream)
        {
            input = new StreamReader(stream);
        }

        // Inicia a thread.
        public void Start()
        {
            Thread t = new Thread(Run);

            // A thread é do tipo background, pois se a aplicação for finalizada, a thread é encerrada.
            t.IsBackground = true;

            t.Start();
        }

        private void Run()
        {
            while (!stop)
            {
                // Aguarda a chegada de uma string, que representa um comando.
                string commandStr = input.ReadLine();

                // Converte a string para um objeto ChatCommand.
                ChatCommand command = ChatCommand.Parse(commandStr);

                // Chama o método para tratar o comando recebido, de acordo com o tipo.
                switch (command.Type)
                {
                    case ChatCommandType.EnterRoom:
                        HandleEnterRoomCommand(command);
                        break;
                    case ChatCommandType.EnterRoomResponse:
                        HandleEnterRoomResponseCommand(command);
                        break;
                    case ChatCommandType.GetMembers:
                        HandleGetMembersCommand(command);
                        break;
                    case ChatCommandType.GetMembersResponse:
                        HandleGetMembersResponseCommand(command);
                        break;
                    case ChatCommandType.SendMessage:
                        HandleSendMessageCommand(command);
                        break;
                    case ChatCommandType.MessageReceived:
                        HandleMessageReceivedCommand(command);
                        break;
                    case ChatCommandType.MemberEntered:
                        HandleMemberEnteredCommand(command);
                        break;
                    case ChatCommandType.MemberLeft:
                        HandleMemberLeftCommand(command);
                        break;
                    case ChatCommandType.MemberLeaving:
                        HandleMemberLeavingCommand(command);
                        break;
                    case ChatCommandType.MemberCanLeave:
                        HandleMemberCanLeaveCommand(command);
                        break;
                    case ChatCommandType.ServerDisconnecting:
                        HandleServerDisconnectingCommand(command);
                        break;
                    default:
                        break;
                }
            }
        }

        // Cada um dos métodos abaixo trata um comando específico. O objetivo é disparar o evento correspondente, assim
        // quem se registra no evento fica sabendo que tipo de comando chegou.
        // Quem recebe o evento pode optar por encerrar o loop de leitura de stream, definindo a property StopInputHandler
        // para o valor true.

        private void HandleEnterRoomCommand(ChatCommand command)
        {
            if (EnterRoom != null)
            {
                MemberEventArgs args = new MemberEventArgs(command.Param);
                EnterRoom(this, args);
                stop = args.StopInputHandler;
            }
        }

        private void HandleEnterRoomResponseCommand(ChatCommand command)
        {
            if (EnterRoomResponse != null)
            {
                bool valid;
                string error;

                if (command.Param == "OK")
                {
                    valid = true;
                    error = null;
                }
                else
                {
                    valid = false;
                    error = command.Param;
                }

                EnterRoomResponseEventArgs args = new EnterRoomResponseEventArgs(valid, error);
                EnterRoomResponse(this, args);
                stop = args.StopInputHandler;
            }
        }

        private void HandleGetMembersCommand(ChatCommand command)
        {
            if (GetMembers != null)
            {
                BaseEventArgs args = new BaseEventArgs();
                GetMembers(this, args);
                stop = args.StopInputHandler;
            }
        }

        private void HandleGetMembersResponseCommand(ChatCommand command)
        {
            if (GetMembersResponse != null)
            {
                // A lista de membros é uma string delimitada por ",".
                List<string> members = new List<string>(command.Param.Split(","));
                MembersEventArgs args = new MembersEventArgs(members);
                GetMembersResponse(this, args);
                stop = args.StopInputHandler;
            }
        }

        private void HandleSendMessageCommand(ChatCommand command)
        {
            if (SendMessage != null)
            {
                MessageEventArgs args = new MessageEventArgs(command.Param);
                SendMessage(this, args);
                stop = args.StopInputHandler;
            }
        }

        private void HandleMessageReceivedCommand(ChatCommand command)
        {
            if (MessageReceived != null)
            {
                MessageEventArgs args = new MessageEventArgs(command.Param);
                MessageReceived(this, args);
                stop = args.StopInputHandler;
            }
        }

        private void HandleMemberEnteredCommand(ChatCommand command)
        {
            if (MemberEntered != null)
            {
                MemberEventArgs args = new MemberEventArgs(command.Param);
                MemberEntered(this, args);
                stop = args.StopInputHandler;
            }
        }

        private void HandleMemberLeftCommand(ChatCommand command)
        {
            if (MemberLeft != null)
            {
                MemberEventArgs args = new MemberEventArgs(command.Param);
                MemberLeft(this, args);
                stop = args.StopInputHandler;
            }
        }

        private void HandleMemberLeavingCommand(ChatCommand command)
        {
            if (MemberLeaving != null)
            {
                MemberEventArgs args = new MemberEventArgs(command.Param);
                MemberLeaving(this, args);
                stop = args.StopInputHandler;
            }
        }

        private void HandleMemberCanLeaveCommand(ChatCommand command)
        {
            if (MemberCanLeave != null)
            {
                MemberEventArgs args = new MemberEventArgs(command.Param);
                MemberCanLeave(this, args);
                stop = args.StopInputHandler;
            }
        }

        private void HandleServerDisconnectingCommand(ChatCommand command)
        {
            if (ServerDisconnecting != null)
            {
                BaseEventArgs args = new BaseEventArgs();
                ServerDisconnecting(this, args);
                stop = args.StopInputHandler;
            }
        }
    }
}
