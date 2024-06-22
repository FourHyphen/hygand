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

            // 初期モード設定
            InitAppMode();

            // ホットキー設定
            SetHotKey();

            // インデックス設定
            InitFileIndex();

            // 画面表示内容初期化
            _history = History.CreateInstance();
            ResetDisplay(_history);

            // 起動後すぐに検索を始められるよう SearchTextBox にフォーカスセット
            SearchText.Focus();
        }

        /// <summary>
        ///  モード初期化
        /// </summary>
        private void InitAppMode()
        {
            _appMode = AppMode.Index;
            Mode.Text = _appMode.ToString();
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
                // 画面を非表示にする
                HideWindow();
            }
            else
            {
                // 前回の画面表示をリセットしてから表示
                Show();
                ResetDisplay(_history);
            }
        }

        /// <summary>
        /// 画面を非表示にする
        /// </summary>
        internal void HideWindow()
        {
            // TODO: Hide 時のイベントがあればそこで処理する
            // キー確認ウィンドウが表示されていれば閉じる
            DestroyKeyCodeWindow();

            Hide();
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
        /// アプリの表示を初期化する
        /// </summary>
        private void ResetDisplay(History history)
        {
            // モード初期化
            InitAppMode();

            // テキストボックス初期化
            SearchText.Text = "";

            // ファイルリスト初期化
            InitDisplayFileList(history);
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
        /// MainWindow でのキーダウン時のイベント
        /// </summary>
        private void EventKeyDowned(object sender, KeyEventArgs e)
        {
            // 単押しの場合                  → キー情報は e.Key に入る
            // System キーとの同時押しの場合 → キー情報は e.SystemKey に入る
            AppKeys.KeyEvent keyEvent = AppKeys.ToKeyEvent(e.Key, e.SystemKey, e.KeyboardDevice.Modifiers);

            // 動作モード切り替え
            // TODO: command 化
            if (keyEvent == AppKeys.KeyEvent.ChangeAppMode)
            {
                ChangeAppMode();
                return;
            }

            // キー入力内容に見合った処理を実行
            MainWindowCommand command = MainWindowCommandFactory.Create(_appMode, keyEvent, this, _history);
            command.Execute();

            return;
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
