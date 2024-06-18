using System;
using System.Windows.Input;

namespace MyFileLauncher
{
    internal class MainWindowCommandFocusedSearchTextFactory
    {
        internal static MainWindowCommand Create(AppMode appMode, AppKeys.KeyEventOnTextBox keyEventOnTextBox, MainWindow mainWindow, KeyEventArgs e)
        {
            // ディレクトリの中に入る要求で、かつ前方一致検索の結果 1 件だけの場合、テキストボックス記載ディレクトリに入る
            if (appMode == AppMode.Directory)
            {
                // TODO: 実装
                //return new MainWindowCommandIntoDirectory(mainWindow);
            }

            if (keyEventOnTextBox == AppKeys.KeyEventOnTextBox.FocusOnFileList)
            {
                return new MainWindowCommandMoveFocusOnFileList(mainWindow, e);
            }

            return new MainWindowCommandEmpty();
        }
    }
}