using DataModels;
using DataProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrderMgmtSystem.Services;
using OrderMgmtSystem.Services.Dialogs.TestDialogs;
using OrderMgmtSystem.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace OrderMgmtSystem.Tests
{
    /// <summary>
    /// Test the core methdos of the AddItemViewModel class.
    /// </summary>
    [TestClass]
    public class AddItemViewModelTests
    {
        private static readonly IOrdersDataProvider ordersData = new TestDataProvider();
        private static readonly IDialogService mockDialogService = new MockDialogService();
        private static readonly DialogViewModelBase<int> mockDialogVM = new MockDialogViewModel(string.Empty, string.Empty);
        private readonly AddItemViewModel viewModel = new AddItemViewModel(ordersData.StockItems, mockDialogService, mockDialogVM)
        {
            SelectedStockItem = ordersData.StockItems.FirstOrDefault()
        };
        private readonly List<OrderItem> returnedItems = new List<OrderItem>
        {
            new OrderItem()
            {
                StockItemId = 1,
                Quantity = 8,
                OnBackOrder = 3
            }
        };

        [TestMethod]
        public void OnAddItem_StockItemIsUpdated()
        {
            // Arrange
            int availableStock = viewModel.SelectedStockItem.InStock;
            // Act
            viewModel.RequestAddItem(string.Empty);
            // Assert
            Assert.IsTrue(viewModel.SelectedStockItem.InStock == availableStock - 3
                || viewModel.SelectedStockItem.InStock == 0);
        }

        [TestMethod]
        public void OnReturnItemToStockList_StockItemIsIncreased()
        {
            // Arrange
            int startingStock = viewModel.SelectedStockItem.InStock;
            OrderItem returnedOrderItem = new OrderItem()
            {
                StockItemId = 1,
                Quantity = 6,
                OnBackOrder = 3
            };

            // Act
            viewModel.ReturnItemToStockList(returnedOrderItem);

            // Assert
            Assert.IsTrue(viewModel.SelectedStockItem.InStock == startingStock
                + returnedOrderItem.Quantity - returnedOrderItem.OnBackOrder);
        }

        [TestMethod]
        public void OnUpdateItemsReturnedToOrder_StockItemIsDecreased()
        {
            // Arrange
            int startingStock = viewModel.SelectedStockItem.InStock;

            // Act
            viewModel.UpdateItemsReturnedToOrder(returnedItems);

            // Assert
            Assert.IsTrue(viewModel.SelectedStockItem.InStock == startingStock
                - (returnedItems[0].Quantity - returnedItems[0].OnBackOrder));

        }
    }
}
