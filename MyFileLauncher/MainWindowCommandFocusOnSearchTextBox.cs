using System.Windows.Controls;

namespace MyFileLauncher
{
    /// <summary>
    /// MainWindow の SearchTextBox にフォーカスを当てる
    /// </summary>
    internal class MainWindowCommandFocusOnSearchTextBox : MainWindowCommand
    {
        private MainWindow _mainWindow;

        internal MainWindowCommandFocusOnSearchTextBox(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        internal override void Execute()
        {
            // フォーカスされている場所がファイルリストの先頭(一番上)でなければ移動しない
            ListViewItem? focused = GetListViewItemFocused(_mainWindow);
            if (focused == null)
            {
                return;
            }

            string? focusedStr = GetListViewItemStringFocused(_mainWindow);
            if (focusedStr != _mainWindow.FileListDisplaying.FileList[0])
            {
                return;
            }

            // フォーカス移動
            SetFocusTextBox(_mainWindow.SearchText);

            // キーカーソル位置をテキストボックスの末尾文字に移動
            _mainWindow.SearchText.Select(_mainWindow.SearchText.Text.Length, 0);
        }
    }
}