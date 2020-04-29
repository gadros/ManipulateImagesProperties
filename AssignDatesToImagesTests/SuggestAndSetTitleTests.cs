using System;
using System.IO;
using ImageFilePropertiesQueryAndEdit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AssignDatesToImagesTests
{
    [TestClass]
    public class SuggestAndSetTitleTests
    {
        [TestMethod]
        public void MoveImageSubjectToTitle_Exists_TitleIsFilled()
        {
            var expectedImageDate = new DateTime(1952, 10, 21); //we require 8 digit chars
            string testImage = $"{expectedImageDate.Year}{expectedImageDate.Month}{expectedImageDate.Day}_MoveSubjectToTitle.jpg";
            File.Copy(TestingImagesNames.LeahOnShip1957, testImage);
            new RunExifTool(testImage, false).SetDateAndTitle(string.Empty, DateTime.MinValue);

            var imageProperties1 = new ImageProperties(testImage);
            Assert.IsNotNull(imageProperties1.Subject);
            Assert.IsNull(imageProperties1.ExTitle);

            var sut = new ImageFilePropertiesChanger(testImage, false);
            sut.ChangeImageProperties();

            var imageProps = new ImageProperties(testImage);
            Assert.AreEqual(expectedImageDate, imageProps.DateTaken);

            string subject = imageProperties1.Subject;
            Assert.AreEqual(subject, imageProps.ExTitle);
        }

        [TestMethod]
        public void ChangeTitle_NoTitleSubjectExists_Updated()
        {
            string imageFileName = TestingImagesNames.ZelmaKoon;
            var existingProperties = new ImageProperties(imageFileName);
            string existingTitle = existingProperties.ExTitle;
            string existingSubject = existingProperties.Subject;
            Assert.AreNotEqual(existingSubject, existingTitle);

            string testImageFileName = "_NoTitleSubjectExists_.jpg";
            File.Copy(imageFileName, testImageFileName);

            var sut = new ImageFilePropertiesChanger(testImageFileName, false);
            sut.ChangeImageProperties();

            string newTitle = new ImageProperties(testImageFileName).ExTitle;
            Assert.AreEqual(existingSubject, newTitle);
        }

        //TODO: yes, yes, should be broken to multiple tests...
        [TestMethod]
        public void SuggestTitle_FromFileName_Success()
        {
            string testImage = TestingImagesNames.NoDateHint;
            var sut = new ImageFilePropertiesChanger(testImage, false);
            Assert.AreEqual("this is the title", sut.GetValueForTitle());

            const string noDateNoTitle = "NoDateHintNoTitle.jpg";
            File.Copy(testImage, noDateNoTitle);
            var exifTool = new RunExifTool(noDateNoTitle, false);
            bool success = exifTool.SetTitle(string.Empty);
            Assert.IsTrue(success);
            sut = new ImageFilePropertiesChanger(noDateNoTitle, true);
            Assert.AreEqual("No Date Hint No Title", sut.GetValueForTitle());
            //__7_0134.jpg, 10A_0216.jpg, 22A_0386.jpg, F1000019.JPG
            string imageFileName = "Leah_Small_Child.jpg";
            File.Copy(noDateNoTitle, imageFileName);
            sut = new ImageFilePropertiesChanger(imageFileName, true);
            Assert.AreEqual("Leah Small Child", sut.GetValueForTitle());

            imageFileName = "Leah_OnShip_2047.jpg";
            File.Copy(noDateNoTitle, imageFileName);
            sut = new ImageFilePropertiesChanger(imageFileName, true);
            Assert.AreEqual("Leah On Ship", sut.GetValueForTitle());

            imageFileName = "LeahOnShip_1957_blah.jpg";
            File.Copy(noDateNoTitle, imageFileName);
            sut = new ImageFilePropertiesChanger(imageFileName, true);
            Assert.AreEqual("Leah On Ship 1957 blah", sut.GetValueForTitle());

            imageFileName = "Zelma_Rosenthal_(KoonZ).jpg";
            File.Copy(noDateNoTitle, imageFileName);
            sut = new ImageFilePropertiesChanger(imageFileName, true);
            Assert.AreEqual("Zelma Rosenthal (Koon Z)", sut.GetValueForTitle());

            imageFileName = "בית_גרטה_פריץ_בנימינה_1871.jpg";
            File.Copy(noDateNoTitle, imageFileName);
            sut = new ImageFilePropertiesChanger(imageFileName, true);
            Assert.AreEqual("בית גרטה פריץ בנימינה", sut.GetValueForTitle());

            imageFileName = "2007_0926Keswick0020.JPG";
            File.Copy(noDateNoTitle, imageFileName);
            sut = new ImageFilePropertiesChanger(imageFileName, true);
            Assert.AreEqual("Keswick", sut.GetValueForTitle());

            imageFileName = "2007_0921EdinburghBotanicGarden38.JPG";
            File.Copy(noDateNoTitle, imageFileName);
            sut = new ImageFilePropertiesChanger(imageFileName, true);
            Assert.AreEqual("Edinburgh Botanic Garden", sut.GetValueForTitle());

            imageFileName = "20070921EdinburghBotanicGarden_36A_0582.jpg";
            File.Copy(noDateNoTitle, imageFileName);
            sut = new ImageFilePropertiesChanger(imageFileName, true);
            Assert.AreEqual("Edinburgh Botanic Garden", sut.GetValueForTitle());

            imageFileName = "20070920EdinburghCastle_F1030008.JPG";
            File.Copy(noDateNoTitle, imageFileName);
            sut = new ImageFilePropertiesChanger(imageFileName, true);
            Assert.AreEqual("Edinburgh Castle", sut.GetValueForTitle());
        }

        [TestMethod]
        public void SuggestTitle_FromFileName_NoLogicInName_SuggestsEmpty()
        {
            const string noDateNoTitle = "NoDateHintNoTitle2.jpg";
            File.Copy(TestingImagesNames.NoDateHint, noDateNoTitle);
            var exifTool = new RunExifTool(noDateNoTitle, false);
            bool success = exifTool.SetTitle(string.Empty);
            Assert.IsTrue(success);

            string imageFileName = "__7_0134.jpg";
            File.Copy(noDateNoTitle, imageFileName);
            var sut = new ImageFilePropertiesChanger(imageFileName, true);
            Assert.AreEqual(string.Empty, sut.GetValueForTitle());

            imageFileName = "10A_0216.jpg";
            File.Copy(noDateNoTitle, imageFileName);
            sut = new ImageFilePropertiesChanger(imageFileName, true);
            Assert.AreEqual(string.Empty, sut.GetValueForTitle());

            imageFileName = "22A_0386.jpg";
            File.Copy(noDateNoTitle, imageFileName);
            sut = new ImageFilePropertiesChanger(imageFileName, true);
            Assert.AreEqual(string.Empty, sut.GetValueForTitle());

            imageFileName = "F1000019.JPG";
            File.Copy(noDateNoTitle, imageFileName);
            sut = new ImageFilePropertiesChanger(imageFileName, true);
            Assert.AreEqual(string.Empty, sut.GetValueForTitle());
        }
    }
}
