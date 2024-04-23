using System;
using System.Windows.Input;

namespace MyFileLauncher
{
    internal class HotKeyItem
    {
        public int RegisteredId { get; }

        public ModifierKeys ModifierKeys { get; }

        public Key Key { get; }

        public EventHandler Handler { get; }

        public HotKeyItem(int id, ModifierKeys modKey, Key key, EventHandler handler)
        {
            RegisteredId = id;
            ModifierKeys = modKey;
            Key = key;
            Handler = handler;
        }
    }
}