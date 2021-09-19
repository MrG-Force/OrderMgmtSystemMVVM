using System;

namespace OrderMgmtSystem.Services.Windows
{
    interface ICloseWindows
    {
        Action Close { get; set; }
    }
}
