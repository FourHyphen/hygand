using System.Windows.Input;
using static MyFileLauncher.AppKeys;

namespace MyFileLauncher
{
    internal class AppKeys
    {
        /// <summary>
        /// アプリケーションのどこにフォーカス当たっていても有効な、キーによる操作内容
        /// </summary>
        internal enum KeyEventOnAnyWhere
        {
            ChangeAppMode,
            None
        }

        /// <summary>
        /// フォーカスがテキストボックスにある場合に有効な、キーによる操作内容
        /// </summary>
        internal enum KeyEventOnTextBox
        {
            FocusOnFileList,
            None
        }

        /// <summary>
        /// フォーカスがファイルリストにある場合に有効な、キーによる操作内容
        /// </summary>
        internal enum KeyEventOnFileList
        {
            FocusOnSearchTextBox,
            FileOpen,
            BackDirectory,
            IntoDirectory,
            ShowPrograms,
            None
        }

        /// <summary>
        /// キー入力内容をフォーカスがどこにあっても有効なアプリケーションイベントに変換する
        /// </summary>
        internal static KeyEventOnAnyWhere ToKeyEventOnAnyWhere(Key key, Key systemKey, ModifierKeys modifier)
        {
            // Ctrl + Shift + 何か
            if (modifier == (ModifierKeys.Control | ModifierKeys.Shift))
            {
                // Ctrl + Shift のみ
                return KeyEventOnAnyWhere.ChangeAppMode;
            }

            return KeyEventOnAnyWhere.None;
        }

        /// <summary>
        /// キー入力内容をフォーカスがテキストボックスにある場合に有効なアプリケーションイベントに変換する
        /// </summary>
        internal static KeyEventOnTextBox ToKeyEventOnTextBox(Key key, Key systemKey, ModifierKeys modifier)
        {
            if (key == Key.Down)
            {
                return KeyEventOnTextBox.FocusOnFileList;
            }

            return KeyEventOnTextBox.None;
        }

        /// <summary>
        /// キー入力内容をフォーカスがファイルリストにある場合に有効なアプリケーションイベントに変換する
        /// </summary>
        internal static KeyEventOnFileList ToKeyEventOnFileList(Key key, Key systemKey, ModifierKeys modifier)
        {
            KeyEventOnFileList keyEvent = ToKeyEventOnFileListConbination(key, modifier);
            if (keyEvent != KeyEventOnFileList.None)
            {
                return keyEvent;
            }

            return ToKeyEventOnFileListOneKey(key);
        }

        /// <summary>
        /// キー入力内容の組み合わせを KeyEventOnFileList に変換する
        /// </summary>
        private static KeyEventOnFileList ToKeyEventOnFileListConbination(Key key, ModifierKeys modifier)
        {
            // Shift + 何か
            if (modifier == ModifierKeys.Shift)
            {
                if (key == Key.F10)
                {
                    return KeyEventOnFileList.ShowPrograms;
                }
            }

            return KeyEventOnFileList.None;
        }

        /// <summary>
        /// 単体キー入力内容をアプリケーションイベント内容に変換する
        /// </summary>
        private static KeyEventOnFileList ToKeyEventOnFileListOneKey(Key key)
        {
            if (key == Key.Up)
            {
                return KeyEventOnFileList.FocusOnSearchTextBox;
            }
            else if (key == Key.Enter)
            {
                return KeyEventOnFileList.FileOpen;
            }
            else if (key == Key.Left)
            {
                return KeyEventOnFileList.BackDirectory;
            }
            else if (key == Key.Right)
            {
                return KeyEventOnFileList.IntoDirectory;
            }

            return KeyEventOnFileList.None;
        }
    }
}