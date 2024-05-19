using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyFileLauncher
{
    public partial class MainWindow : Window
    {
        private const int BufferDisplayFileListWidth = 20;

        private const int DefaultDisplayFileListScrollViewerHeight = 200;

        // internal では画面に反映されなかったため public
        public FileListDisplaying FileListDisplaying { get; } = new FileListDisplaying();

        private AppHotKey _appHotKey;

        private History _history;

        private FileIndex _fileIndex;

        private KeyCodeWindow? _keyCodeWindow;

        public MainWindow()
        {
            InitializeComponent();

            // データバインドに必要
            DataContext = this;

            // ホットキー設定
            _appHotKey = new AppHotKey(this);
            SetHotKey(_appHotKey);

            // インデックス設定
            InitFileIndex();

            // ファイルリスト初期化
            _history = History.CreateInstance();
            InitDisplayFileList(_history);

            // 起動後すぐに検索を始められるよう SearchTextBox にフォーカスセット
            SearchText.Focus();
        }

        /// <summary>
        /// アプリケーションに使用するホットキーを設定
        /// </summary>
        private void SetHotKey(AppHotKey appHotKey)
        {
            appHotKey.RegisterTogglingDisplayOnOff((_, __) => { ToggleDisplayOnOff(); });
        }

        /// <summary>
        /// 画面の表示非表示を切り替える
        /// </summary>
        private void ToggleDisplayOnOff()
        {
            if (Visibility == Visibility.Visible)
            {
                Hide();

                // キー確認ウィンドウが表示されていれば閉じる
                DestroyKeyCodeWindow();
            }
            else
            {
                Show();
            }
        }

        /// <summary>
        /// キー確認ウィンドウを破棄する
        /// </summary>
        private void DestroyKeyCodeWindow()
        {
            if (_keyCodeWindow == null)
            {
                return;
            }

            if (_keyCodeWindow.ShowActivated)
            {
                _keyCodeWindow.Close();
            }

            _keyCodeWindow = null;
        }

        /// <summary>
        /// FileIndex インスタンスを初期化する
        /// </summary>
        private void InitFileIndex()
        {
            // インデックス読んでメモリに展開
            _fileIndex = FileIndex.CreateInstance();
            if (_fileIndex.Exists())
            {
                return;
            }

            // インデックスが存在しないなら作成して良いかを確認
            if (DoUserWantToCreateIndex())
            {
                ScanInfo si = ScanInfo.CreateInstance();
                _fileIndex.CreateIndexFile(si);
            }
        }

        /// <summary>
        /// インデックス作成するかをユーザーに確認する
        /// </summary>
        private bool DoUserWantToCreateIndex()
        {
            MessageBoxResult mbr = MessageBox.Show("インデックスを作成しますか？", "Index.info does not exist.", MessageBoxButton.YesNo);
            return (mbr == MessageBoxResult.Yes);
        }

        /// <summary>
        /// DisplayFileList 初期設定
        /// </summary>
        private void InitDisplayFileList(History history)
        {
            // ファイルリストの初期値に履歴をセット
            FileListDisplaying.Update(history);

            // スクロールバー表示のための ScrollViewer 横幅設定
            DisplayFileListScrollViewer.MaxWidth = this.Width - BufferDisplayFileListWidth;

            // スクロールバー表示のための ScrollViewer 高さ設定
            // ファイルリストの高さ最大値は MainWindow 次第、Load 時など特定不可の場合は初期値設定
            if (FileListArea.ActualHeight != double.NaN && FileListArea.ActualHeight > 0.0)
            {
                DisplayFileListScrollViewer.MaxHeight = (double)FileListArea.ActualHeight;
            }
            else
            {
                DisplayFileListScrollViewer.MaxHeight = DefaultDisplayFileListScrollViewerHeight;
            }
        }

        /// <summary>
        /// イベント: テキストボックス内のキー押下時
        /// </summary>
        private void EventSearchTextKeyDowned(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // 単押しの場合                  → キー情報は e.Key に入る
            // System キーとの同時押しの場合 → キー情報は e.SystemKey に入る
            AppKeys.KeyEventType ket = AppKeys.ToKeyEventType(e.Key, e.SystemKey, e.KeyboardDevice.Modifiers);
            if (ket == AppKeys.KeyEventType.FocusOnFileList)
            {
                MoveFocusOnFileList();

                // 処理済みにしないとフォーカス移動後に下キー押下時の既定処理が走ってしまう
                e.Handled = true;
            }
        }

        /// <summary>
        /// フォーカスをファイルリストに移動する
        /// </summary>
        private void MoveFocusOnFileList()
        {
            // _fileListDisplay.DisplayFileList.Focus() ではファイルリストの末尾などにフォーカス移動した
            if (FileListDisplaying.FileList.Count() == 0)
            {
                return;
            }

            var obj = DisplayFileList.ItemContainerGenerator.ContainerFromIndex(0);
            if (obj is ListViewItem target)
            {
                target.Focus();
            }
        }

        /// <summary>
        /// イベント: テキストボックス内の文字列でインデックスを検索して結果を表示
        /// </summary>
        private void EventSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            FileListDisplaying.Update(_history, _fileIndex, SearchText.Text);
        }

        /// <summary>
        /// イベント：DisplayFileList のキーダウン時
        /// </summary>
        private void EventDisplayFileListKeyDowned(object sender, KeyEventArgs e)
        {
            // 単押しの場合                  → キー情報は e.Key に入る
            // System キーとの同時押しの場合 → キー情報は e.SystemKey に入る
            ProcessDisplayFileListEvent(e.Key, e.SystemKey, e.KeyboardDevice.Modifiers);
        }

        /// <summary>
        /// DisplayFileList 押下キーに応じた処理を実行する
        /// </summary>
        private void ProcessDisplayFileListEvent(Key key, Key systemKey, ModifierKeys modifier)
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
            Hide();
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
            string? dirPath = GetBackDirectoryPath(focusedFilePath, SearchText.Text);
            if (dirPath == null)
            {
                return;
            }

            // 更新
            UpdateOfDirectoryInfo(dirPath);
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
        /// ディレクトリの情報で画面を更新
        /// </summary>
        private void UpdateOfDirectoryInfo(string dirPath)
        {
            // テキストボックスにディレクトリセット
            SearchText.Text = dirPath;

            // 検索結果には当該ディレクトリ内のファイルをセット
            FileListDisplaying.Update(dirPath);

            // ファイルパスが長い時に真に見たいのはファイル / ディレクトリ名のため横スクロールを右端に設定
            DisplayFileListScrollViewer.ScrollToRightEnd();
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
            if (focusedStr != FileListDisplaying.FileList[0])
            {
                return;
            }

            // フォーカス移動
            SearchText.Focus();
        }

        /// <summary>
        /// キー確認ウィンドウ表示選択時イベント
        /// </summary>
        private void ToolKeyCheckClick(object sender, RoutedEventArgs e)
        {
            ShowKeyCodeWindow();
        }

        /// <summary>
        /// キー確認ウィンドウを表示
        /// </summary>
        private void ShowKeyCodeWindow()
        {
            // 複合代入: null なら右辺を実行
            _keyCodeWindow ??= new KeyCodeWindow();
            _keyCodeWindow.Show();
        }

        /// <summary>
        /// 画面クローズ時イベント
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            DestroyKeyCodeWindow();
            _appHotKey.Dispose();
            base.OnClosed(e);
        }
    }
}
