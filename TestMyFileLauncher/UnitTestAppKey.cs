﻿using System.Security.Cryptography;
using System.Windows.Input;

namespace TestMyFileLauncher
{
    [TestClass]
    public class UnitTestAppKey
    {
        /// <summary>
        /// 所定のキー入力をファイルオープンイベントに変換する
        /// </summary>
        [TestMethod]
        public void ToKeyEventTypeFileOpenFromInputKey()
        {
            // 変換
            MyFileLauncher.AppKeys.KeyEventType ket = MyFileLauncher.AppKeys.ToKeyEventType(key: Key.Enter,
                                                                                            systemKey: Key.None,
                                                                                            modifier: ModifierKeys.None);

            // 確認
            Assert.AreEqual(expected: MyFileLauncher.AppKeys.KeyEventType.FileOpen, actual: ket) ;
        }

        /// <summary>
        /// 所定のキー入力をディレクトリを 1 つ戻るイベントに変換する
        /// </summary>
        [TestMethod]
        public void ToKeyEventTypeBackDirectoryFromInputKey()
        {
            // 変換
            MyFileLauncher.AppKeys.KeyEventType ket = MyFileLauncher.AppKeys.ToKeyEventType(key: Key.Left,
                                                                                            systemKey: Key.None,
                                                                                            modifier: ModifierKeys.None);

            // 確認
            Assert.AreEqual(expected: MyFileLauncher.AppKeys.KeyEventType.BackDirectory, actual: ket);
        }

        /// <summary>
        /// 所定のキー入力をディレクトリに入るイベントに変換する
        /// </summary>
        [TestMethod]
        public void ToKeyEventTypeIntoDirectoryFromInputKey()
        {
            // 変換
            MyFileLauncher.AppKeys.KeyEventType ket = MyFileLauncher.AppKeys.ToKeyEventType(key: Key.Right,
                                                                                            systemKey: Key.None,
                                                                                            modifier: ModifierKeys.None);

            // 確認
            Assert.AreEqual(expected: MyFileLauncher.AppKeys.KeyEventType.IntoDirectory, actual: ket);
        }

        /// <summary>
        /// 所定のキー入力をフォーカスをファイルリストに移動するイベントに変換する
        /// </summary>
        [TestMethod]
        public void ToKeyEventTypeFocusOnFileListFromInputKey()
        {
            // 変換
            MyFileLauncher.AppKeys.KeyEventType ket = MyFileLauncher.AppKeys.ToKeyEventType(key: Key.Down,
                                                                                            systemKey: Key.None,
                                                                                            modifier: ModifierKeys.None);

            // 確認
            Assert.AreEqual(expected: MyFileLauncher.AppKeys.KeyEventType.FocusOnFileList, actual: ket);
        }

        /// <summary>
        /// 所定外のキー入力は None イベントとなる
        /// </summary>
        [TestMethod]
        public void ToKeyEventTypeNoneFromInputKey()
        {
            // 変換
            MyFileLauncher.AppKeys.KeyEventType ket = MyFileLauncher.AppKeys.ToKeyEventType(key: Key.LeftShift,
                                                                                            systemKey: Key.RightShift,
                                                                                            modifier: ModifierKeys.Control);

            // 確認
            Assert.AreEqual(expected: MyFileLauncher.AppKeys.KeyEventType.None, actual: ket);
        }
    }
}
