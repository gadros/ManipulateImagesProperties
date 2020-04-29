using System.IO;
using ImageFilePropertiesQueryAndEdit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

    namespace AssignDatesToImagesTests
{
    [TestClass]
    public class RunExifToolTests
    {
        [TestMethod]
        public void GetImageProperties_FileExists_Success()
        {
            string imageFileName = TestingImagesNames.LeahOnShip1957;
            var runExifTool = new RunExifTool(imageFileName, false);
            string imageProperties = runExifTool.GetImageProperties();
            Assert.IsNotNull(imageProperties);
        }

        [TestMethod]
        public void GetImageProperties_NoFile_EmptyResult()
        {
            var runExifTool = new RunExifTool("a", true);
            string imageProperties = runExifTool.GetImageProperties();
            Assert.IsTrue(string.IsNullOrEmpty(imageProperties));
        }

        [TestMethod]
        public void SetImageProperties_SetTitle_TitleMatches()
        {
            string testImage = "_SetTitle_.jpg";
            File.Copy(TestingImagesNames.LeahOnShip1957, testImage);
            var runExifTool = new RunExifTool(testImage, false);
            const string testHadSetTitle = "Test Had Set ExTitle";
            runExifTool.SetTitle(testHadSetTitle);
            var imageProperties = new ImageProperties(testImage);
            Assert.AreEqual(testHadSetTitle, imageProperties.ExTitle);
        }
    }
}
