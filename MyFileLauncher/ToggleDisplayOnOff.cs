using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MyFileLauncher
{
    internal class ToggleDisplayOnOff : IDisposable
    {
        // Capslock キーを拾うためグローバルフックをかける
        protected const int WH_KEYBOARD_LL = 0x000D;
        protected const int WM_KEYDOWN = 0x0100;
        protected const int WM_KEYUP = 0x0101;
        protected const int WM_SYSKEYDOWN = 0x0104;
        protected const int WM_SYSKEYUP = 0x0105;

        [StructLayout(LayoutKind.Sequential)]
        public class KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public KBDLLHOOKSTRUCTFlags flags;
            public uint time;
            public UIntPtr dwExtraInfo;
        }

        [Flags]
        public enum KBDLLHOOKSTRUCTFlags : uint
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

        public class OriginalKeyEventArg : EventArgs
        {
            public int KeyCode { get; }

            public OriginalKeyEventArg(int keyCode)
            {
                KeyCode = keyCode;
            }
        }

        private KeyboardProc _proc;
        private IntPtr _hookId = IntPtr.Zero;

        // デリゲートをフィールドに配置することで GC に回収させない
        public delegate void KeyEventHandler(object sender, OriginalKeyEventArg e);

        /// <summary>
        /// キーダウン時の処理をここに設定
        /// </summary>
        public event KeyEventHandler KeyDownEvent;

        /// <summary>
        /// キーボードグローバルフックをかける
        /// </summary>
        public void Hook()
        {
            if (_hookId != IntPtr.Zero)
            {
                return;
            }

            _proc = HookCallBack;
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

                KeyDownEvent?.Invoke(this, new OriginalKeyEventArg(vkKeyCode));
                // TODO: 可変にする
                if (vkKeyCode == 240)    // 240 = Capslock
                {
                    // 従来のキーイベントを破棄する
                    return new IntPtr(1);
                }
            }

            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        /// <summary>
        /// フックを解除する
        /// </summary>
        public void UnHook()
        {
            UnhookWindowsHookEx(_hookId);
            _hookId = IntPtr.Zero;
        }

        public void Dispose()
        {
            UnHook();
        }
    }
}
