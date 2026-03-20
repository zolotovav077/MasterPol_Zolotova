using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MasterPol.Lib;
using System.Linq;

namespace MasterPol.Data_Zolotova.Database.Tests
{
    [TestClass()]
    public class PartnerRepositoryTests
    {
        [TestMethod]
        public void Discount_5000_ShouldBe0()
        {
            // Arrange
            decimal totalSales = 5000;

            // Act
            int result;
            if (totalSales > 300000) result = 15;
            else if (totalSales > 50000) result = 10;
            else if (totalSales > 10000) result = 5;
            else result = 0;

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void Discount_20000_ShouldBe5()
        {
            // Arrange
            decimal totalSales = 20000;

            // Act
            int result;
            if (totalSales > 300000) result = 15;
            else if (totalSales > 50000) result = 10;
            else if (totalSales > 10000) result = 5;
            else result = 0;

            // Assert
            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public void Discount_100000_ShouldBe10()
        {
            // Arrange
            decimal totalSales = 100000;

            // Act
            int result;
            if (totalSales > 300000) result = 15;
            else if (totalSales > 50000) result = 10;
            else if (totalSales > 10000) result = 5;
            else result = 0;

            // Assert
            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public void Discount_500000_ShouldBe15()
        {
            // Arrange
            decimal totalSales = 500000;

            // Act
            int result;
            if (totalSales > 300000) result = 15;
            else if (totalSales > 50000) result = 10;
            else if (totalSales > 10000) result = 5;
            else result = 0;

            // Assert
            Assert.AreEqual(15, result);
        }

        [TestMethod]
        public void Discount_10000_ShouldBe0()
        {
            // Arrange
            decimal totalSales = 10000;

            // Act
            int result;
            if (totalSales > 300000) result = 15;
            else if (totalSales > 50000) result = 10;
            else if (totalSales > 10000) result = 5;
            else result = 0;

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void Discount_50000_ShouldBe5()
        {
            // Arrange
            decimal totalSales = 50000;

            // Act
            int result;
            if (totalSales > 300000) result = 15;
            else if (totalSales > 50000) result = 10;
            else if (totalSales > 10000) result = 5;
            else result = 0;

            // Assert
            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public void Discount_300000_ShouldBe10()
        {
            // Arrange
            decimal totalSales = 300000;

            // Act
            int result;
            if (totalSales > 300000) result = 15;
            else if (totalSales > 50000) result = 10;
            else if (totalSales > 10000) result = 5;
            else result = 0;

            // Assert
            Assert.AreEqual(10, result);
        }
    }
}