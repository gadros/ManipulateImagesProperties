using System;
using System.IO;
using ImageFilePropertiesQueryAndEdit;

namespace AssignDatesToImagesTests
{
    internal class TestingImagesNames
    {
        private static readonly string TestingImagesFolder = Path.Combine(Environment.CurrentDirectory, "TestingImages");
        public static string LeahOnShip1957 = "Leah_OnShip_1957.jpg";
        public static string LeahSmallChild = "Leah_Small_Child.jpg";
        public static string ZelmaKoon = "Zelma_Rosenthal_(Koon).jpg";
        public static string ShmuelRahel = "1977_1980_1.jpg";
        public static string GretaShmuel = "גרטה_שמואל_1976.jpg";
        public static string HouseBuild = "בית_גרטה_פריץ_בנימינה_1936.jpg";
        public static string AlonTechnion = "אלון_טכניון_1990.jpg";
        public static string HaimBaruch = "חיים וברוך 1990.jpg";
        public static string AlongHaim = "אלון ןחיים 1990.jpg";
        public static string GadBrit = "1963_1967_1.jpg";
        public static string YearUnderscoreMonthUnderscoreAndHebrewCharacters = "פיקניק_סחנה_07_1962.jpg";
        public static string NoDateHint = "NoDateHint.jpg";
    }

    internal class ImagePropertiesValidator
    {
        private readonly string m_imagedFileName;
        private readonly ImageProperties m_imageProperties;

        public ImagePropertiesValidator(string imageFileName)
        {
            m_imagedFileName = imageFileName;
            m_imageProperties = new ImageProperties(m_imagedFileName);
        }
    }
}
