using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFileLauncher
{
    internal class History
    {
        private const int MaxHistoryNum = 20;
        private const string HistoryFileName = "History.info";

        private readonly string _historyFilePath;

        private string[] _files;

        internal string[] Files
        {
            get
            {
                string[] dst = new string[_files.Count()];
                Array.Copy(_files, dst, _files.Count());
                return dst;
            }
        }

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
            _historyFilePath = historyFilePath;
            _files = System.IO.File.ReadAllLines(historyFilePath);
        }

        internal void Add(string filePath)
        {
            // 履歴なので先頭(再新)に追加
            _files = _files.Prepend(filePath).ToArray<string>();

            // 履歴保持数を上回るなら末尾を削除
            if (_files.Count() >= MaxHistoryNum)
            {
                _files = _files[0..MaxHistoryNum];
            }

            // 履歴をファイルに保持
            CreateFile(_files);
        }

        /// <summary>
        /// array を改行付き平坦化した文字列を中身としたファイルを作成する(既に存在するなら上書き保存する)
        /// </summary>
        private void CreateFile(string[] array)
        {
            string contents = string.Join("\r\n", array) + "\r\n";
            System.IO.File.WriteAllText(_historyFilePath, contents);
        }
    }
}
