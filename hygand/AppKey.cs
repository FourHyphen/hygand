using System.Windows.Input;

namespace hygand
{
    internal static class AppKeys
    {
        /// <summary>
        /// キーによる操作内容
        /// </summary>
        internal enum KeyEvent
        {
            ChangeAppMode,
            DownFileList,
            UpFileList,
            FileOpen,
            BackDirectory,
            IntoDirectory,
            ShowPrograms,
            None
        }

        /// <summary>
        /// キー入力内容をアプリケーションイベント種別に変換する
        /// </summary>
        internal static KeyEvent ToKeyEvent(Key key, Key systemKey, ModifierKeys modifier)
        {
            KeyEvent keyEvent = ToKeyEventConbination(systemKey, modifier);
            if (keyEvent != KeyEvent.None)
            {
                return keyEvent;
            }

            return ToKeyEventOneKey(key);
        }

        /// <summary>
        /// キー入力内容の組み合わせを KeyEvent に変換する
        /// </summary>
        private static KeyEvent ToKeyEventConbination(Key systemKey, ModifierKeys modifier)
        {
            // Ctrl + Shift + 何か
            if (modifier == (ModifierKeys.Control | ModifierKeys.Shift))
            {
                // Ctrl + Shift のみ
                return KeyEvent.ChangeAppMode;
            }

            // Shift + 何か
            if (modifier == ModifierKeys.Shift)
            {
                if (systemKey == Key.F10)
                {
                    return KeyEvent.ShowPrograms;
                }
            }

            return KeyEvent.None;
        }

        /// <summary>
        /// 単体キー入力内容をアプリケーションイベント内容に変換する
        /// </summary>
        private static KeyEvent ToKeyEventOneKey(Key key)
        {
            if (key == Key.Enter)
            {
                return KeyEvent.FileOpen;
            }
            else if (key == Key.Down)
            {
                return KeyEvent.DownFileList;
            }
            else if (key == Key.Up)
            {
                return KeyEvent.UpFileList;
            }
            else if (key == Key.Left)
            {
                return KeyEvent.BackDirectory;
            }
            else if (key == Key.Right)
            {
                return KeyEvent.IntoDirectory;
            }

            return KeyEvent.None;
        }
    }
}