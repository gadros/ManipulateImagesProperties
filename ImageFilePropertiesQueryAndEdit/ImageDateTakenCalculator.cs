using System;
using System.IO;
using System.Text.RegularExpressions;
using NLog;

namespace ImageFilePropertiesQueryAndEdit
{
    public class ImageDateTakenCalculator
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();
        private readonly FileInfo m_imageFileInfo;
        private readonly string m_imageTitle;

        public ImageDateTakenCalculator(ImageProperties imageProperties, FileInfo imageFileInfo)
        {
            m_imageFileInfo = imageFileInfo;
            m_imageTitle = imageProperties.ExTitle;
            if (string.IsNullOrEmpty(m_imageTitle))
            {
                m_imageTitle = imageProperties.Description;
            }
        }

        public DateTime GetValueForDateTaken()
        {
            if (!string.IsNullOrEmpty(m_imageTitle))
            {
                var calculateDateTakenByImageTitle = new CalculateDateTakenByImageTitle(m_imageTitle, m_imageFileInfo.FullName);
                DateTime dateTaken = calculateDateTakenByImageTitle.GetDateTakenFromTitle();
                if (dateTaken > DateTime.MinValue)
                {
                    return dateTaken;
                }
            }

            string imageFileName = m_imageFileInfo.Name;

            if (TryGetDateByName(imageFileName, out var valueForDateTaken)) return valueForDateTaken;

            return GetDateValueByDirectoryName();
        }

        private DateTime GetDateValueByDirectoryName()
        {
            string directoryName = m_imageFileInfo.DirectoryName;

            if (!string.IsNullOrWhiteSpace(directoryName))
            {
                int lastSlash = directoryName.LastIndexOf('\\');
                if (lastSlash > -1)
                {
                    directoryName = directoryName.Substring(lastSlash + 1, directoryName.Length - lastSlash - 1);
                }

                if (TryGetDateByName(directoryName, out var valueForDateTaken)) return valueForDateTaken;

                return DateTime.MinValue;
            }

            return DateTime.MinValue;
        }

        private bool TryGetDateByName(string name, out DateTime valueForDateTaken)
        {
            if (TryGetDateByFullDateValue(name, out valueForDateTaken))
            {
                return true;
            }
            //order is important because of regex of year-month might scoop the 1983_1995 and try parse it to the invalid value: 1983/19
            if (TryGetDateByYearsRange(name, out valueForDateTaken))
            {
                return true;
            }

            if (TryGetDateValueByYearMonth(name, out valueForDateTaken))
            {
                return true;
            }

            if (TryGetDateByYear(name, out valueForDateTaken))
            {
                return true;
            }

            valueForDateTaken = DateTime.MinValue;
            return false;
        }

