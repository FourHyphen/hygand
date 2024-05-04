﻿using System.Windows.Input;

namespace MyFileLauncher
{
    internal class AppKeys
    {
        /// <summary>
        /// アプリケーションで有効な、キーによる操作内容
        /// </summary>
        internal enum KeyEventType
        {
            EnterKey,
            Else
        }

        /// <summary>
        /// キー入力内容をアプリケーションイベント内容に変換する
        /// </summary>
        public static KeyEventType ToKeyEventType(Key key, Key systemKey, ModifierKeys modifier)
        {
            // 単押しの場合                  → キー情報は e.Key に入る
            // System キーとの同時押しの場合 → キー情報は e.SystemKey に入る
            KeyEventType keyEventType = ToKeyEventTypeConbination(systemKey, modifier);
            if (keyEventType != KeyEventType.Else)
            {
                return keyEventType;
            }

            return ToKeyEventTypeOneKey(key);
        }

        /// <summary>
        /// キー入力内容の組み合わせをアプリケーションイベント内容に変換する
        /// </summary>
        private static KeyEventType ToKeyEventTypeConbination(Key key, ModifierKeys modifier)
        {
            // 現在単キーのみだが将来の拡張機能としてインタフェースを実装しておく
            // Ctrl + Shift + 何か
            if (modifier == (ModifierKeys.Control | ModifierKeys.Shift))
            {
                // nothing
            }

            // Ctrl + 何か
            if (modifier == ModifierKeys.Control)
            {
                // nothing
            }

            // Shift + 何か
            if (modifier == ModifierKeys.Shift)
            {
                if (key == Key.F10)
                {
                    // nothing;
                }
            }

            // Alt + 何か
            if (modifier == ModifierKeys.Alt)
            {
                // nothing
            }

            return KeyEventType.Else;
        }

        private static KeyEventType ToKeyEventTypeOneKey(Key key)
        {
            if (key == Key.Enter)
            {
                return KeyEventType.EnterKey;
            }

            return KeyEventType.Else;
        }
    }
}