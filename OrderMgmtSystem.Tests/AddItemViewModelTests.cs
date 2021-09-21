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
        private readonly AddItemViewModel viewModel = new AddItemViewModel(ordersData.StockItems, mockDialogService, mockDialogVM)
        {
            SelectedStockItem = ordersData.StockItems.FirstOrDefault()
        };

        [TestMethod]
        public void OnAddItem_StockItemIsUpdated()
        {
            // Arrange
            int availableStock = viewModel.SelectedStockItem.InStock;
            // Act
            viewModel.AddItem(string.Empty);
            // Assert
            Assert.IsTrue(viewModel.SelectedStockItem.InStock == availableStock - 3 
                || viewModel.SelectedStockItem.InStock == 0);
        }
    }
}
