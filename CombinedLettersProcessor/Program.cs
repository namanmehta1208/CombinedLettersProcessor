using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CombinedLettersProcessor
{
    public interface ILetterService
    {
        void CombineTwoLetters(string inputFile1, string inputFile2, string resultFile);
    }

    //Interface
    public class LetterService : ILetterService
    {
        public void CombineTwoLetters(string inputFile1, string inputFile2, string resultFile)
        {
            string content1 = File.ReadAllText(inputFile1);
            string content2 = File.ReadAllText(inputFile2);
            string combinedContent = content1 + Environment.NewLine + content2;
            File.WriteAllText(resultFile, combinedContent);
        }
    }

    class Program
    {
        static ILetterService letterService = new LetterService();

        static void Main(string[] args)
        {
            string rootPath = @"D:\Jobs\CombinedLetters";
            string inputPath = Path.Combine(rootPath, "Input");
            string archivePath = Path.Combine(rootPath, "Archive");
            string outputPath = Path.Combine(rootPath, "Output");
            string currentDateForVal = DateTime.Now.ToString("yyyyMMdd");
            string reportName = DateTime.Now.ToString("MM/dd/yyyy") + " Report.txt";
            string reportPath = Path.Combine(outputPath, reportName);

            DateTime lastProcessedDate = GetLastProcessedDate(outputPath) ?? DateTime.Today.AddDays(-1);
            DateTime currentDate = DateTime.Today;

            //Two conditions before doing anything
            //1. It checks if this console app has been already run for today.
            //2. It check if the directories in Admission and Scholarship have a folder with todays date.
            if (!Directory.Exists(Path.Combine(inputPath, "Admission", currentDateForVal)) ||
                !Directory.Exists(Path.Combine(inputPath, "Scholarship", currentDateForVal)))
            {
                Console.WriteLine($"No folders for {DateTime.Now.ToString("MM/dd/yyyy")} in Admission or Scholarship folder");
                return;
            }

            if (File.Exists(reportPath))
            {
                Console.WriteLine($"This console app has already been run for {DateTime.Now.ToString("MM/dd/yyyy")}");
                return;
            }

            // This for loop starts with the lastProcessDate + 1 and will process till it becomes equal to the current date.
            for (DateTime date = lastProcessedDate.AddDays(1); date <= currentDate; date = date.AddDays(1))
            {
                string dateFormatted = date.ToString("yyyyMMdd");

                Console.WriteLine($"Processing for date: {date.ToString("MM/dd/yyyy")}");

                if (!Directory.Exists(Path.Combine(inputPath, "Admission", dateFormatted)) || !Directory.Exists(Path.Combine(inputPath, "Scholarship", dateFormatted)))
                {
                    Console.WriteLine($"No folders for {date.ToString("MM/dd/yyyy")} in Admission or Scholarship folder");
                    continue;
                }

                ArchiveFiles(inputPath, archivePath, dateFormatted);
                ProcessLetters(inputPath, outputPath, dateFormatted, letterService);
            }
        }

        static void ArchiveFiles(string inputPath, string archivePath, string date)
        {
            // Define paths for both Admission and Scholarship for the specific date
            string admissionPath = Path.Combine(inputPath, "Admission", date);
            string scholarshipPath = Path.Combine(inputPath, "Scholarship", date);

            // Copy to Archive
            ArchiveDirectory(admissionPath, Path.Combine(archivePath, "Admission", date));
            ArchiveDirectory(scholarshipPath, Path.Combine(archivePath, "Scholarship", date));
        }

        static void ArchiveDirectory(string sourcePath, string destinationPath)
        {
            if (!Directory.Exists(sourcePath))
            {
                Console.WriteLine($"There was some problem during archiving, please check the source path");
                return;  // If the source directory does not exist, exit the method
            }

            // Create the destination directory if it doesn't exist
            if (!Directory.Exists(destinationPath))
            {
                Directory.CreateDirectory(destinationPath);
            }

            // Copy each file to the new directory
            foreach (string file in Directory.GetFiles(sourcePath))
            {
                string destinationFile = Path.Combine(destinationPath, Path.GetFileName(file));
                File.Copy(file, destinationFile, true);
            }
        }


        static void ProcessLetters(string inputPath, string outputPath, string date, ILetterService letterService)
        {
            // Ensure output directory exists
            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            // Getting paths for Admission and Scholarship letters for the given date
            string admissionPath = Path.Combine(inputPath, "Admission", date);
            string scholarshipPath = Path.Combine(inputPath, "Scholarship", date);

            // Check if directories exist before proceeding
            if (!Directory.Exists(admissionPath) || !Directory.Exists(scholarshipPath))
            {
                Console.WriteLine($"Missing directories for date {date}. Admission or Scholarship data is not complete.");
                return;
            }

            // Read and organize files by UniversityID
            var admissionFiles = Directory.GetFiles(admissionPath, "*.txt")
                                           .Select(Path.GetFileNameWithoutExtension)
                                           .Where(f => f.StartsWith("admission-"))
                                           .ToDictionary(f => f.Split('-')[1], f => f);
            var scholarshipFiles = Directory.GetFiles(scholarshipPath, "*.txt")
                                             .Select(Path.GetFileNameWithoutExtension)
                                             .Where(f => f.StartsWith("scholarship-"))
                                             .ToDictionary(f => f.Split('-')[1], f => f);

            List<string> combinedIDs = new List<string>();

            // Process matching UIDs
            foreach (var id in admissionFiles.Keys.Intersect(scholarshipFiles.Keys))
            {
                string admissionFileFullPath = Path.Combine(admissionPath, $"{admissionFiles[id]}.txt");
                string scholarshipFileFullPath = Path.Combine(scholarshipPath, $"{scholarshipFiles[id]}.txt");
                string outputFile = Path.Combine(outputPath, $"combined-{id}.txt");

                // Combine letters using the provided service
                letterService.CombineTwoLetters(admissionFileFullPath, scholarshipFileFullPath, outputFile);
                combinedIDs.Add(id);
            }

            // Generate report for each day processed
            GenerateReport(outputPath, date, combinedIDs);
        }

        //Method for generating the report in the Output folder
        static void GenerateReport(string outputPath, string date, List<string> combinedIDs)
        {
            string reportFileName = DateTime.ParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy") + " Report.txt";
            string reportPath = Path.Combine(outputPath, reportFileName);

            using (StreamWriter sw = new StreamWriter(reportPath))
            {
                sw.WriteLine($"{DateTime.ParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("MM/dd/yyyy")} Report");
                sw.WriteLine("-------------------------------------------");
                sw.WriteLine($"Number of combined letters: {combinedIDs.Count}");
                foreach (string id in combinedIDs)
                {
                    sw.WriteLine(id);
                }
            }
        }

        //Method to check the latest "Report.txt", if this returns null then check the format of the date, current is "MM-dd-yyyy"
        static DateTime? GetLastProcessedDate(string outputPath)
        {
            var reportFiles = Directory.GetFiles(outputPath, "* Report.txt")
                                       .Select(Path.GetFileName)
                                       .Select(file => DateTime.TryParseExact(file.Split(' ')[0], "MM-dd-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date) ? date : (DateTime?)null)
                                       .Where(date => date != null)
                                       .OrderByDescending(date => date)
                                       .FirstOrDefault();

            return reportFiles;
        }
    }
}
