using System;
using System.Windows.Input;

namespace MyFileLauncher
{
    internal class MainWindowCommandFocusedSearchTextFactory
    {
        internal static MainWindowCommand Create(AppMode appMode, AppKeys.KeyEventOnTextBox keyEventOnTextBox, MainWindow mainWindow, KeyEventArgs e)
        {
            // テキストボックス記載ディレクトリの中に入るのは条件付きのため注意
            if (keyEventOnTextBox == AppKeys.KeyEventOnTextBox.IntoDirectory)
            {
                if (appMode == AppMode.Directory)
                {
                    return new SearchTextCommandIntoDirectory(mainWindow);
                }
            }
            else if (keyEventOnTextBox == AppKeys.KeyEventOnTextBox.FocusOnFileList)
            {
                return new MainWindowCommandMoveFocusOnFileList(mainWindow, e);
            }

            return new MainWindowCommandEmpty();
        }
    }
}