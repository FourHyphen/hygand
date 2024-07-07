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
        internal string GetSelectedFilePath()
        {
            if (_fileList.Count == 0)
            {
                return "";
            }

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
            UpdateFileList(sliced);
        }

        /// <summary>
        /// Slice(list.Count >= 20, 20) -> return list.ToArray()[0..20]
        /// Slice(list.Count <  20, 20) -> return list.ToArray()
        /// </summary>
        private string[] Slice(string[] array, int end)
        {
            if (array.Length < end)
            {
                return array;
            }

            return array[0..end].ToArray();
        }

        /// <summary>
        /// FileList を更新する
        /// </summary>
        /// <param name="initSelectFilePath">初期状態で選択しておきたいファイルのパス</param>
        private void UpdateFileList(string[] files, string initSelectFilePath = "")
        {
            int initSelectIndex = GetInitSelectIndex(files, initSelectFilePath);
            FileList = ToFileDisplaying(files, initSelectIndex);
        }

        /// <summary>
        /// 初期状態で選択しておきたいファイルパスのインデックスを返す
        /// ファイルパスがファイル一覧に存在しなかった場合は 0 を返す
        /// </summary>
        private int GetInitSelectIndex(string[] files, string initSelectFilePath)
        {
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i] == initSelectFilePath)
                {
                    return i;
                }
            }

            return 0;
        }

        /// <summary>
        /// string 配列を FileDisplay のコレクションに変換する
        /// </summary>
        private ObservableCollection<FileDisplaying> ToFileDisplaying(string[] files, int selectedIndex)
        {
            ObservableCollection<FileDisplaying> fileDisplayings = new ObservableCollection<FileDisplaying>();
            for (int i = 0; i < files.Length; i++)
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
            // ユーザーがドライブのパス入力中などでディレクトリパス取得不可の場合は何もしない
            string? dirPath = System.IO.Path.GetDirectoryName(searchText);
            if (dirPath == null || dirPath == String.Empty || !System.IO.Directory.Exists(dirPath))
            {
                // 入力パスがちょうどディレクトリそのものの場合は処理する(ドライブ直下を想定)
                if (!System.IO.Directory.Exists(searchText))
                {
                    return;
                }
            }

            string start = "";
            if (System.IO.Directory.Exists(searchText))
            {
                // 入力パスがちょうどディレクトリそのものの場合、このディレクトリの中身を全て表示する
                dirPath = searchText;
            }
            else
            {
                // 入力パスがディレクトリ＋ファイル・ディレクトリ名の一部の場合
                //  -> 存在するディレクトリの中にある、"一部" に前方一致するファイル・ディレクトリを表示する
                start = System.IO.Path.GetFileName(searchText);
            }

            // TODO: 例外時の打ち上げ方法の再検討
            string[]? files = GetFilesAndDirectories(dirPath!, start);
            UpdateFileList(files!, initSelectFilePath);
        }

        /// <summary>
        /// 存在するディレクトリの中にある、ファイル・ディレクトリをフルパス形式で返す
        /// </summary>
        /// <param name="start">ディレクトリの中の前方一致するファイル・ディレクトリを返す場合はこれを指定してください</param>
        private string[] GetFilesAndDirectories(string dirPath, string start = "")
        {
            // アクセス権なし例外(System.Unauthorized)制御の簡略化のため start = "" を使用して関数を 1 つに集約した
            // ディレクトリの中にあるディレクトリの一覧取得
            string[] dirs = System.IO.Directory.GetDirectories(dirPath, "*", System.IO.SearchOption.TopDirectoryOnly);

            // ディレクトリの中にあるファイルの一覧取得
            string[] files = dirs.Concat(System.IO.Directory.GetFiles(dirPath, "*", System.IO.SearchOption.TopDirectoryOnly)).ToArray();

            if (start == "")
            {
                return files;
            }

            return files.Where(path => System.IO.Path.GetFileName(path).StartsWith(start)).ToArray();
        }
    }
}
