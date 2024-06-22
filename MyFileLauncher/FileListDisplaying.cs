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

        private List<FileDisplaying> _fileList = new List<FileDisplaying>();

        // internal では画面に反映されなかったため public
        public List<FileDisplaying> FileList
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
        /// SelectedIndex を 1 増やす
        /// すでにインデックス最大値なら何もしない
        /// </summary>
        internal void IncrementSelectedIndex()
        {
            int nowSelected = _fileList.FindIndex(0, file => file.IsSelected);
            int newSelected = nowSelected + 1;

            ChangeSelectedIndex(nowSelected, newSelected);
        }

        private void ChangeSelectedIndex(int nowSelected, int newSelected)
        {
            if (newSelected < 0 || newSelected >= _fileList.Count)
            {
                return;
            }

            _fileList[nowSelected].IsSelected = false;
            _fileList[newSelected].IsSelected = true;
        }

        /// <summary>
        /// SelectedIndex を 1 減らす
        /// すでにインデックス最大値なら何もしない
        /// </summary>
        internal void DecrementSelectedIndex()
        {
            int nowSelected = _fileList.FindIndex(0, file => file.IsSelected);
            int newSelected = nowSelected - 1;

            ChangeSelectedIndex(nowSelected, newSelected);
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
            string[] sliced = Slice(files, DisplayingNum);
            FileList = ToFileListDisplaying(sliced, 0);
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

        private List<FileDisplaying> ToFileListDisplaying(string[] files, int selectedIndex)
        {
            List<FileDisplaying> fileDisplayings = files.Select(file => new FileDisplaying(file)).ToList();
            fileDisplayings[selectedIndex].IsSelected = true;

            return fileDisplayings;
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
        internal void UpdateOfDirectory(string searchText)
        {
            // 入力パスがちょうどディレクトリそのものの場合、このディレクトリの中身を全て表示する
            if (System.IO.Directory.Exists(searchText))
            {
                FileList = ToFileListDisplaying(GetFilesAndDirectories(searchText), 0);
                return;
            }

            // ユーザーがドライブのパス入力中などでディレクトリパス取得不可の場合は何もしない
            string? dirPath = System.IO.Path.GetDirectoryName(searchText);
            if (dirPath == null || dirPath == String.Empty || !System.IO.Directory.Exists(dirPath))
            {
                return;
            }

            // 入力パスがディレクトリ＋ファイル・ディレクトリ名の一部の場合
            // 存在するディレクトリの中にある、"一部" に前方一致するファイル・ディレクトリを表示する
            string start = System.IO.Path.GetFileName(searchText);
            FileList = ToFileListDisplaying(GetFilesAndDirectoriesStartsWith(dirPath!, start), 0);
        }

        /// <summary>
        /// 存在するディレクトリの中にある、全てのファイル・ディレクトリをフルパス形式で返す
        /// </summary>
        private string[] GetFilesAndDirectories(string dirPath)
        {
            // ディレクトリの中にあるディレクトリの一覧取得
            string[] dirs = System.IO.Directory.GetDirectories(dirPath, "*", System.IO.SearchOption.TopDirectoryOnly);

            // ディレクトリの中にあるファイルの一覧取得
            return dirs.Concat(System.IO.Directory.GetFiles(dirPath, "*", System.IO.SearchOption.TopDirectoryOnly)).ToArray();
        }

        /// <summary>
        /// 存在するディレクトリの中にある、前方一致するファイル・ディレクトリをフルパス形式で返す
        /// </summary>
        private string[] GetFilesAndDirectoriesStartsWith(string dirPath, string start)
        {
            string[] files = GetFilesAndDirectories(dirPath);

            return files.Where(path => System.IO.Path.GetFileName(path).StartsWith(start)).ToArray();
        }
    }
}
