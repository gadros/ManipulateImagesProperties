using NLog;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChangeImagesPropertiesApp
{
    internal class FolderHierarchyImageFilesManipulator
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();
        //TODO: make this configurable?
        private static readonly string[] s_extensionsOfFilesToManipulate = { ".jpg"/* ,".avi" isn't supported */, ".mp4", ".mpg"};
        private readonly bool m_simulationOnly;

        public FolderHierarchyImageFilesManipulator(bool simulationOnly)
        {
            m_simulationOnly = simulationOnly;
        }

        internal void OperateOnImagesInFolder(string directoryName)
        {
            var directoryInfo = new DirectoryInfo(directoryName);
            OperateOnImagesInDirectoryTree(directoryInfo, SingleImageFileOperation);
        }

        /// <summary>
        /// synchronously operates on image files in current folder and recursively operates on sub-folders
        /// </summary>
        /// <param name="currentFolder"></param>
        /// <param name="operationOnImage"></param>
        private void OperateOnImagesInDirectoryTree(DirectoryInfo currentFolder, Action<string> operationOnImage)
        {
            s_logger.Info($"operating on folder {currentFolder}");

            var files = GetImageFilesInFolder(currentFolder);

            Task operationOnImageFilesInCurrentFolder = null;
            if (files != null && files.Any())
            {

                try
                {
                    s_logger.Debug($"{files.Length} image files in {currentFolder.Name}");
                    operationOnImageFilesInCurrentFolder = OperateOnImagesInFolderAsync(files, operationOnImage);
                }
                catch (AggregateException ae)
                {
                    s_logger.Error(ae.ToString);
                }
            }

            DirectoryInfo[] subDirs = null;
            try
            {
                subDirs = currentFolder.GetDirectories();
            }
            catch (Exception e)
            {
                s_logger.Error(e);
            }

            var cts = new CancellationTokenSource();
            Task[] thisAndSubFoldersWork = new Task[0];
            if (subDirs != null)
            {
                if (operationOnImageFilesInCurrentFolder != null)
                {
                    thisAndSubFoldersWork = new Task[subDirs.Length + 1];
                    thisAndSubFoldersWork[0] = operationOnImageFilesInCurrentFolder;
                }
                else
                {
                    thisAndSubFoldersWork = new Task[subDirs.Length];
                }
            }
            else if (operationOnImageFilesInCurrentFolder != null)
            {
                thisAndSubFoldersWork = new[] {operationOnImageFilesInCurrentFolder};
            }

            subDirs?.Select(
                dirInfo =>
                {
                    return Task.Run(() =>
                    {
                        try
                        {
                            OperateOnImagesInDirectoryTree(dirInfo, operationOnImage);
                        }
                        catch (AggregateException ae)
                        {
                            s_logger.Error(ae.Flatten().ToString);
                        }
                    }, cts.Token);
                })
                .ToArray().CopyTo(thisAndSubFoldersWork, operationOnImageFilesInCurrentFolder != null ? 1 : 0);

            if (thisAndSubFoldersWork.Any())
            {
                Task.WaitAll(thisAndSubFoldersWork, cts.Token);
            }
        }

        //executes the complete operations per a single image file
        private void SingleImageFileOperation(string imageFileName)
        {
            var singleImageFileManipulator = new SingleImageFileManipulator(m_simulationOnly);
            singleImageFileManipulator.CheckAndChange(imageFileName);

        }

        private static FileInfo[] GetImageFilesInFolder(DirectoryInfo currentFolder)
        {
            FileInfo[] files = null;
            try
            {
                files = currentFolder.GetFiles().Where(fi => s_extensionsOfFilesToManipulate.Contains(fi.Extension.ToLower())).ToArray();
                files = files.Where(fn => !fn.Name.Contains("_original")).ToArray();
            }
            catch (UnauthorizedAccessException e)
            {
                s_logger.Error(e);
            }
            catch (DirectoryNotFoundException e)
            {
                s_logger.Error(e);
            }

            return files;
        }

        private Task OperateOnImagesInFolderAsync(FileInfo[] fileInfos, Action<string> opOnImage)
        {
            if (fileInfos == null || opOnImage == null)
            {
                return Task.CompletedTask;
            }

            return Task.Run(()=>
            {
                Parallel.ForEach(fileInfos, fi =>
                {
                    try
                    {
                        string imageFileName = fi.FullName;
                        s_logger.Info($"operating on image {imageFileName}");
                        opOnImage(imageFileName);
                    }
                    catch (Exception e)
                    {
                        s_logger.Warn($"failed on {fi.FullName}. {e}");
                    }
                });
            });
        }
    }
}