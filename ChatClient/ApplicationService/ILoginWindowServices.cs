using ChatClient.Model;
using System;

namespace ChatClient.ApplicationService
{
    // Serviços para o LoginViewModel se comunicar com a tela de login.
    public interface ILoginWindowServices
    {
        void ShowErrorConnectionDialog(string errorMessage);
        void OpenChatWindow(Client client);
        void RunOnUIThread(Action action);
    }
}
