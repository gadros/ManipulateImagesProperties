using System;
using System.IO;
using ImageFilePropertiesQueryAndEdit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AssignDatesToImagesTests
{
    //TODO: incomplete?
    [TestClass]
    public class DateTakenCalculatorTests
    {
        [TestMethod]
        public void CalculateDateTaken_bySlash_Success()
        {
            string imageFileName = TestingImagesNames.ShmuelRahel;

            var dateTakenCalculator = new ImageDateTakenCalculator(new ImageProperties(imageFileName),new FileInfo(imageFileName) );
            DateTime valueForDateTaken = dateTakenCalculator.GetValueForDateTaken();
            Assert.AreEqual(new DateTime(1979, 8, 1), valueForDateTaken);
        }

        [TestMethod]
        public void CalculateDateTaken_byUnderscoreWithMonthAndDate_Success()
        {
            string imageFileName = TestingImagesNames.GretaShmuel;

            var dateTakenCalculator = new ImageDateTakenCalculator(new ImageProperties(imageFileName), new FileInfo(imageFileName));
            DateTime valueForDateTaken = dateTakenCalculator.GetValueForDateTaken();
            Assert.AreEqual(new DateTime(1976, 3, 1), valueForDateTaken);

            imageFileName = TestingImagesNames.AlonTechnion;
            dateTakenCalculator = new ImageDateTakenCalculator(new ImageProperties(imageFileName), new FileInfo(imageFileName));
            valueForDateTaken = dateTakenCalculator.GetValueForDateTaken();
            Assert.AreEqual(new DateTime(1990, 3, 1), valueForDateTaken);
        }

        [TestMethod]
        public void CalculateDateTaken_byYearsRange_Success()
        {
            string imageFileName = TestingImagesNames.HouseBuild;

            var dateTakenCalculator = new ImageDateTakenCalculator(new ImageProperties(imageFileName), new FileInfo(imageFileName));
            DateTime valueForDateTaken = dateTakenCalculator.GetValueForDateTaken();
            Assert.AreEqual(new DateTime(1934, 7, 1), valueForDateTaken);
        }

        [TestMethod]
        public void CalculateDateTaken_byYear_Success()
        {
            string imageFileName = TestingImagesNames.HaimBaruch;

            var dateTakenCalculator = new ImageDateTakenCalculator(new ImageProperties(imageFileName), new FileInfo(imageFileName));
            DateTime valueForDateTaken = dateTakenCalculator.GetValueForDateTaken();
            Assert.AreEqual(new DateTime(1988, 1, 1), valueForDateTaken);
        }

        [TestMethod]
        public void CalculateDateTaken_byFullDateIsImageTitle_Success()
        {
            string imageFileName = TestingImagesNames.GadBrit;

            var dateTakenCalculator = new ImageDateTakenCalculator(new ImageProperties(imageFileName), new FileInfo(imageFileName));
            DateTime valueForDateTaken = dateTakenCalculator.GetValueForDateTaken();
            Assert.AreEqual(new DateTime(1964, 10, 15), valueForDateTaken);
        }

        [TestMethod]
        public void CalculateDateTaken_ByFullDateOnImageName_Success()
        {
            string imageFileName = TestingImagesNames.NoDateHint;

            string imageStartsWith8DigitDate = "19771027_" + imageFileName;
            File.Copy(imageFileName, imageStartsWith8DigitDate);
            var dateTakenCalculator = new ImageDateTakenCalculator(new ImageProperties(imageStartsWith8DigitDate), new FileInfo(imageStartsWith8DigitDate));
            DateTime valueForDateTaken = dateTakenCalculator.GetValueForDateTaken();
            Assert.AreEqual(new DateTime(1977, 10, 27), valueForDateTaken);
        }

        [TestMethod]
        public void CalculateDateTaken_NoHint_ReturnsDateTimeMin()
        {
            string imageFileName = TestingImagesNames.NoDateHint;

            var dateTakenCalculator = new ImageDateTakenCalculator(new ImageProperties(imageFileName), new FileInfo(imageFileName));
            DateTime valueForDateTaken = dateTakenCalculator.GetValueForDateTaken();
            Assert.AreEqual(DateTime.MinValue, valueForDateTaken);
        }

        [TestMethod]
        public void CalculateDateTaken_ManyVariationsOfDatesWithDifferentUnderScorePositions_Success()
        {
            string imageFileName = TestingImagesNames.NoDateHint;

            string yearMonthUnderscoreDayAndFileName = "200910_03" + imageFileName;
            File.Copy(imageFileName, yearMonthUnderscoreDayAndFileName);
            var dateTakenCalculator = new ImageDateTakenCalculator(new ImageProperties(yearMonthUnderscoreDayAndFileName), new FileInfo(yearMonthUnderscoreDayAndFileName));
            DateTime valueForDateTaken = dateTakenCalculator.GetValueForDateTaken();
            Assert.AreEqual(new DateTime(2009, 10, 3), valueForDateTaken);

            string yearUnderscoreMonthAndDay = "1962_0629_CanIgnore.jpg";
            File.Copy(imageFileName, yearUnderscoreMonthAndDay);
            dateTakenCalculator = new ImageDateTakenCalculator(new ImageProperties(yearUnderscoreMonthAndDay), new FileInfo(yearUnderscoreMonthAndDay));
            valueForDateTaken = dateTakenCalculator.GetValueForDateTaken();
            Assert.AreEqual(new DateTime(1962, 6, 29), valueForDateTaken);

            string yearAndMonthUnderscoreDay = "196206_29_CanIgnore.jpg";
            File.Copy(imageFileName, yearAndMonthUnderscoreDay);
            dateTakenCalculator = new ImageDateTakenCalculator(new ImageProperties(yearAndMonthUnderscoreDay), new FileInfo(yearAndMonthUnderscoreDay));
            valueForDateTaken = dateTakenCalculator.GetValueForDateTaken();
            Assert.AreEqual(new DateTime(1962, 6, 29), valueForDateTaken);

            string yearAndMonthUnderscoreAndBlahToIgnore = "196206_CanIgnore.jpg";
            File.Copy(imageFileName, yearAndMonthUnderscoreAndBlahToIgnore);
            dateTakenCalculator = new ImageDateTakenCalculator(new ImageProperties(yearAndMonthUnderscoreAndBlahToIgnore), new FileInfo(yearAndMonthUnderscoreAndBlahToIgnore));
            valueForDateTaken = dateTakenCalculator.GetValueForDateTaken();
            Assert.AreEqual(new DateTime(1962, 6, 1), valueForDateTaken);

            string yearUnderscoreMonthAndBlahToIgnore = "1962_07_CanIgnore.jpg";
            File.Copy(imageFileName, yearUnderscoreMonthAndBlahToIgnore);
            dateTakenCalculator = new ImageDateTakenCalculator(new ImageProperties(yearUnderscoreMonthAndBlahToIgnore), new FileInfo(yearUnderscoreMonthAndBlahToIgnore));
            valueForDateTaken = dateTakenCalculator.GetValueForDateTaken();
            Assert.AreEqual(new DateTime(1962, 7, 1), valueForDateTaken);

            string yearUnderscoreMonthUnderscoreAndHebrewCharacters = TestingImagesNames.YearUnderscoreMonthUnderscoreAndHebrewCharacters;
            dateTakenCalculator = new ImageDateTakenCalculator(new ImageProperties(yearUnderscoreMonthUnderscoreAndHebrewCharacters), new FileInfo(yearUnderscoreMonthUnderscoreAndHebrewCharacters));
            valueForDateTaken = dateTakenCalculator.GetValueForDateTaken();
            Assert.AreEqual(new DateTime(1962, 7, 1), valueForDateTaken);
        }

        [TestMethod]
        public void CalculateDateTaken_ManyVariationsOfDatesWithDifferentUnderScorePositions_NoUnderscoreAfterDate_Success()
        {
            string imageFileName = TestingImagesNames.NoDateHint;

            string yearUnderscoreMonthAndDay = "1962_0629CanIgnore.jpg";
            File.Copy(imageFileName, yearUnderscoreMonthAndDay);
            var dateTakenCalculator = new ImageDateTakenCalculator(new ImageProperties(yearUnderscoreMonthAndDay), new FileInfo(yearUnderscoreMonthAndDay));
            DateTime valueForDateTaken = dateTakenCalculator.GetValueForDateTaken();
            Assert.AreEqual(new DateTime(1962, 6, 29), valueForDateTaken);

            string yearAndMonthUnderscoreDay = "196206_29CanIgnore.jpg";
            File.Copy(imageFileName, yearAndMonthUnderscoreDay);
            dateTakenCalculator = new ImageDateTakenCalculator(new ImageProperties(yearAndMonthUnderscoreDay), new FileInfo(yearAndMonthUnderscoreDay));
            valueForDateTaken = dateTakenCalculator.GetValueForDateTaken();
            Assert.AreEqual(new DateTime(1962, 6, 29), valueForDateTaken);

            string yearAndMonthUnderscoreAndBlahToIgnore = "196206CanIgnore.jpg";
            File.Copy(imageFileName, yearAndMonthUnderscoreAndBlahToIgnore);
            dateTakenCalculator = new ImageDateTakenCalculator(new ImageProperties(yearAndMonthUnderscoreAndBlahToIgnore), new FileInfo(yearAndMonthUnderscoreAndBlahToIgnore));
            valueForDateTaken = dateTakenCalculator.GetValueForDateTaken();
            Assert.AreEqual(new DateTime(1962, 6, 1), valueForDateTaken);

            string yearUnderscoreMonthAndDayBlahToIgnore = "2007_0922HeartOfScotland0014.JPG";
            File.Copy(imageFileName, yearUnderscoreMonthAndDayBlahToIgnore);
            dateTakenCalculator = new ImageDateTakenCalculator(new ImageProperties(yearUnderscoreMonthAndDayBlahToIgnore), new FileInfo(yearUnderscoreMonthAndDayBlahToIgnore));
            valueForDateTaken = dateTakenCalculator.GetValueForDateTaken();
            Assert.AreEqual(new DateTime(2007, 9, 22), valueForDateTaken);
            
            string yearUnderscoreMonthAndBlahToIgnore = "1962_07CanIgnore.jpg";
            File.Copy(imageFileName, yearUnderscoreMonthAndBlahToIgnore);
            dateTakenCalculator = new ImageDateTakenCalculator(new ImageProperties(yearUnderscoreMonthAndBlahToIgnore), new FileInfo(yearUnderscoreMonthAndBlahToIgnore));
            valueForDateTaken = dateTakenCalculator.GetValueForDateTaken();
            Assert.AreEqual(new DateTime(1962, 7, 1), valueForDateTaken);
        }

        [TestMethod]
        public void CalculateDateTaken_ByDirectoryYearsRange_Success()
        {
            string imageFileName = TestingImagesNames.NoDateHint;
            string imageFolder = Path.Combine(Environment.CurrentDirectory, "1966_1970");
            if (!Directory.Exists(imageFolder))
            {
                Directory.CreateDirectory(imageFolder);
            }
            File.Copy(imageFileName, Path.Combine(imageFolder, imageFileName));

            imageFileName = Path.Combine(imageFolder, imageFileName);
            var dateTakenCalculator = new ImageDateTakenCalculator(new ImageProperties(imageFileName), new FileInfo(imageFileName));
            DateTime valueForDateTaken = dateTakenCalculator.GetValueForDateTaken();
            Assert.AreEqual(new DateTime(1968, 1, 1), valueForDateTaken);
        }

        [TestMethod]
        public void CalculateDateTaken_ByDirectoryYear_Success()
        {
            string imageFileName = TestingImagesNames.NoDateHint;
            string imageFolder = Path.Combine(Environment.CurrentDirectory, "1977");
            if (!Directory.Exists(imageFolder))
            {
                Directory.CreateDirectory(imageFolder);
            }
            File.Copy(imageFileName, Path.Combine(imageFolder, imageFileName));

            imageFileName = Path.Combine(imageFolder, imageFileName);
            var dateTakenCalculator = new ImageDateTakenCalculator(new ImageProperties(imageFileName), new FileInfo(imageFileName));
            DateTime valueForDateTaken = dateTakenCalculator.GetValueForDateTaken();
            Assert.AreEqual(new DateTime(1977, 1, 1), valueForDateTaken);
        }

        [TestMethod]
        public void CalculateDateTaken_ByDirectoryName_Success()
        {
            string imageFileName = TestingImagesNames.NoDateHint;
            string imageFolder = Path.Combine(Environment.CurrentDirectory, "19770201");
            if (!Directory.Exists(imageFolder))
            {
                Directory.CreateDirectory(imageFolder);
            }
            File.Copy(imageFileName, Path.Combine(imageFolder, imageFileName));

            imageFileName = Path.Combine(imageFolder, imageFileName);
            var dateTakenCalculator = new ImageDateTakenCalculator(new ImageProperties(imageFileName), new FileInfo(imageFileName));
            DateTime valueForDateTaken = dateTakenCalculator.GetValueForDateTaken();
            Assert.AreEqual(new DateTime(1977, 2, 1), valueForDateTaken);
        }

        [TestMethod]
        public void CalculateDateTaken_ValidatePriorityOfCalculation_Success()
        {
            DateTime dateByFolderName = new DateTime(2001, 11, 29);
            string imageFolder = Path.Combine(Environment.CurrentDirectory, $"{dateByFolderName.Year}{dateByFolderName.Month}{dateByFolderName.Day}_CanIgnoreThis");
            if (!Directory.Exists(imageFolder))
            {
                Directory.CreateDirectory(imageFolder);
            }

            //date value in title takes precedence
            string dateInTitle = Path.Combine(imageFolder, TestingImagesNames.GretaShmuel);
            File.Copy(TestingImagesNames.GretaShmuel, dateInTitle);
            var dateTakenCalculator = new ImageDateTakenCalculator(new ImageProperties(dateInTitle), new FileInfo(dateInTitle));
            DateTime valueForDateTaken = dateTakenCalculator.GetValueForDateTaken();
            Assert.AreEqual(new DateTime(1976, 3, 1), valueForDateTaken);

            //fallback to date value in image file name
            string dateInName = Path.Combine(imageFolder, "19611231_"+ TestingImagesNames.NoDateHint);
            File.Copy(TestingImagesNames.NoDateHint, dateInName);
            dateTakenCalculator = new ImageDateTakenCalculator(new ImageProperties(dateInName), new FileInfo(dateInName));
            valueForDateTaken = dateTakenCalculator.GetValueForDateTaken();
            Assert.AreEqual(new DateTime(1961, 12, 31), valueForDateTaken);

            //least priority fallback to date by folder name
            string noDateHintInName = Path.Combine(imageFolder, TestingImagesNames.NoDateHint);
            File.Copy(TestingImagesNames.NoDateHint, noDateHintInName);
            dateTakenCalculator = new ImageDateTakenCalculator(new ImageProperties(noDateHintInName), new FileInfo(noDateHintInName));
            valueForDateTaken = dateTakenCalculator.GetValueForDateTaken();
            Assert.AreEqual(dateByFolderName, valueForDateTaken);
        }
    }
}
