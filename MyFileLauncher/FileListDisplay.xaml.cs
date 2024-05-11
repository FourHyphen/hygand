using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace MyFileLauncher
{
    // INotifyPropertyChanged は変更を画面に反映するのに必要
    public partial class FileListDisplay : UserControl, INotifyPropertyChanged
    {
        private const int DisplayingNum = 20;

        private MainWindow _mainWindow;

        private History _history;

        // internal では画面に反映されなかったため public
        public string[] FileList { get; private set; } = new string[0];

        public event PropertyChangedEventHandler? PropertyChanged;

        internal FileListDisplay(MainWindow mainWindow, History history)
        {
            // DisplayFileList の初期化に必要
            InitializeComponent();

            // データバインドに必要
            DataContext = this;

            mainWindow.FileListArea.Children.Add(this);
            _mainWindow = mainWindow;

            // ファイルリストの初期値に履歴をセット
            _history = history;
            Update(_history.Files);
        }

        internal void Update(string[] files)
        {
            // 全件表示は時間がかかり過ぎるため一部を表示する
            FileList = Slice(files, DisplayingNum);
            NotifyPropertyChanged(nameof(FileList));
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

        private void NotifyPropertyChanged(string propertyName)
        {
            var e = new PropertyChangedEventArgs(propertyName);
            PropertyChanged?.Invoke(this, e);
        }

        /// <summary>
        /// キーダウンイベント
        /// </summary>
        private void KeyDowned(object sender, KeyEventArgs e)
        {
            // 単押しの場合                  → キー情報は e.Key に入る
            // System キーとの同時押しの場合 → キー情報は e.SystemKey に入る
            KeyDowned(e.Key, e.SystemKey, e.KeyboardDevice.Modifiers);
        }

        /// <summary>
        /// キーダウン時の処理
        /// </summary>
        private void KeyDowned(Key key, Key systemKey, ModifierKeys modifier)
        {
            AppKeys.KeyEventType keyEventType = AppKeys.ToKeyEventType(key, systemKey, modifier);
            if (keyEventType == AppKeys.KeyEventType.FileOpen)
            {
                DoKeyEventFileOpen();
            }
            else if (keyEventType == AppKeys.KeyEventType.BackDirectory)
            {
                DoKeyEventBackDirectory();
            }
        }

        /// <summary>
        /// キーイベントによるファイルオープンを実行
        /// </summary>
        private void DoKeyEventFileOpen()
        {
            ListViewItem? focused = GetListViewItemFocused();
            if (focused != null)
            {
                string filePath = (string)focused!.Content;
                OpenFile(filePath);

                // 履歴に追加
                _history.Add(filePath);

                // ファイルを開いたら用は済んだのでメインウィンドウを非表示化
                _mainWindow.Hide();
            }
        }

        /// <summary>
        /// フォーカスが当てられている ListViewItem を返す、何も当たっていなければ null を返す
        /// </summary>
        private ListViewItem? GetListViewItemFocused()
        {
            // 参考: https://threeshark3.com/binding-listbox-focus/
            for (int i = 0; i < DisplayFileList.Items.Count; i++)
            {
                var obj = DisplayFileList.ItemContainerGenerator.ContainerFromIndex(i);
                if (obj is ListViewItem target)
                {
                    if (target.IsFocused)
                    {
                        return target;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 登録されたプログラムでファイルを開く
        /// </summary>
        private void OpenFile(string filePath)
        {
            Type? type = Type.GetTypeFromProgID("Shell.Application");
            if (type == null)
            {
                return;
            }

            // 参照に Microsoft Shell Controls And Automation を追加することで Shell32 を参照できる
            Shell32.Shell? shell = (Shell32.Shell?)Activator.CreateInstance(type!);
            if (shell == null)
            {
                return;
            }

            shell!.Open(filePath);
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(shell!);
        }

        /// <summary>
        /// キーイベントによるディレクトリ階層を 1 つ戻る処理を実行
        /// </summary>
        private void DoKeyEventBackDirectory()
        {
            ListViewItem? focused = GetListViewItemFocused();
            if (focused == null)
            {
                return;
            }

            // ディレクトリパス取得
            string dirPath = GetBackDirectoryPath((string)focused!.Content, _mainWindow.SearchText.Text);

            // テキストボックスにディレクトリセット
            _mainWindow.SearchText.Text = dirPath;

            // 検索結果には当該ディレクトリ内のファイルをセット
            string[] files = System.IO.Directory.GetFiles(dirPath, "*", System.IO.SearchOption.TopDirectoryOnly);
            UpdateAll(files);
        }

        /// <summary>
        /// 1 階層戻る先のディレクトリパスを返す
        /// </summary>
        private string GetBackDirectoryPath(string focusedFilePath, string searchText)
        {
            // 今のテキストボックスがディレクトリのパスであればこれの 1 階層上を返す
            if (System.IO.Directory.Exists(searchText))
            {
                return System.IO.Path.GetDirectoryName(searchText)!;
            }

            // 選択されているファイルのディレクトリパスを返す
            return System.IO.Path.GetDirectoryName(focusedFilePath)!;
        }

        /// <summary>
        /// ファイルリスト全件を表示する
        /// </summary>
        private void UpdateAll(string[] filePathes)
        {
            FileList = filePathes;
            NotifyPropertyChanged(nameof(FileList));
        }
    }
}
