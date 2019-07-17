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
            Console.WriteLine("Please input the current week number :");
            int currentWeek = int.Parse(Console.ReadLine());
            //Console.WriteLine(currentWeek);
            var toDelete = ListEmptyFolders(currentWeek);
            if (toDelete.Any())
            {
                Console.WriteLine("Folders to delete: ");
                foreach (var element in toDelete)
                {
                    Console.WriteLine("DEL " + element);
                }
                Console.Read();
                DeleteFoldersFromArray(toDelete);
            }
            else
            {
                Console.WriteLine("Nothing to delete");
            }
            Console.Read();
        }

        static string[] ListEmptyFolders(int currentWeek)
        {
            var toDelete = new ArrayList();
            Console.WriteLine("Pick a root directory");
            String path = Console.ReadLine();
            if (!Directory.Exists(path))
            {
                Console.WriteLine("Invalid root path.");
                return null;
            }
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
                            if (!subContent.Any())
                            {
                                toDelete.Add(dayDirOrContent);
                            }
                        }
                    }

                    if (toDelete.Count != 0 &&
                        Directory.EnumerateDirectories(weekDir, "",SearchOption.TopDirectoryOnly).Count() == Directory.EnumerateFileSystemEntries(weekDir,"", SearchOption.TopDirectoryOnly).Count() &&
                        oldCount + content.Count() == toDelete.Count)
                    {
                        toDelete.RemoveRange(oldCount, content.Count());
                        toDelete.Add(weekDir);
                    }
                }
            }
            return (string[])toDelete.ToArray(typeof(string));
        }

        static void DeleteFoldersFromArray(string[] folders)
        {
            foreach (var folder in folders)
            {
                Directory.Delete(folder, true);
            }
        }
    }
}
