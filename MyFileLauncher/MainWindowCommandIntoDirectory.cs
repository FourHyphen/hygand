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
            // 現在選択されているファイルパスを取得
            string? selectedFilePath = _mainWindow.FileListDisplaying.GetSelectedFilePath();
            if (selectedFilePath == null)
            {
                return;
            }

            // 選択されているパスがディレクトリでない場合は入れないのでここで終了
            if (!System.IO.Directory.Exists(selectedFilePath))
            {
                return;
            }

            // 更新
            UpdateOfDirectoryInfo(_mainWindow, selectedFilePath);
        }
    }
}