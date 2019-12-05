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
    }
}
