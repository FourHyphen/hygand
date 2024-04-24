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
        private const string IniFileName = "HotKey.ini";
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
            (ModifierKeys, Key) keys = GetHotKeyTogglingDisplayOnOff();
            _hotKey.Register(keys.Item1, keys.Item2, handler);
        }

        private (ModifierKeys, Key) GetHotKeyTogglingDisplayOnOff()
        {
            AppIniFile ini = new AppIniFile(IniFileName);
            return ini.TogglingDisplayOnOff();
        }

        public void Dispose()
        {
            _hotKey?.Dispose();
        }
    }
}
