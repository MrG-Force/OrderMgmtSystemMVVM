using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderMgmtSystem.Services.Dialogs.TestDialogs
{
    public class MockDialogService : IDialogService
    {
        public T OpenDialog<T>(DialogViewModelBase<T> viewModel)
        {
            return viewModel.DialogResult;
        }
    }
}
