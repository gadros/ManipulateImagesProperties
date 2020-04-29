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
        public void GetImageProperties_SetTitleAndDate_ValuesMatch()
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
