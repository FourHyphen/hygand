﻿namespace hygand
{
    /// <summary>
    /// ディレクトリに入る
    /// </summary>
    internal class MainWindowCommandIntoDirectory : MainWindowCommand
    {
        private MainWindow _mainWindow;

        internal MainWindowCommandIntoDirectory(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        private protected override Result ExecuteCore()
        {
            // 現在選択されているファイルパスを取得
            string? selectedFilePath = _mainWindow.FileListDisplaying.GetSelectedFilePath();
            if (selectedFilePath == null)
            {
                return Result.NoProcess;
            }

            // 選択されているパスがディレクトリでない場合は入れないのでここで終了
            if (!System.IO.Directory.Exists(selectedFilePath))
            {
                return Result.NoProcess;
            }

            // 更新
            return UpdateDisplay(_mainWindow, selectedFilePath);
        }
    }
}