namespace MyFileLauncher
{
    internal static class MainWindowCommandFactory
    {
        internal static MainWindowCommand Create(AppMode appMode, AppKeys.KeyEvent keyEvent, MainWindow mainWindow, History history)
        {
            if (keyEvent == AppKeys.KeyEvent.DownFileList)
            {
                return new MainWindowCommandDownFileList(mainWindow);
            }
            else if (keyEvent == AppKeys.KeyEvent.UpFileList)
            {
                return new MainWindowCommandUpFileList(mainWindow);
            }

            if (appMode == AppMode.Index)
            {
                return CreateOfIndexMode(keyEvent, mainWindow, history);
            }
            else if (appMode == AppMode.Directory)
            {
                return CreateOfDirectoryMode(keyEvent, mainWindow, history);
            }

            return new MainWindowCommandEmpty();
        }

        /// <summary>
        /// インデックスモードの場合の Command を返す
        /// </summary>
        private static MainWindowCommand CreateOfIndexMode(AppKeys.KeyEvent keyEvent, MainWindow mainWindow, History history)
        {
            return keyEvent switch
            {
                AppKeys.KeyEvent.FileOpen     => new MainWindowCommandFileOpen(mainWindow, history),
                AppKeys.KeyEvent.ShowPrograms => new MainWindowCommandShowPrograms(mainWindow, history),
                _                             => new MainWindowCommandEmpty(),
            };
        }

        /// <summary>
        /// ディレクトリモードの場合の Command を返す
        /// </summary>
        private static MainWindowCommand CreateOfDirectoryMode(AppKeys.KeyEvent keyEvent, MainWindow mainWindow, History history)
        {
            return keyEvent switch
            {
                AppKeys.KeyEvent.FileOpen      => new MainWindowCommandFileOpen(mainWindow, history),
                AppKeys.KeyEvent.ShowPrograms  => new MainWindowCommandShowPrograms(mainWindow, history),
                AppKeys.KeyEvent.BackDirectory => new MainWindowCommandBackDirectory(mainWindow),
                AppKeys.KeyEvent.IntoDirectory => new MainWindowCommandIntoDirectory(mainWindow),
                _                              => new MainWindowCommandEmpty(),
            };
        }
    }
}
