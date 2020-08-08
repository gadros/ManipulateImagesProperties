using System;
using System.IO;
using System.Text;
using ImageFilePropertiesQueryAndEdit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AssignDatesToImagesTests
{
    [TestClass]
    public class ImagePropertiesTests
    {
        [TestMethod]
        public void SetImageProperties_SetTitleAndDate_ValuesMatch()
        {
            string imageFileName = "SetTitleAndDate.jpg";
            File.Copy(TestingImagesNames.LeahOnShip1957, imageFileName);

            var runExifTool = new RunExifTool(imageFileName, false);
            const string testHadSetTitle = "Test Had Set ExTitle";
            DateTime expectedDateTaken = new DateTime(1980, 12, 12);
            runExifTool.SetDateAndTitle(testHadSetTitle, expectedDateTaken);
            var imageProperties = new ImageProperties(imageFileName);
            Assert.AreEqual(testHadSetTitle, imageProperties.ExTitle);
            Assert.AreEqual(expectedDateTaken, imageProperties.DateTaken);
        }

        [TestMethod]
        public void SetImageProperties_FullFileNamePathContainsSpacesAndHebrew_SetTitleAndDate_ValuesMatch()
        {
            const string subFolder = "20080328 מרפסת ותפלץ\\20080328";
            string copyToFolder = Path.Combine(Environment.CurrentDirectory, subFolder);
            if (!Directory.Exists(copyToFolder))
            {
                Directory.CreateDirectory(copyToFolder);
            }

            string imageFileName = Path.Combine(copyToFolder, TestingImagesNames.LeahOnShip1957);
            File.Copy(TestingImagesNames.LeahOnShip1957, imageFileName);

            var runExifTool = new RunExifTool(imageFileName, false);
            const string testHadSetTitle = "דפנה לולו";
            DateTime expectedDateTaken = new DateTime(1980, 12, 12);
            runExifTool.SetDateAndTitle(testHadSetTitle, expectedDateTaken);
            var imageProperties = new ImageProperties(imageFileName);
            Assert.AreEqual(testHadSetTitle, imageProperties.ExTitle);
            Assert.AreEqual(expectedDateTaken, imageProperties.DateTaken);
        }

        [TestMethod]
        public void FuckingChangeChars()
        {
            string orig = "׳׳׳” ׳‘׳׳—׳ ׳” ׳§׳™׳¥ ׳׳•׳’׳•׳¡׳˜ 1952";
            Encoding iso = Encoding.ASCII;//.GetEncoding("ISO-8859-1");
            Encoding utf8 = Encoding.UTF8;
            byte[] utfBytes = iso.GetBytes(orig);
            byte[] isoBytes = Encoding.Convert( iso, utf8, utfBytes);
            string msg = iso.GetString(isoBytes);
            Console.WriteLine(msg);
        }
    }
}
