using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFileLauncher
{
    internal class History
    {
        private static readonly string HistoryFileName = "History.info";

        internal IReadOnlyCollection<string> Files { get; }

        internal static History CreateInstance()
        {
            string historyFilePath = System.IO.Path.GetFullPath(HistoryFileName);
            return CreateInstance(historyFilePath);
        }

        internal static History CreateInstance(string historyFilePath)
        {
            return new History(historyFilePath);
        }

        private History(string historyFilePath)
        {
            Files = System.IO.File.ReadAllLines(historyFilePath);
        }
    }
}
