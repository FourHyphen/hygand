using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MyFileLauncher
{
    internal class AppHotKey : IDisposable
    {
        private HotKey _hotKey;

        internal static AppHotKey CreateInstance(Window window)
        {
            return new AppHotKey(window);
        }

        private AppHotKey(Window window)
        {
            _hotKey = new HotKey(window);
        }

        internal void RegisterTogglingDisplayOnOff(EventHandler handler)
        {
            _hotKey.Register(ModifierKeys.Shift, Key.CapsLock, handler);
        }

        public void Dispose()
        {
            _hotKey?.Dispose();
        }
    }
}
