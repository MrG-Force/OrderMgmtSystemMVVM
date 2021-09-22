using System;

namespace OrderMgmtSystem.Services.Windows
{
    public interface IChildWindowService
    {
        event Action<int> ChildWindowClosed;

        void OpenWindow();
    }
}