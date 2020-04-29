using System;
using System.IO;
using System.Reflection;
using NLog;

namespace ChangeImagesPropertiesApp
{
    internal class Program
    {
        enum ExecuteProgram
        {
            Yes,
            No,
            ShowHelp
        }

        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();
        private const string c_rootFolderParamPrefix = "folder";
        private const string c_simulationParamPrefix = "simulation";


        private static void Main(string[] args)
        {
            (ExecuteProgram execute, string rootFolder, bool simulation) executionArguments = ParseArgs(args);
            if (executionArguments.execute == ExecuteProgram.No)
            {
                return;
            }

            if (executionArguments.execute == ExecuteProgram.ShowHelp)
            {
                ShowHelp();
                return;
            }

            try
            {
                AppDomain.CurrentDomain.UnhandledException += UnhandledException;
                s_logger.Info("*********************************** start *************************************");
                s_logger.Info($"root folder={executionArguments.rootFolder}, simulation mode={executionArguments.simulation}");
                var p = new Program();
                p.OperateOnImagesInFolder(executionArguments.rootFolder, executionArguments.simulation);
            }
            finally
            {
                s_logger.Info("*********************************** exiting *************************************");
                LogManager.Flush();
                LogManager.Shutdown();
            }
        }

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            s_logger.Fatal(e.ExceptionObject);
        }

        private static void ShowHelp()
        {
            string applicationName = Assembly.GetExecutingAssembly().GetName().Name;
            Console.WriteLine("Description");
            Console.WriteLine($"\t{applicationName} assigns a 'date taken' value and title to image files found in a folder hierarchy,");
            Console.WriteLine("\tso they will show organized when uploading them to google photos.");
            Console.WriteLine("\t!!! your files will be modified in place with no backup !!!");
            Console.WriteLine("\nUsage");
            Console.WriteLine($"\t{applicationName} {c_rootFolderParamPrefix}=root_folder {c_simulationParamPrefix}=false");
            Console.WriteLine("\troot_folder - the root folder where files will be manipulated (recursively)");
            Console.WriteLine("\tsimulation - simulation mode. accepts a boolean value, defaulting to false, when true the files are not updated only read");
        }

        private static (ExecuteProgram execute, string rootFolder, bool simulation) ParseArgs(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                return (ExecuteProgram.ShowHelp, null, true);
            }

            if (args.Length == 1)
            {
                string param0 = args[0];
                if (param0.ToLower() == "help" || param0.Contains("?"))
                {
                    return (ExecuteProgram.ShowHelp, null, true);
                }

                return GetFolderParameter(param0);
            }

            (ExecuteProgram execute, string rootFolder, bool simulation) resultOfParseParam0 = GetFolderParameter(args[0]);
            if (resultOfParseParam0.execute != ExecuteProgram.Yes)
            {
                return resultOfParseParam0;
            }

            if (args[1].Length < c_simulationParamPrefix.Length + 2 || !args[1].StartsWith(c_simulationParamPrefix))
            {
                Console.WriteLine("failed parsing simulation parameter\n\n");
                return (ExecuteProgram.ShowHelp, null, true);
            }

            string param1 = args[1].Substring(c_simulationParamPrefix.Length + 1, args[1].Length - c_simulationParamPrefix.Length - 1);
            bool.TryParse(param1, out var simulation);

            return (ExecuteProgram.Yes, resultOfParseParam0.rootFolder, simulation);
        }

        private static (ExecuteProgram execute, string rootFolder, bool simulation) GetFolderParameter(string param0)
        {
            if (!param0.StartsWith($"{c_rootFolderParamPrefix}=") || param0.Length < c_rootFolderParamPrefix.Length + 2)
            {
                Console.WriteLine("failed recognizing the root folder parameter\n\n");
                return (ExecuteProgram.ShowHelp, null, true);
            }

            param0 = param0.Substring(c_rootFolderParamPrefix.Length + 1, param0.Length - c_rootFolderParamPrefix.Length - 1);
            if (param0 == ".")
            {
                {
                    return (ExecuteProgram.Yes, Environment.CurrentDirectory, false);
                }
            }

            if (param0[0] == '"'  && param0[param0.Length-1] == '"')
            {
                param0 = param0.Substring(1, param0.Length - 2);
            }

            if (!CheckFolderExists(param0))
            {
                Console.WriteLine($"folder {param0} doesn't exist");
                return (ExecuteProgram.No, null, true);
            }

            return (ExecuteProgram.Yes, param0, false);
        }

        private static bool CheckFolderExists(string folderName)
        {
            try
            {
                return Directory.Exists(folderName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return false;
        }

        private void OperateOnImagesInFolder(string rootFolder, bool simulationOnly)
        {
            var fhim = new FolderHierarchyImageFilesManipulator(simulationOnly);
            fhim.OperateOnImagesInFolder(rootFolder);
        }
    }
}