namespace MyFileLauncher
{
    /// <summary>
    /// ディレクトリ階層を 1 つ戻る
    /// </summary>
    internal class MainWindowCommandBackDirectory : MainWindowCommand
    {
        private MainWindow _mainWindow;

        internal MainWindowCommandBackDirectory(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        internal override void Execute()
        {
            // 今選択されているファイルパスを取得
            string? selectedFilePath = _mainWindow.FileListDisplaying.GetSelectedFilePath();
            if (selectedFilePath == null)
            {
                return;
            }

            // 戻り先ディレクトリパス取得
            string? dirPath = GetBackDirectoryPath(selectedFilePath, _mainWindow.SearchText.Text);
            if (dirPath == null)
            {
                return;
            }

            // 戻り先の選択状態を再現するための、今表示されているディレクトリパス取得
            string willSelectDirPath = GetNowDisplayingDirPath(selectedFilePath);

            // 更新(移動元の選択状態を再現)
            UpdateOfDirectoryInfo(_mainWindow, dirPath, willSelectDirPath);
        }

        /// <summary>
        /// 1 階層戻る先のディレクトリパスを返す
        /// </summary>
        private string? GetBackDirectoryPath(string selectedFilePath, string searchText)
        {
            // 今のテキストボックスがディレクトリのパスであればこれの 1 階層上を返す
            if (System.IO.Directory.Exists(searchText))
            {
                return System.IO.Path.GetDirectoryName(searchText)!;
            }

            // 選択されているファイルのディレクトリパスを返す
            return System.IO.Path.GetDirectoryName(selectedFilePath)!;
        }

        /// <summary>
        /// 今表示しているディレクトリのパスを返す
        /// </summary>
        private string GetNowDisplayingDirPath(string selectedFilePath)
        {
            // 今選択されているファイルパスの 1 階層上が、今表示しているディレクトリのパス
            return System.IO.Path.GetDirectoryName(selectedFilePath);
        }
    }
}