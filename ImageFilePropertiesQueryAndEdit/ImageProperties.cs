using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using Shell32;

namespace ImageFilePropertiesQueryAndEdit
{
    public class ImageProperties
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private readonly string m_imageFileName;
        private readonly Dictionary<string, string> m_imageProperties;
        private string[] m_propsSeparated;
        private readonly FileInfo m_imageFileInfo;
        private readonly bool m_simulationMode;

        /// <summary>
        /// the constructor fills image properties through the file system and Exif
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="imageFileInfo"></param>
        /// <param name="simulationMode"></param>
        public ImageProperties(string filename, FileInfo imageFileInfo = null, bool simulationMode = false)
        {
            m_imageFileName = filename;
            if (!Path.IsPathRooted(m_imageFileName))
            {
                m_imageFileName = Path.Combine(Environment.CurrentDirectory, m_imageFileName);
            }

            m_simulationMode = simulationMode;
            if (imageFileInfo != null)
            {
                m_imageFileInfo = imageFileInfo;
                if (!m_imageFileInfo.Exists)
                {
                    throw new FileNotFoundException(null, m_imageFileName);
                }
            }
            else
            {
                ValidateImageExists();
            }
            m_imageProperties = new Dictionary<string, string>();
            m_imageProperties = LoadImageProperties(); //don't want to do the right thing and separate between ctor and operation
        }

        private void ValidateImageExists()
        {
            if (!File.Exists(m_imageFileName))
            {
                throw new FileNotFoundException(null, m_imageFileName);
            }
        }

        private Dictionary<string, string> LoadImageProperties()
        {
            LoadFileProperties();
            AddFileSystemProperties();
            var runExifTool = new RunExifTool(m_imageFileName, m_simulationMode);
            string imageProperties = runExifTool.GetImageProperties();
            if (!string.IsNullOrEmpty(imageProperties))
            {
                m_propsSeparated = imageProperties.Split(new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries);

                FillExifProperty(ImagePropertyIds.Title);
                FillExifProperty(ImagePropertyIds.ExifToolTitle);
                FillExifProperty(ImagePropertyIds.ExifToolSubject);
                FillExifProperty(ImagePropertyIds.ExifDateTaken);
            }
            else
            {
                s_logger.Debug($"didn't get image properties through Exif for {m_imageFileName}");
            }

            return m_imageProperties;
        }

        private void LoadFileProperties()
        {
            Type t = Type.GetTypeFromProgID("Shell.Application");
            dynamic shl = Activator.CreateInstance(t);

            Folder fldr = shl.NameSpace(Path.GetDirectoryName(m_imageFileName));
            FolderItem itm = fldr.ParseName(Path.GetFileName(m_imageFileName));

            for (int i = 0; i < 100; i++) //100? i think i saw there were many but not a fixed number
            {
                string propValue = fldr.GetDetailsOf(itm, i);
                if (propValue != string.Empty)
                {
                    string propertyKey = fldr.GetDetailsOf(null, i);
                    if (!m_imageProperties.ContainsKey(propertyKey))
                    {
                        m_imageProperties.Add(propertyKey, propValue);
                    }
                    else
                    {
                        s_logger.Warn("attempted to add twice {0}", propertyKey);
                    }
                }
            }
        }

        private void AddFileSystemProperties()
        {
            if (m_imageFileInfo != null)
            {
                m_imageProperties.Add(ImagePropertyIds.FileName, m_imageFileInfo.Name);
                m_imageProperties.Add(ImagePropertyIds.DirectoryName, m_imageFileInfo.DirectoryName);
                m_imageProperties.Add(ImagePropertyIds.Extension, m_imageFileInfo.Extension);
            }
        }

        private void FillExifProperty(string exifToolPropertyName)
        {
            string property = m_propsSeparated.FirstOrDefault(line => line.StartsWith(exifToolPropertyName));
            if (property != null)
            {
                int colonLocation = property.IndexOf(':');
                string propertyValue = property.Substring(colonLocation + 1, property.Length - colonLocation - 1).Trim();
                if (!string.IsNullOrEmpty(propertyValue))
                {
                    m_imageProperties.Add(exifToolPropertyName, propertyValue);
                }
            }
        }

        public string Description => GetImageStringProperty(ImagePropertyIds.Description); //this is what shows in google photos as the description. so this is what we'll set when referring to title

        public string Title => GetImageStringProperty(ImagePropertyIds.Title);

        public string ExTitle => GetImageStringProperty(ImagePropertyIds.ExifToolTitle);

        public string Name => GetImageStringProperty(ImagePropertyIds.Name);

        public string DirectoryName => GetImageStringProperty(ImagePropertyIds.DirectoryName);

        public string Subject => GetImageStringProperty(ImagePropertyIds.ExifToolSubject);

        public string Comment => GetImageStringProperty(ImagePropertyIds.ExifToolComment);

        private string GetImageStringProperty(string propertyKey)
        {
            if (m_imageProperties.ContainsKey(propertyKey))
            {
                return m_imageProperties[propertyKey];
            }
            //s_logger.Warn("couldn't find {0} in {1}", propertyKey, m_imageFileName);
            return null;
        }

        public DateTime DateTaken
        {
            get
            {
                string dateTaken = GetImageStringProperty(ImagePropertyIds.ExifDateTaken);
                if (dateTaken != null)
                {
                    return new DateTime(int.Parse(dateTaken.Substring(0, 4)), int.Parse(dateTaken.Substring(5, 2)), int.Parse(dateTaken.Substring(8, 2)));
                }

                return DateTime.MinValue;
            }
        }
    }
}
