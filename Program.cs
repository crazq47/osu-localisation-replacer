using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
/// 🇬🇧 Shortening paths
/// 🇺🇦 Скорочення шляхів
using DefaultValues = Program.Parameters.DefaultValues;
using Localisation = Files.Localisation;

/// <summary>
/// 🇬🇧 Main class of the program. <br/>
/// 🇺🇦 Основний клас програми.
/// </summary>
class Program
{
    /// <summary>
    /// 🇬🇧 The executing method of the program. <br/>
    /// 🇺🇦 Виконуючий метод програми.
    /// </summary>
    static async Task Main(string[] args)
    {
        /// 🇬🇧 Handling errors at program startup
        /// 🇺🇦 Обробка помилок при запуску програми
        if (!Directory.Exists(Paths.CustomLocalisationFolder))
        {
            Directory.CreateDirectory(Paths.CustomLocalisationFolder);
            MessageBox.Show($"Custom Folder has been created in:\n '{Paths.CustomLocalisationFolder}'\n\nPlease, add replacement files...");
            return;
        }
        /// 
        if (!File.Exists(Paths.OsuApplicationFile))
        {
            MessageBox.Show("Executable file not found.");
            return;
        }

        /// 🇬🇧 Create and run the game process
        /// 🇺🇦 Створення та запуск процесу гри
        Process osuProcess = await Files.Osu.LaunchApplicationAsync(Paths.OsuApplicationFile);
        ///
        /// 🇬🇧 Replacing files from 'Localisation\custom' folder and checks every n seconds
        /// 🇺🇦 Заміна файлів з теки "Localisation\custom" та перевірки кожні n секунд
        Localisation.ReplaceWithUpdates(Paths.LocalisationFolder, Paths.CustomLocalisationFolder,
            Parameters.GetUpdateIntervalArg(args), Parameters.GetUpdatePeriodArg(args));
        ///
        /// 🇬🇧 Restart a game process
        /// 🇺🇦 Перезапуск процесу гри
        await Files.Osu.RestartAfterReplaceAsync(osuProcess, Paths.OsuApplicationFile);
        ///
        /// 🇬🇧 Debug logging
        /// 🇺🇦 Ведення журналу відлагодження
        Parameters.ProvideLogging(args);
        
        return;
    }

    /// <summary>
    /// 🇬🇧 Subclass for working with program parameters. <br/>
    /// 🇺🇦 Підклас для роботи з параметрами програми.
    /// </summary>
    internal class Parameters
    {
        /// <summary>
        /// 🇬🇧 Subclass for working with default program parameters. <br/>
        /// 🇺🇦 Підклас для роботи зі стандартними параметрами програми.
        /// </summary>
        internal class DefaultValues
        {
            /// <summary>
            /// 🇬🇧 Gets or sets interval between checks (in milliseconds). <br/>
            /// 🇺🇦 Отримує або задає інтервал між перевірками (у мілісекундах).
            /// </summary>
            /// 
            /// <returns>
            /// 🇬🇧 <see langword="int"/> number of the interval. The initial value is 100 ms. <br/>
            /// 🇺🇦 Число <see langword="int"/> інтервалу. Початкове значення — 100 мс.
            /// </returns>
            public static int UpdateInterval = 100;
            ///
            /// <summary>
            /// 🇬🇧 Gets or sets a time period during which the checks should be performed (in seconds). <br/>
            /// 🇺🇦 Отримує або задає проміжок часу, протягом якого повинні виконуватися перевірки (у секундах).
            /// </summary>
            /// 
            /// <returns>
            /// 🇬🇧 <see langword="int"/> number of the period. The initial value is 2 sec. <br/>
            /// 🇺🇦 Число <see langword="int"/> проміжку часу. Початкове значення — 2 сек.
            /// </returns>
            public static int UpdatePeriod = 2;
            /// 
            /// <summary>
            /// 🇬🇧 Indicates whether logging is disabled. <br/>
            /// 🇺🇦 Вказує, чи відключено ведення логів.
            /// </summary>
            /// 
            /// <returns>
            /// 🇬🇧 The value <see langword="true"/> indicates that logging is disabled. The initial value is <see langword="false"/>. <br/>
            /// 🇺🇦 Значення <see langword="true"/> вказує, що ведення логування вимкнено. Початкове значення — <see langword="false"/>.
            /// </returns>
            public static bool DisableLogging = false;
        }

        /// <summary>
        /// 🇬🇧 Gets the interval in miliseconds between checks from the command line arguments using <see cref="GetIntArg"/>. <br/>
        /// 🇺🇦 Отримує інтервал у мілісекундах між перевірками з аргументів командного рядка, використовуючи <see cref="GetIntArg"/>.
        /// </summary>
        /// 
        /// <param name="args">
        /// 🇬🇧 An array of command line arguments. <br/>
        /// 🇺🇦 Масив аргументів командного рядка. </param>
        /// 
        /// <returns>
        /// 🇬🇧 The interval between checks in the form <see cref="TimeSpan"/>. <br/>
        /// The value <see langword="-1"/> returns <see cref="DefaultValues.UpdateInterval"/>. <br/><br/>
        /// 
        /// 🇺🇦 Інтервал між перевірками у вигляді <see cref="TimeSpan"/>. <br/>
        /// Значення <see langword="-1"/> повертає <see cref="DefaultValues.UpdateInterval"/>.
        /// </returns>
        public static TimeSpan GetUpdateIntervalArg(string[] args)
        {
            string searchedArg = "updateInterval=";
            return TimeSpan.FromMilliseconds(GetIntArg(args, searchedArg) != -1 ?
                GetIntArg(args, searchedArg) : DefaultValues.UpdateInterval);
        }

        /// <summary>
        /// 🇬🇧 Gets the time period in seconds for completing checks from command line arguments using <see cref="GetIntArg"/>. <br/>
        /// 🇺🇦 Отримує проміжок часу у секундах завершення перевірок з аргументів командного рядка, використовуючи <see cref="GetIntArg"/>.
        /// </summary>
        /// 
        /// <param name="args">
        /// 🇬🇧 An array of command line arguments. <br/>
        /// 🇺🇦 Масив аргументів командного рядка. </param>
        /// 
        /// <returns>
        /// 🇬🇧 The time period for completing the checks in the form <see cref="TimeSpan"/>. <br/>
        /// The value <see langword="-1"/> returns <see cref="DefaultValues.UpdatePeriod"/>. <br/><br/>
        /// 
        /// 🇺🇦 Проміжок часу завершення перевірок у вигляді <see cref="TimeSpan"/>. <br/>
        /// Значення <see langword="-1"/> повертає <see cref="DefaultValues.UpdatePeriod"/>.
        /// </returns>
        public static TimeSpan GetUpdatePeriodArg(string[] args)
        {
            string searchedArg = "updatesPeriod=";
            return TimeSpan.FromSeconds(GetIntArg(args, searchedArg) != -1 ?
                GetIntArg(args, searchedArg) : DefaultValues.UpdatePeriod);
        }

