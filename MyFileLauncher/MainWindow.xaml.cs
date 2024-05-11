using System;
using System.Windows;
using System.Windows.Controls;

namespace MyFileLauncher
{
    public partial class MainWindow : Window
    {
        private AppHotKey _appHotKey;

        private History _history;

        private FileIndex _fileIndex;

        private FileListDisplay _fileListDisplay;

        private KeyCodeWindow? _keyCodeWindow;

        public MainWindow()
        {
            InitializeComponent();

            _appHotKey = new AppHotKey(this);
            SetHotKey(_appHotKey);

            InitFileIndex();

            // ファイルリストの初期値に履歴をセット
            _history = History.CreateInstance();
            _fileListDisplay = new FileListDisplay(this, _history);

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
        /// テキストボックス内の文字列でインデックスを検索して結果を表示
        /// </summary>
        private void EventSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            SearchAndUpdateFileList(SearchText.Text);
        }

        /// <summary>
        /// 検索した結果でファイルリストを更新する
        /// </summary>
        private void SearchAndUpdateFileList(string word)
        {
            string[] result = MyFileLauncher.FileSearch.Search(_history, _fileIndex, word);
            _fileListDisplay.UpdatePart(result);
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
