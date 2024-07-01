namespace MyFileLauncher
{
    internal class MainWindowCommandUpdateOfDirectory : MainWindowCommand
    {
        private MainWindow _mainWindow;
        private string _searchText;

        public MainWindowCommandUpdateOfDirectory(MainWindow mainWindow, string searchText)
        {
            _mainWindow = mainWindow;
            _searchText = searchText;
        }

        internal override Result Execute()
        {
            // TODO: FileListDisplaying.UpdateOfDirectory() のアクセス権なし例外制御実装、Result.Unauthorized を返す
            _mainWindow.FileListDisplaying.UpdateOfDirectory(_searchText);

            return Result.Success;
        }
    }
}