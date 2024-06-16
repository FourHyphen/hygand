namespace MyFileLauncher
{
    /// <summary>
    /// ディレクトリ階層を 1 つ戻る
    /// </summary>
    internal class DisplayFileListCommandBackDirectory : DisplayFileListCommand
    {
        private MainWindow _mainWindow;

        internal DisplayFileListCommandBackDirectory(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        internal override void Execute()
        {
            // 今フォーカスされているファイルパスを取得
            string? focusedFilePath = GetListViewItemStringFocused(_mainWindow);
            if (focusedFilePath == null)
            {
                return;
            }

            // 戻り先ディレクトリパス取得
            string? dirPath = GetBackDirectoryPath(focusedFilePath, _mainWindow.SearchText.Text);
            if (dirPath == null)
            {
                return;
            }

            // 戻り先でフォーカスを当てるための、今表示されているディレクトリパス取得
            string willFocusDirPath = GetNowDisplayingDirPath(focusedFilePath);

            // 更新
            UpdateOfDirectoryInfo(_mainWindow, dirPath);

            // 移動元にフォーカスを当て、移動前の状態に戻す
            SetFocusListViewItem(_mainWindow, willFocusDirPath);
        }

        /// <summary>
        /// 1 階層戻る先のディレクトリパスを返す
        /// </summary>
        private string? GetBackDirectoryPath(string focusedFilePath, string searchText)
        {
            // 今のテキストボックスがディレクトリのパスであればこれの 1 階層上を返す
            if (System.IO.Directory.Exists(searchText))
            {
                return System.IO.Path.GetDirectoryName(searchText)!;
            }

            // 選択されているファイルのディレクトリパスを返す
            return System.IO.Path.GetDirectoryName(focusedFilePath)!;
        }

        /// <summary>
        /// 今表示しているディレクトリのパスを返す
        /// </summary>
        private string GetNowDisplayingDirPath(string focusedFilePath)
        {
            // 今フォーカスが当たっているファイルパスの 1 階層上が、今表示しているディレクトリのパス
            return System.IO.Path.GetDirectoryName(focusedFilePath);
        }
    }
}