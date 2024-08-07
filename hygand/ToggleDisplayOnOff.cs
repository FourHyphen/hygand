﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace hygand
{
    internal class ToggleDisplayOnOff : IDisposable
    {
        // Capslock キーを拾うためグローバルフックをかける
        #region Win32API
        protected const int WH_KEYBOARD_LL = 0x000D;
        protected const int WM_KEYDOWN = 0x0100;
        protected const int WM_KEYUP = 0x0101;
        protected const int WM_SYSKEYDOWN = 0x0104;
        protected const int WM_SYSKEYUP = 0x0105;

        [StructLayout(LayoutKind.Sequential)]
        private class KBDLLHOOKSTRUCT
        {
            internal uint vkCode;
            internal uint scanCode;
            internal KBDLLHOOKSTRUCTFlags flags;
            internal uint time;
            internal UIntPtr dwExtraInfo;
        }

        [Flags]
        private enum KBDLLHOOKSTRUCTFlags : uint
        {
            KEYEVENTF_EXTENDEDKEY = 0x0001,
            KEYEVENTF_KEYUP = 0x0002,
            KEYEVENTF_SCANCODE = 0x0008,
            KEYEVENTF_UNICODE = 0x0004,
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, KeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private delegate IntPtr KeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        #endregion

        internal class OriginalKeyEventArg : EventArgs
        {
            internal int KeyCode { get; }

            internal OriginalKeyEventArg(int keyCode)
            {
                KeyCode = keyCode;
            }
        }

        // デリゲートをフィールドに配置することで GC に回収させない
        internal delegate void KeyEventHandler(object sender, OriginalKeyEventArg e);

        private event KeyEventHandler _keyDownEvent;
        private short _keyDownEventKeyCode;
        private KeyboardProc _proc;    // フィールドにないとエラーした
        private IntPtr _hookId = IntPtr.Zero;

        /// <summary>
        /// キーフックを開始
        /// </summary>
        internal ToggleDisplayOnOff(KeyEventHandler keyDownEvent, short keyDownEventKeyCode)
        {
            _keyDownEvent += keyDownEvent;
            _keyDownEventKeyCode = keyDownEventKeyCode;
            _proc = HookCallBack;
            Hook();
        }

        /// <summary>
        /// キーボードグローバルフックをかける
        /// </summary>
        private void Hook()
        {
            using var curProcess = Process.GetCurrentProcess();
            using ProcessModule curModule = curProcess.MainModule!;

            // TODO: 失敗時の上位への通知
            if (curModule?.ModuleName == null)
            {
                return;
            }

            // WH_KEYBOARD_LL: これでキーボードの低レベルのイベントを拾う
            // 第二引数: コールバック用のデリゲート
            _hookId = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, GetModuleHandle(curModule.ModuleName!), 0);
        }

        /// <summary>
        /// イベント発生時に実行されるコールバック関数
        /// </summary>
        private IntPtr HookCallBack(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                var kb = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
                int vkKeyCode = (int)kb.vkCode;

                // 画面表示オンオフを切り替えるイベントのみ処理する
                if (vkKeyCode == _keyDownEventKeyCode)
                {
                    _keyDownEvent?.Invoke(this, new OriginalKeyEventArg(vkKeyCode));

                    // 従来のキーイベントを破棄する(例: CapsLock キーでの画面表示切り替えの場合、CapsLock キーの本来の機能を呼ばない)
                    return new IntPtr(1);
                }
            }

            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        /// <summary>
        /// フックを解除する
        /// </summary>
        private void UnHook()
        {
            UnhookWindowsHookEx(_hookId);
            _hookId = IntPtr.Zero;
        }

        /// <summary>
        /// キーフックを解除する
        /// </summary>
        public void Dispose()
        {
            UnHook();
        }
    }
}
