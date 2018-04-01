using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SplitedAssetsCombiner
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("按任意键开始");
            Console.ReadKey();
            Console.WriteLine();

            string[] files = Directory.GetFiles(args.Length > 0 ? args[1] : Environment.CurrentDirectory);
            Dictionary<string, Dictionary<int, string>> splitedFiles = new Dictionary<string, Dictionary<int, string>>();

            foreach (string file in files)
            {
                if (file.Contains(".split"))
                {
                    string targetName = file.Substring(0, file.LastIndexOf(".split"));
                    int index = int.Parse(file.Substring(file.LastIndexOf(".split") + 6));
                    Dictionary<int, string> splitedFile;
                    if (!splitedFiles.ContainsKey(targetName))
                    {
                        splitedFile = new Dictionary<int, string>();
                        splitedFiles.Add(targetName, splitedFile);
                    }
                    else
                    {
                        splitedFile = splitedFiles[targetName];
                    }
                    splitedFile.Add(index, file);
                }
            }

            splitedFiles = splitedFiles.OrderBy(o => o.Key).ToDictionary(k => k.Key, e => e.Value);
            for (int i = 0; i < splitedFiles.Count; i++)
            {
                KeyValuePair<string, Dictionary<int, string>> d = splitedFiles.ElementAt(i);
                splitedFiles[d.Key] = d.Value.OrderBy(o => o.Key).ToDictionary(k => k.Key, e => e.Value);
            }

            foreach (KeyValuePair<string, Dictionary<int, string>> d in splitedFiles)
            {
                Console.WriteLine("目标文件: " + d.Key);
                Stream target = File.Open(d.Key, FileMode.Create, FileAccess.ReadWrite);
                foreach (string splitedFile in d.Value.Values)
                {
                    Console.Write("        - " + splitedFile);
                    Stream source = File.OpenRead(splitedFile);
                    source.CopyTo(target);
                    source.Close();
                    File.Delete(splitedFile);
                    Console.WriteLine(" 已写入");
                }
                target.Close();
                Console.WriteLine();
            }

            Console.WriteLine("已完成");
            Console.ReadKey();
        }
    }
}
