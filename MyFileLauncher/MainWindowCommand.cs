using System.Windows.Controls;
using System;

namespace MyFileLauncher
{
    internal abstract class MainWindowCommand
    {
        internal abstract void Execute();

        /// <summary>
        /// ディレクトリの情報で画面を更新
        /// </summary>
        protected void UpdateOfDirectoryInfo(MainWindow mainWindow, string dirPath, string initSelectFilePath = "")
        {
            // テキストボックスにディレクトリセット
            mainWindow.SearchText.Text = dirPath;

            // 検索結果には当該ディレクトリ内のファイルをセット
            // TDOO: アクセス権がなかった場合の System.UnauthorizedAccessException への対応
            mainWindow.FileListDisplaying.UpdateOfDirectory(dirPath, initSelectFilePath);

            // ファイルパスが長い時に真に見たいのはファイル / ディレクトリ名のため横スクロールを右端に設定
            mainWindow.DisplayFileListScrollViewer.ScrollToRightEnd();
        }

        /// <summary>
        /// テキストボックスの末尾にキーカーソルをセットする
        /// </summary>
        protected void SetKeyCursolEndOfTextBox(TextBox textBox)
        {
            textBox.Select(textBox.Text.Length, 0);
        }
    }
}
