using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Diagnostics;

namespace NkfLib
{
#if false // 【弱参照のプロパティ変更通知イベントの廃止】
    /// <summary>
    /// プロパティ変更通知用インターフェース
    /// </summary>
    public interface IBindablePropertyChanged
    {
        /// <summary>
        /// プロパティ変更通知を行う
        /// </summary>
        void OnBindablePropertyChanged(Object sender, PropertyChangedEventArgs args);
    }
#endif

    /// <summary>
    /// プロパティ変更通知用基底クラス
    /// ・[Serializable]属性を使用しているが、PCLでは使用できない
    /// </summary>
    [Serializable]
    public class BindableBase : INotifyPropertyChanged
    {
#if false // 【弱参照のプロパティ変更通知イベントの廃止】
        // 弱参照のプロパティ変更通知リスト
        List<WeakReference<IBindablePropertyChanged>> targets_ = new List<WeakReference<IBindablePropertyChanged>>();
#endif

        // INotifyPropertyChanged
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual bool SetProperty<T>(ref T field, T value, [CallerMemberName]string name = null)
        {
            if (Equals(field, value)) return false;

            field = value;
            RaisePropertyChanged(name);
            return true;
        }

        void RaisePropertyChanged(string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

#if false // 【弱参照のプロパティ変更通知イベントの廃止】
            // 弱参照のプロパティ変更通知インターフェースへの通知
            var targets = targets_.ToList();
            foreach (var target in targets) {
                IBindablePropertyChanged obj;
                if (target.TryGetTarget(out obj)) {
                    obj.OnBindablePropertyChanged(this, new PropertyChangedEventArgs(name));
                }
                else {
                    targets_.Remove(target);
                }
            }
#endif
#if EDITOR
            // @note ここでMainWindowに変更有りを指定しているため、汎用的に使えない
            MainWindow.Current.IsUpdatedContext = true; // 変更有り
#endif
        }

        /// <summary>
        /// プロパティ変更通知を行う
        /// </summary>
        public void NoticePropertyChanged(string name = null)
        {
            if (name != null) {
                RaisePropertyChanged(name);
            }
            else {
                foreach (var prop in GetType().GetProperties()) {
                    RaisePropertyChanged(prop.Name);
                }
            }
        }

#if false // 【弱参照のプロパティ変更通知イベントの廃止】
        /// <summary>
        /// プロパティ変更通知インターフェースの追加（弱参照）
        /// </summary>
        /// <param name="target"></param>
        public void AddBindablePropetyChanged(IBindablePropertyChanged target)
        {
            targets_.Add(new WeakReference<IBindablePropertyChanged>(target));
        }
#endif

        /*
        // @note 作っては見たものの、エラー時にもデータを更新することになりそうなので、いまいち
        // INotifyDataErrorInfo
        [field: NonSerialized]
        private Dictionary<string, bool> errors_ = new Dictionary<string, bool>();
        [field: NonSerialized]
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        protected virtual void SetError(bool value, [CallerMemberName] string name = null)
        {
            bool last = false;
            if (errors_.ContainsKey(name)) {
                last = errors_[name];
            }
            errors_[name] = value;

            //if (value != last) {
                RiseErrorsChanged(name);
            //}
        }

        protected void RiseErrorsChanged([CallerMemberName] string propertyName = null)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public IEnumerable GetErrors(string name)
        {
            if (!errors_.ContainsKey(name) || !errors_[name]) return null;
            return new [] { "error" };
        }

        public bool HasErrors
        {
            get { return errors_.Values.Any(e => e); }
        }
        */
    }
}
