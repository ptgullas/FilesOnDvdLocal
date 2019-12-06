using System;
using System.Data;
using FilesOnDvdLocal.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FilesOnDvdLocal.Tests
{
    [TestClass]
    public class AccessRetrieverTests
    {
        [TestMethod]
        public void GetSeries_Temp_ActuallyGetsSeries() {
            string dbLocation = @"c:\temp\Files on Dvd.accdb";
            AccessRetriever myRetriever = new AccessRetriever(dbLocation);
            DataTable dt = myRetriever.GetSeries();
            string seriesToLookUp = "in the vIP";
            DataRow[] selectedRows = dt.Select($"Series = '{seriesToLookUp}'");
        }

        [TestMethod]
        public void GetSeriesAndPerformersAndAliases_Temp_ActuallyGetsSeries() {
            string dbLocation = @"c:\temp\Files on Dvd.accdb";
            AccessRetriever myRetriever = new AccessRetriever(dbLocation);
            DataSet resultSet = myRetriever.GetSeriesAndPerformersAndAliases();
            DataTable seriesTable = resultSet.Tables[0];
            string seriesToLookUp = "doctor who podshock";
            DataRow[] selectedSeriesRows = seriesTable.Select($"Series = '{seriesToLookUp}'");

            DataTable performersTable = resultSet.Tables[1];
            string performerToLookUp = "christine mendoza";
            DataRow[] selectedPerformersRows = performersTable.Select($"Performer = '{performerToLookUp}'");

            DataTable aliasTable = resultSet.Tables[2];
            string aliasToLookUp = "lucky";
            DataRow[] selectedAliasRows = aliasTable.Select($"Alias = '{aliasToLookUp}'");


            Console.WriteLine("debug this line");
        }

        [TestMethod]
        public void UpdateDiscs_Temp_ActuallyUpdateDiscs() {
            string dbLocation = @"c:\temp\Files on Dvd.accdb";
            AccessRetriever myRetriever = new AccessRetriever(dbLocation);
            DataSet dataSet = myRetriever.GetDiscs();
            DataRow newDisc = dataSet.Tables[0].NewRow();
            newDisc["DiscName"] = "TestDisc22";
            newDisc["Wallet"] = 1;
            newDisc["Notes"] = "I hope this works22";
            dataSet.Tables[0].Rows.Add(newDisc);
            myRetriever.UpdateDiscs(dataSet);
            Console.WriteLine("Debug this line");
        }

        [TestMethod]
        public void UpdateFileEntries_Temp_ActuallyUpdateFileEntries() {
            string dbLocation = @"c:\temp\Files on Dvd.accdb";
            AccessRetriever myRetriever = new AccessRetriever(dbLocation);
            DataSet dataSet = myRetriever.GetFileEntries();
            DataRow newEntry = dataSet.Tables[0].NewRow();
            newEntry["Filename"] = "Succession - 1.01 - Pilot";
            newEntry["Genre"] = 3;
            newEntry["Disc"] = 224;
            newEntry["Series"] = 14;
            dataSet.Tables[0].Rows.Add(newEntry);

            DataRow newEntry2 = dataSet.Tables[0].NewRow();
            newEntry2["Filename"] = "Buffy the Vampire Slayer - 1.01 - Welcome to the Hellmouth";
            newEntry2["Genre"] = 3;
            newEntry2["Disc"] = 224;
            newEntry2["Series"] = 15;
            dataSet.Tables[0].Rows.Add(newEntry2);

            myRetriever.UpdateFileEntries(dataSet);
            Console.WriteLine("Debug this line");
        }
    }
}
