using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace ImageFilePropertiesQueryAndEdit
{
    //TODO: move to use batch operations instead of running a separate ExifTool process per file...this will require consumers to change as well
    //TODO: to async?
    public class RunExifTool
    {
        const string c_exifToolFileName = @".\exiftool.exe";

        private static readonly Logger s_logger;
        private static readonly bool s_debugLoggingActive;

        static RunExifTool()
        {
            s_logger = LogManager.GetCurrentClassLogger();
            s_debugLoggingActive = s_logger.IsDebugEnabled;
        }

        private readonly string m_imageFileName;
        private string m_exifOutput;
        private readonly bool m_simulationMode;

        public RunExifTool(string imageFileName, bool simulationMode)
        {
            m_imageFileName = imageFileName;
            if (!Path.IsPathRooted(m_imageFileName))
            {
                m_imageFileName = Path.Combine(Environment.CurrentDirectory, m_imageFileName);
            }
            m_simulationMode = simulationMode;
        }

        public string GetImageProperties()
        {
            if (RunQueryUsingAFile("-all -s"))
            {
                return m_exifOutput;
            }

            return null;
        }

        private bool RunUpdateUsingAFile(string exifToolArguments)
        {
            return ExecuteExifOperationUsingAFile(Update, exifToolArguments, true);
        }

        private bool RunQueryUsingAFile(string exifToolArguments)
        {
            return ExecuteExifOperationUsingAFile(Query, exifToolArguments, false);
        }

        /// <summary>
        /// so the reason i use a file to contain the image file name is that when the image file name (full path) contains mixed english-numbers-hebrew
        /// character then ExifTool returns an error and the suggested path was to wrap the image file name like this
        /// </summary>
        /// <param name="syncExifOperation">a function that accepts a string for the file to operate on and returns bool on success. responsible for error handling and reporting</param>
        /// <param name="exifToolArguments"></param>
        /// <param name="writeArgumentsToFile">when true these are written to the arg file. when false the arguments are sent on the commandline</param>
        /// <returns>true on successful ExifTool operation</returns>
        private bool ExecuteExifOperationUsingAFile(Func<string, bool> syncExifOperation, string exifToolArguments, bool writeArgumentsToFile)
        {
            string tempFileName = Path.GetTempFileName();
            using (var fs = new StreamWriter(tempFileName))
            {
                fs.WriteLine(m_imageFileName);
                if (s_debugLoggingActive) s_logger.Debug($"exiftool arguments:\n{exifToolArguments}");
                if(writeArgumentsToFile) fs.WriteLine(exifToolArguments);
            }

            bool querySuccess = syncExifOperation(writeArgumentsToFile
                ? $"-charset filename=utf-8 -@ {tempFileName}"
                : $"{exifToolArguments} -charset filename=utf-8 -@ {tempFileName}");


            //i had a worry that the program will exit and we won't complete clearing all, but i didn't see that this is the case
            //TODO: consider replacing with centralizing and use producer-consumer
            Task.Run(() =>
            {
                if (File.Exists(tempFileName))
                    File.Delete(tempFileName);
            });

            return querySuccess;
        }

        private bool Query(string arguments)
        {
            var psi = new ProcessStartInfo
            {
                FileName = c_exifToolFileName,
                UseShellExecute = false,
                Arguments = arguments,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding =  Encoding.UTF8,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            //if (m_simulationMode)
            //{
            //    s_logger.Info($"simulate query {psi.FileName} {psi.Arguments}");
            //    return true;
            //}

            var p = new Process {StartInfo = psi};
            try
            {
                if(s_debugLoggingActive) s_logger.Debug($"before: {psi.FileName} {psi.Arguments}");

                p.Start();

                var output = p.StandardOutput.ReadToEnd();
                string err = p.StandardError.ReadToEnd();

                p.WaitForExit();

                if (p.ExitCode != 0)
                {
                    s_logger.Error("failed querying {0}. exit code: {1}. {2}", m_imageFileName, p.ExitCode, err);
                    return false;
                }

                m_exifOutput = output;
                if(s_debugLoggingActive) s_logger.Debug($"metadata of {m_imageFileName}:\n{m_exifOutput}");
            }
            catch (Exception e)
            {
                s_logger.Error(e, "Batch file execution failed.");
                return false;
            }

            return true;
        }

        private bool Update(string arguments)
        {
            arguments = "-overwrite_original_in_place " + arguments;
            var psi = new ProcessStartInfo
            {
                FileName = c_exifToolFileName,
                UseShellExecute = false,
                Arguments = arguments,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };
            if (m_simulationMode)
            {
                s_logger.Info($"simulate run {psi.FileName} {psi.Arguments}");
                return true;
            }

            var p = new Process { StartInfo = psi };
            try
            {
                if(s_debugLoggingActive) s_logger.Debug($"before: {psi.FileName} {psi.Arguments}");
                p.Start();

                var output = p.StandardOutput.ReadToEnd();
                string err = p.StandardError.ReadToEnd();

                p.WaitForExit();

                if (p.ExitCode != 0)
                {
                    s_logger.Error($"failed changing {m_imageFileName}. exit code: {p.ExitCode}. {err}");
                    return false;
                }

                m_exifOutput = output;
            }
            catch (Exception e)
            {
                s_logger.Error(e, "Batch file execution failed.");
                return false;
            }

            if(s_debugLoggingActive) s_logger.Debug($"successfully updated: {m_imageFileName}");
            return true;
        }

        public bool SetTitle(string titleValue)
        {
            return RunUpdateUsingAFile(GetSetStatementForTitle(titleValue));
        }

        private string GetSetStatementForTitle(string title)
        {
            return $"-EXIF:XPTitle={title}\n-Description={title}\n-Title={title}\n";
        }

        public bool SetDateAndTitle(string titleValue, DateTime dateTaken)
        {
            if (dateTaken != DateTime.MinValue)
            {
                string updateDateCreatedParameter = GetCreatedDateForUpdate(dateTaken);
                if (!string.IsNullOrEmpty(titleValue))
                {
                    return RunUpdateUsingAFile($"{updateDateCreatedParameter}\n{GetSetStatementForTitle(titleValue)}");
                }

                return RunUpdateUsingAFile($"{updateDateCreatedParameter}");
            }

            return RunUpdateUsingAFile(GetSetStatementForTitle(titleValue));
        }

        private string GetCreatedDateForUpdate(DateTime dateTaken)
        {
            string dateTakenString = $"{dateTaken.Year}:{dateTaken.Month}:{dateTaken.Day} 12:0:0";
            return $"-EXIF:CreateDate={dateTakenString}\n-DateTimeOriginal={dateTakenString}";
        }

        public bool SetDateTaken(DateTime dateTaken)
        {
            return RunUpdateUsingAFile(GetCreatedDateForUpdate(dateTaken));
        }
    }
}
