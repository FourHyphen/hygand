using System.Linq;

namespace MyFileLauncher
{
    internal class MainWindowCommandDownFileList : MainWindowCommand
    {
        private MainWindow _mainWindow;

        internal MainWindowCommandDownFileList(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        internal override void Execute()
        {
            _mainWindow.FileListDisplaying.IncrementSelectedIndex();
        }
    }
}