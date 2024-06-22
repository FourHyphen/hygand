namespace MyFileLauncher
{
    internal class MainWindowCommandUpFileList : MainWindowCommand
    {
        private MainWindow _mainWindow;

        internal MainWindowCommandUpFileList(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        internal override void Execute()
        {
            _mainWindow.FileListDisplaying.DecrementSelectedIndex();
        }
    }
}