        /// <summary>
        /// 🇬🇧 Gets the value of the command line argument to enable or disable debugging logging. <br/>
        /// Defaults to <see langword="false"/> if the value is not specified in the command line arguments. <br/><br/>
        /// 
        /// 🇺🇦 Отримує значення аргументу командного рядка для включення або вимкнення ведення журналу відлагодження. <br/>
        /// За замовчуванням встановлюється в <see langword="false"/>, якщо значення не вказане в аргументах командного рядка.
        /// </summary>
        /// 
        /// <param name="args">
        /// 🇬🇧 An array of command line arguments. <br/>
        /// 🇺🇦 Масив аргументів командного рядка. </param>
        /// 
        /// <returns>
        /// 🇬🇧 The value of the <see cref="bool"/> argument of the "enableLogging" command line argument. The default is <see langword="false"/>. <br/>
        /// 🇺🇦 <see cref="bool"/> значення  аргумента "enableLogging" командного рядка. За замовчуванням — <see langword="false"/>.
        /// </returns>
        public static bool GetDisableLoggingArg(string[] args)
        {
            string searchedArg = "disableLogging=";
            DefaultValues.DisableLogging = GetBoolArg(args, searchedArg);

            return DefaultValues.DisableLogging;
        }

        /// <summary>
        /// 🇬🇧 Gets an integer argument from the command line by the specified key.
        /// Searches for the required argument by the specified key in the array of command line arguments.<br/>
        /// If the argument is found and its <see langword="value"/> is an integer, returns this <see langword="value"/>.
        /// Otherwise, it returns <see langword="-1"/>. <br/><br/>
        /// 
        /// 🇺🇦 Отримує цілочисловий аргумент з командного рядка за заданим ключем.
        /// Пошукує необхідний аргумент за вказаним ключем в масиві аргументів командного рядка.<br/>
        /// Якщо аргумент знайдений і його значення <see langword="value"/> є цілочисельним, повертає це значення <see langword="value"/>.
        /// В іншому випадку повертає <see langword="-1"/>.
        /// </summary>
        /// 
        /// <param name="args">
        /// 🇬🇧 An array of command line arguments. <br/>
        /// 🇺🇦 Масив аргументів командного рядка. </param>
        /// 
        /// <param name="searchedArg">
        /// 🇬🇧 The key by which to search for an integer argument. <br/>
        /// 🇺🇦 Ключ, за яким слід шукати цілочисловий аргумент.</param>
        /// 
        /// <returns>
        /// 🇬🇧 The integer <see langword="value"/> of the argument from the command line or <see langword="-1"/> if the argument is missing or not recognized. <br/>
        /// 🇺🇦 Цілочислове значення <see langword="value"/> аргумента з командного рядка або <see langword="-1"/>, якщо аргумент відсутній або не розпізнано.
        /// </returns>
        public static int GetIntArg(string[] args, string searchedArg)
        {
            /// Search for the required argument
            /// Пошук необхідного аргументу
            string valuedArg = args.FirstOrDefault(arg => arg.StartsWith(searchedArg));
            if (valuedArg != null && int.TryParse(valuedArg.Substring(searchedArg.Length), out int value))
            {
                return value;
            }

            return -1;
        }

        /// <summary>
        /// 🇬🇧 Gets a Boolean argument from the command line by the specified key. <br/>
        /// The value from the command line arguments is used. If no value is specified,
        /// the default is used <see langword="false"/>. <br/><br/>
        /// 
        /// 🇺🇦 Отримує булевий аргумент з командного рядка за заданим ключем. <br/>
        /// Використовується значення з аргументів командного рядка. Якщо значення не вказане, 
        /// за замовчуванням використовується — <see langword="false"/>.
        /// </summary>
        /// 
        /// <param name="args">
        /// 🇬🇧 An array of command line arguments. <br/>
        /// 🇺🇦 Масив аргументів командного рядка.</param>
        /// 
        /// <param name="searchedArg">
        /// 🇬🇧 The key to finding a Boolean argument. <br/>
        /// 🇺🇦 Ключ, за яким слід шукати булевий аргумент.</param>
        /// 
        /// <returns>
        /// 🇬🇧 <see cref="bool"/> value of the argument from the command line or <see langword="false"/> if the argument is missing or not recognized. <br/>
        /// 🇺🇦 Булеве значення аргумента з командного рядка або <see langword="false"/>, якщо аргумент <see cref="bool"/> відсутній або не розпізнано.
        /// </returns>
        public static bool GetBoolArg(string[] args, string searchedArg)
        {
            /// 🇬🇧 Using value from command line arguments
            /// If no value is specified, it defaults to false
            /// 
            /// 🇺🇦 Використання значення з аргументів командного рядку
            /// Якщо значення не вказане, за замовчуванням використовується false
            string valuedArg = args.FirstOrDefault(arg => arg.StartsWith(searchedArg));
            if (valuedArg != null && bool.TryParse(valuedArg.Substring(searchedArg.Length), out bool result))
            {
                return result;
            }

            return false;
        }

