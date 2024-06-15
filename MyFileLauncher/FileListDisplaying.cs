using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFileLauncher
{
    public class FileListDisplaying : INotifyPropertyChanged
    {
        private const int DisplayingNum = 20;

        public event PropertyChangedEventHandler? PropertyChanged;

        private string[] _fileList = new string[0];

        // internal では画面に反映されなかったため public
        public string[] FileList
        {
            get
            {
                return _fileList;
            }
            set
            {
                _fileList = value;
                NotifyPropertyChanged(nameof(FileList));
            }
        }

        /// <summary>
        /// プロパティ更新を通知
        /// </summary>
        private void NotifyPropertyChanged(string propertyName)
        {
            var e = new PropertyChangedEventArgs(propertyName);
            PropertyChanged?.Invoke(this, e);
        }

        /// <summary>
        /// 履歴をセットする
        /// </summary>
        internal void UpdateOfHistory(History history)
        {
            UpdatePart(history.Files);
        }

        /// <summary>
        /// ファイルリスト表示を更新する(全件表示は時間がかかり過ぎるため一部を表示する)
        /// </summary>
        private void UpdatePart(string[] files)
        {
            FileList = Slice(files, DisplayingNum);
        }

        /// <summary>
        /// Slice(list.Count >= 20, 20) -> return list.ToArray()[0..20]
        /// Slice(list.Count <  20, 20) -> return list.ToArray()
        /// </summary>
        private string[] Slice(string[] array, int end)
        {
            if (array.Count() < end)
            {
                return array;
            }

            return array[0..end].ToArray();
        }

        /// <summary>
        /// テキストボックス内の文字列でインデックスを検索した結果をセットする
        /// </summary>
        internal void UpdateOfIndex(History history, FileIndex fileIndex, string word)
        {
            string[] result = MyFileLauncher.FileSearch.Search(history, fileIndex, word);
            UpdatePart(result);
        }

        /// <summary>
        /// ディレクトリの中身をセットする
        /// </summary>
        /// <remarks>アクセス権がないなどの場合は呼び出し元で制御すること</remarks>
        internal void UpdateOfDirectory(string dirPath)
        {
            // ディレクトリの中にあるディレクトリの一覧取得
            string[] dirs = System.IO.Directory.GetDirectories(dirPath, "*", System.IO.SearchOption.TopDirectoryOnly);

            // ディレクトリの中にあるファイルの一覧取得
            FileList = dirs.Concat(System.IO.Directory.GetFiles(dirPath, "*", System.IO.SearchOption.TopDirectoryOnly)).ToArray();
        }
    }
}
