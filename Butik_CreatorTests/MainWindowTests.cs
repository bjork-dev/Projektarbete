using Microsoft.VisualStudio.TestTools.UnitTesting;
using Butik_Creator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Butik_Creator.Tests
{
    [TestClass()]
    public class MainWindowTests
    {
        [TestMethod()]
        public void LoadStoreCsvTest() // Test to load a csv file containing the items, if the path is found and items are extracted, return true.
                                        //If the list is empty after method run, return fail.
        {
            var testList = new List<Store>();
            string testPath = "testCSV.csv";
            try
            {
                bool result = MainWindow.LoadStoreCsv(testPath, testList);
                Assert.AreEqual(true, result);
                if (testList.Count == 0)
                {
                    Assert.Fail("List is empty");
                }
            }
            catch(Exception e)
            {
                Assert.Fail(e.Message);
            }   
        }

        [TestMethod()]
        public void AssortIsDuplicateTest() //Tests if there are multiple store items with the same name, if there is, the method returns true.
        {
            List<Store> storeList = new List<Store>();

            storeList.Add(new Store { Name = "tes", Price = 10, Description = "test", ImageName = "lundgrens.png" });
            storeList.Add(new Store { Name = "tEsT", Price = 10, Description = "test", ImageName = "lundgrens.png" });
            storeList.Add(new Store { Name = "banana", Price = 10, Description = "test", ImageName = "lundgrens.png" });
            storeList.Add(new Store { Name = "test", Price = 10, Description = "test", ImageName = "lundgrens.png" });

            const string name = "test";

            bool result = MainWindow.AssortIsDuplicate(name, storeList);
            Assert.AreEqual(true, result); //Should return true since there are 2 items with the same name
        }
    }
}