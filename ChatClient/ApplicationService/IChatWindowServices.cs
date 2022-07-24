using System;

namespace ChatClient.ApplicationService
{
    // Serviços para o ChatViewModel se comunicar com a tela do chat.
    public interface IChatWindowServices
    {
        void RunOnUIThread(Action value);
        void ScrollMessagesToEnd();
        void CloseWindow();
    }
}
