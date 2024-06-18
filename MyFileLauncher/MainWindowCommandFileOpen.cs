namespace MyFileLauncher
{
    /// <summary>
    /// 既定のプログラムでファイルを開く
    /// </summary>
    internal class MainWindowCommandFileOpen : MainWindowCommand
    {
        private MainWindow _mainWindow;
        private History _history;

        internal MainWindowCommandFileOpen(MainWindow mainWindow, History history)
        {
            _mainWindow = mainWindow;
            _history = history;
        }

        internal override void Execute()
        {
            // フォーカスの当たっているファイルパスを取得
            string? focusedFilePath = GetListViewItemStringFocused(_mainWindow);
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
    }
}