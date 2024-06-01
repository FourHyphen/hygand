using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyFileLauncher
{
    public partial class FileContextMenuWindow : Window
    {
        // Shell32.FolderItemVerb の Name をバインドしても表示されなかったため、表示用のリスト
        public List<string> FileContextNames { get; private set; } = new List<string>();

        private List<Shell32.FolderItemVerb> _fileContextMenu = new List<Shell32.FolderItemVerb>();

        public FileContextMenuWindow(string filePath)
        {
            InitializeComponent();
            DataContext = this;

            InitFileContextMenu(filePath);
        }

        /// <summary>
        /// ファイルコンテキストメニューの初期
        /// </summary>
        /// <remarks>COM Object の解放を GC に任せるため、実行後必ず GC を呼び出すこと</remarks>
        // COM Object の解放を GC に任せるが、勝手にインライン展開されると生存区間がコード上の表現とズレる可能性があるのでその防止
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void InitFileContextMenu(string filePath)
        {
            // "プログラムから開く" の画面を出してもあまり役に立たなかった
            //  -> コンテキストメニューを取得して表示する
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

            // TODO: ファイル／ディレクトリの FolderItem を直接取得する方法がわからず、いったん一階層上を経由した
            Shell32.Folder folder = shell.NameSpace(System.IO.Path.GetDirectoryName(filePath));
            Shell32.FolderItem folderItem = folder.ParseName(System.IO.Path.GetFileName(filePath));

            // ファイルのコンテキストメニューリストを取得
            Shell32.FolderItemVerbs verbs = folderItem.Verbs();

            // Shell32.FolderItemVerb が "開く" とかに相当
            foreach (Shell32.FolderItemVerb verb in verbs)
            {
                FileContextNames.Add(verb.Name);
                _fileContextMenu.Add(verb);
            }
        }

        /// <summary>
        /// ウィンドウロード時イベント
        /// </summary>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            SetFocusFileMenuListFirst();
        }

        /// <summary>
        /// ファイルのコンテキストメニュー一覧の先頭にフォーカスを当てる
        /// </summary>
        private void SetFocusFileMenuListFirst()
        {
            var obj = FileMenuList.ItemContainerGenerator.ContainerFromIndex(0);
            if (obj is ListViewItem target)
            {
                target.Focus();
                Keyboard.Focus(target);
            }
        }

        /// <summary>
        /// マウスイベント: 左クリック時
        /// </summary>
        private void MouseLeftButtonDownClicked(object sender, MouseButtonEventArgs e)
        {
            ListViewItem selected = (ListViewItem)sender;
            ExecuteContextAndCloseWindow((string)selected.Content);
        }

        /// <summary>
        /// コンテキストを実行してウィンドウを閉じる
        /// </summary>
        private void ExecuteContextAndCloseWindow(string verbName)
        {
            Shell32.FolderItemVerb verb = _fileContextMenu.Where(fe => fe.Name == verbName).First();
            verb.DoIt();

            Close();
        }

        /// <summary>
        /// ファイルコンテキストメニューイベント: キー押下時
        /// </summary>
        private void FileContextMenuItemKeyDowned(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ExecuteContextAndCloseWindow((string)FileMenuList.SelectedItem);
            }
        }

        /// <summary>
        /// 画面クローズ時イベント
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            base.OnClosed(e);
        }
    }
}
