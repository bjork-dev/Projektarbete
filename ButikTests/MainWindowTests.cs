using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Butik.Tests
{
    [TestClass()]
    public class MainWindowTests
    {
        // Tests for method LoadDiscounts that loads saved discounts from a file. Method ignores rows with incorrect data, codes duplication. 

        //public static void LoadDiscounts(List<CodeDiscount> discountsList, string path, ObservableCollection<string> discountsShow = null)


        [TestMethod()]
        public void Typical() // without optional parameter
        {
            List<CodeDiscount> keys = new List<CodeDiscount>();
            Butik.MainWindow.LoadDiscounts(keys, "Typical.csv");

            Assert.AreEqual("code1", keys[0].Code);
            Assert.AreEqual("code5", keys[1].Code);
            Assert.AreEqual("code10", keys[2].Code);
            Assert.AreEqual("watertower", keys[3].Code);
            Assert.AreEqual(1, keys[0].Discount);
            Assert.AreEqual(5, keys[1].Discount);
            Assert.AreEqual(10, keys[2].Discount);
            Assert.AreEqual(100, keys[3].Discount);
        }

        [TestMethod()]
        public void TwoLists() // with optional parameter
        {
            List<CodeDiscount> keys = new List<CodeDiscount>();
            ObservableCollection<string> discountsShow = new ObservableCollection<string>();
            Butik.MainWindow.LoadDiscounts(keys, "Typical.csv", discountsShow);

            Assert.AreEqual("code1", keys[0].Code);
            Assert.AreEqual("code5", keys[1].Code);
            Assert.AreEqual("code10", keys[2].Code);
            Assert.AreEqual("watertower", keys[3].Code);
            Assert.AreEqual(1, keys[0].Discount);
            Assert.AreEqual(5, keys[1].Discount);
            Assert.AreEqual(10, keys[2].Discount);
            Assert.AreEqual(100, keys[3].Discount);
            Assert.AreEqual("code1   1 %", discountsShow[0]);
            Assert.AreEqual("code5   5 %", discountsShow[1]);
            Assert.AreEqual("code10   10 %", discountsShow[2]);
            Assert.AreEqual("watertower   100 %", discountsShow[3]);
        }
        [TestMethod()]
        public void WrongCode() // incorrect codes - too short, too long, with charachters in addition to letters and numbers
        {
            List<CodeDiscount> keys = new List<CodeDiscount>();
            Butik.MainWindow.LoadDiscounts(keys, "WrongCode.csv");

            Assert.AreEqual("code1", keys[0].Code);
            Assert.AreEqual(1, keys[0].Discount);
        }
        [TestMethod()]
        public void WrongDiscount() // incorrect discounts - too big, too small, double
        {
            List<CodeDiscount> keys = new List<CodeDiscount>();
            Butik.MainWindow.LoadDiscounts(keys, "WrongDiscount.csv");

            Assert.AreEqual("code1", keys[0].Code);
            Assert.AreEqual(1, keys[0].Discount);
        }

        [TestMethod()]
        public void DuplicationCode()
        {
            List<CodeDiscount> keys = new List<CodeDiscount>();
            Butik.MainWindow.LoadDiscounts(keys, "DuplicationCode.csv");

            Assert.AreEqual("code1", keys[0].Code);
            Assert.AreEqual("code5", keys[1].Code);
            Assert.AreEqual(1, keys[0].Discount);
            Assert.AreEqual(1, keys[1].Discount);
        }
        [TestMethod()]
        public void WrongFormat() // missing/excess elements
        {
            List<CodeDiscount> keys = new List<CodeDiscount>();
            Butik.MainWindow.LoadDiscounts(keys, "WrongFormat.csv");

            Assert.AreEqual("code1", keys[0].Code);
            Assert.AreEqual("code5", keys[1].Code);
            Assert.AreEqual("code10", keys[2].Code);
            Assert.AreEqual("watertower", keys[3].Code);
            Assert.AreEqual(1, keys[0].Discount);
            Assert.AreEqual(5, keys[1].Discount);
            Assert.AreEqual(10, keys[2].Discount);
            Assert.AreEqual(100, keys[3].Discount);
        }
    }
}