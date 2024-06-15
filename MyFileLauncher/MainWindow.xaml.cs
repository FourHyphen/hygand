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
            FileListDisplaying.UpdateOfIndex(_history, _fileIndex, SearchText.Text);
        }

        /// <summary>
        /// イベント：DisplayFileList のキーダウン時
        /// </summary>
        private void EventDisplayFileListKeyDowned(object sender, KeyEventArgs e)
        {
            // 単押しの場合                  → キー情報は e.Key に入る
            // System キーとの同時押しの場合 → キー情報は e.SystemKey に入る
            Key key = e.Key;
            Key systemKey = e.SystemKey;
            ModifierKeys modifier = e.KeyboardDevice.Modifiers;

            new MainWindowProcessKeyDowned(this, _history).Execute(key, systemKey, modifier);
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
