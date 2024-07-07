using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFileLauncher
{
    internal static class FileSearch
    {
        internal static string[] Search(History history, FileIndex index, string text)
        {
            HashSet<string> historySearched = history.Search(text);
            HashSet<string> indexSearched = index.Search(text);

            // 履歴が先頭になり、重複がないように合体
            return historySearched.ToArray().Concat(indexSearched.ToArray()).Distinct().ToArray();
        }
    }
}
