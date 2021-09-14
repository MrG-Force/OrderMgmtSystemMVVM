using DataModels;
using DataProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrderMgmtSystem.Services;
using OrderMgmtSystem.Services.Dialogs.TestDialogs;
using OrderMgmtSystem.ViewModels;
using System.Linq;

namespace OrderMgmtSystem.Tests
{
    [TestClass]
    public class AddItemViewModelTests
    {
        private static readonly IOrdersDataProvider ordersData = new RandomDataProvider();
        private static readonly IDialogService mockDialogService = new MockDialogService();
        private static readonly DialogViewModelBase<int> mockDialogVM = new MockDialogViewModel();
        private readonly AddItemViewModel viewModel = new AddItemViewModel(ordersData.StockItems, mockDialogService, mockDialogVM);

        [TestMethod]
        public void OnAddItem_StockItemIsUpdated()
        {
            // Arrange
            StockItem selectedItem = ordersData.StockItems.FirstOrDefault();
            int availableStock = selectedItem.InStock;
            // Act
            viewModel.AddItem(selectedItem);
            // Assert
            Assert.IsTrue(selectedItem.InStock == availableStock - 3 || selectedItem.InStock == 0);
        }



    }
}
