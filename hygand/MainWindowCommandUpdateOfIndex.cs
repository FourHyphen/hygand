namespace hygand
{
    internal class MainWindowCommandUpdateOfIndex : MainWindowCommand
    {
        private MainWindow _mainWindow;
        private string _searchText;
        private History _history;
        private FileIndex _fileIndex;

        internal MainWindowCommandUpdateOfIndex(MainWindow mainWindow, string searchText, History history, FileIndex fileIndex)
        {
            _mainWindow = mainWindow;
            _searchText = searchText;
            _history = history;
            _fileIndex = fileIndex;
        }

        private protected override Result ExecuteCore()
        {
            _mainWindow.FileListDisplaying.UpdateOfIndex(_history, _fileIndex, _searchText);
            return Result.Success;
        }
    }
}