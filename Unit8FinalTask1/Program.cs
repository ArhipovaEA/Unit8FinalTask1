using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Unit8FinalTask1
{
    class Program
    {
        
        static void Main()
        {
            string PathForDel;
            int LenPath ;
            do
            {               
                Console.WriteLine("Укажите путь по которому необходимо удалить данные: ");
                PathForDel = Console.ReadLine();
                LenPath = PathForDel.Length;
            }
            while (LenPath < 3);

            DeleteAllBefor30Min(PathForDel);
        }

        static void DelFile(string filePath, int MinDel)
        {
            TimeSpan interval;

            if (File.Exists(filePath))
            {
                Console.WriteLine("Обработка ... " + filePath);

                interval = DateTime.Now - File.GetLastAccessTime(filePath);

                if (interval.TotalMinutes > MinDel)
                {
                    if (ChekAccessUser(filePath))
                    {
                        File.Delete(filePath);
                        Console.WriteLine("Файл {0} удален ", filePath);
                    }
                    else
                    {
                        Console.WriteLine("Нет прав на удаление " + filePath);
                    }
                }
            }
            else
            {
                Console.WriteLine("Файл не существует - " + filePath);
            }
        }

        static void DeleteAllBefor30Min(string PathDel)
        {
            if (Directory.Exists(PathDel))
            {
                Console.WriteLine("Обработка началась... " + PathDel);
                try
                {
                    var folderInfo = Directory.GetDirectories(PathDel);
                    for (int i = 0; i < folderInfo.Length; i++)
                    {
                        if (ChekAccessUser(folderInfo[i]))
                        {
                            DeleteAllBefor30Min(folderInfo[i]);
                            DirectoryInfo dirInfo = new DirectoryInfo(folderInfo[i]);
                            if (dirInfo.GetFiles().Length == 0)
                            {
                                Directory.Delete(folderInfo[i]);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Нет прав на удаление " + folderInfo[i]);
                        }

                    }

                    var fileNames = Directory.GetFiles(PathDel);

                    for (int i = 0; i < fileNames.Length; i++)
                    {
                            DelFile(fileNames[i], 30);
                     }




                }
                catch (Exception ex)
                {
                    Console.WriteLine("Произошла ошибка: " + ex.Message);
                }

                Console.WriteLine("Обработка завершилась... " + PathDel);
            }
            else
            {
                Console.WriteLine("Данная директория не существует - " + PathDel);
             }
        }

        static bool ChekAccessUser(string PathCheck)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(PathCheck);
                WindowsIdentity wi = WindowsIdentity.GetCurrent();
                DirectorySecurity ds = dir.GetAccessControl(AccessControlSections.Access);
                AuthorizationRuleCollection rules = ds.GetAccessRules(true, true, typeof(SecurityIdentifier));
                foreach (FileSystemAccessRule rl in rules)
                {
                    if (((rl.FileSystemRights & FileSystemRights.Delete) == FileSystemRights.Delete))
                    {
                          if (rl.AccessControlType == AccessControlType.Allow)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                    }

                }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Произошла ошибка: " + ex.Message);
            return false;
        }
    }
    }
}
