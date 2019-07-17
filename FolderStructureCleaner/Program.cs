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
            Console.WriteLine("Please inout the current week number :");
            int currentWeek = int.Parse(Console.ReadLine());
            //Console.WriteLine(currentWeek);
            var toDelete = ListEmptyFolders(currentWeek);
            //DeleteFoldersFromList(toDelete);
            Console.Read();
        }

        static string[] ListEmptyFolders(int currentWeek)
        {
            var toDelete = new ArrayList();
            String path = "D:\\Alexi Tessier\\git\\FolderStructureCleaner\\FolderStructureCleaner\\bin\\Debug\\netcoreapp2.1\\test";
            var dirs = Directory.EnumerateDirectories(path, "??????", SearchOption.TopDirectoryOnly);
            foreach (var dir in dirs)
            {
                var weekDirs = Directory.EnumerateDirectories(dir, "semaine ??", SearchOption.TopDirectoryOnly);
                var pastWeekDirs = weekDirs.Where(s => int.Parse(s.Split(' ', StringSplitOptions.None).Last()) < currentWeek);
                foreach (var weekDir in pastWeekDirs)
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
                                toDelete.Add(dayDirOrContent.ToString());
                            }
                        }
                    }

                    if (toDelete.Count != 0 &&
                        Directory.EnumerateDirectories(weekDir, "",SearchOption.TopDirectoryOnly).Count() == Directory.EnumerateFileSystemEntries(weekDir,"", SearchOption.TopDirectoryOnly).Count() &&
                        oldCount + content.Count() == toDelete.Count)
                    {
                        toDelete.RemoveRange(oldCount, content.Count());
                        toDelete.Add(weekDir.ToString());
                    }
                }
            }

            foreach (var element in toDelete)
            {
                Console.WriteLine("DEL " + element);
            }
            return (string[])toDelete.ToArray(typeof(string));
        }
    }
}
