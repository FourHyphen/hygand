using System.Windows.Controls;
using System;
using System.Windows;

namespace hygand
{
    internal abstract class MainWindowCommand
    {
        /// <summary>
        /// Command 実行結果
        /// </summary>
        internal enum Result
        {
            Success,
            NoProcess,
            FailedUnauthorizedAccess,
            FailedUnknow
        }

        private protected abstract Result ExecuteCore();

        internal void Execute(MainWindow mainWindow)
        {
            // ロールバック用に現在の情報確保
            string beforeSearchText = mainWindow.SearchText.Text;
            string beforeSelectFilePath = mainWindow.FileListDisplaying.GetSelectedFilePath();

            // 処理が余計なイベントを発行しないよう一時的にイベントを無効化
            mainWindow.DisableEventSearchText();

            // 処理
            Result result = ExecuteCore();

            // ロールバックの必要があればロールバック
            if (DoNeedRollBack(result))
            {
                // 画面を処理前の状態に更新
                UpdateDisplay(mainWindow, beforeSearchText, beforeSelectFilePath);
            }

            // 再現性のある失敗時はメッセージ表示
            if (result == Result.FailedUnauthorizedAccess)
            {
                MessageBox.Show("アクセス権がありませんでした");
            }

            // イベントを再度有効化
            mainWindow.EnableEventSearchText();
        }

        /// <summary>
        /// ロールバックが必要かを返す
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool DoNeedRollBack(Result result)
        {
            // 処理失敗ならロールバックが必要とする
            return !(result == Result.Success || result == Result.NoProcess);
        }

        /// <summary>
        /// 画面表示を更新する
        /// </summary>
        protected Result UpdateDisplay(MainWindow mainWindow, string dirPath, string initSelectFilePath = "")
        {
            // テキストボックス表示を更新
            UpdateSearchText(mainWindow, dirPath);

            // ファイルリスト表示を更新
            return UpdateFileListDisplaying(mainWindow, dirPath, initSelectFilePath);
        }

        /// <summary>
        /// SearchTextBox の表示を更新
        /// </summary>
        private void UpdateSearchText(MainWindow mainWindow, string dirPath)
        {
            mainWindow.SearchText.Text = dirPath;

            // 入力を続行できるようカーソル位置を末尾に設定
            mainWindow.SearchText.Select(mainWindow.SearchText.Text.Length, 0);
        }

        /// <summary>
        /// ディレクトリの情報でファイルリストの表示を更新
        /// </summary>
        private Result UpdateFileListDisplaying(MainWindow mainWindow, string dirPath, string initSelectFilePath = "")
        {
            // 当該ディレクトリ内のファイルをセット
            try
            {
                mainWindow.FileListDisplaying.UpdateOfDirectory(dirPath, initSelectFilePath);
            }
            catch (System.UnauthorizedAccessException)
            {
                return Result.FailedUnauthorizedAccess;
            }

            // ファイルパスが長い時に真に見たいのはファイル / ディレクトリ名のため横スクロールを右端に設定
            mainWindow.DisplayFileListScrollViewer.ScrollToRightEnd();

            return Result.Success;
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
