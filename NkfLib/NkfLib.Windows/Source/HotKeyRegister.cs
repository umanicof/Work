using NkfLib;
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

namespace NkfLib
{
    public class HotKeyItem
    {
        public ModifierKeys ModifierKeys { get; private set; }
        public Key Key { get; private set; }
        public EventHandler Handler { get; set; }

        public HotKeyItem(ModifierKeys modKey, Key key, EventHandler handler)
        {
            this.ModifierKeys = modKey;
            this.Key = key;
            this.Handler = handler;
        }
    }

    /// <summary>
    /// HotKeyの登録・解除クラス
    /// ・出典：https://gogowaten.hatenablog.com/entry/2020/12/11/132125
    /// 　　　  https://sourcechord.hatenablog.com/entry/2017/02/13/005456
    /// ※そのうちRoutedCommandを登録できるよう修正するかも
    /// </summary>
    public class HotKeyRegister : DisposePattern
    {
        private Dictionary<int, HotKeyItem> _hotkeyList = new Dictionary<int, HotKeyItem>();

        private const int MAX_HOTKEY_ID = 0xC000;

        #region WindowsAPI
        private const int WM_HOTKEY = 0x0312;

        [DllImport("user32.dll")]
        private static extern int RegisterHotKey(IntPtr hWnd, int id, int modKey, int vKey);

        [DllImport("user32.dll")]
        private static extern int UnregisterHotKey(IntPtr hWnd, int id);
        #endregion WindowsAPI

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="window"></param>
        public HotKeyRegister()
        {
            ComponentDispatcher.ThreadPreprocessMessage += ComponentDispatcher_ThreadPreprocessMessage;

            disposed += () => UnregisterAll();
        }

        /// <summary>
        /// APIコールバック関数
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="handled"></param>
        private void ComponentDispatcher_ThreadPreprocessMessage(ref MSG msg, ref bool handled)
        {
            if (msg.message != WM_HOTKEY) 
                return;

            var id = msg.wParam.ToInt32();
            if (!_hotkeyList.TryGetValue(id, out HotKeyItem hotkey))
                return;

            hotkey.Handler?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 引数で指定された内容で、HotKeyを登録します。
        /// ・同じModifierKeys, Keyが登録された場合、イベントハンドラが登録された上で同じidが返却されます。
        /// </summary>
        /// <param name="modKey"></param>
        /// <param name="key"></param>
        /// <param name="handler"></param>
        /// <returns>id</returns>
        public int Register(ModifierKeys modKey, Key key, EventHandler handler)
        {
            var modKeyNum = (int)modKey;
            var vKey = KeyInterop.VirtualKeyFromKey(key);

            // 同じModifierKey、Keyのデータを検索。存在するならイベントハンドラを追加する
            (int id, var hotkey) = _hotkeyList.FirstOrDefault(x => x.Value.ModifierKeys == modKey && x.Value.Key == key);
            if (hotkey != null)
            {
                DebugLog.WriteLine($"[HotKeyRegister] Register Add Hander id:{id}");
                hotkey.Handler += handler;
                return id;
            }

            // HotKey登録
            var ids = _hotkeyList.Keys.ToHashSet();
            for (id = 0; id < MAX_HOTKEY_ID; ++id)
            {
                if (ids.Contains(id))
                    continue;

                var ret = RegisterHotKey(IntPtr.Zero, id, modKeyNum, vKey); // ウィンドウハンドルは指定せず、メッセージキューのメッセージを受ける
                if (ret != 0)
                {
                    DebugLog.WriteLine($"[HotKeyRegister] Register Add List id:{id}");
                    // HotKeyのリストに追加
                    hotkey = new HotKeyItem(modKey, key, handler);
                    _hotkeyList.Add(id, hotkey);
                    return id;
                }
            }

            return -1;
        }

        /// <summary>
        /// 指定idのHotKeyを登録解除します。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Unregister(int id)
        {
            bool success = UnregisterHotKey(IntPtr.Zero, id) != 0;
            if (success)
            {
                _hotkeyList.Remove(id);
                DebugLog.WriteLine($"[HotKeyRegister] Unregister id:{id}");
            }
            return success;
        }

        /// <summary>
        /// 指定modKeyとkeyに対応するHotKeyを登録解除します。
        /// </summary>
        /// <param name="modKey"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Unregister(ModifierKeys modKey, Key key)
        {
            var item = _hotkeyList.FirstOrDefault(x => x.Value.ModifierKeys == modKey && x.Value.Key == key);

            if (item.Equals(default(KeyValuePair<int, HotKeyItem>))) // 初期値（検索失敗）
                return false;

            return Unregister(item.Key);
        }

        /// <summary>
        /// 指定ハンドラを持つHotKeyを登録解除します。
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public bool Unregister(EventHandler handler)
        {
            var item = _hotkeyList.FirstOrDefault(x => x.Value.Handler.GetInvocationList().Contains(handler));

            if (item.Equals(default(KeyValuePair<int, HotKeyItem>))) // 初期値（検索失敗）
                return false;

            return (item.Value.Handler == null) ? Unregister(item.Key) : true;
        }

        /// <summary>
        /// 登録済みのすべてのHotKeyを解除します。
        /// </summary>
        /// <returns></returns>
        public void UnregisterAll()
        {
            foreach (var item in this._hotkeyList.ToArray())
            {
               Unregister(item.Key);
            }
        }
    }
}