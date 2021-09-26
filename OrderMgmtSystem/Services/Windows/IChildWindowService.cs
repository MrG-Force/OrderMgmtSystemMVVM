using System;

namespace OrderMgmtSystem.Services.Windows
{
    public interface IChildWindowService
    {
        event EventHandler<int> ChildWindowClosed;

        void OpenWindow();
    }
}