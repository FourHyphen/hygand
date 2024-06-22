using System;
using System.Windows.Controls;

namespace MyFileLauncher
{
    /// <summary>
    /// ファイルに紐づくプログラムの一覧を表示する
    /// </summary>
    internal class MainWindowCommandShowPrograms : MainWindowCommand
    {
        private MainWindow _mainWindow;
        private History _history;

        internal MainWindowCommandShowPrograms(MainWindow mainWindow, History history)
        {
            _mainWindow = mainWindow;
            _history = history;
        }

        internal override void Execute()
        {
            // 選択されているファイルパスを取得
            string? selectedFilePath = GetDisplayingFileListSelected(_mainWindow);
            if (selectedFilePath == null)
            {
                return;
            }

            // コンテキストメニュー一覧ウィンドウを表示
            FileContextMenuWindow window = new FileContextMenuWindow(selectedFilePath);
            window.Owner = _mainWindow;
            window.Show();

            // コンテキストメニュー一覧ウィンドウが閉じるまでメイン画面を無効化
            _mainWindow.IsEnabled = false;    // FileContextMenuWindow に最初からキーボードフォーカスを当てるための無効化
            window.Closed += FileContextMenuWindowClosed;
        }

        /// <summary>
        /// コンテキストメニュー一覧ウィンドウクローズ時のイベント
        /// </summary>
        private void FileContextMenuWindowClosed(object? sender, EventArgs e)
        {
            // 無効化からの復帰
            _mainWindow.IsEnabled = true;

            // 検索テキストボックスにフォーカス移動
            SetFocusTextBox(_mainWindow.SearchText);

            if (!(sender is FileContextMenuWindow))
            {
                // 操作したファイルパス不明、ここは基本的に通らない想定
                return;
            }

            // コンテキストを実行した場合、用は済んだので履歴に追加してメイン画面を非表示化
            FileContextMenuWindow fcmw = (FileContextMenuWindow)sender;
            if (fcmw.DidExecuteContext)
            {
                _history.Add(fcmw.FilePath);
                _mainWindow.HideWindow();
            }
        }

        /// <summary>
        /// テキストボックスにフォーカスを当てる
        /// </summary>
        private void SetFocusTextBox(TextBox textBox)
        {
            // Dispatcher 使わないと DisplayFileList ->  TextBox 方向にフォーカス移動できなかった
            textBox.Dispatcher.BeginInvoke(new System.Action(() =>
            {
                textBox.Focus();
                System.Windows.Input.Keyboard.Focus(textBox);
            }), null);
        }
    }
}