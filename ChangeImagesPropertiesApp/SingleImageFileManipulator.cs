using System;
using System.IO;
using ImageFilePropertiesQueryAndEdit;
using NLog;

namespace ChangeImagesPropertiesApp
{
    //this class would probably be replaced if move to batch operations
    internal class SingleImageFileManipulator
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private readonly bool m_simulationOnly;

        public SingleImageFileManipulator(bool simulationOnly)
        {
            m_simulationOnly = simulationOnly;
        }

        internal void CheckAndChange(string imageFileName)
        {
            var imagePropsChanger = new ImageFilePropertiesChanger(imageFileName, m_simulationOnly);
            ImageFilePropertiesChanger.PropertiesChangeResult changeResult = imagePropsChanger.ChangeImageProperties();

            switch (changeResult)
            {
                case ImageFilePropertiesChanger.PropertiesChangeResult.Success:
                    s_logger.Debug($"successfully changed {imageFileName}");
                    break;
                case ImageFilePropertiesChanger.PropertiesChangeResult.Failure:
                    s_logger.Warn($"failed changing {imageFileName}");
                    break;
                default:
                    break;
            }
        }

        private ImageFilePropertiesChanger.PropertiesChangeResult ChangeImageFileProperties(string imageFileName)
        {
            var changeImageAttributes = new ImageFilePropertiesChanger(imageFileName, m_simulationOnly);
            return changeImageAttributes.ChangeImageProperties();
        }

        private static (string fileName, string folderName) FillFileSystemProps(string imageName)
        {
            try
            {
                FileInfo imgFileInfo = new FileInfo(imageName);
                return (imgFileInfo.Name, imgFileInfo.DirectoryName);
            }
            catch (Exception e)
            {
                s_logger.Error(e);
            }

            return (string.Empty, string.Empty);
        }
    }
}