        /// <summary>
        /// 🇬🇧 Saves logs to a file, taking into account the presence of command line arguments.
        /// Checks for the presence of the 'disableLogging' argument, which requires disabling logging.
        /// Якщо відсутній або встановлено значення <see langword="false"/>, то виконує запис різноманітної інформації у файл логів. <br/><br/>
        /// 
        /// 🇺🇦 Забезпечує запис логів у файл, враховуючи наявність аргументів командного рядка.
        /// Перевіряє наявність аргумента "disableLogging", який вимагає вимкнення логування.
        /// Якщо відсутній або встановлено значення <see langword="false"/>, то виконує запис різноманітної інформації у файл логів.
        /// </summary>
        /// 
        /// <param name="args">
        /// 🇬🇧 An array of command line arguments. <br/>
        /// 🇺🇦 Масив аргументів командного рядка.</param>
        public static void ProvideLogging(string[] args)
        {
            /// 🇬🇧 Path to the log file
            /// 🇺🇦 Шлях до файлу логів
            string logFilePath = Paths.ApplicationLogFile;

            if (File.Exists(logFilePath))
            {
                Debbuging.ClearLogFile(logFilePath);
            }

            if (!GetDisableLoggingArg(args))
            {
                /// 🇬🇧 Record the product version and its path
                /// 🇺🇦 Запис версії продукту та його шлях
                Debbuging.WriteLogFile(logFilePath, $"{Debbuging.SetLogData()} Registered new Startup!");
                Debbuging.WriteLogFile(logFilePath, $"{Debbuging.SetLogData()} Location: {Assembly.GetEntryAssembly()?.Location}");
                Debbuging.WriteLogFile(logFilePath, $"{Debbuging.SetLogData()} Version: {Assembly.GetEntryAssembly()?.GetName().Version}");
                Debbuging.WriteLogFile(logFilePath, $"{Debbuging.SetLogData()}");
                Debbuging.WriteLogFile(logFilePath, $"{Debbuging.SetLogData()} Data processing...");
                Debbuging.WriteLogFile(logFilePath, $"{Debbuging.SetLogData()}");
                /// 🇬🇧 Writing the GetUpdateInterval and GetUpdatePeriod values ​​to a file
                /// 🇺🇦 Запис значень GetUpdateInterval та GetUpdatePeriod у файл
                Debbuging.WriteLogFile(logFilePath, $"{Debbuging.SetLogData()} Current Update Interval: \t{GetUpdateIntervalArg(args).TotalMilliseconds} ms.");
                Debbuging.WriteLogFile(logFilePath, $"{Debbuging.SetLogData()} Current Updates Period: \t{GetUpdatePeriodArg(args).TotalSeconds} sec.");
                Debbuging.WriteLogFile(logFilePath, $"{Debbuging.SetLogData()}");
                /// 🇬🇧 Write details about command line arguments to a file
                /// 🇺🇦 Запис деталей про аргументи командного рядка у файл
                if (args.Any())
                {
                    Debbuging.WriteLogFile(logFilePath, $"{Debbuging.SetLogData()} Object Arguments: \t\t{string.Join(", ", args)}");
                }
                else
                {
                    Debbuging.WriteLogFile(logFilePath, $"{Debbuging.SetLogData()} Contains no object arguments.");
                }
                Debbuging.WriteLogFile(logFilePath, $"{Debbuging.SetLogData()}");
                /// 🇬🇧 Write details about the replaced localization files to a file
                /// 🇺🇦 Запис деталей про замінені файли локалізації у файл
                if (Localisation.ReplacedFilesList.Any())
                {
                    Debbuging.WriteLogFile(logFilePath, $"{string.Join(Environment.NewLine, Localisation.ReplacedFilesList.Distinct().Select((file, index) => $"{Debbuging.SetLogData()} Replaced File: \t\t{file}, {Localisation.FileLinesCountList.ElementAtOrDefault(index)} line(s), {Localisation.FileSizesList.ElementAtOrDefault(index) / 1024} kb."))}");
                    Debbuging.WriteLogFile(logFilePath, $"{Debbuging.SetLogData()} Replaced Files Total: \t{Localisation.ReplacedFilesList.Distinct().Count()} file(s)");
                    if (Localisation.ReplacedFilesList.Count > 1)
                    {
                        Debbuging.WriteLogFile(logFilePath, $"{Debbuging.SetLogData()} Iterations Per File: \t{Localisation.ActualIterations / Localisation.ReplacedFilesList.Count}");
                    }
                    Debbuging.WriteLogFile(logFilePath, $"{Debbuging.SetLogData()} Iterations Total: \t\t{Localisation.ActualIterations}");
                    Debbuging.WriteLogFile(logFilePath, $"{Debbuging.SetLogData()} Iterations Expect: \t{Localisation.ExpectedIterations}");
                }
                else
                {
                    Debbuging.WriteLogFile(logFilePath, $"{Debbuging.SetLogData()} No need to replace files.");
                }
                Debbuging.WriteLogFile(logFilePath, $"{Debbuging.SetLogData()}");
                /// 🇬🇧 Recording details about the operation of the application
                /// 🇺🇦 Запис деталей про роботу застосунку
                Debbuging.WriteLogFile(logFilePath, $"{Debbuging.SetLogData()} Osu! Application Working...");
                Debbuging.WriteLogFile(logFilePath, $"{Debbuging.SetLogData()} Restarting: \t\t{Files.Osu.Status.Restarting}");
                Debbuging.WriteLogFile(logFilePath, $"{Debbuging.SetLogData()} Launching: \t\t{Files.Osu.Status.Launching}");
                Debbuging.WriteLogFile(logFilePath, $"{Debbuging.SetLogData()}");
                Debbuging.WriteLogFile(logFilePath, $"{Debbuging.SetLogData()} End data processing...");
                ///
                /// 🇬🇧 Write details about the name of the application process to a file
                /// 🇺🇦 Запис деталей про назву процесу застосунку у файл
                ///WriteLog(logFilePath, $"Application Process Name: {Files.Osu.GetProcessName(Paths.OsuApplicationFile)}");
            }
        }
    }

    /// <summary>
    /// 🇬🇧 Debbuging subclass of the program. <br/>
    /// 🇺🇦 Підклас відлагодження програми.
    /// </summary>
    internal class Debbuging
    {
        /// <summary>
        /// 🇬🇧 Generates a <see cref="string"/> with the current date and time for use in the log file. <br/>
        /// 🇺🇦 Генерує рядок <see cref="string"/> з поточними датою та часом для використання у файлі логів.
        /// </summary>
        /// 
        /// <returns>
        /// 🇬🇧 A string with the local date and time in the format 'yyyy-MM-ddTHH:mm:ss'. <br/>
        /// 🇺🇦 Рядок з місцевими датою та часом у форматі "yyyy-MM-ddTHH:mm:ss".
        /// </returns>
        public static string SetLogData()
        {
             return $"{DateTime.Now:yyyy-MM-ddTHH:mm:ss}:";
        }

        /// <summary>
        /// 🇬🇧 Writes the message to the log file at the specified path. <br/>
        /// 🇺🇦 Записує повідомлення у файл логування за вказаним шляхом.
        /// </summary>
        /// 
        /// <param name="filePath">
        /// 🇬🇧 Path to the logging file. <br/>
        /// 🇺🇦 Шлях до файлу логування.</param>
        /// 
        /// <param name="message">
        /// 🇬🇧 Messages to be written to a file. <br/>
        /// 🇺🇦 Повідомлення для запису в файл.</param>
        public static void WriteLogFile(string filePath, string message)
        {
            try
            {
                using (StreamWriter sw = File.AppendText(filePath))
                {
                    sw.WriteLine(message);
                }
            }
            catch (Exception ex)
            {
                /// 🇬🇧 Handling errors when writing to a file
                /// 🇺🇦 Обробка помилок при записі в файл
                MessageBox.Show($"Error writing log file: {ex.Message}");
            }
        }

