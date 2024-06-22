namespace MyFileLauncher
{
    /// <summary>
    /// ディレクトリに入る
    /// </summary>
    internal class MainWindowCommandIntoDirectory : MainWindowCommand
    {
        private MainWindow _mainWindow;

        internal MainWindowCommandIntoDirectory(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        internal override void Execute()
        {
            // フォーカスされているファイルパスを取得
            string? focusedFilePath = GetListViewItemStringFocused(_mainWindow);
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
            UpdateOfDirectoryInfo(_mainWindow, focusedFilePath);
        }
    }
}