using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ImageFilePropertiesQueryAndEdit
{
    public class ImageFilePropertiesChanger
    {
        public enum PropertiesChangeResult
        {
            NoNeed,
            Success,
            Failure
        }

        private const string c_EXIF_DTOrig_Format = "yyyy:MM:dd HH:mm:ss";
        private const int c_EXIF_DTOrig = 36867;
        private static readonly Regex s_colonRegEx = new Regex(":");

        /* references related to getting or manipulating metadata of images
         * 2020-04-29: maybe should be deleted. didn't look at what's here in a long while
         * exiftool
         * --------
         * show date taken: exiftool.exe -EXIF:CreateDate Leah.jpg
         * show all attribute, show the attribute name: exiftool.exe "-all" -s lll.jpg
         * set date taken: exiftool.exe "-EXIF:CreateDate=1958:1:1 1:1:1" Leah.jpg
         * set date and title: exiftool.exe "-EXIF:CreateDate=1959:2:1 0:0:0" "-EXIF:XPTitle=bogobog" lll.jpg
         * http://www.sno.phy.queensu.ca/~phil/exiftool/
         * exiv2net
         * http://stackoverflow.com/questions/180030/how-can-i-find-out-when-a-picture-was-actually-taken-in-c-sharp-running-on-vista
         * http://www.sno.phy.queensu.ca/~phil/exiftool/
         * http://stackoverflow.com/questions/1038206/net-c-sharp-library-for-lossless-exif-rewriting
         * http://www.codeproject.com/Articles/3721/Photo-Properties
         * PropertyItem.Type Property - https://msdn.microsoft.com/en-us/library/system.drawing.imaging.propertyitem.type%28v=vs.110%29.aspx
         * PropertyItem.Id - https://msdn.microsoft.com/en-us/library/system.drawing.imaging.propertyitem.id%28v=vs.110%29.aspx
         * ImageFormat - https://msdn.microsoft.com/en-us/library/system.drawing.imaging.imageformat%28v=vs.110%29.aspx
         * How to modify Exif - https://social.msdn.microsoft.com/Forums/en-US/5717c15c-1e16-44f0-bb23-ca9ab7ef3cfe/how-to-modify-exif-of-digital-photo-by-c?forum=csharplanguage
         * https://en.wikipedia.org/wiki/Exchangeable_image_file_format
         * http://stackoverflow.com/questions/5708434/how-to-use-shell32-within-a-c-sharp-application, http://stackoverflow.com/questions/2483659/interop-type-cannot-be-embedded
         * http://blog.rodhowarth.com/2008/06/how-to-set-custom-attributes-file.html
         */

        private readonly string m_imageFileName;
        private FileInfo m_imageFileInfo;
        private readonly ImageProperties m_imageProperties;

        private static readonly DateTime s_minDateToTriggerDateChange = new DateTime(2010, 1, 1);
        private ImageProperties m_originalImageProperties;
        private readonly bool m_simulationMode;

        public ImageFilePropertiesChanger(string imageFileName, bool simulationOnly)
        {
            m_imageFileName = imageFileName;
            m_simulationMode = simulationOnly;
            m_originalImageProperties = m_imageProperties = PrepareImageProperties();
        }

        public PropertiesChangeResult ChangeImageProperties()
        {
            bool needToSetDateTaken = NeedToSetDateTaken();
            bool needToSetTitle = TitleIsMissing();

            if (needToSetTitle || needToSetDateTaken)
            {
                return UpdateImageAttributes();
            }

            return PropertiesChangeResult.NoNeed;
        }

        internal bool ChangedImageTitle()
        {
            m_originalImageProperties = PrepareImageProperties();
            bool needToSetTitle = TitleIsMissing();
            if (needToSetTitle)
            {
                string valueForTitle = GetValueForTitle();
                if (valueForTitle != null)
                {
                    if (valueForTitle.Contains("\""))
                    {
                        valueForTitle = EscapeDoubleQuotes(valueForTitle);
                    }
                    return ChangeImageTitle(valueForTitle);
                }

                return false;
            }

            return false;
        }

        private ImageProperties PrepareImageProperties()
        {
            ValidateImageExists();
            m_imageFileInfo = new FileInfo(m_imageFileName);
            return new ImageProperties(m_imageFileName, m_imageFileInfo, m_simulationMode);
        }

        private void ValidateImageExists()
        {
            if (!File.Exists(m_imageFileName))
            {
                throw new FileNotFoundException(null, m_imageFileName);
            }
        }

        private string EscapeDoubleQuotes(string valueForTitle)
        {
            List<char> newChars = new List<char>(valueForTitle.Length);
            foreach (char c in valueForTitle)
            {
                if (!c.Equals('"'))
                {
                    newChars.Add(c);
                }
                else
                {
                    newChars.Add('\\');
                    newChars.Add(c);
                }
            }
            string newTitle = new string(newChars.ToArray());
            return newTitle;
        }

        private bool ChangeImageTitle(string valueForTitle)
        {
            var runExifTool = new RunExifTool(m_imageFileName, m_simulationMode);
            return runExifTool.SetTitle(valueForTitle);
        }

        private PropertiesChangeResult UpdateImageAttributes()
        {
            string valueForTitle = GetValueForTitle();
            DateTime valueForDateTaken = GetValueForDateTaken();
            if (!string.IsNullOrEmpty(valueForTitle) || valueForDateTaken != DateTime.MinValue)
            {
                var runExifTool = new RunExifTool(m_imageFileName, m_simulationMode);
                bool operationResult;

                if (!string.IsNullOrEmpty(valueForTitle) && valueForDateTaken != DateTime.MinValue)
                {
                    operationResult = runExifTool.SetDateAndTitle(valueForTitle, valueForDateTaken);
                }
                else if (valueForDateTaken == DateTime.MinValue)
                {
                    operationResult = runExifTool.SetTitle(valueForTitle);
                }
                else
                {
                    operationResult = runExifTool.SetDateTaken(valueForDateTaken); 
                }

                return operationResult ? PropertiesChangeResult.Success : PropertiesChangeResult.Failure;
            }

            return PropertiesChangeResult.Failure;
        }

        private bool NeedToSetDateTaken()
        {
            DateTime dateTaken = m_originalImageProperties.DateTaken;
            if (dateTaken == DateTime.MinValue)
            {
                return true;
            }
            bool needToSetDateTaken = dateTaken > s_minDateToTriggerDateChange;
            return needToSetDateTaken;
        }

        private DateTime GetValueForDateTaken()
        {
            var dateTakenCalculator = new ImageDateTakenCalculator(m_imageProperties, m_imageFileInfo);
            return dateTakenCalculator.GetValueForDateTaken();
        }

        private bool TitleIsMissing()
        {
            if (!string.IsNullOrEmpty(m_imageProperties.ExTitle))
            {
                return false;
            }

            return true;
        }

        internal string GetValueForTitle()
        {
            if (!string.IsNullOrEmpty(m_imageProperties.ExTitle))
            {
                return m_imageProperties.ExTitle;
            }

            if (!string.IsNullOrEmpty(m_imageProperties.Title))
            {
                return m_imageProperties.Title;
            }

            if (!string.IsNullOrEmpty(m_imageProperties.Description))
            {
                return m_imageProperties.Description;
            }

            if (!string.IsNullOrEmpty(m_imageProperties.Subject))
            {
                return m_imageProperties.Subject;
            }

            string subjectOrComment = m_imageProperties.Comment;
            if (!string.IsNullOrEmpty(subjectOrComment))
            {
                return subjectOrComment;
            }

            return GetTitleFromName(m_imageFileInfo);
        }

        //at the moment i've decided to deduce a title from the file name and not to fallback to extract from folder name
        private string GetTitleFromName(FileInfo imageFileInfo)
        {
            if (imageFileInfo == null)
            {
                return null;
            }
            string fileName = imageFileInfo.Name;
            if (string.IsNullOrEmpty(fileName))
            {
                return null;
            }

            fileName = fileName.Substring(0, fileName.Length - imageFileInfo.Extension.Length);
            fileName = RemoveDateNameParts(fileName);
            string[] nameParts = fileName.Substring(0, fileName.Length)
                .Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < nameParts.Length; i++)
            {
                nameParts[i] = AdjustNamePartValueForTitle(nameParts[i]);
            }

            //drop the last parts if it's only numbers or single characters(many image files end with a running number)
            if (nameParts[nameParts.Length - 1].All(char.IsDigit)
                || nameParts[nameParts.Length - 1].Length == 1
                || char.IsDigit(nameParts[nameParts.Length - 1][nameParts[nameParts.Length - 1].Length - 1]))
            {
                ClearLastPartsThatAreFluff(nameParts);
            }

            return string.Join(" ", nameParts.Where(part => !string.IsNullOrEmpty(part)));
        }

        private static void ClearLastPartsThatAreFluff(string[] nameParts)
        {
            for (int i = nameParts.Length - 1; i > -1; i--)
            {
                if (nameParts[i].All(char.IsDigit))
                {
                    nameParts[i] = string.Empty;
                    continue;
                }
                if (nameParts[i].TrimEnd().Length == 1)
                {
                    nameParts[i] = string.Empty;
                    continue;
                }

                string[] suspectedFluff = nameParts[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                bool isFluff = true;
                if (suspectedFluff.Any())
                {
                    foreach (string part in suspectedFluff)
                    {
                        if (part.Length != 1 && !long.TryParse(part, out long _))
                        {
                            isFluff = false;
                            break;
                        }
                    }
                }

                if (isFluff)
                {
                    nameParts[i] = string.Empty;
                    continue;
                }

                if (char.IsDigit(nameParts[i][nameParts[i].Length - 1]))
                {
                    string part = nameParts[i];
                    int j;
                    for (j = part.Length - 1; j > -1; j--)
                    {
                        if (!char.IsDigit(part[j]))
                        {
                            break;
                        }
                    }

                    nameParts[i] = part.Substring(0, j);
                    if (nameParts[i].Length > 1)
                    {
                        break;
                    }
                }
            }
        }

        private string RemoveDateNameParts(string fileName)
        {
            string shortedIfMatchFound = ImageDateTakenCalculator.NameLikeYyyyMmddOrYyyymmDd.Replace(fileName, string.Empty);
            if (shortedIfMatchFound != fileName)
            {
                return shortedIfMatchFound;
            }

            shortedIfMatchFound = ImageDateTakenCalculator.NameLikeYyyymmdd.Replace(fileName, string.Empty);
            if (shortedIfMatchFound != fileName)
            {
                return shortedIfMatchFound;
            }

            shortedIfMatchFound = ImageDateTakenCalculator.NameIsMonthAndYearLikeMMYyyyOryyyymmOryyyyMM.Replace(fileName, string.Empty);
            if (shortedIfMatchFound != fileName)
            {
                return shortedIfMatchFound;
            }

            return fileName;
        }

        private string AdjustNamePartValueForTitle(string namePart)
        {
            bool containsDigits = namePart.Any(char.IsDigit);
            if (containsDigits)
            {
                namePart = Regex.Replace(namePart, @"(\d+|[A-z]+)", m => $"{m.Value} ");
            }

            namePart = Regex.Replace(namePart, "[a-z][A-Z]", m => $"{m.Value[0]} {m.Value[1]}");

            return namePart.Trim();
        }

        public string GetModifiedImageFileName()
        {
            string onlyFileName = m_imageFileInfo.Name.Substring(0, m_imageFileInfo.Name.Length - m_imageFileInfo.Extension.Length);
            string newImageFileName = Path.Combine(m_imageFileInfo.DirectoryName, onlyFileName + ".a" + m_imageFileInfo.Extension);
            return newImageFileName;
        }
    }
}
