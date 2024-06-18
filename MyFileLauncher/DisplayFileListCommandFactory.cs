namespace MyFileLauncher
{
    internal static class DisplayFileListCommandFactory
    {
        /// <summary>
        /// DisplayFileListCommand ファクトリメソッド
        /// </summary>
        internal static DisplayFileListCommand Create(AppMode mode, AppKeys.KeyEventOnFileList keyEventType, MainWindow mainWindow, History history)
        {
            if (mode == AppMode.Index)
            {
                return CreateOfIndexMode(keyEventType, mainWindow, history);
            }
            else if (mode == AppMode.Directory)
            {
                return CreateOfDirectoryMode(keyEventType, mainWindow, history);
            }

            return new DisplayFileListCommandEmpty();
        }

        /// <summary>
        /// インデックスモードの場合の Command を返す
        /// </summary>
        private static DisplayFileListCommand CreateOfIndexMode(AppKeys.KeyEventOnFileList keyEventType, MainWindow mainWindow, History history)
        {
            return keyEventType switch
            {
                AppKeys.KeyEventOnFileList.FileOpen             => new DisplayFileListCommandFileOpen(mainWindow, history),
                AppKeys.KeyEventOnFileList.FocusOnSearchTextBox => new DisplayFileListCommandFocusOnSearchTextBox(mainWindow),
                AppKeys.KeyEventOnFileList.ShowPrograms         => new DisplayFileListCommandShowPrograms(mainWindow, history),
                _                                               => new DisplayFileListCommandEmpty(),
            };
        }

        /// <summary>
        /// ディレクトリモードの場合の Command を返す
        /// </summary>
        private static DisplayFileListCommand CreateOfDirectoryMode(AppKeys.KeyEventOnFileList keyEventType, MainWindow mainWindow, History history)
        {
            return keyEventType switch
            {
                AppKeys.KeyEventOnFileList.FileOpen             => new DisplayFileListCommandFileOpen(mainWindow, history),
                AppKeys.KeyEventOnFileList.FocusOnSearchTextBox => new DisplayFileListCommandFocusOnSearchTextBox(mainWindow),
                AppKeys.KeyEventOnFileList.ShowPrograms         => new DisplayFileListCommandShowPrograms(mainWindow, history),
                AppKeys.KeyEventOnFileList.BackDirectory        => new DisplayFileListCommandBackDirectory(mainWindow),
                AppKeys.KeyEventOnFileList.IntoDirectory        => new DisplayFileListCommandIntoDirectory(mainWindow),
                _                                               => new DisplayFileListCommandEmpty(),
            };
        }
    }
}
