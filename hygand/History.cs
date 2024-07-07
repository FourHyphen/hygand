using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hygand
{
    internal class History
    {
        private const int MaxHistoryNum = 20;
        private const string HistoryFileName = "History.info";

        private readonly string _historyFilePath;

        private string[] _files = new string[0];

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

            // 初回実行時など、履歴は存在しないこともあるため有無確認
            if (System.IO.File.Exists(historyFilePath))
            {
                _files = System.IO.File.ReadAllLines(historyFilePath);
            }
        }

        internal void Add(string filePath)
        {
            if (_files.Contains(filePath))
            {
                // すでに履歴に存在するので追加ではなく先頭(最新)に移動する
                MoveFirst(filePath);
            }
            else
            {
                // 履歴に追加
                AddCore(filePath);
            }

            // 履歴をファイルに保持
            CreateFile(_files);
        }

        /// <summary>
        /// 履歴のファイルを先頭に移動する
        /// </summary>
        private void MoveFirst(string filePath)
        {
            // いったん削除してから先頭に追加することで先頭への移動とする
            var tmp = _files.Where(s => s != filePath);
            _files = tmp.Prepend(filePath).ToArray();
        }

        /// <summary>
        /// 履歴にファイルを追加する
        /// </summary>
        private void AddCore(string filePath)
        {
            // 履歴なので先頭(再新)に追加
            _files = _files.Prepend(filePath).ToArray();

            // 履歴保持数を上回るなら末尾を削除
            if (_files.Count() >= MaxHistoryNum)
            {
                _files = _files[0..MaxHistoryNum];
            }
        }

        /// <summary>
        /// array を改行付き平坦化した文字列を中身としたファイルを作成する(既に存在するなら上書き保存する)
        /// </summary>
        private void CreateFile(string[] array)
        {
            string contents = string.Join("\r\n", array) + "\r\n";
            System.IO.File.WriteAllText(_historyFilePath, contents);
        }

        /// <summary>
        /// インデックスを部分一致検索する
        /// </summary>
        internal HashSet<string> Search(string word)
        {
            if (_files.Count() == 0)
            {
                return new HashSet<string>();
            }

            // 検証に使えるよう結果を直接 return しない
            var result = _files.Where(s => s.Contains(word)).ToHashSet();
            return result;
        }
    }
}
