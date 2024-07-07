namespace MyFileLauncher
{
    internal class MainWindowCommandUpdateOfDirectory : MainWindowCommand
    {
        private MainWindow _mainWindow;
        private readonly string _searchText;

        public MainWindowCommandUpdateOfDirectory(MainWindow mainWindow, string searchText)
        {
            _mainWindow = mainWindow;
            _searchText = searchText;
        }

        private protected override Result ExecuteCore()
        {
            try
            {
                _mainWindow.FileListDisplaying.UpdateOfDirectory(_searchText);
            }
            catch (System.UnauthorizedAccessException)
            {
                // アクセス権がなかった場合
                return Result.FailedUnauthorizedAccess;
            }

            return Result.Success;
        }
    }
}