using System;
using System.Collections;
using System.IO;
using System.Linq;

namespace FolderStructureCleaner
{
    class Program
    {
        private static string [] daysOfWeek = new string[] {"lundi", "mardi", "mercredi", "jeudi", "vendredi", "samedi"};

        static void Main(string[] args)
        {
            //int currentWeek = int.Parse(Console.ReadLine());
            //Console.WriteLine(currentWeek);
            ListEmptyFolders();
            Console.Read();
        }

        static string[] ListEmptyFolders()
        {
            var toDelete = new ArrayList();
            String path = "D:\\Alexi Tessier\\git\\FolderStructureCleaner\\FolderStructureCleaner\\bin\\Debug\\netcoreapp2.1\\test";
            var dirs = Directory.EnumerateDirectories(path, "??????", SearchOption.TopDirectoryOnly);
            foreach (var dir in dirs)
            {
                var weekDirs = Directory.EnumerateDirectories(dir, "semaine ??", SearchOption.TopDirectoryOnly);
                foreach (var weekDir in weekDirs)
                {
                    var content = Directory.EnumerateDirectories(weekDir);
                    int oldCount = toDelete.Count;
                    foreach (var dayDirOrContent in content)
                    {
                        string dayDirOrContentName = dayDirOrContent.Split('\\').Last();
                        if (daysOfWeek.Contains(dayDirOrContentName))
                        {
                            var subContent = Directory.EnumerateFileSystemEntries(dayDirOrContent);
                            if (subContent.Count() < 1)
                            {
                                toDelete.Add(dayDirOrContent);
                            }
                        }
                    }

                    if (Directory.EnumerateDirectories(weekDir, "",SearchOption.TopDirectoryOnly).Count() == Directory.EnumerateFileSystemEntries(weekDir,"", SearchOption.TopDirectoryOnly).Count() &&
                        oldCount + content.Count() == toDelete.Count &&
                        toDelete.Count != 0)
                    {
                        toDelete.RemoveRange(oldCount, content.Count());
                        toDelete.Add(weekDir);
                    }
                }

            
                Console.WriteLine("-----------------------------");
            }

            foreach (var element in toDelete)
            {
                Console.WriteLine("DEL " + element);
            }
            return null;
        }
    }
}
