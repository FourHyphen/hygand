using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyFileLauncher
{
    // INotifyPropertyChanged は変更を画面に反映するのに必要
    public partial class FileListDisplay : UserControl, INotifyPropertyChanged
    {
        private const int DisplayingNum = 20;

        private const int DefaultDisplayFileListHeight = 200;

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
            UpdatePart(_history.Files);
        }

        /// <summary>
        /// ファイルリスト表示を更新する(全件表示は時間がかかり過ぎるため一部を表示する)
        /// </summary>
        internal void UpdatePart(string[] files)
        {
            Update(Slice(files, DisplayingNum));
        }

        /// <summary>
        /// ファイルリスト表示を更新する
        /// </summary>
        private void Update(string[] files)
        {
            FileList = files;

            // スクロールバー表示のため DisplayFileList の高さ設定
            SetMaxHeightOfDisplayFileList();

            NotifyPropertyChanged(nameof(FileList));
        }

        /// <summary>
        /// DisplayFileList の最大高を設定
        /// </summary>
        private void SetMaxHeightOfDisplayFileList()
        {
            // ファイルリストの高さ最大値は MainWindow 次第、Load 時など特定不可の場合に対応して初期値設定
            DisplayFileList.MaxHeight = DefaultDisplayFileListHeight;
            if (_mainWindow.FileListArea.ActualHeight != double.NaN && _mainWindow.FileListArea.ActualHeight > 0.0)
            {
                DisplayFileList.MaxHeight = (double)_mainWindow.FileListArea.ActualHeight;
            }
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
            else if (keyEventType == AppKeys.KeyEventType.IntoDirectory)
            {
                DoKeyEventIntoDirectory();
            }
            else if (keyEventType == AppKeys.KeyEventType.FocusOnSearchTextBox)
            {
                DoKeyEventFocusOnSearchTextBox();
            }
        }

        /// <summary>
        /// キーイベントによるファイルオープンを実行
        /// </summary>
        private void DoKeyEventFileOpen()
        {
            // フォーカスの当たっているファイルパスを取得
            string? focusedFilePath = GetListViewItemStringFocused();
            if (focusedFilePath == null)
            {
                return;
            }

            // ファイルを開く
            OpenFile(focusedFilePath);

            // 履歴に追加
            _history.Add(focusedFilePath);

            // ファイルを開いたら用は済んだのでメインウィンドウを非表示化
            _mainWindow.Hide();
        }

        /// <summary>
        /// フォーカスが当てられている ListViewItem の Content を返す、何も当たっていなければ null を返す
        /// </summary>
        private string? GetListViewItemStringFocused()
        {
            ListViewItem? focused = GetListViewItemFocused();
            if (focused == null)
            {
                return null;
            }

            return (string)focused.Content;
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
            // フォーカスされているファイルパスを取得
            string? focusedFilePath = GetListViewItemStringFocused();
            if (focusedFilePath == null)
            {
                return;
            }

            // 戻り先ディレクトリパス取得
            string? dirPath = GetBackDirectoryPath(focusedFilePath, _mainWindow.SearchText.Text);
            if (dirPath == null)
            {
                return;
            }

            // 更新
            UpdateOfDirectoryInfo(dirPath);
        }

        /// <summary>
        /// ディレクトリの情報で画面を更新
        /// </summary>
        private void UpdateOfDirectoryInfo(string dirPath)
        {
            // テキストボックスにディレクトリセット
            _mainWindow.SearchText.Text = dirPath;

            // 検索結果には当該ディレクトリ内のファイルをセット
            string[] dirs = System.IO.Directory.GetDirectories(dirPath, "*", System.IO.SearchOption.TopDirectoryOnly);
            string[] files = dirs.Concat(System.IO.Directory.GetFiles(dirPath, "*", System.IO.SearchOption.TopDirectoryOnly)).ToArray();
            Update(files);
        }

        /// <summary>
        /// 1 階層戻る先のディレクトリパスを返す
        /// </summary>
        private string? GetBackDirectoryPath(string focusedFilePath, string searchText)
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
        /// キーイベントによるディレクトリに入る処理を実行
        /// </summary>
        private void DoKeyEventIntoDirectory()
        {
            // フォーカスされているファイルパスを取得
            string? focusedFilePath = GetListViewItemStringFocused();
            if (focusedFilePath == null)
            {
                return;
            }

            // フォーカスされているパスがディレクトリでない場合は入れないのでここで終了
            if (!System.IO.Directory.Exists(focusedFilePath))
            {
                return;
            }

            // 更新
            UpdateOfDirectoryInfo(focusedFilePath);
        }

        /// <summary>
        /// キーイベントによるフォーカスを検索テキストボックスに移動する処理を実行
        /// </summary>
        private void DoKeyEventFocusOnSearchTextBox()
        {
            // フォーカスされている場所がファイルリストの先頭(一番上)でなければ移動しない
            ListViewItem? focused = GetListViewItemFocused();
            if (focused == null)
            {
                return;
            }

            string? focusedStr = GetListViewItemStringFocused();
            if (focusedStr != FileList[0])
            {
                return;
            }

            // フォーカス移動
            _mainWindow.SearchText.Focus();
        }

        /// <summary>
        /// ファイルリストの先頭にフォーカスを当てる
        /// </summary>
        internal void SetFocusDisplayFileListFirst()
        {
            if (FileList.Count() == 0)
            {
                return;
            }

            var obj = DisplayFileList.ItemContainerGenerator.ContainerFromIndex(0);
            if (obj is ListViewItem target)
            {
                target.Focus();
            }
        }
    }
}
