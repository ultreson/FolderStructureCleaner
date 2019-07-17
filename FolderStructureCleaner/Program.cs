using System;
using System.Collections;
using System.Collections.Generic;
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
            Console.WriteLine("Pick a root directory");
            string path = Console.ReadLine();
            string[] toDelete = ListEmptyFolders(path, currentWeek);
            if (toDelete.Any())
            {
                Console.WriteLine("Folders to delete: ");
                foreach (string element in toDelete)
                {
                    Console.WriteLine("DEL " + element);
                }
                Console.WriteLine("Press any key to confirm the deletion");
                Console.Read();
                DeleteFoldersFromArray(toDelete);
            }
            else
            {
                Console.WriteLine("Nothing to delete");
            }
            Console.Read();
        }

        static string[] ListEmptyFolders(string path, int currentWeek)
        {
            ArrayList toDelete = new ArrayList();
            if (!Directory.Exists(path))
            {
                Console.WriteLine("Invalid root path.");
                return null;
            }
            IEnumerable<string> dirs = Directory.EnumerateDirectories(path, "??????", SearchOption.TopDirectoryOnly);
            foreach (string dir in dirs)
            {
                IEnumerable<string> weekDirs = Directory.EnumerateDirectories(dir, "semaine ??", SearchOption.TopDirectoryOnly);
                IEnumerable<string> pastWeekDirs = weekDirs.Where(s => int.Parse(s.Split(' ', StringSplitOptions.None).Last()) < currentWeek);
                foreach (string weekDir in pastWeekDirs)
                {
                    var content = Directory.EnumerateDirectories(weekDir);
                    int oldCount = toDelete.Count;
                    foreach (string dayDirOrContent in content)
                    {
                        string dayDirOrContentName = dayDirOrContent.Split('\\').Last();
                        if (daysOfWeek.Contains(dayDirOrContentName))
                        {
                            IEnumerable<string> subContent = Directory.EnumerateFileSystemEntries(dayDirOrContent);
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
                        //toDelete.RemoveRange(oldCount, content.Count());
                        toDelete.Add(weekDir);
                    }
                }
            }
            return (string[])toDelete.ToArray(typeof(string));
        }

        static void DeleteFoldersFromArray(string[] folders)
        {
            foreach (string folder in folders)
            {
                if (Directory.Exists(folder))
                {
                    Directory.Delete(folder, false);
                }
            }
        }
    }
}
