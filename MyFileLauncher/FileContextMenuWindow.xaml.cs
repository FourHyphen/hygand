using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyFileLauncher
{
    public partial class FileContextMenuWindow : Window
    {
        public List<string> FileExecuteNames { get; private set; } = new List<string>();

        private List<Shell32.FolderItemVerb> _fileExecutes = new List<Shell32.FolderItemVerb>();

        public FileContextMenuWindow(string filePath)
        {
            InitializeComponent();
            DataContext = this;

            InitMenu(filePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>COM Object の解放を GC に任せるため、実行後必ず GC を呼び出すこと</remarks>
        // COM Object の解放を GC に任せるが、勝手にインライン展開されると生存区間がコード上の表現とズレる可能性があるのでその防止
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void InitMenu(string filePath)
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

            // Shell32.FolderItemVerb が "開く" とか
            foreach (Shell32.FolderItemVerb verb in verbs)
            {
                FileExecuteNames.Add(verb.Name);
                _fileExecutes.Add(verb);
            }
        }

        public void SetFocusFileMenuList()
        {
            var obj = FileMenuList.ItemContainerGenerator.ContainerFromIndex(0);
            if (obj is ListViewItem target)
            {
                target.Focus();
                Keyboard.Focus(target);
            }
        }

        private void MouseLeftButtonDownClicked(object sender, MouseButtonEventArgs e)
        {
            ListViewItem selected = (ListViewItem)sender;
            Execute((string)selected.Content);
        }

        private void Execute(string verbName)
        {
            Execute(_fileExecutes.Where(fe => fe.Name == verbName).First());
        }

        private void Execute(Shell32.FolderItemVerb verb)
        {
            verb.DoIt();
            Close();
        }

        private void FileMenuItemKeyDowned(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Execute((string)FileMenuList.SelectedItem);
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
