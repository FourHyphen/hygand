using System.Windows.Controls;

namespace MyFileLauncher
{
    internal class MainWindowCommandUpFileList : MainWindowCommand
    {
        private MainWindow _mainWindow;

        internal MainWindowCommandUpFileList(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        /// <summary>
        /// 前を選択状態にする
        /// 上げたときにファイルリストの画面表示からはみ出さないよう、スクロールを調整する
        /// </summary>
        internal override void Execute()
        {
            // 前を選択状態にする
            _mainWindow.FileListDisplaying.SelectBack();

            // 1 行の高さを取得(できないならここで終了)
            double rowHeight = GetListViewRowHeight(_mainWindow);
            if (rowHeight == double.NaN)
            {
                return;
            }

            // スクロールできないならここで終了
            if (!CanScroll(_mainWindow))
            {
                return;
            }

            // 今のスクロール位置が表示範囲に合っていない場合はスクロールを調整
            if (DoNeedScroll(rowHeight))
            {
                UpScrollRow(rowHeight);
            }
        }

        /// <summary>
        /// スクロールが必要かを返す
        /// </summary>
        private bool DoNeedScroll(double rowHeight)
        {
            // 移動後選択状態の行番号
            int afterBackRowNum = _mainWindow.FileListDisplaying.GetSelectedIndex() + 1;

            // 移動後選択行番号が先頭の場合、スクロールバーが少しだけ下がっている場合を考慮してスクロール必要とする
            if (afterBackRowNum == 1)
            {
                return true;
            }

            // 今表示してる最上部の行番号
            // スクロールバーの現在地が ListViewItem いくつ分の高さにあるかで算出(見切れてる行をカウントしないよう切り上げ)
            // すでに先頭行を表示している場合はスクロールバー現在地は最上部と見なしてスクロール不要
            double tmp = _mainWindow.DisplayFileListScrollViewer.VerticalOffset / rowHeight;
            int nowDisplayingTopRowNum = (int)System.Math.Ceiling(tmp);
            if (nowDisplayingTopRowNum == 1)
            {
                return false;
            }

            // 移動後選択行番号が今表示している最上部の行番号に収まっていないならスクロール必要
            return (afterBackRowNum < (nowDisplayingTopRowNum + 1));
        }

        /// <summary>
        /// スクロール位置を 1 行分上げる
        /// </summary>
        private void UpScrollRow(double rowHeight)
        {
            double nowSchrollVerticalOffset = _mainWindow.DisplayFileListScrollViewer.VerticalOffset;
            _mainWindow.DisplayFileListScrollViewer.ScrollToVerticalOffset(nowSchrollVerticalOffset - rowHeight);
        }
    }
}