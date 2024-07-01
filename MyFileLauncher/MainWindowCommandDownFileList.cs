using System.Linq;
using System.Windows.Controls;

namespace MyFileLauncher
{
    internal class MainWindowCommandDownFileList : MainWindowCommand
    {
        private MainWindow _mainWindow;

        internal MainWindowCommandDownFileList(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        /// <summary>
        /// 次を選択状態にする
        /// 下げたときにファイルリストの画面表示からはみ出さないよう、スクロールを調整する
        /// </summary>
        internal override Result Execute()
        {
            // 1 行の高さを取得(できないならここで終了)
            double rowHeight = GetListViewRowHeight(_mainWindow);
            if (rowHeight == double.NaN)
            {
                return Result.NoProcess;
            }

            // 次を選択状態にする
            _mainWindow.FileListDisplaying.SelectNext();

            // スクロールできないならここで終了
            if (!CanScroll(_mainWindow))
            {
                return Result.Success;
            }

            // 今のスクロール位置が表示範囲に合っていない場合はスクロールを調整
            if (DoNeedScroll(rowHeight))
            {
                DownScrollRow(rowHeight);
            }

            return Result.Success;
        }

        /// <summary>
        /// スクロールが必要かを返す
        /// </summary>
        private bool DoNeedScroll(double rowHeight)
        {
            // スクロールせずに収まる行番号(見切れている分をカウントしないよう切り捨て)
            int maxInViewerRowNum = (int)System.Math.Floor((_mainWindow.DisplayFileListScrollViewer.ActualHeight) / rowHeight);

            // 今表示してる最上部の行番号
            // スクロールバーの現在地(どれだけ最上部から下に移動しているか)が ListViewItem いくつ分の高さにあるかで算出
            int nowDisplayingTopRowNum = (int)System.Math.Floor(_mainWindow.DisplayFileListScrollViewer.VerticalOffset / rowHeight);

            // 今スクロール不要で表示できる最下部の行番号(見切れている分をカウントしないよう 1 減算)
            int nowDisplayableWithoutScrollBottomRowNum = nowDisplayingTopRowNum + maxInViewerRowNum - 1;

            // 移動後選択状態の行番号
            int afterNextRowNum = _mainWindow.FileListDisplaying.GetSelectedIndex() + 1;

            // 移動後選択行番号が今スクロール不要で表示できる最下部の行番号に収まっていないならスクロール必要
            return (afterNextRowNum > nowDisplayableWithoutScrollBottomRowNum);
        }

        /// <summary>
        /// スクロール位置を 1 行分下げる
        /// </summary>
        private void DownScrollRow(double rowHeight)
        {
            double nowSchrollVerticalOffset = _mainWindow.DisplayFileListScrollViewer.VerticalOffset;
            _mainWindow.DisplayFileListScrollViewer.ScrollToVerticalOffset(nowSchrollVerticalOffset + rowHeight);
        }
    }
}
