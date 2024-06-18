using System;
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
        private AppMode _appMode = AppMode.Index;
        private KeyCodeWindow? _keyCodeWindow;

        public MainWindow()
        {
            InitializeComponent();

            // データバインドに必要
            DataContext = this;

            // ホットキー設定
            SetHotKey();

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
        private void SetHotKey()
        {
            _appHotKey = new AppHotKey(ToggleDisplayOnOff);
        }

        /// <summary>
        /// 画面の表示非表示を切り替える
        /// </summary>
        private void ToggleDisplayOnOff(object? sender, ToggleDisplayOnOff.OriginalKeyEventArg e)
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
            FileListDisplaying.UpdateOfHistory(history);

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
        /// MainWindows でのキーダウン時のイベント
        /// </summary>
        private void EventKeyDowned(object sender, KeyEventArgs e)
        {
            // 単押しの場合                  → キー情報は e.Key に入る
            // System キーとの同時押しの場合 → キー情報は e.SystemKey に入る
            AppKeys.KeyEventOnAnyWhere keyEventOnAnyWhere = AppKeys.ToKeyEventOnAnyWhere(e.Key, e.SystemKey, e.KeyboardDevice.Modifiers);

            // フォーカスがどこに当たっていても動作モード切り替えは有効
            if (keyEventOnAnyWhere == AppKeys.KeyEventOnAnyWhere.ChangeAppMode)
            {
                ChangeAppMode();
                return;
            }

            // ファイルリスト内でのキー押下時
            if (IsFocusedDisplayFileList())
            {
                EventKeyDownedOnDisplayFileList(e);
                return;
            }

            // 検索テキストボックス内でのキー押下時
            if (SearchText.IsFocused)
            {
                EventKeyDownedOnSearchText(e);
                return;
            }
        }

        /// <summary>
        /// アプリ動作モードを切り替える
        /// </summary>
        private void ChangeAppMode()
        {
            if (_appMode == AppMode.Index)
            {
                _appMode = AppMode.Directory;
            }
            else
            {
                _appMode = AppMode.Index;
            }

            Mode.Text = _appMode.ToString();
        }

        /// <summary>
        /// DisplayFileList にフォーカスが当たっているかを返す
        /// </summary>
        private bool IsFocusedDisplayFileList()
        {
            // DisplayFileList そのものにフォーカスが当たっているか
            if (DisplayFileList.IsFocused)
            {
                return true;
            }

            // DisplayFileList の ListViewItem のいずれかにフォーカスが当たっているか
            for (int i = 0; i < DisplayFileList.Items.Count; i++)
            {
                var obj = DisplayFileList.ItemContainerGenerator.ContainerFromIndex(i);
                if (obj is ListViewItem target)
                {
                    if (target.IsFocused)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// フォーカスが DisplayFileList に当たっている場合のキー押下時処理
        /// </summary>
        private void EventKeyDownedOnDisplayFileList(KeyEventArgs e)
        {
            // キー入力内容に見合った処理を実行
            AppKeys.KeyEventOnFileList keyEventOnFileList = AppKeys.ToKeyEventOnFileList(e.Key, e.SystemKey, e.KeyboardDevice.Modifiers);
            MainWindowCommand command = MainWindowCommandFocusedFileListFactory.Create(_appMode, keyEventOnFileList, this, _history);
            command.Execute();

            return;
        }

        /// <summary>
        /// フォーカスが SearchText に当たっている場合のキー押下時処理
        /// </summary>
        private void EventKeyDownedOnSearchText(KeyEventArgs e)
        {
            // キー入力内容に見合った処理を実行
            AppKeys.KeyEventOnTextBox keyEventOnTextBox = AppKeys.ToKeyEventOnTextBox(e.Key, e.SystemKey, e.KeyboardDevice.Modifiers);
            MainWindowCommand command = MainWindowCommandFocusedSearchTextFactory.Create(_appMode, keyEventOnTextBox, this, e);
            command.Execute();

            return;
        }

        /// <summary>
        /// イベント: テキストボックス内の文字列で検索して結果を表示
        /// </summary>
        private void EventSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_appMode == AppMode.Index)
            {
                FileListDisplaying.UpdateOfIndex(_history, _fileIndex, SearchText.Text);
            }
            else if (_appMode == AppMode.Directory)
            {
                FileListDisplaying.UpdateOfDirectory(SearchText.Text);
            }
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
            _appHotKey?.Dispose();

            base.OnClosed(e);
        }
    }
}
