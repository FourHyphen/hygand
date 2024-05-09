using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        private void SetHotKey(AppHotKey appHotKey)
        {
            appHotKey.RegisterTogglingDisplayOnOff((_, __) => { ToggleDisplayOnOff(); });
        }

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
            _fileListDisplay.Update(result);
        }

        private void ToolKeyCheckClick(object sender, RoutedEventArgs e)
        {
            ShowKeyCodeWindow();
        }

        private void ShowKeyCodeWindow()
        {
            // 複合代入: null なら右辺を実行
            _keyCodeWindow ??= new KeyCodeWindow();
            _keyCodeWindow.Show();
        }

        protected override void OnClosed(EventArgs e)
        {
            DestroyKeyCodeWindow();
            _appHotKey.Dispose();
            base.OnClosed(e);
        }
    }
}
