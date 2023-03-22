using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TClass.Net
{
    public class LogWriter
    {
        public string LogFolderPath { get; set; }

        public LogWriter(string logFolderPath)
        {
            LogFolderPath = logFolderPath;
            RemoveOldFiles();
        }
        public void SaveLog(string log)
        {
            try
            {
                if (!Directory.Exists(LogFolderPath))
                    Directory.CreateDirectory(LogFolderPath);

                string file = GetLogFile();

                using (StreamWriter writer = File.AppendText(file))
                {
                    writer.WriteLine($"[{DateTime.UtcNow:dd-MM-yy HH:mm:ss}]--> {log}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        private string GetLogFile()
        {
            string fileName = $"{DateTime.UtcNow:dd-MM-yyyy-HH}-log.txt";
            string filePath = Path.Combine(LogFolderPath, fileName);

            if (!File.Exists(filePath))
            {
                File.Create(filePath).Dispose();
            }

            return filePath;
        }

        public void RemoveOldFiles()
        {
            try
            {
                string[] files = Directory.GetFiles(LogFolderPath);
                foreach (string file in files)
                {
                    if (File.GetCreationTime(file) < DateTime.Now.AddDays(-30))
                    {
                        File.Delete(file);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}