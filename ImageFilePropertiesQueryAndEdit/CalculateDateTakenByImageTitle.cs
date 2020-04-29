using System;
using System.Text.RegularExpressions;
using NLog;

namespace ImageFilePropertiesQueryAndEdit
{
    //TODO: duplicate logic. merge logic and regex here with ImageDateTakenCalculator
    public class CalculateDateTakenByImageTitle
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();
        private readonly string m_imageTitle;

        private readonly Regex m_dayMonthYearBySlash = new Regex("[0-9][0-9]/[0-9][0-9]/[0-9][0-9]");
        private readonly Regex m_dayMonthYearBySlashLong = new Regex("[0-9][0-9]/[0-9][0-9]/19[0-9][0-9]");
        private readonly Regex m_yearMonthBySlash = new Regex("[0-9][0-9]/19[0-9][0-9]");
        private readonly Regex m_yearMonthWithUnderscore = new Regex("([0-9][0-9]_19[0-9][0-9])|(19[0-9][0-9])_[0-9][0-9]");
        private readonly Regex m_yearsRange = new Regex("(19[0-9][0-9]_19[0-9][0-9])|(19[0-9][0-9]-19[0-9][0-9])");
        private readonly Regex m_yearValue = new Regex("19[0-9][0-9]");
        private readonly string m_imageName;

        public CalculateDateTakenByImageTitle(string imageTitle, string imageName)
        {
            m_imageTitle = imageTitle;
            m_imageName = imageName;
        }

        public DateTime GetDateTakenFromTitle()
        {
            if (!string.IsNullOrEmpty(m_imageTitle))
            {
                // maybe the title contains a value like 01/08/1956. i.e.: 1st. August 1956
                Match match = m_dayMonthYearBySlash.Match(m_imageTitle);
                if (match.Success)
                {
                    return GetDateTakenByFullDate(match);
                }
                match = m_dayMonthYearBySlashLong.Match(m_imageTitle);
                if (match.Success)
                {
                    return GetDateTakenByFullDate(match);
                }

                // maybe the title contains a value like 08/1956. i.e.: August 1956
                match = m_yearMonthBySlash.Match(m_imageTitle);
                if (match.Success)
                {
                    return GetDateTakenWithSeparatingChar(match, '/');
                }

                // maybe the title contains a value like 08_1956. i.e.: August 1956
                match = m_yearMonthWithUnderscore.Match(m_imageTitle);
                if (match.Success)
                {
                    //HACK: is there real value in dates range? setting the mean?? just calculate it and remove the stupid logic
                    // but take care, maybe it's a years range. example: 1951_1956. i.e.: construct a date from the dates mean
                    Match yearsRangeMatch = m_yearsRange.Match(m_imageTitle);
                    if (yearsRangeMatch.Success)
                    {
                        return GetDateTakenByYearsRangeMatch(yearsRangeMatch);
                    }

                    return GetDateTakenWithSeparatingChar(match, '_');
                }

                // maybe the title contains only a year value like 1956. then constcrut the date 1/1/1956
                match = m_yearValue.Match(m_imageTitle);
                if (match.Success)
                {
                    if (!int.TryParse(match.Value,out var year))
                    {
                        s_logger.Warn("in {0} failed parsing by slash from title: {1}", m_imageName, m_imageTitle);
                        return DateTime.MinValue;
                    }
                    return new DateTime(year, 1, 1);
                }

                match = m_yearsRange.Match(m_imageTitle);
                if (match.Success)
                {
                    return GetDateTakenByYearsRangeMatch(match);
                }
            }

            return DateTime.MinValue;
        }

        private DateTime GetDateTakenByYearsRangeMatch(Match match)
        {
            char separatingChar = match.Value.Contains("_") ? '_' : '-';

            string[] monthYear = match.Value.Split(new[] { separatingChar });
            int year1;
            int year2;
            if (!int.TryParse(monthYear[0], out year1))
            {
                s_logger.Warn("in {0} failed parsing by slash from title: {1}", m_imageName, m_imageTitle);

                return DateTime.MinValue;
            }
            if (!int.TryParse(monthYear[1], out year2))
            {
                s_logger.Warn("in {0} failed parsing by slash from title: {1}", m_imageName, m_imageTitle);

                return DateTime.MinValue;
            }

            int startYear = year1;
            if (year1 > year2)
            {
                startYear = year2;
            }
            int yearsDifference = Math.Abs(year1 - year2);
            int addMonths = 6*yearsDifference;

            DateTime taken= new DateTime(startYear, 1, 1);
            taken = taken.AddMonths(addMonths);
            return taken;
        }

        private DateTime GetDateTakenWithSeparatingChar(Match match, char separatingChar)
        {
            string[] monthYear = match.Value.Split(new[] {separatingChar});
            if (!int.TryParse(monthYear[0], out var month))
            {
                s_logger.Warn("in {0} failed parsing by slash from title: {1}", m_imageName, m_imageTitle);

                return DateTime.MinValue;
            }
            if (!int.TryParse(monthYear[1], out var year))
            {
                s_logger.Warn("in {0} failed parsing by slash from title: {1}", m_imageName, m_imageTitle);

                return DateTime.MinValue;
            }
            return new DateTime(year, month, 1);
        }

        private DateTime GetDateTakenByFullDate(Match match)
        {
            string[] dayMonthYear = match.Value.Split(new[] {'/'});

            if (!int.TryParse(dayMonthYear[0], out var day))
            {
                s_logger.Warn("in {0} failed parsing by slash from title: {1}", m_imageName, m_imageTitle);

                return DateTime.MinValue;
            }

            if (!int.TryParse(dayMonthYear[1], out var month))
            {
                s_logger.Warn("in {0} failed parsing by slash from title: {1}", m_imageName, m_imageTitle);

                return DateTime.MinValue;
            }

            if (!int.TryParse(dayMonthYear[2], out var year))
            {
                s_logger.Warn("in {0} failed parsing by slash from title: {1}", m_imageName, m_imageTitle);

                return DateTime.MinValue;
            }

            if (year < 100)
            {
                year += 1900;
            }

            return new DateTime(year, month, day);
        }
    }
}