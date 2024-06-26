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
        /// ListView の 1 行の高さを返す。1 行を取得できない場合は NaN を返す
        /// </summary>
        protected double GetListViewRowHeight(MainWindow mainWindow)
        {
            if (mainWindow.FileListDisplaying.FileList.Count == 0)
            {
                return double.NaN;
            }

            if (mainWindow.DisplayFileList.ItemContainerGenerator.ContainerFromIndex(0) is not ListViewItem item)
            {
                return double.NaN;
            }

            return item.ActualHeight;
        }

        /// <summary>
        ///  スクロール可能かを返す
        /// </summary>
        protected bool CanScroll(MainWindow mainWindow)
        {
            // スクロール可能な範囲がないなら全項目表示されているためスクロール不可
            return (mainWindow.DisplayFileListScrollViewer.ScrollableHeight > 0.0);
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
