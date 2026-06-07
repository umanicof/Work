using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace NkfLib
{
    /// <summary>
    /// WPFのUI用ユーティリティ
    /// </summary>
    public static partial class UIElementUtil
    {
        /// <summary>
        /// UIオブジェクトの子リストの取得（型指定）
        /// ・@note ContentControlなどの子リストを持たないUIオブジェクトで機能しない
        /// </summary>
        public static IEnumerable<T> FindChildren<T>(DependencyObject obj) where T : DependencyObject
        {
            if (obj != null) {
                foreach (var child in LogicalTreeHelper.GetChildren(obj)) {
                    if (child != null && child is T) {
                        yield return (T)child;
                    }

                    if (child is DependencyObject) {
                        foreach (T target in FindChildren<T>((DependencyObject)child)) {
                            yield return target;
                        }
                    }
                }
            }
        }
        // VisualTreeHelper使用
        // ・ContentControlなどの子リストを持たないUIオブジェクトで機能する
        public static IEnumerable<T> FindChildren2<T>(DependencyObject obj) where T : DependencyObject
        {
            if (obj != null) {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); ++i) {
                    var child = VisualTreeHelper.GetChild(obj, i);
                    if (child != null && child is T) {
                        yield return (T)child;
                    }

                    if (child is DependencyObject) {
                        foreach (T target in FindChildren2<T>((DependencyObject)child)) {
                            yield return target;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// UIオブジェクトの子の取得（型指定）
        /// </summary>
        public static T FindChild<T>(DependencyObject obj) where T : DependencyObject
        {
            if (obj != null) {
                foreach (var child in LogicalTreeHelper.GetChildren(obj)) {
                    if (child != null && child is T) {
                        return (T)child;
                    }

                    if (child is DependencyObject) {
                        T target = FindChild<T>((DependencyObject)child);
                        if (target != null) {
                            return target;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        ///  UIオブジェクトの子の取得（名前指定、ListViewItem用）
        /// ・なぜかListViewItemでは他のFindChildByNameメソッドなどでは取得できなかった。研究要
        /// ・@note ContentControlなどの子リストを持たないUIオブジェクトで機能しない
        /// </summary>
        public static FrameworkElement FindByName(FrameworkElement root, string name)
        {
            if (root == null) {
                DebugLog.WriteLine("FindByName root is null.");
                return null;
            }

            Stack<FrameworkElement> tree = new Stack<FrameworkElement>();
            tree.Push(root);

            while (tree.Count > 0) {
                FrameworkElement current = tree.Pop();
                if (current.Name == name)
                    return current;

                int count = VisualTreeHelper.GetChildrenCount(current);
                for (int i = 0; i < count; ++i) {
                    DependencyObject child = VisualTreeHelper.GetChild(current, i);
                    if (child is FrameworkElement)
                        tree.Push((FrameworkElement)child);
                }
            }

            return null;
        }

        /// <summary>
        /// UIオブジェクトの子の取得（名前指定）
        /// ・@note ContentControlなどの子リストを持たないUIオブジェクトで機能しない
        /// </summary>
        public static FrameworkElement FindChildByName(DependencyObject obj, string name)
        {
            if (obj != null) {
                foreach (var child in LogicalTreeHelper.GetChildren(obj)) {
                    if (child is FrameworkElement && ((FrameworkElement)child).Name == name) {
                        return (FrameworkElement)child;
                    }

                    if (child is DependencyObject) {
                        FrameworkElement target = FindChildByName((DependencyObject)child, name);
                        if (target != null) {
                            return target;
                        }
                    }
                }
            }
            return null;
        }
        // VisualTreeHelper使用
        // ・ContentControlなどの子リストを持たないUIオブジェクトで機能する
        public static FrameworkElement FindChildByName2(DependencyObject obj, string name)
        {
            if (obj != null) {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); ++i) {
                    var child = VisualTreeHelper.GetChild(obj, i);
                    if (child is FrameworkElement && ((FrameworkElement)child).Name == name) {
                        return (FrameworkElement)child;
                    }

                    if (child is DependencyObject) {
                        FrameworkElement target = FindChildByName((DependencyObject)child, name);
                        if (target != null) {
                            return target;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// UIオブジェクトの子リストの取得（タグ指定）
        /// ・@note ContentControlなどの子リストを持たないUIオブジェクトで機能しない
        /// </summary>
        public static IEnumerable<FrameworkElement> FindChildrenByTag(DependencyObject obj, string tag)
        {
            if (obj != null) {
                foreach (var child in LogicalTreeHelper.GetChildren(obj)) {
                    if (child is FrameworkElement && ((string)((FrameworkElement)child).Tag) == tag) {
                        yield return (FrameworkElement)child;
                    }

                    if (child is DependencyObject) {
                        foreach (FrameworkElement childOfChild in FindChildrenByTag((DependencyObject)child, tag)) {
                            yield return childOfChild;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// UIオブジェクトの子の取得（タグ指定）
        /// ・@note ContentControlなどの子リストを持たないUIオブジェクトで機能しない
        /// </summary>
        public static FrameworkElement FindChildByTag(DependencyObject obj, string tag)
        {
            if (obj != null) {
                foreach (var child in LogicalTreeHelper.GetChildren(obj)) {
                    if (child is FrameworkElement && ((string)((FrameworkElement)child).Tag) == tag) {
                        return (FrameworkElement)child;
                    }

                    if (child is DependencyObject) {
                        FrameworkElement target = FindChildByTag((DependencyObject)child, tag);
                        if (target != null) {
                            return target;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// UIオブジェクトの親の取得（型指定）
        /// </summary>
        public static T FindParent<T>(FrameworkElement obj) where T : FrameworkElement
        {
            if (obj == null) return null;
            if (obj.Parent is T) return (T)obj.Parent;

            return FindParent<T>(obj.Parent as FrameworkElement);
        }

        /// <summary>
        /// UIオブジェクトの階層表示
        /// ・@note ContentControlなどの子リストを持たないUIオブジェクトで機能しない
        /// </summary>
        public static void WriteLineHierarchy(DependencyObject obj, int hierarchy = 0)
        {
            if (obj != null) {
                foreach (var child in LogicalTreeHelper.GetChildren(obj)) {
                    if (child is FrameworkElement) {
                        string sname = ((FrameworkElement)child).Name;
                        if (!String.IsNullOrEmpty(sname)) {
                            sname = " Name:" + sname;
                        }
                        string stag = (string)((FrameworkElement)child).Tag;
                        if (!String.IsNullOrEmpty(stag)) {
                            stag = " Tag:" + stag;
                        }

                        Debug.WriteLine("{0}{1}{2}{3}", new String(' ', hierarchy), child.GetType().Name, sname, stag);
                    }

                    if (child is DependencyObject) {
                        WriteLineHierarchy((DependencyObject)child, hierarchy + 1);
                    }
                }
            }
        }

        /// <summary>
        /// UIオブジェクトの階層表示
        /// </summary>
        public static void WriteLineHierarchy2(DependencyObject obj, int hierarchy = 0)
        {
            if (obj != null) {
                if (obj is FrameworkElement) {
                    string sname = ((FrameworkElement)obj).Name;
                    if (!String.IsNullOrEmpty(sname)) {
                        sname = " Name:" + sname;
                    }
                    string stag = (string)((FrameworkElement)obj).Tag;
                    if (!String.IsNullOrEmpty(stag)) {
                        stag = " Tag:" + stag;
                    }
                    Debug.WriteLine("{0}{1}{2}{3}", new String(' ', hierarchy), obj.GetType().Name, sname, stag);
                }
                else {
                    Debug.WriteLine("{0}{1}", new String(' ', hierarchy), obj.GetType().Name);
                }

                foreach (var child in LogicalTreeHelper.GetChildren(obj)) {
                    if (child is DependencyObject) {
                        WriteLineHierarchy((DependencyObject)child, hierarchy + 1);
                    }
                }
            }
        }

        /// <summary>
        /// リストビューに含まれるコントロールからインデックスを取得（要バインディング）
        /// </summary>
        public static int GetListViewIndexByControl(ListView listView, FrameworkElement control)
        {
            return listView.Items.IndexOf(control.DataContext);
        }

        /// <summary>
        /// カーソル位置の座標を取得
        /// </summary>
        /// <param name="visual"></param>
        /// <returns></returns>
        public static System.Windows.Point GetMousePosition(Visual visual)
        {
            System.Windows.Point swp = new System.Windows.Point(
                    System.Windows.Forms.Cursor.Position.X,
                    System.Windows.Forms.Cursor.Position.Y);

            return visual.PointFromScreen(swp);
        }

        /// <summary>
        /// コントロールのキャプチャ
        /// </summary>
        /// <param name="target"></param>
        /// <param name="fileName"></param>
        ///
        public static BitmapSource DrawToBitmap(Visual target)
        {
            if (target == null) return null;

            Rect bounds = VisualTreeHelper.GetDescendantBounds(target);
            RenderTargetBitmap bitmap = new RenderTargetBitmap((Int32)bounds.Width, (Int32)bounds.Height, 96, 96, PixelFormats.Pbgra32);
            
            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext context = visual.RenderOpen()) {
                VisualBrush visualBrush = new VisualBrush(target);
                context.DrawRectangle(visualBrush, null, new Rect(new Point(), bounds.Size));
            }

            bitmap.Render(visual);

            return bitmap;
        }

        /// <summary>
        /// コントロールのキャプチャ
        /// </summary>
        public static BitmapSource DrawToBitmap(FrameworkElement element, bool loadedWait = true)
        {
            if (element == null) return null;

            element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            element.Arrange(new Rect(element.DesiredSize));

            if (loadedWait) { //@note ロード待ちをすると都合が悪い場合があったのでとりあえずフラグを用意
                element.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { })); // ロード・その他のイベント終了待ち
            }

            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)element.Width, (int)element.Height, Setting.Dpi, Setting.Dpi, PixelFormats.Pbgra32);

            //Rect bounds = VisualTreeHelper.GetDescendantBounds(element);
            //RenderTargetBitmap bitmap = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, 96.0, 96.0, PixelFormats.Pbgra32);
            bitmap.Render(element);

            return bitmap;
        }    
    }
}
