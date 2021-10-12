using System;

namespace OrderMgmtSystem.Services.Windows
{
    /// <summary>
    /// Defines a method container for closing a windows.
    /// </summary>
    interface ICloseWindows
    {
        Action Close { get; set; }
    }
}
