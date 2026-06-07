using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Controls.Primitives;
using System.Diagnostics;


namespace NkfLib
{
    /// <summary>
    /// 選択可能オブジェクト（Grid）
    /// ・EditorのDesignSelectableから改変
    /// ・Mix-in的な記述も考えたが、そこまで必要なさそう
    /// ・選択中のオブジェクトをstaicメソッドで取得可能
    /// ・複数選択は非対応
    /// </summary>
    public class Selectable : Grid
    {
        static List<Selectable> Objects { get; } = new List<Selectable>(); // オブジェクトのリスト

        /// <summary>
        /// 選択中のインデックス
        /// </summary>
        static int selectedIndex_ = -1;
        public static int SelectedIndex
        {
            get { return selectedIndex_; }
            set {
                Debug.Assert(value >= -1 && value < Objects.Count); // パラメータチェック
                if (selectedIndex_ == value) return;
                var oldValue = selectedIndex_;
                selectedIndex_ = value; // イベント発行前に切り替えておく必要がある

                // 選択枠の削除、追加
                Selectable obj;
                if (oldValue != -1) {
                    obj = Objects[oldValue];
                    obj.RemoveBorder();
                    obj.SelectionChanged?.Invoke(obj); // イベント発行
                }
                if (value != -1) {
                    obj = Objects[value];
                    obj.AddBorder();
                    obj.SelectionChanged?.Invoke(obj); // イベント発行
                }
            }
        }

        /// <summary>
        /// 選択変更イベント
        /// </summary>
        public event Action<Selectable> SelectionChanged;

        /// <summary>
        /// 自身のインデックス
        /// </summary>
        int Index { get { return Objects.IndexOf(this); } }
        
        /// <summary>
        /// 選択中
        /// </summary>
        public bool IsSelected
        {
            get { return Index == SelectedIndex; }
            set {
                if (value) {
                    SelectedIndex = Index;
                }
                else {
                    if (Index == SelectedIndex) {
                        SelectedIndex = -1;
                    } 
                }
            }
        }

        /// <summary>
        /// 選択枠
        /// </summary>
        Border border { get; } = new Border { BorderBrush = Brushes.Aqua, BorderThickness = new Thickness(2) };
        
        /// <summary>
        /// 選択枠の追加
        /// </summary>
        void AddBorder()
        {
            Children.Add(border);
        }

        /// <summary>
        /// 選択枠の削除
        /// </summary>
        void RemoveBorder()
        {
            Children.Remove(border);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Selectable()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;

            MouseLeftButtonDown += OnMouseLeftButtonDown;
        }

        /// <summary>
        /// ロードイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnLoaded(Object sender, RoutedEventArgs e)
        {
            Objects.Add(this);
        }

        /// <summary>
        /// アンロードイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnUnloaded(Object sender, RoutedEventArgs e)
        {
            // SelectedIndexの補正
            if (selectedIndex_ != -1) {
                if (selectedIndex_ == Index) {
                    selectedIndex_ = -1;
                }
                if (selectedIndex_ > Index) {
                    --selectedIndex_;
                }
            }

            Objects.Remove(this);
        }

        /// <summary>
        /// 押下イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IsSelected = true;
            e.Handled = true; // イベントキャンセル
        }

        /// <summary>
        /// 前のオブジェクトの選択
        /// </summary>
        /// <returns>true:範囲を超えた</returns>
        public static bool PrevSelect()
        {
            if (Objects.Count <= 0) return true;
            if (SelectedIndex == 0) return true; // ローテートさせずに返す

            if (SelectedIndex == -1) {
                SelectedIndex = Objects.Count - 1;
            }
            else {
                SelectedIndex = Util.Loop(SelectedIndex - 1, Objects.Count);
            }

            return false;
        }

        /// <summary>
        /// 次のオブジェクトの選択
        /// </summary>
        /// <returns>true:範囲を超えた</returns>
        public static bool NextSelect()
        {
            if (Objects.Count <= 0) return true;
            if (SelectedIndex == Objects.Count - 1) return true; // ローテートさせずに返す

            if (SelectedIndex == -1) {
                SelectedIndex = 0;
            }
            else {
                SelectedIndex = Util.Loop(SelectedIndex + 1, Objects.Count);
            }

            return false;
        }
    }
}