        /// <summary>
        /// 🇬🇧 Clears the contents of the log file at the specified <see href="path"/>. <br/>
        /// 🇺🇦 Очищує вміст файлу логування за вказаним <see href="шляхом"/>.
        /// </summary>
        /// 
        /// <param name="filePath">
        /// 🇬🇧 Path to the logging file. <br/>
        /// 🇺🇦 Шлях до файлу логування.</param>
        public static void ClearLogFile(string filePath)
        {
            try
            {
                if (!DefaultValues.DisableLogging)
                {
                    File.WriteAllText(filePath, string.Empty);
                }
            }
            catch (Exception ex)
            {
                /// 🇬🇧 Handling errors when writing to a file
                /// 🇺🇦 Обробка помилок при записі в файл
                MessageBox.Show($"Error clearing log file: {ex.Message}");
            }
        }

        /// <summary>
        /// 🇬🇧 Generates a logging record for the replaced files along with additional information. <br/>
        /// 🇺🇦 Формує запис логування для замінених файлів разом із додатковою інформацією.
        /// </summary>
        /// 
        /// <returns>
        /// 🇬🇧 Logging for replaced files or a message if no files were replaced. <br/>
        /// 🇺🇦 Запис логування для замінених файлів або повідомлення, якщо файлів не було замінено.</returns>
        public static string GetReplacedFilesLogWithData()
        {
            if (!Localisation.ReplacedFilesList.Any())
            {
                return $"{SetLogData()} Files not replaced.";
            }

            var logEntries = Localisation.ReplacedFilesList
                .Distinct()
                .Select((file, index) =>
                    $"{SetLogData()} Replaced File: \t\t{file}, " +
                    $"{Directories.FileLinesCountList.ElementAtOrDefault(index)} line(s), " +
                    $"{Directories.FileSizesList.ElementAtOrDefault(index) / 1024} kb.");

            return string.Join(Environment.NewLine, logEntries);
        }

        /// <summary>
        /// 🇬🇧 Gets the number of iterations per file in log format. <br/>
        /// 🇺🇦 Отримує кількість ітерацій на файл у лог-форматі.
        /// </summary>
        /// 
        /// <returns>
        /// 🇬🇧 Logging record with the number of iterations per file or <see langword="null"/> if no files were replaced. <br/>
        /// 🇺🇦 Запис логування з кількістю ітерацій на файл або <see langword="null"/>, якщо файлів не було замінено.
        /// </returns>
        public static string GetIterationsPerFile()
        {
            return Localisation.ReplacedFilesList.Any() ? $"{Debbuging.SetLogData()} Iterations Per File: \t{Localisation.ReplacedFilesList.Count / Localisation.ReplacedFilesList.Distinct().Count()}" : null;
        }
    }
}

/// <summary>
/// 🇬🇧 Class for working with third-party files. <br/>
/// 🇺🇦 Клас для роботи зі сторонніми файлами.
/// </summary>
class Files
{
    /// <summary>
    /// 🇬🇧 A subclass for working with game. <br/>
    /// 🇺🇦 Підклас для роботи з самою грою.
    /// </summary>
    public class Osu
    {
        /// <summary>
        /// 🇬🇧 A subclass for informing about the status of the game. <br/>
        /// 🇺🇦 Підклас для інформування про статус гри.
        /// </summary>
        public class Status
        {
            /// <summary>
            /// 🇬🇧 Gets or sets a value that allows you to determine the status of the game when it is launched. <br/>
            /// 🇺🇦 Отримує або задає значення, що дозволяє визначити статус роботи гри під час її запуску.
            /// </summary>
            /// 
            /// <returns>
            /// 🇬🇧 The value <see langword="false"/> is the initial value. <br/>
            /// 🇺🇦 Значення <see langword="false"/> — початкове значення. <br/>
            /// </returns>
            public static string Launching = "false";
            /// 
            /// <summary>
            /// 🇬🇧 Gets or sets a <see cref="string"/> value that allows you to determine the status of the game when it is restarted. <br/>
            /// 🇺🇦 Отримує або задає <see cref="string"/> значення, що дозволяє визначити статус роботи гри під час її перезапуску.
            /// </summary>
            /// 
            /// <returns>
            /// 🇬🇧 The value <see langword="false"/> is the initial value. <br/>
            /// 🇺🇦 Значення <see langword="false"/> — початкове значення. <br/>
            /// </returns>
            public static string Restarting = "false";
        }
        
        public static string ApplicationFile = "osu!.exe";
        public static string ProccesName = "osu!";

        /// <summary>
        /// 🇬🇧 Declares and starts the process at the specified path. <br/>
        /// 🇺🇦 Оголошує та запускає процес за вказаним шляхом.
        /// </summary>
        /// 
        /// <param name="appPath">
        /// 🇬🇧 Path to the game executable file. <br/>
        /// 🇺🇦 Шлях до виконуючого файлу гри.</param>
        /// 
        /// <param name="args">
        /// 🇬🇧 Arguments to start the game. Optional value, the default is <see cref="string.Empty"/>. <br/>
        /// 🇺🇦 Аргументи для запуску гри. Необов'язкове значення, за замовчуванням — <see cref="string.Empty"/>.</param>
        /// 
        /// <returns>
        /// 🇬🇧 The <see cref="Task"/>&lt;<see cref="Process"/>&gt; is declared and started. <br/>
        /// 🇬🇧 Game launch status for <see cref="Status.Launching"/>. <br/><br/>
        /// 
        /// 🇺🇦 Оголошений та запущений процес <see cref="Task"/>&lt;<see cref="Process"/>&gt;. <br/>
        /// 🇺🇦 Статус запуску гри для <see cref="Status.Launching"/>.
        /// </returns>
        public static Process LaunchApplication(string appPath, string args = "")
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = appPath,
                    Arguments = args,
                    UseShellExecute = true,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    CreateNoWindow = true
                };

                Process process = Process.Start(startInfo);

                if (process != null)
                {
                    Status.Launching = $"New process started. ID: {process.Id}";
                    return process;
                }

