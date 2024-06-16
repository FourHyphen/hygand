namespace MyFileLauncher
{
    internal static class DisplayFileListCommandFactory
    {
        /// <summary>
        /// DisplayFileListCommand ファクトリメソッド
        /// </summary>
        internal static DisplayFileListCommand Create(string mode, AppKeys.KeyEventType keyEventType, MainWindow mainWindow, History history)
        {
            if (mode == "index")
            {
                return CreateOfIndexMode(keyEventType, mainWindow, history);
            }
            else if (mode == "directory")
            {
                return CreateOfDirectoryMode(keyEventType, mainWindow, history);
            }

            return new DisplayFileListCommandEmpty();
        }

        /// <summary>
        /// インデックスモードの場合の Command を返す
        /// </summary>
        private static DisplayFileListCommand CreateOfIndexMode(AppKeys.KeyEventType keyEventType, MainWindow mainWindow, History history)
        {
            return keyEventType switch
            {
                AppKeys.KeyEventType.FileOpen             => new DisplayFileListCommandFileOpen(mainWindow, history),
                AppKeys.KeyEventType.FocusOnSearchTextBox => new DisplayFileListCommandFocusOnSearchTextBox(mainWindow),
                AppKeys.KeyEventType.ShowPrograms         => new DisplayFileListCommandShowPrograms(mainWindow, history),
                _                                         => new DisplayFileListCommandEmpty(),
            };
        }

        /// <summary>
        /// ディレクトリモードの場合の Command を返す
        /// </summary>
        private static DisplayFileListCommand CreateOfDirectoryMode(AppKeys.KeyEventType keyEventType, MainWindow mainWindow, History history)
        {
            return keyEventType switch
            {
                AppKeys.KeyEventType.FileOpen             => new DisplayFileListCommandFileOpen(mainWindow, history),
                AppKeys.KeyEventType.FocusOnSearchTextBox => new DisplayFileListCommandFocusOnSearchTextBox(mainWindow),
                AppKeys.KeyEventType.ShowPrograms         => new DisplayFileListCommandShowPrograms(mainWindow, history),
                AppKeys.KeyEventType.BackDirectory        => new DisplayFileListCommandBackDirectory(mainWindow),
                AppKeys.KeyEventType.IntoDirectory        => new DisplayFileListCommandIntoDirectory(mainWindow),
                _                                         => new DisplayFileListCommandEmpty(),
            };
        }
    }
}
