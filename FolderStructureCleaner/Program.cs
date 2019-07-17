using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FolderStructureCleaner
{
    class Program
    {
        /// <summary>
        /// List of names for days of the week for matching directory names.
        /// </summary>
        private static string[] daysOfWeek = new string[] {"lundi", "mardi", "mercredi", "jeudi", "vendredi", "samedi"};

        /// <summary>
        /// Manages the inputs for the folder deletion.
        /// </summary>
        /// <param name="args">Command line arguments for executable.</param>
        static void Main(string[] args)
        {
            Console.WriteLine("Pick a root directory");
            string path = Console.ReadLine();

            Console.WriteLine("Please input the current week number :");
            int currentWeek = int.Parse(Console.ReadLine());

            string[] toDelete = ListEmptyFolders(path, currentWeek);
            if (toDelete != null && toDelete.Any())
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

            Console.WriteLine("Press any key to continue");
            Console.Read();
        }

        /// <summary>
        /// Checks in the given path for any empty week/day folders under the given week number and lists them.
        /// </summary>
        /// <param name="path">root path of the folder structure</param>
        /// <param name="currentWeek">number of the current week, will not delete any week equal or higher</param>
        /// <returns>An array with the full path of directories to delete, null if parameters are invalid.</returns>
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
                IEnumerable<string> weekDirs =
                    Directory.EnumerateDirectories(dir, "semaine ??", SearchOption.TopDirectoryOnly);
                IEnumerable<string> pastWeekDirs = weekDirs.Where(s =>
                    int.Parse(s.Split(' ').Last()) < currentWeek);
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
                        Directory.EnumerateDirectories(weekDir, "", SearchOption.TopDirectoryOnly).Count() ==
                        Directory.EnumerateFileSystemEntries(weekDir, "", SearchOption.TopDirectoryOnly).Count() &&
                        oldCount + content.Count() == toDelete.Count)
                    {
                        //toDelete.RemoveRange(oldCount, content.Count());
                        toDelete.Add(weekDir);
                    }
                }
            }

            return (string[]) toDelete.ToArray(typeof(string));
        }

        /// <summary>
        /// Deletes folders in given array non recursively.
        /// </summary>
        /// <param name="directories">Array of paths to directories to delete.</param>
        static void DeleteFoldersFromArray(string[] directories)
        {
            foreach (string directory in directories)
            {
                if (Directory.Exists(directory))
                {
                    Directory.Delete(directory, false);
                }
            }
        }
    }
}