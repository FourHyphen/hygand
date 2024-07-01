using System;

namespace MyFileLauncher
{
    /// <summary>
    /// 既定のプログラムでファイルを開く
    /// </summary>
    internal class MainWindowCommandFileOpen : MainWindowCommand
    {
        private MainWindow _mainWindow;
        private History _history;

        internal MainWindowCommandFileOpen(MainWindow mainWindow, History history)
        {
            _mainWindow = mainWindow;
            _history = history;
        }

        internal override Result Execute()
        {
            // 現在選択されているファイルパスを取得
            string? selectedFilePath = _mainWindow.FileListDisplaying.GetSelectedFilePath();
            if (selectedFilePath == null)
            {
                return Result.NoProcess;
            }

            // ファイルを開く
            OpenFile(selectedFilePath);

            // 履歴に追加
            _history.Add(selectedFilePath);

            // ファイルを開いたら用は済んだのでメインウィンドウを非表示化
            _mainWindow.HideWindow();

            return Result.Success;
        }

        /// <summary>
        /// 登録されたプログラムでファイルを開く
        /// </summary>
        private void OpenFile(string filePath)
        {
            Type? type = Type.GetTypeFromProgID("Shell.Application");
            if (type == null)
            {
                return;
            }

            // 参照に Microsoft Shell Controls And Automation を追加することで Shell32 を参照できる
            Shell32.Shell? shell = (Shell32.Shell?)Activator.CreateInstance(type!);
            if (shell == null)
            {
                return;
            }

            shell!.Open(filePath);
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(shell!);
        }
    }
}