                Status.Launching = "Failed to start the process.";
                return null;
            }
            catch (Exception ex)
            {
                Status.Launching = $"Error starting process: {ex.Message}";
                return null;
            }
        }

        /// <summary>
        /// 🇬🇧 <see langword="async"/> method <see cref="LaunchApplication"/> without arguments. <br/>
        /// 🇺🇦 Асинхронний метод <see cref="LaunchApplication"/> без аргументів.
        /// </summary>
        /// 
        /// <param name="appPath">
        /// 🇬🇧 Path to the game executable file. <br/>
        /// 🇺🇦 Шлях до виконуючого файлу гри.</param>
        /// 
        /// <returns>
        /// 🇬🇧 Declared and started <see langword="async"/> <see cref="Task"/>&lt;<see cref="Process"/>&gt;. <br/>
        /// 🇺🇦 Оголошений та запущений асинхронний процес <see langword="async"/> <see cref="Task"/>&lt;<see cref="Process"/>&gt;.
        /// </returns>
        public static async Task<Process> LaunchApplicationAsync(string appPath)
        {
            return await Task.Run(() => {
                return LaunchApplication(appPath);
            });
        }

        /// <summary>
        /// 🇬🇧 <see langword="async"/> <see cref="Task"/> method for closing the <see cref="Process"/>. <br/>
        /// 🇺🇦 Асинхронний <see langword="async"/> <see cref="Task"/> метод для закриття процесу <see cref="Process"/>.
        /// </summary>
        /// 
        /// <param name="process">
        /// 🇬🇧 Declared <see cref="Process"/>, which will be closed. <br/>
        /// 🇺🇦 Оголошений процес <see cref="Process"/>, який буде закритий.</param>
        /// 
        /// <returns>
        /// 🇬🇧 Abort the declared <see cref="Process"/>. <br/>
        /// 🇺🇦 Заркиття оголошеного процесу <see cref="Process"/>.
        /// </returns>
        public static async Task CloseApplicationAsync(Process process)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (process != null && !process.HasExited)
                    {
                        /// 🇬🇧 Closing the main window
                        /// 🇺🇦 Закриття головного вікна
                        process.CloseMainWindow();
                        Status.Restarting = $"Process {process.Id} closed.";
                        ///
                        int period = (DefaultValues.UpdatePeriod * 100) - DefaultValues.UpdateInterval;
                        /// 🇬🇧 Waiting for n seconds to close
                        /// 🇺🇦 Очікування n секунд на закриття
                        if (!process.WaitForExit(period)) 
                        {
                            /// 🇬🇧 If it was not possible to close the main window — Kill
                            /// 🇺🇦 Якщо не вдалося закрити головне вікно, використовуємо Kill
                            process.Kill();
                            /// 🇬🇧 Waiting for the process to close completely
                            /// 🇺🇦 Очікування, поки процес повністю закриється
                            process.WaitForExit();
                            ///
                            Status.Restarting = $"Process {process.Id} killed.";
                        }
                    }
                    else
                    {
                        Status.Restarting = "No active process to close.";
                    }
                }
                catch (Exception ex)
                {
                    Status.Restarting = $"Error closing process: {ex.Message}";
                }
            });
        }

        /// <summary>
        /// 🇬🇧 <see langword="async"/> <see cref="Task"/> method for restaring the <see cref="Process"/>. <br/>
        /// 🇺🇦 Асинхронний <see langword="async"/> <see cref="Task"/> метод для перезапуску процесу <see cref="Process"/>.
        /// </summary>
        /// 
        /// <param name="process">
        /// 🇬🇧 Declared <see cref="Process"/>, which will be restarted. <br/>
        /// 🇺🇦 Оголошений процес <see cref="Process"/>, який буде перезапущений.</param>
        /// 
        /// <param name="appPath">
        /// 🇬🇧 Path to the game executable file. <br/>
        /// 🇺🇦 Шлях до виконуючого файлу гри.</param>
        /// 
        /// <returns>
        /// 🇬🇧 Restart the declared <see cref="Process"/>. <br/>
        /// 🇺🇦 Перезапуск оголошеного процесу <see cref="Process"/>.
        /// </returns>
        public static async Task RestartApplicationAsync(Process process, string appPath)
        {
            await CloseApplicationAsync(process);
            ///
            /// 🇬🇧 Wait before closing
            /// 🇺🇦 Очікування перед закриттям
            await Task.Delay(DefaultValues.UpdatePeriod);
            ///
            await LaunchApplicationAsync(appPath);
        }

        /// <summary>
        /// 🇬🇧 <see langword="async"/> <see cref="Task"/> method that adds a check for overwritten localsation files for <see cref="RestartApplicationAsync"/>. <br/>
        /// 🇺🇦 Асинхронний <see langword="async"/> <see cref="Task"/> метод який додає перевірку на замінені файли локалізації для <see cref="RestartApplicationAsync"/>.
        /// </summary>
        /// 
        /// <param name="process">
        /// 🇬🇧 Declared <see cref="Process"/>, which will be restarted. <br/>
        /// 🇺🇦 Оголошений процес <see cref="Process"/>, який буде перезапущений.</param>
        /// 
        /// <param name="appPath">
        /// 🇬🇧 Path to the game executable file. <br/>
        /// 🇺🇦 Шлях до виконуючого файлу гри.</param>
        /// 
        /// <returns>
        /// 🇬🇧 <see langword="async"/> process <see cref="RestartApplicationAsync"/>. <br/>
        /// 🇺🇦 Асинхронний <see langword="async"/> процес <see cref="RestartApplicationAsync"/>.
        /// </returns>
        public static async Task RestartAfterReplaceAsync(Process process, string appPath)
        {
            /// Checks if the list of replaced files contains any element.
            /// Перевірка, чи містить список замінених файлів будь-який елемент.
            if (Localisation.ReplacedFilesList.Any())
            {
                await RestartApplicationAsync(process, appPath);
            }
        }

        /// <summary>
        /// 🇬🇧 Starts the process at the specified path. <br/>
        /// 🇺🇦 Запускає процес за вказаним шляхом.
        /// </summary>
        /// 
        /// <param name="appPath">
        /// 🇬🇧 Path to the game executable file. <br/>
        /// 🇺🇦 Шлях до виконуючого файлу гри.</param>
        /// 
        /// <returns>
        /// 🇬🇧 The name of the running <see cref="Process"/>. <br/>
        /// 🇺🇦 Назва запущеного процесу <see cref="Process"/>.
        /// </returns>
        public static string GetProcessName(string appPath) 
        {
            /// 🇬🇧 Run the application process
            /// 🇺🇦 Запуск прикладового процесу
            ProcessStartInfo startInfo = new ProcessStartInfo(appPath);
            Process process = Process.Start(startInfo);
            ///
            /// 🇬🇧 Get and output the name of the process
            /// 🇺🇦 Отримання та вивід ім'я процесу
            string processName = process.ProcessName;

            return processName;
        }
    }

    /// <summary>
    /// 🇬🇧 Subclass for working with localisation files. <br/>
    /// 🇺🇦 Підклас для роботи з файлами локалізації.
    /// </summary>
    public class Localisation
    {
        /// <summary>
        /// 🇬🇧 A <see cref="string"/> list containing the path of all replaced files. <br/>
        /// 🇺🇦 Список <see cref="string"/>, у якому зберігаються шлях усіх замінених файлів.
        /// </summary>
        /// 
        /// <returns>
        /// 🇬🇧 Gets or sets the element paths in the <see cref="List{T}"/> interface of replaced files. <br/>
        /// 🇺🇦 Отримує або задає шляхи елементів у інтерфейсі <see cref="List{T}"/> замінених файлів.
        /// </returns>
        public static List<string> ReplacedFilesList = new List<string>();
        /// 
        /// <summary>
        /// 🇬🇧 A <see cref="long"/> list that stores the sizes of all replaced files. <br/>
        /// 🇺🇦 Список <see cref="long"/>, у якому зберігаються розміри усіх замінених файлів.
        /// </summary>
        /// 
        /// <returns>
        /// 🇬🇧 Gets or sets the element sizes in the <see cref="List{T}"/> interface of replaced files. <br/>
        /// 🇺🇦 Отримує або задає розміри елементів у інтерфейсі <see cref="List{T}"/> замінених файлів.
        /// </returns>
        public static List<long> FileSizesList = new List<long>();
        /// 
        /// <summary>
        /// 🇬🇧 A <see cref="long"/> list that stores the line count of all replaced files. <br/>
        /// 🇺🇦 Список <see cref="long"/>, у якому зберігаються кількість рядків усіх замінених файлів.
        /// </summary>
        /// 
        /// <returns>
        /// 🇬🇧 Gets or sets the element lines count in the <see cref="List{T}"/> interface of replaced files. <br/>
        /// 🇺🇦 Отримує або задає кількість рядків елементів у інтерфейсі <see cref="List{T}"/> замінених файлів.
        /// </returns>
        public static List<long> FileLinesCountList = new List<long>();
        ///
        /// <summary>
        /// 🇬🇧 Expected number of iterations. <br/>
        /// 🇺🇦 Очікувана кількість ітерацій.
        /// </summary>
        /// 
        /// <returns>
        /// 🇬🇧 Number of expected iterations to replace files. <br/>
        /// 🇺🇦 Кількість очікуваних ітерацій заміни файлів.
        /// </returns>
        public static long ExpectedIterations = 0;
        /// 
        /// <summary>
        /// 🇬🇧 Actual number of iterations. <br/>
        /// 🇺🇦 Дійсна кількість ітерацій.
        /// </summary>
        /// 
        /// <returns>
        /// 🇬🇧 Number of actual iterations to replace files. <br/>
        /// 🇺🇦 Кількість дійсних ітерацій заміни файлів.
        /// </returns>
        public static long ActualIterations = 0;
        
        /// <summary>
        /// 🇬🇧 Replaces localization files in a folder with the same files from another folder. 
        /// Note that files will only be replaced if the file being replaced is larger than the original file. <br/><br/> 
        /// 
        /// 🇺🇦 Замінює файли локалізації у теці на ті ж самі файли з іншої теки. 
        /// Зверніть увагу, що файли будуть замінені лише у тому випадку, якщо замінюваний файл більший за файл-оригінал.
        /// </summary>
        /// 
        /// <param name="localisationFolder">
        /// 🇬🇧 A folder that accepts replaceable files. <br/>
        /// 🇺🇦 Тека яка приймає файли для заміни.</param>
        /// 
        /// <param name="customLocalisationFolder">
        /// 🇬🇧 A folder containing files to replace. <br/>
        /// 🇺🇦 Тека яка містить файли для заміни.</param>
        /// 
        /// <returns>
        /// 🇬🇧 Location, size and line count of the replaced files in folders. <br/>
        /// Uses: <see cref="ReplacedFilesList"/>, <see cref="FileSizesList"/>, <see cref="FileLinesCountList"/>. <br/><br/> 
        /// 
        /// 🇺🇦 Розташування, розмір та кільксть рядків замінених файлів у теці. <br/>
        /// Використовує: <see cref="ReplacedFilesList"/>, <see cref="FileSizesList"/>, <see cref="FileLinesCountList"/>.
        /// </returns>
        public static void Replace(string localisationFolder, string customLocalisationFolder)
        {
            string[] customLocalisations = Directory.GetFiles(customLocalisationFolder);

            foreach (string customLocalisation in customLocalisations)
            {
                /// 🇬🇧 Get localisation file paths
                /// 🇺🇦 Отримання шляхів файлів локалізації
                string customFileName = Path.GetFileName(customLocalisation);
                string originalLocalisation = Path.Combine(localisationFolder, customFileName);
                ///
                /// 🇬🇧 Check for a file with the same name in the second folder
                /// Checking the size of the first file compared to the second
                /// 
                /// 🇺🇦 Перевірка наявності файлу з однаковим іменем у другій теці
                /// Перевірка розміру першого файлу в порівнянні з другим
                long originalFileSize = GetFileSize(originalLocalisation);
                long customFileSize = GetFileSize(customLocalisation);
                ///
                /// 🇬🇧 Check if the file in the 'Localization' folder is larger
                /// 🇺🇦 Перевірка, чи має файл у теці "Localization" більший розмір
                if (originalFileSize < customFileSize)
                {
                    /// 🇬🇧 Replace file in 'Localization' with file from 'Localization\\custom'
                    /// 🇺🇦 Заміна файлу у "Localization" файлом із "Localization\\custom"
                    File.Copy(customLocalisation, originalLocalisation, true);
                    /// 🇬🇧 Add the file to the list
                    /// 🇺🇦 Додавання файлу до списку
                    ReplacedFilesList.Add(originalLocalisation);
                    FileSizesList.Add(customFileSize);
                    FileLinesCountList.Add(GetFileLinesCount(customLocalisation));
                }
            }
        }

        /// <summary>
        /// 🇬🇧 Sets the updating range and calls <see cref="Replace"/>. <br/>
        /// 🇺🇦 Встановлює діапазон перевірок та викликає <see cref="Replace"/>.
        /// </summary>
        /// 
        /// <param name="localisationFolder">
        /// 🇬🇧 A folder that accepts replaceable files. <br/>
        /// 🇺🇦 Тека яка приймає файли для заміни.</param>
        /// 
        /// <param name="customLocalisationFolder">
        /// 🇬🇧 A folder containing files to replace. <br/>
        /// 🇺🇦 Тека яка містить файли для заміни.</param>
        /// 
        /// <param name="dueTime">
        /// 🇬🇧 Time interval between iterations (in milliseconds). <br/>
        /// 🇺🇦 Проміжок часу між ітераціями (у мілісекундах).</param>
        /// 
        /// <param name="period">
        /// 🇬🇧 Time period after which the iteration will complete (in seconds). <br/>
        /// 🇺🇦 Період часу, після якого ітерації буде завершено (у секундах).</param>
        /// 
        /// <returns>
        /// 🇬🇧 The value of the expected and actual number of iterations. <br/>
        /// Uses: <see cref="ExpectedIterations"/>, <see cref="ActualIterations"/>. <br/><br/> 
        /// 
        /// 🇺🇦 Значення очікуваної та дійсної кількості ітерацій. <br/>
        /// Використовує: <see cref="ExpectedIterations"/>, <see cref="ActualIterations"/>.
        /// </returns>
        public static void ReplaceWithUpdates(string localisationFolder, string customLocalisationFolder, TimeSpan dueTime, TimeSpan period)
        {
            ExpectedIterations = (int)(period.TotalMilliseconds / dueTime.TotalMilliseconds);
            /// 🇬🇧 Start the timer to call the method every n miliseconds
            /// 🇺🇦 Запуск таймера для виклику методу кожні n мілісекунд
            System.Threading.Timer timer = new System.Threading.Timer(Replace, null, TimeSpan.Zero, dueTime);
            ///
            /// 🇬🇧 Wait n seconds before ending the program
            /// 🇺🇦 Очікування n секунд перед завершенням програми
            Thread.Sleep(period);
            ///
            /// 🇬🇧 Stop the timer before the program ends
            /// 🇺🇦 Зупинка таймера перед завершенням програми
            timer.Dispose();
            ///
            void Replace(object state)
            {
                /// 🇬🇧 Checking the file sizes in the 'Localisation\custom' folder and updating them
                /// 🇺🇦 Перевірка розмірів файлів у теці "Localisation\custom" і їх оновлення
                Localisation.Replace(localisationFolder, customLocalisationFolder);
                ActualIterations++;
                ///
                /// 🇬🇧 If want to stop the timer after n seconds
                /// 🇺🇦 Якщо потрібно зупинити таймер після n секунд
                ///timer.Dispose();
            }
        }
    }

    /// <summary>
    /// 🇬🇧 Checks whether the file sizes in two dictionaries <see cref="Dictionary{string, long}"/> have changed. <br/>
    /// The method compares the number and size of files in the original and new dictionaries. <br/><br/>
    /// 
    /// 🇺🇦 Перевіряє, чи змінилися розміри файлів в двох словниках <see cref="Dictionary{string, long}"/>. <br/>
    /// Метод порівнює кількість та значення розмірів файлів у вихідному та новому словниках.
    /// </summary>
    /// 
    /// <param name="initialSizes">
    /// 🇬🇧 Dictionary with output file sizes. <br/>
    /// 🇺🇦 Словник із вихідними розмірами файлів.</param>
    /// 
    /// <param name="newSizes">
    /// 🇬🇧 Dictionary with new file sizes. <br/>
    /// 🇺🇦 Словник із новими розмірами файлів.</param>
    /// 
    /// <returns>
    /// 🇬🇧 <see langword="True"/> if there are changes in file sizes; <see langword="False"/> if the file sizes are the same. <br/>
    /// 🇺🇦 <see langword="True"/>, якщо є зміни в розмірах файлів; <see langword="False"/>, якщо розміри файлів однакові.
    /// </returns>
    static bool IsChanged(Dictionary<string, long> initialSizes, Dictionary<string, long> newSizes)
    {
        return initialSizes.Count == newSizes.Count &&
               initialSizes.All(entry => newSizes.ContainsKey(entry.Key) && newSizes[entry.Key] != entry.Value);
    }

    /// <summary>
    /// 🇬🇧 Gets the file size at the specified path.
    /// If the file exists, the method returns its size in <see langword="bytes"/>. <br/> 
    /// If the file does not exist, the method returns <see langword="-1"/>. <br/><br/> 
    /// 
    /// 🇺🇦 Отримує розмір файлу за вказаним шляхом.
    /// Якщо файл існує, метод повертає його розмір у <see langword="байтах"/>. <br/> 
    /// Якщо файл не існує, метод повертає <see langword="-1"/>.
    /// </summary>
    /// 
    /// <param name="filePath">
    /// 🇬🇧 The path to the file. <br/> 
    /// 🇺🇦 Шлях до файлу.</param>
    /// 
    /// <returns>
    /// 🇬🇧 File size in <see langword="bytes"/> or <see langword="-1"/> if the file does not exist. <br/> 
    /// 🇺🇦 Розмір файлу у <see langword="байтах"/> або <see langword="-1"/>, якщо файл не існує.
    /// </returns>
    public static long GetFileSize(string filePath)
    {
        if (File.Exists(filePath))
        {
            FileInfo fileInfo = new FileInfo(filePath);
            return fileInfo.Length;
        }

        return -1;
    }

    /// <summary>
    /// 🇬🇧 Gets the number of non-empty lines in the file at the specified path.
    /// If the file exists, the method reads its contents and counts the number of non-empty lines.<br/> 
    /// If the file does not exist, the method returns <see langword="-1"/>.<br/><br/> 
    /// 
    /// 🇺🇦 Отримує кількість не порожніх рядків у файлі за вказаним шляхом.
    /// Якщо файл існує, метод читає його вміст і підраховує кількість не порожніх рядків.<br/> 
    /// Якщо файл не існує, метод повертає <see langword="-1"/>.<br/> 
    /// </summary>
    /// 
    /// <param name="filePath">
    /// 🇬🇧 The path to the file. <br/> 
    /// 🇺🇦 Шлях до файлу.</param>
    /// 
    /// <returns>
    /// 🇬🇧 The number of non-empty lines in the file or <see langword="-1"/> if the file does not exist. <br/> 
    /// 🇺🇦 Кількість не порожніх рядків у файлі або <see langword="-1"/>, якщо файл не існує.
    /// </returns>
    public static long GetFileLinesCount(string filePath)
    {
        if (File.Exists(filePath))
        {
            long nonEmptyLinesCount = 0;

            using (StreamReader reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        nonEmptyLinesCount++;
                    }
                }
            }

            return nonEmptyLinesCount;
        }

        return -1;
    }
}

