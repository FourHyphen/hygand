using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyFileLauncher
{
    internal class MainWindowProcessKeyDowned
    {
        private MainWindow _mainWindow;

        private History _history;

        internal MainWindowProcessKeyDowned(MainWindow mainWindow, History history)
        {
            _mainWindow = mainWindow;
            _history = history;
        }

        /// <summary>
        /// DisplayFileList 押下キーに応じた処理を実行する
        /// </summary>
        internal void Execute(Key key, Key systemKey, ModifierKeys modifier)
        {
            AppKeys.KeyEventType keyEventType = AppKeys.ToKeyEventType(key, systemKey, modifier);
            if (keyEventType == AppKeys.KeyEventType.FileOpen)
            {
                DoKeyEventFileOpen();
            }
            else if (keyEventType == AppKeys.KeyEventType.BackDirectory)
            {
                DoKeyEventBackDirectory();
            }
            else if (keyEventType == AppKeys.KeyEventType.IntoDirectory)
            {
                DoKeyEventIntoDirectory();
            }
            else if (keyEventType == AppKeys.KeyEventType.FocusOnSearchTextBox)
            {
                DoKeyEventFocusOnSearchTextBox();
            }
            else if (keyEventType == AppKeys.KeyEventType.ShowPrograms)
            {
                DoKeyEventShowPrograms();
            }
        }

        /// <summary>
        /// キーイベントによるファイルオープンを実行
        /// </summary>
        private void DoKeyEventFileOpen()
        {
            // フォーカスの当たっているファイルパスを取得
            string? focusedFilePath = GetListViewItemStringFocused();
            if (focusedFilePath == null)
            {
                return;
            }

            // ファイルを開く
            OpenFile(focusedFilePath);

            // 履歴に追加
            _history.Add(focusedFilePath);

            // ファイルを開いたら用は済んだのでメインウィンドウを非表示化
            _mainWindow.Hide();
        }

        /// <summary>
        /// フォーカスが当てられている ListViewItem の Content を返す、何も当たっていなければ null を返す
        /// </summary>
        private string? GetListViewItemStringFocused()
        {
            ListViewItem? focused = GetListViewItemFocused();
            if (focused == null)
            {
                return null;
            }

            return (string)focused.Content;
        }

        /// <summary>
        /// フォーカスが当てられている ListViewItem を返す、何も当たっていなければ null を返す
        /// </summary>
        private ListViewItem? GetListViewItemFocused()
        {
            // 参考: https://threeshark3.com/binding-listbox-focus/
            for (int i = 0; i < _mainWindow.DisplayFileList.Items.Count; i++)
            {
                var obj = _mainWindow.DisplayFileList.ItemContainerGenerator.ContainerFromIndex(i);
                if (obj is ListViewItem target)
                {
                    if (target.IsFocused)
                    {
                        return target;
                    }
                }
            }

            return null;
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

        /// <summary>
        /// キーイベントによるディレクトリ階層を 1 つ戻る処理を実行
        /// </summary>
        private void DoKeyEventBackDirectory()
        {
            // 今フォーカスされているファイルパスを取得
            string? focusedFilePath = GetListViewItemStringFocused();
            if (focusedFilePath == null)
            {
                return;
            }

            // 戻り先ディレクトリパス取得
            string? dirPath = GetBackDirectoryPath(focusedFilePath, _mainWindow.SearchText.Text);
            if (dirPath == null)
            {
                return;
            }

            // 戻り先でフォーカスを当てるための、今表示されているディレクトリパス取得
            string willFocusDirPath = GetNowDisplayingDirPath(focusedFilePath);

            // 更新
            UpdateOfDirectoryInfo(dirPath);

            // 移動元にフォーカスを当て、移動前の状態に戻す
            SetFocusListViewItem(willFocusDirPath);
        }

        /// <summary>
        /// 1 階層戻る先のディレクトリパスを返す
        /// </summary>
        private string? GetBackDirectoryPath(string focusedFilePath, string searchText)
        {
            // 今のテキストボックスがディレクトリのパスであればこれの 1 階層上を返す
            if (System.IO.Directory.Exists(searchText))
            {
                return System.IO.Path.GetDirectoryName(searchText)!;
            }

            // 選択されているファイルのディレクトリパスを返す
            return System.IO.Path.GetDirectoryName(focusedFilePath)!;
        }

        /// <summary>
        /// 今表示しているディレクトリのパスを返す
        /// </summary>
        private string GetNowDisplayingDirPath(string focusedFilePath)
        {
            // 今フォーカスが当たっているファイルパスの 1 階層上が、今表示しているディレクトリのパス
            return System.IO.Path.GetDirectoryName(focusedFilePath);
        }

        /// <summary>
        /// ディレクトリの情報で画面を更新
        /// </summary>
        private void UpdateOfDirectoryInfo(string dirPath)
        {
            // テキストボックスにディレクトリセット
            _mainWindow.SearchText.Text = dirPath;

            // 検索結果には当該ディレクトリ内のファイルをセット
            _mainWindow.FileListDisplaying.Update(dirPath);

            // ファイルパスが長い時に真に見たいのはファイル / ディレクトリ名のため横スクロールを右端に設定
            _mainWindow.DisplayFileListScrollViewer.ScrollToRightEnd();
        }

        /// <summary>
        /// キーイベントによるディレクトリに入る処理を実行
        /// </summary>
        private void DoKeyEventIntoDirectory()
        {
            // フォーカスされているファイルパスを取得
            string? focusedFilePath = GetListViewItemStringFocused();
            if (focusedFilePath == null)
            {
                return;
            }

            // フォーカスされているパスがディレクトリでない場合は入れないのでここで終了
            if (!System.IO.Directory.Exists(focusedFilePath))
            {
                return;
            }

            // 更新
            UpdateOfDirectoryInfo(focusedFilePath);
        }

        /// <summary>
        /// キーイベントによるフォーカスを検索テキストボックスに移動する処理を実行
        /// </summary>
        private void DoKeyEventFocusOnSearchTextBox()
        {
            // フォーカスされている場所がファイルリストの先頭(一番上)でなければ移動しない
            ListViewItem? focused = GetListViewItemFocused();
            if (focused == null)
            {
                return;
            }

            string? focusedStr = GetListViewItemStringFocused();
            if (focusedStr != _mainWindow.FileListDisplaying.FileList[0])
            {
                return;
            }

            // フォーカス移動
            _mainWindow.SearchText.Focus();
        }

        /// <summary>
        /// キーイベントによるプログラム一覧表示を実行
        /// </summary>
        private void DoKeyEventShowPrograms()
        {
            // フォーカスされているファイルパスを取得
            string? focusedFilePath = GetListViewItemStringFocused();
            if (focusedFilePath == null)
            {
                return;
            }

            // コンテキストメニュー一覧ウィンドウを表示
            FileContextMenuWindow window = new FileContextMenuWindow(focusedFilePath);
            window.Owner = _mainWindow;
            window.Show();

            // コンテキストメニュー一覧ウィンドウが閉じるまでメイン画面を無効化
            _mainWindow.IsEnabled = false;    // FileContextMenuWindow に最初からキーボードフォーカスを当てるための無効化
            window.Closed += FileContextMenuWindowClosed;
        }

        /// <summary>
        /// コンテキストメニュー一覧ウィンドウクローズ時のイベント
        /// </summary>
        private void FileContextMenuWindowClosed(object? sender, EventArgs e)
        {
            // 無効化からの復帰
            _mainWindow.IsEnabled = true;

            if (!(sender is FileContextMenuWindow))
            {
                // 操作したファイルパス不明のため検索テキストボックスにフォーカス移動
                // ここは基本的に通らない想定
                _mainWindow.SearchText.Focus();
                System.Windows.Input.Keyboard.Focus(_mainWindow.SearchText);
                return;
            }

            // ウィンドウ閉じただけだとフォーカスは宙ぶらりんになったので明示的に指定
            FileContextMenuWindow fcmw = (FileContextMenuWindow)sender;
            SetFocusListViewItem(fcmw.FilePath);

            // コンテキストを実行した場合、用は済んだので履歴に追加してメイン画面を非表示化
            if (fcmw.DidExecuteContext)
            {
                _history.Add(fcmw.FilePath);
                _mainWindow.Hide();
            }
        }

        /// <summary>
        /// ListView の特定の Item にフォーカスを当てる
        /// </summary>
        private void SetFocusListViewItem(string filePath)
        {
            // ListView 更新直後だと ListView の Item が空になったため、ListView に Item がセットされるようにする
            _mainWindow.UpdateLayout();

            for (int i = 0; i < _mainWindow.DisplayFileList.Items.Count; i++)
            {
                var obj = _mainWindow.DisplayFileList.ItemContainerGenerator.ContainerFromIndex(i);
                if (obj is ListViewItem target)
                {
                    if ((string)target.Content == filePath)
                    {
                        target.Focus();
                        return;
                    }
                }
            }
        }
    }
}
