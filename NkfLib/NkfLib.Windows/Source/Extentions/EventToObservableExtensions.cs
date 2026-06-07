using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NkfLib
{
    // <summary>
    /// WPFのイベント => Observable 変換用ヘルパクラス
    /// ・Disposeの呼び出しが必要かどうかの判断について
    ///   IObservableのSubscribeを呼ぶと、イベントにハンドラが追加された上で、disposableを返却する。
    ///   重要なのはハンドラであって、disposableは解放するための手段に過ぎない。別々に考えると良い。
    ///   ここの例で言えば、自身のクラスのイベントにハンドラが追加されただけなのだから、自身のクラスが破棄されるのであれば自動的にハンドラも解除される。
    ///   すなわち自身のクラスのイベントから作ったObservableであれば、disposable.Dispaseを呼び出す必要はない。
    /// </summary>
    public static partial class EventToObservableExtensions
    {
        ///------------------------------------------------------------------
        /// UIElement
        ///------------------------------------------------------------------
        /// <summary>
        /// MouseDownイベントObservable
        /// </summary>
        public static IObservable<MouseButtonEventArgs> MouseDownAsObservable(this UIElement self)
        {
            return Observable.FromEvent<MouseButtonEventHandler, MouseButtonEventArgs>(
                    h => (s, e) => h(e),
                    h => self.MouseDown += h,
                    h => self.MouseDown -= h);
        }

        /// <summary>
        /// MouseMoveイベントObservable
        /// </summary>
        public static IObservable<MouseEventArgs> MouseMoveAsObservable(this UIElement self)
        {
            return Observable.FromEvent<MouseEventHandler, MouseEventArgs>(
                    h => (s, e) => h(e),
                    h => self.MouseMove += h,
                    h => self.MouseMove -= h);
        }

        /// <summary>
        /// MouseMoveイベントObservable
        /// </summary>
        public static IObservable<MouseButtonEventArgs> MouseUpAsObservable(this UIElement self)
        {
            return Observable.FromEvent<MouseButtonEventHandler, MouseButtonEventArgs>(
                    h => (s, e) => h(e),
                    h => self.MouseUp += h,
                    h => self.MouseUp -= h);
        }

        /// <summary>
        /// MouseEnterイベントObservable
        /// </summary>
        public static IObservable<MouseEventArgs> MouseEnterAsObservable(this UIElement self)
        {
            return Observable.FromEvent<MouseEventHandler, MouseEventArgs>(
                    h => (s, e) => h(e),
                    h => self.MouseEnter += h,
                    h => self.MouseEnter -= h);
        }

        /// <summary>
        /// MouseEnterイベントObservable
        /// </summary>
        public static IObservable<MouseEventArgs> MouseLeaveAsObservable(this UIElement self)
        {
            return Observable.FromEvent<MouseEventHandler, MouseEventArgs>(
                    h => (s, e) => h(e),
                    h => self.MouseLeave += h,
                    h => self.MouseLeave -= h);
        }

        ///------------------------------------------------------------------
        /// FrameworkElement
        ///------------------------------------------------------------------
        /// <summary>
        /// InitializeイベントObservable
        /// </summary>
        public static IObservable<EventArgs> InitializeAsObservable(this FrameworkElement self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Initialized += h,
                h => self.Initialized -= h);
        }

        /// <summary>
        /// LoadedイベントObservable
        /// </summary>
        public static IObservable<RoutedEventArgs> LoadedAsObservable(this FrameworkElement self)
        {
            return Observable.FromEvent<RoutedEventHandler, RoutedEventArgs>(
                h => (s, e) => h(e),
                h => self.Loaded += h,
                h => self.Loaded -= h);
        }

        /// <summary>
        /// UnloadedイベントObservable
        /// </summary>
        public static IObservable<RoutedEventArgs> UnloadedAsObservable(this FrameworkElement self)
        {
            return Observable.FromEvent<RoutedEventHandler, RoutedEventArgs>(
                h => (s, e) => h(e),
                h => self.Loaded += h,
                h => self.Loaded -= h);
        }

        /// <summary>
        /// Canvas内におけるFrameworkElementのバウンディングボックスを指定する
        /// </summary>
        /// <param name="element"></param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public static FrameworkElement SetCanvasBounds(this FrameworkElement self, Rect bounds)
        {
            self.Width = bounds.Width;
            self.Height = bounds.Height;
            Canvas.SetTop(self, bounds.Top);
            Canvas.SetLeft(self, bounds.Left);
            return self;
        }

        /// <summary>
        /// Canvas内におけるFrameworkElementのバウンディングボックスを取得する
        /// </summary>
        /// <param name="element"></param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public static Rect GetCanvasBounds(this FrameworkElement element)
        {
            Rect rect = new();
            rect.Width = element.ActualWidth;
            rect.Height = element.ActualHeight;
            rect.Y = Canvas.GetTop(element);
            rect.X = Canvas.GetLeft(element);
            return rect;
        }

        ///------------------------------------------------------------------
        /// Window
        ///------------------------------------------------------------------
        /// <summary>
        /// ActivatedイベントObservable
        /// </summary>
        public static IObservable<EventArgs> ActivatedAsObservable(this Window self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Activated += h,
                h => self.Activated -= h);
        }
        
        /// <summary>
        /// DeactivatedイベントObservable
        /// </summary>
        public static IObservable<EventArgs> DeactivatedAsObservable(this Window self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Deactivated += h,
                h => self.Deactivated -= h);
        }

        /// <summary>
        /// ClosingイベントObservable
        /// </summary>
        public static IObservable<CancelEventArgs> ClosingAsObservable(this Window self)
        {
            return Observable.FromEvent<CancelEventHandler, CancelEventArgs>(
                h => (s, e) => h(e),
                h => self.Closing += h,
                h => self.Closing -= h);
        }

        /// <summary>
        /// ClosingイベントObservable
        /// </summary>
        public static IObservable<EventArgs> ClosedAsObservable(this Window self)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                h => (s, e) => h(e),
                h => self.Closed += h,
                h => self.Closed -= h);
        }
    }
}