/// <summary>
/// 🇬🇧 Class for working with directories. <br/>
/// 🇺🇦 Клас для роботи з директоріями.
/// </summary>
class Directories
{
    /// <summary>
    /// 🇬🇧 A list containing file sizes in <see langword="bytes"/>. <br/>
    /// 🇺🇦 Список, який містить розміри файлів у <see langword="байтах"/>.
    /// </summary>
    /// 
    /// <returns>
    /// 🇬🇧 File size in <see langword="bytes"/>. <br/>
    /// 🇺🇦 Розмір файлу у <see langword="байтах"/>.
    /// </returns>
    public static List<long> FileSizesList = new List<long>();
    ///
    /// <summary>
    /// 🇬🇧 A list containing the number of lines in the files. <br/>
    /// 🇺🇦 Список, який містить кількість рядків у файлах.
    /// </summary>
    /// 
    /// <returns>
    /// 🇬🇧 The number of lines in the file. <br/>
    /// 🇺🇦 Кількість рядків у файлу.
    /// </returns>
    public static List<long> FileLinesCountList = new List<long>();

    /// <summary>
    /// 🇬🇧 Gets file sizes in <see langword="bytes"/> for the specified file paths using <see cref="Files.GetFileSize"/>. <br/>
    /// File sizes are written to the list <see cref="FileSizesList"/>. <br/><br/>
    /// 🇺🇦 Отримує розміри файлів у <see langword="байтах"/> для заданих шляхів до файлів за допомогою <see cref="Files.GetFileSize"/>. <br/>
    /// Розміри файлів записуються у список <see cref="FileSizesList"/>.
    /// </summary>
    /// 
    /// <param name="filePaths">
    /// 🇬🇧 An array of file paths. <br/>
    /// 🇺🇦 Масив шляхів до файлів.</param>
    /// 
    /// <returns>
    /// 🇬🇧 An array containing file sizes in <see langword="bytes"/>. <br/>
    /// 🇺🇦 Масив, що містить розміри файлів у <see langword="байтах"/>.
    /// </returns>
    public static long[] GetFileSizesInFolder(string[] filePaths)
    {
        long[] fileSizes = new long[filePaths.Length];

        for (int i = 0; i < filePaths.Length; i++)
        {
            fileSizes[i] = Files.GetFileSize(filePaths[i]);
            FileSizesList.Add(fileSizes[i]);
        }

        return fileSizes;
    }

