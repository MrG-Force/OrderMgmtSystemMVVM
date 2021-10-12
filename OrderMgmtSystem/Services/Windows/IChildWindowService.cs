using System;

namespace OrderMgmtSystem.Services.Windows
{
    /// <summary>
    /// Defines a Window service to open new windows.
    /// </summary>
    public interface IChildWindowService
    {
        event EventHandler<int> ChildWindowClosed;

        void OpenWindow();
    }
}