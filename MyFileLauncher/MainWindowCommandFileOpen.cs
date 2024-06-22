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
            // 現在選択されているファイルパスを取得
            string? selectedFilePath = GetDisplayingFileListSelected(_mainWindow);
            if (selectedFilePath == null)
            {
                return;
            }

            // ファイルを開く
            OpenFile(selectedFilePath);

            // 履歴に追加
            _history.Add(selectedFilePath);

            // ファイルを開いたら用は済んだのでメインウィンドウを非表示化
            _mainWindow.HideWindow();
        }
    }
}