    /// <summary>
    /// 🇬🇧 Gets the number of filled lines in files for the specified file paths using <see cref="Files.GetFileLinesCount"/>. <br/>
    /// The number of filled file lines is written to the list <see cref="FileLinesCountList"/>. <br/><br/>
    /// 🇺🇦 Отримує кількість заповнених рядків у файлах для заданих шляхів до файлів за допомогою <see cref="Files.GetFileLinesCount"/>. <br/>
    /// Кількість заповнених рядків файлів записуються у список <see cref="FileLinesCountList"/>.
    /// </summary>
    /// 
    /// <param name="filePaths">
    /// 🇬🇧 An array of file paths. <br/>
    /// 🇺🇦 Масив шляхів до файлів.</param>
    /// 
    /// <returns>
    /// 🇬🇧 An array containing the number of file lines. <br/>
    /// 🇺🇦 Масив, що містить кількість рядків файлів.
    /// </returns>
    public static long[] GetFileLinesCountInFolder(string[] filePaths)
    {
        long[] linesCount = new long[filePaths.Length];

        for (int i = 0; i < filePaths.Length; i++)
        {
            linesCount[i] = Files.GetFileLinesCount(filePaths[i]);
            FileLinesCountList.Add(linesCount[i]);
        }

        return linesCount;
    }
}

