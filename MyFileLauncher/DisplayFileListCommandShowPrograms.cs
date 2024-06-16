using System;
using System.Windows.Controls;

namespace MyFileLauncher
{
    /// <summary>
    /// ファイルに紐づくプログラムの一覧を表示する
    /// </summary>
    internal class DisplayFileListCommandShowPrograms : DisplayFileListCommand
    {
        private MainWindow _mainWindow;
        private History _history;

        internal DisplayFileListCommandShowPrograms(MainWindow mainWindow, History history)
        {
            _mainWindow = mainWindow;
            _history = history;
        }

        internal override void Execute()
        {
            // フォーカスされているファイルパスを取得
            string? focusedFilePath = GetListViewItemStringFocused(_mainWindow);
            if (focusedFilePath == null)
            {
                return;
            }

            // コンテキストメニュー一覧ウィンドウを表示
            FileContextMenuWindow window = new FileContextMenuWindow(focusedFilePath);
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

            if (!(sender is FileContextMenuWindow))
            {
                // 操作したファイルパス不明のため検索テキストボックスにフォーカス移動
                // ここは基本的に通らない想定
                SetFocusTextBox(_mainWindow.SearchText);
                return;
            }

            // ウィンドウ閉じただけだとフォーカスは宙ぶらりんになったので明示的に指定
            FileContextMenuWindow fcmw = (FileContextMenuWindow)sender;
            SetFocusListViewItem(_mainWindow, fcmw.FilePath);

            // コンテキストを実行した場合、用は済んだので履歴に追加してメイン画面を非表示化
            if (fcmw.DidExecuteContext)
            {
                _history.Add(fcmw.FilePath);
                _mainWindow.Hide();
            }
        }
    }
}