        private static bool TryGetDateByFullDateValue(string imageFileName, out DateTime valueForDateTaken)
        {
            Match match = NameLikeYyyymmdd.Match(imageFileName);
            if (match.Success)
            {
                string[] datesParts = match.Value.Substring(0, match.Value.Length).Split(new []{'_'}, StringSplitOptions.RemoveEmptyEntries);

                if (datesParts.Length == 1)
                {
                    if (int.TryParse(datesParts[0].Substring(0, 4), out int year) &&
                        int.TryParse(datesParts[0].Substring(4, 2), out int month) &&
                        int.TryParse(datesParts[0].Substring(6, 2), out int day))
                    {
                        {
                            valueForDateTaken = new DateTime(year, month, day);
                            return true;
                        }
                    }

                    throw new ApplicationException(
                        $"failed parsing date although regex matched {nameof(NameLikeYyyymmdd)}, value was {imageFileName}");
                }
            }

            match = NameLikeYyyyMmddOrYyyymmDd.Match(imageFileName);
            if (match.Success)
            {
                string[] datesParts = match.Value.Substring(0, match.Value.Length).Split(new []{'_'}, StringSplitOptions.RemoveEmptyEntries);

                if (datesParts.Length == 2)
                {
                    if (datesParts[0].Length == 6)
                    {
                        if (int.TryParse(datesParts[0].Substring(0, 4), out int year) &&
                            int.TryParse(datesParts[0].Substring(4, 2), out int month) &&
                            int.TryParse(datesParts[1].Substring(0, 2), out int day))
                        {
                            {
                                valueForDateTaken = new DateTime(year, month, day);
                                return true;
                            }
                        }

                        throw new ApplicationException(
                            $"failed parsing date although regex matched {nameof(NameLikeYyyyMmddOrYyyymmDd)}, value was {imageFileName} (first part length 6");
                    }
                    if (datesParts[0].Length == 4)
                    {
                        if (int.TryParse(datesParts[0].Substring(0, 4), out int year) &&
                            int.TryParse(datesParts[1].Substring(0, 2), out int month) &&
                            int.TryParse(datesParts[1].Substring(2, 2), out int day))
                        {
                            {
                                valueForDateTaken = new DateTime(year, month, day);
                                return true;
                            }
                        }


                        throw new ApplicationException(
                            $"failed parsing date although regex matched {nameof(NameLikeYyyyMmddOrYyyymmDd)}, value was {imageFileName} (first part length 4");
                    }
                }

                throw new ApplicationException(
                    $"failed parsing date although regex matched {nameof(NameLikeYyyyMmddOrYyyymmDd)}, value was {imageFileName} but i found {datesParts.Length} parts");
            }

            valueForDateTaken = DateTime.MinValue;
            return false;
        }

        private static bool TryGetDateByYear(string name, out DateTime valueForDateTaken)
        {
            var match = s_yearValue.Match(name);
            if (match.Success)
            {
                if (int.TryParse(match.Value, out var year))
                {
                    {
                        valueForDateTaken = new DateTime(year, 1, 1);
                        return true;
                    }
                }

                s_logger.Warn($"although matched {nameof(s_yearValue)} failed determining date using year from: {name}");
                {
                    valueForDateTaken = DateTime.MinValue;
                    return true;
                }
            }

            valueForDateTaken = DateTime.MinValue;
            return false;
        }

        private static bool TryGetDateValueByYearMonth(string imageFileName, out DateTime dateTime)
        {
            var match = NameIsMonthAndYearLikeMMYyyyOryyyymmOryyyyMM.Match(imageFileName);
            if (match.Success)
            {
                string[] monthYear = match.Value.Substring(0, match.Value.Length).Split(new []{'_'}, StringSplitOptions.RemoveEmptyEntries);
                int year, month;

                if (monthYear.Length == 1 && monthYear[0].Length > 5)
                {
                    if (int.TryParse(monthYear[0].Substring(0, 4), out year) &&
                        int.TryParse(monthYear[0].Substring(4, 2), out month))
                    {
                        dateTime = new DateTime(year, month, 1);
                        return true;
                    }

                    throw new ApplicationException($"failed parsing {monthYear[0]} to a date value. for the name {imageFileName}");
                }

                if (monthYear.Length == 2)
                {
                    if (monthYear[0].Length == 4 && monthYear[1].Length == 2)
                    {
                        if (int.TryParse(monthYear[0].Substring(0, 4), out year) &&
                            int.TryParse(monthYear[1].Substring(0, 2), out month))
                        {
                            dateTime = new DateTime(year, month, 1);
                            return true;
                        }
                    }

                    if (monthYear[0].Length == 2 && monthYear[1].Length == 4)
                    {
                        if (int.TryParse(monthYear[0].Substring(0, 2), out month) &&
                            int.TryParse(monthYear[1].Substring(0, 4), out year))
                        {
                            dateTime = new DateTime(year, month, 1);
                            return true;
                        }
                    }

                    throw new ApplicationException(
                        $"failed parsing {match.Value} to a date value using year and month. for the name {imageFileName}");
                }
            }

            dateTime = DateTime.MinValue;
            return false;
        }

