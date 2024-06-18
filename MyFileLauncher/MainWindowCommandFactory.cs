namespace MyFileLauncher
{
    internal static class MainWindowCommandFocusedFileListFactory
    {
        /// <summary>
        /// フォーカスが DisplayFileList にある場合のファクトリメソッド
        /// </summary>
        internal static MainWindowCommand Create(AppMode mode, AppKeys.KeyEventOnFileList keyEventType, MainWindow mainWindow, History history)
        {
            if (mode == AppMode.Index)
            {
                return CreateOfIndexMode(keyEventType, mainWindow, history);
            }
            else if (mode == AppMode.Directory)
            {
                return CreateOfDirectoryMode(keyEventType, mainWindow, history);
            }

            return new MainWindowCommandEmpty();
        }

        /// <summary>
        /// インデックスモードの場合の Command を返す
        /// </summary>
        private static MainWindowCommand CreateOfIndexMode(AppKeys.KeyEventOnFileList keyEventType, MainWindow mainWindow, History history)
        {
            return keyEventType switch
            {
                AppKeys.KeyEventOnFileList.FileOpen             => new MainWindowCommandFileOpen(mainWindow, history),
                AppKeys.KeyEventOnFileList.FocusOnSearchTextBox => new MainWindowCommandFocusOnSearchTextBox(mainWindow),
                AppKeys.KeyEventOnFileList.ShowPrograms         => new MainWindowCommandShowPrograms(mainWindow, history),
                _                                               => new MainWindowCommandEmpty(),
            };
        }

        /// <summary>
        /// ディレクトリモードの場合の Command を返す
        /// </summary>
        private static MainWindowCommand CreateOfDirectoryMode(AppKeys.KeyEventOnFileList keyEventType, MainWindow mainWindow, History history)
        {
            return keyEventType switch
            {
                AppKeys.KeyEventOnFileList.FileOpen             => new MainWindowCommandFileOpen(mainWindow, history),
                AppKeys.KeyEventOnFileList.FocusOnSearchTextBox => new MainWindowCommandFocusOnSearchTextBox(mainWindow),
                AppKeys.KeyEventOnFileList.ShowPrograms         => new MainWindowCommandShowPrograms(mainWindow, history),
                AppKeys.KeyEventOnFileList.BackDirectory        => new MainWindowCommandBackDirectory(mainWindow),
                AppKeys.KeyEventOnFileList.IntoDirectory        => new DisplayFileListCommandIntoDirectory(mainWindow),
                _                                               => new MainWindowCommandEmpty(),
            };
        }
    }
}