/// <summary>
/// 🇬🇧 Class for working with paths. <br/>
/// 🇺🇦 Клас для роботи зі шляхами.
/// </summary>
class Paths
{
    /// <summary>
    /// 🇬🇧 Full path to the program folder, <see langword="readonly"/>. <br/>
    /// 🇺🇦 Повний шлях до теки програми, <see langword="readonly"/>.
    /// </summary>
    /// 
    /// <returns>
    /// 🇬🇧 Base directory of the program. <br/>
    /// 🇺🇦 Базова директорія програми.
    /// </returns>
    public static readonly string ApplicationFolder = AppDomain.CurrentDomain.BaseDirectory;
    /// 
    /// <summary>
    /// 🇬🇧 Full path to the localisation folder, <see langword="readonly"/>. <br/>
    /// 🇺🇦 Повний шлях до теки локалізації, <see langword="readonly"/>.
    /// </summary>
    /// 
    /// <returns>
    /// 🇬🇧 Localisation directory. <br/>
    /// 🇺🇦 Директорія локалізації.
    /// </returns>
    public static readonly string LocalisationFolder = Path.Combine(ApplicationFolder, "Localisation");
    /// 
    /// <summary>
    /// 🇬🇧 Full path to the custom localisations folder, <see langword="readonly"/>. <br/>
    /// 🇺🇦 Повний шлях до теки користувацьких локалізацій, <see langword="readonly"/>.
    /// </summary>
    /// 
    /// <returns>
    /// 🇬🇧 Custom localisations directory. <br/>
    /// 🇺🇦 Директорія користувацьких локалізацій.
    /// </returns>
    public static readonly string CustomLocalisationFolder = Path.Combine(LocalisationFolder, "custom");
    /// 
    /// <summary>
    /// 🇬🇧 Full path to the game application, <see langword="readonly"/>. <br/>
    /// 🇺🇦 Повний шлях до застосунку гри, <see langword="readonly"/>.
    /// </summary>
    /// 
    /// <returns>
    /// 🇬🇧 Osu application. <br/>
    /// 🇺🇦 Застосунок гри.
    /// </returns>
    public static readonly string OsuApplicationFile = Path.Combine(ApplicationFolder, "osu!.exe");
    /// 
    /// <summary>
    /// 🇬🇧 Full path to the logs folder, <see langword="readonly"/>. <br/>
    /// 🇺🇦 Повний шлях до теки логів, <see langword="readonly"/>.
    /// </summary>
    /// 
    /// <returns>
    /// 🇬🇧 Logs directory. <br/>
    /// 🇺🇦 Директорія логів.
    /// </returns>
    public static readonly string OsuApplicationLogFolder = Path.Combine(ApplicationFolder, "Logs");
    /// 
    /// <summary>
    /// 🇬🇧 Full path to the program log file, <see langword="readonly"/>. <br/>
    /// 🇺🇦 Повний шлях до файлу логів програми, <see langword="readonly"/>.
    /// </summary>
    /// 
    /// <returns>
    /// 🇬🇧 Program log file. <br/>
    /// 🇺🇦 Файл логів програми.
    /// </returns>
    public static readonly string ApplicationLogFile = Path.Combine(OsuApplicationLogFolder, "osu!loc.log");
}