        private bool TryGetDateByYearsRange(string name, out DateTime dateTaken)
        {
            Match match = s_yearsRange.Match(name);
            if (match.Success)
            {
                {
                    dateTaken = GetDateTakenByFileNameYearsRange(match);
                    return dateTaken> DateTime.MinValue;
                }
            }

            dateTaken = DateTime.MinValue;
            return false;
        }

        private DateTime GetDateTakenByFileNameYearsRange(Match match)
        {
            var separatingChar = match.Value.Contains("_") ? '_' : '-';

            string[] yearsRanges = match.Value.Split(new []{separatingChar}, StringSplitOptions.RemoveEmptyEntries );
            if (yearsRanges.Length != 2)
            {
                throw new ApplicationException($"expected 2 years value separated by '_' or '-'. but the value was {match.Value}");
            }

            if (!int.TryParse(yearsRanges[0], out var year1))
            {
                s_logger.Warn("failed determining date for image: {0}", m_imageFileInfo.FullName);
                return DateTime.MinValue;
            }
            if (!int.TryParse(yearsRanges[1], out var year2))
            {
                s_logger.Warn("failed determining date for image: {0}", m_imageFileInfo.FullName);
                return DateTime.MinValue;
            }

            int startYear = year1;
            if (year1 > year2)
            {
                startYear = year2;
            }
            int yearsDifference = Math.Abs(year1 - year2);
            int addMonths = 6*yearsDifference;

            DateTime taken = new DateTime(startYear, 1, 1);
            taken = taken.AddMonths(addMonths);
            return taken;
        }

        private static readonly Regex s_dayMonthYearBySlash = new Regex("[0-9][0-9]/[0-9][0-9]/[0-9][0-9]");
        private static readonly Regex s_dayMonthYearBySlashLong = new Regex("[0-9][0-9]/[0-9][0-9]/19[0-9][0-9]");
        private static readonly Regex s_yearMonthBySlash = new Regex("[0-9][0-9]/19[0-9][0-9]");
        private static readonly Regex s_yearMonthWithUnderscore = new Regex("([0-9][0-9]_19[0-9][0-9])|(19[0-9][0-9])_[0-9][0-9]");
        private static readonly Regex s_yearsRange = new Regex("([1,2][9,0][0-9][0-9]_[1,2][9,0][0-9][0-9])|([1,2][9,0][0-9][0-9]-[1,2][9,0][0-9][0-9])");

        // 19xx or 20xx
        private static readonly Regex s_yearValue = new Regex("(19[0-9][0-9])|(20[0,1][0-9])");

        // 19620714 or 20160714 (test: full date)
        internal static readonly Regex NameLikeYyyymmdd = new Regex("(19[0-9][0-9][0,1][0-9][0,1,2,3][0-9])|(20[0-9][0-9][0,1][0-9][0,1,2,3][0-9])");

        // _07_1962, 196207_ (test: "196206_CanIgnore.jpg"), 1967_07_ (test: "1962_07_CanIgnore.jpg_
        internal static readonly Regex NameIsMonthAndYearLikeMMYyyyOryyyymmOryyyyMM = new Regex("(_[0,1][0-9]_19[0-9][0-9])|(_[0,1][0-9]_20[0-9][0-9])|(19[0-9][0-9][0,1][0-9])|(20[0-9][0-9][0,1][0-9])|(19[0-9][0-9]_[0,1][0-9])|(20[0-9][0-9]_[0,1][0-9])");

        // 1962_0714 or 2016_0714 (tests: 1962_0629_CanIgnore.jpg, 2007_922HeartOfScotland0014.JPG) or 196207_14 or 201607_14 (test: "196206_29_CanIgnore.jpg") //
        internal static readonly Regex NameLikeYyyyMmddOrYyyymmDd = new Regex("(19[0-9][0-9][0,1][0-9]_[0,1,2,3][0-9])|(20[0-9][0-9][0,1][0-9]_[0,1,2,3][0-9])|(19[0-9][0-9]_[0,1][0-9][0,1,2,3][0-9])|(20[0-9][0-9]_[0,1][0-9][0,1,2,3][0-9])");
    }
}