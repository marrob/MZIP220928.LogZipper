
/**
 * Robi, nincs kedved írni egy kis progit, ami egy mappában lévő előző hónapban készült 
 * .log fileokat 7zipel betömöríti egy parent_folder_name_yyyymm_log.zip nevű mappába?
 * csak mert szerintem a TTC500 log mappában van néhány ezer fájl, és lassan nem fog megnyílni sem... 
 * ezt a kis progit meg meg tudnánk hívni a schedulerből mondjuk adott hónap másodikán.
 * Imre
 *
 * User Manual
 * 
 * 1. Ne létezzen már azonos nevű ZIP fájl a mappában!
 * 2. Ha nem tud egy fájlt zippelni, akkor vége és törlés nélkül hibüzenttel kilép
 * 3. Ha nem tudo egy vagy több fált törölni, akkor azokat kihagyja és folytatja tovább... 
 * 4. Ha a cél könyvtár nem létezik, akkor hibaüzenttel jelzi
 * 
 * 
 * Task Scheduler
 * Actions -> Edit Action
 * Program/Scirpt:"D:\...\Knv.MZIP220908.ZipLogger" (idézőjelek között! )NE LEGYEN utolsó '\' JEL!!!!!!!!!!!!!!
 * Add arguments: -dir d:\measuremnts\TTC500_log  
 * 
 * Triggers -> ...
 * 
 */
namespace Knv.MZIP220928.LogZipper
{
    using System;
    using System.IO;
    using System.IO.Compression; //->https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.zipfile?redirectedfrom=MSDN&view=netframework-4.8
    using System.Diagnostics;

    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
                Console.WriteLine("Please give me the Target directory... Example: Knv.MZIP220908.ZipLogger.exe -dir d:\\measuremnts\\TTC500_log");
           
            else if (args[0] == "-dir")
                if (!string.IsNullOrEmpty(args[1]))
                    if (!Directory.Exists(args[1]))
                        Console.WriteLine($"Dirctory does't exist: {args[1]}");
                    else
                        LetsDoit(args[1]);
            else
                Console.WriteLine("Invalid argument...");
            Console.ReadKey();
        }

        static void LetsDoit(string directory)
        {
            var sw = new Stopwatch();
            sw.Start();
            Console.WriteLine($"Source directory {directory}");
            var files = Directory.GetFiles(directory,"*.log");
            Console.WriteLine($"Total files: {files.Length} (*.log)");
            var dt = DateTime.Now;
            var zipFileName = $"{Path.GetFileName(directory)}_{dt.Year:0000}{dt.Month:00}{dt.Day:00}_log";
            var zipFilePath = $"{directory}\\{zipFileName}.zip";
            Console.WriteLine($"Zip File path:{zipFilePath}");
            Console.WriteLine($"\r\nZip is running... Please Wait");
            try
            {
                using (var zip = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
                {
                    foreach (var file in files)
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write($"Compress: {Path.GetFileName(file)}");
                        zip.CreateEntryFromFile(file, Path.GetFileName(file), CompressionLevel.Optimal);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("OK");
                    }
                }
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine($"\r\nDelete is running... Please Wait");
                foreach (var file in files)
                {
                    try
                    {
                        File.Delete(file);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write($"Delete: {Path.GetFileName(file)}");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("OK");
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"FAILED-{ex.Message}");
                    }
                }
                sw.Stop();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Mission Completed. Elapsed:{sw.ElapsedMilliseconds/1000}sec");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
            }
        }
    }
}
