using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        // List では要素の更新が画面に反映されなかった
        private ObservableCollection<FileDisplaying> _fileList = new ObservableCollection<FileDisplaying>();

        // internal では画面に反映されなかったため public
        public ObservableCollection<FileDisplaying> FileList
        {
            get
            {
                return _fileList;
            }
            private set
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
        /// 次のファイルを選択状態にする
        /// すでに末尾なら何もしない
        /// </summary>
        internal void SelectNext()
        {
            int nowSelected = GetSelectedIndex();
            int newSelected = nowSelected + 1;

            ChangeSelectedIndex(nowSelected, newSelected);
        }

        /// <summary>
        /// 現在選択されている位置(インデックス)を返す
        /// </summary>
        internal int GetSelectedIndex()
        {
            for (int i = 0; i < _fileList.Count; i++)
            {
                if (_fileList[i].IsSelected)
                {
                    return i;
                }
            }

            return 0;
        }

        /// <summary>
        /// 選択されている位置(インデックス)を変更する
        /// </summary>
        private void ChangeSelectedIndex(int nowSelected, int newSelected)
        {
            if (newSelected < 0 || newSelected >= _fileList.Count)
            {
                return;
            }

            _fileList[nowSelected] = new FileDisplaying(_fileList[nowSelected].FilePath, false);
            _fileList[newSelected] = new FileDisplaying(_fileList[newSelected].FilePath, true);
        }

        /// <summary>
        /// 1 つ前のファイルを選択状態にする
        /// すでに先頭なら何もしない
        /// </summary>
        internal void SelectBack()
        {
            int nowSelected = GetSelectedIndex();
            int newSelected = nowSelected - 1;

            ChangeSelectedIndex(nowSelected, newSelected);
        }

        /// <summary>
        /// 現在選択されている item のファイルパスを返す
        /// </summary>
        internal string? GetSelectedFilePath()
        {
            // TODO: ? なしにする
            return _fileList[GetSelectedIndex()].FilePath;
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
            FileList = ToFileDisplaying(sliced, 0);
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
        /// string 配列を FileDisplay のコレクションに変換する
        /// </summary>
        private ObservableCollection<FileDisplaying> ToFileDisplaying(string[] files, int selectedIndex)
        {
            ObservableCollection<FileDisplaying> fileDisplayings = new ObservableCollection<FileDisplaying>();
            for (int i = 0; i < files.Count(); i++)
            {
                if (i == selectedIndex)
                {
                    fileDisplayings.Add(new FileDisplaying(files[i], true));
                }
                else
                {
                    fileDisplayings.Add(new FileDisplaying(files[i], false));
                }
            }

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
        /// 選択状態にしたいファイルがある場合、そのファイルを初期選択状態にする
        /// </summary>
        /// <remarks>アクセス権がないなどの場合は呼び出し元で制御すること</remarks>
        internal void UpdateOfDirectory(string searchText, string initSelectFilePath = "")
        {
            // 入力パスがちょうどディレクトリそのものの場合、このディレクトリの中身を全て表示する
            if (System.IO.Directory.Exists(searchText))
            {
                FileList = ToFileDisplaying(GetFilesAndDirectories(searchText), 0);
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
            string[] files = GetFilesAndDirectoriesStartsWith(dirPath!, start);

            int initSelectIndex = GetInitSelectIndex(files, initSelectFilePath);
            FileList = ToFileDisplaying(GetFilesAndDirectoriesStartsWith(dirPath!, start), initSelectIndex);
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

        private int GetInitSelectIndex(string[] files, string initSelectFilePath)
        {
            if (initSelectFilePath != "")
            {
                for (int i = 0; i < files.Count(); i++)
                {
                    if (files[i] == initSelectFilePath)
                    {
                        return i;
                    }
                }
            }

            return 0;
        }
    }
}
