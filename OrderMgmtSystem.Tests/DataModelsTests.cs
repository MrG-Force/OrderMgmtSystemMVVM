using DataModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace OrderMgmtSystem.Tests
{
    [TestClass]
    public class DataModelsTests
    {
        [TestMethod]
        public void TwoOrderVarsCanReferenceSameObject()
        {
            Order order1 = GetOrder();
            Order order2 = order1;

            Assert.AreSame(order1, order2);
        }

        [TestMethod]
        public void CanSetDateTimeFromReference()
        {
            Order order1 = GetOrder();
            Order order2 = order1;

            SetOrderDateToNow(order2);

            Assert.AreEqual(order1.DateTime, order2.DateTime);
        }

        [TestMethod]
        public void CSharpIsPassByValue()
        {
            Order order1 = GetOrder(1);
            GetOrderSetId(order1, 2);

            Assert.IsTrue(order1.Id == 1);
        }

        [TestMethod]
        public void CSharpCanPassByRef()
        {
            Order order1 = GetOrder(1);
            GetOrderSetId(ref order1, 2);

            Assert.IsTrue(order1.Id == 2);
        }

        private void GetOrderSetId(Order order, int id)
        {
            order = new Order(id);
        }

        private void GetOrderSetId(ref Order order, int id)
        {
            order = new Order(id);
        }

        private Order GetOrder()
        {
            return new Order();
        }

        private Order GetOrder(int id)
        {
            return new Order(id);
        }

        private void SetOrderDateToNow(Order order)
        {
            order.DateTime = DateTime.Now;
        }
    }
}
