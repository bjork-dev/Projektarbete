﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Butik_Creator;
using System;
using System.Collections.Generic;
using System.Text;
using Butik;
using System.IO;
using System.Threading;



/*
 * 
 * STATHREAD
 * It tells the compiler that you're in a Single Thread Apartment model. This is an evil COM thing, it's usually used for Windows Forms (GUI's) as that uses Win32 for its drawing, 
 * which is implemented as STA. If you are using something that's STA model from multiple threads then you get corrupted objects.
 *
 * This is why you have to invoke onto the Gui from another thread (if you've done any forms coding).
 *
 * Basically don't worry about it, just accept that Windows GUI threads must be marked as STA otherwise weird stuff happens.
 * 
 * 
 * 
*/



namespace Butik_Creator.Tests
{
[TestClass()]
public class MainWindowTests
{
    [TestMethod()]
    public void LoadStoreCsvTest() // Test to load a csv file containing the items, if the path is found and items are extracted, return true.
                                   //If the list is empty after method run, return fail.
    {
        Thread STAThread = new Thread(() =>
        {
            var testList = new List<Store>();
            string testPath = "testCSV.csv";

            bool result = MainWindow.LoadStoreCsv(testPath, testList);
            Assert.AreEqual(true, result);
            if (testList.Count == 0)
            {
                Assert.Fail("List is empty");
            }

        });
        STAThread.SetApartmentState(ApartmentState.STA); //Test would not work without this for some reason

        STAThread.Start();

        STAThread.Join();
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
    [TestMethod()]
    public void LoadImagesTest()
    {
        Thread STAThread = new Thread(() =>
        {


            string testPath = System.Environment.CurrentDirectory;
            bool result = MainWindow.AddImages(testPath);
            Assert.AreEqual(true, result);
        });
        STAThread.SetApartmentState(ApartmentState.STA); //Test would not work without this for some reason

        STAThread.Start();

        STAThread.Join();
    }
}
}