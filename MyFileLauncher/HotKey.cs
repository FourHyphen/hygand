using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace MyFileLauncher
{
    internal class HotKey : IDisposable
    {
        private const int MAX_HOTKEY_REGISTER_NUM = 0xC000;
        private const int WM_HOTKEY = 0x0312;    // WM_HOTKEY メッセージ固定値

        private readonly IntPtr _windowHandle;

        private List<HotKeyItem> _registeredHotKeys = new List<HotKeyItem>();
        private bool _disposed = false;

        [DllImport("user32.dll")]
        private static extern int RegisterHotKey(IntPtr hWnd, int id, int modKey, int vKey);

        [DllImport("user32.dll")]
        private static extern int UnregisterHotKey(IntPtr hWnd, int id);

        public HotKey(Window window)
        {
            _windowHandle = new WindowInteropHelper(window).Handle;

            // キーボードメッセージ受信時イベント登録
            ComponentDispatcher.ThreadPreprocessMessage += EventHotKey;
        }

        private void EventHotKey(ref MSG msg, ref bool handled)
        {
            if (msg.message != WM_HOTKEY)
            {
                return;
            }

            int id = msg.wParam.ToInt32();
            HotKeyItem item = _registeredHotKeys.First(o => o.RegisteredId == id);
            item?.Handler?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// HotKey を登録
        /// </summary>
        public bool Register(ModifierKeys modKey, Key key, EventHandler handler)
        {
            // 登録可能な最大数チェック
            if (_registeredHotKeys.Count >= MAX_HOTKEY_REGISTER_NUM)
            {
                return false;
            }

            // HotKey 登録
            if (!RegisterCore(modKey, key))
            {
                return false;
            }

            // 登録済み HotKey リストに追加
            HotKeyItem item = new HotKeyItem(_registeredHotKeys.Count, modKey, key, handler);
            _registeredHotKeys.Add(item);
            return true;
        }

        private bool RegisterCore(ModifierKeys modKey, Key key)
        {
            int vKey = KeyInterop.VirtualKeyFromKey(key);

            // 戻り値: 成功なら 0 以外、失敗なら 0
            int ret = RegisterHotKey(_windowHandle, _registeredHotKeys.Count, (int)modKey, vKey);
            return (ret != 0);
        }

        /// <summary>
        /// HotKey 登録を全て解除
        /// </summary>
        public bool UnregisterAll()
        {
            // HotKey 登録解除に成功したら登録済み HotKey リストから削除
            _registeredHotKeys.RemoveAll(item => Unregister(item));

            // 登録済み HotKey リストが空なら登録解除に全て成功
            return _registeredHotKeys.Count() == 0;
        }

        private bool Unregister(HotKeyItem item)
        {
            // HotKey 登録解除(戻り値: 成功なら 0 以外、失敗なら 0)
            int ret = UnregisterHotKey(_windowHandle, item.RegisteredId);
            return (ret != 0);
        }

        /// <summary>
        /// 指定の HotKey 登録を解除
        /// </summary>
        public bool Unregister(ModifierKeys modKey, Key key)
        {
            HotKeyItem? item = _registeredHotKeys.FirstOrDefault(o => o.ModifierKeys == modKey && o.Key == key);
            if (item == null)
            {
                return false;
            }

            return Unregister(item!);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // マネージリソースの破棄
            }

            // アンマネージリソースの破棄
            if (!UnregisterAll())
            {
                Debug.WriteLine("Failed to unregister hot keys.", "ERROR");
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
