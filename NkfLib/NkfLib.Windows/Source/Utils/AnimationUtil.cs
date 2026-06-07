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

using System.Windows.Media.Animation;
using System.Diagnostics;

namespace NkfLib
{
    /// <summary>
    /// アニメーションユーティリティ
    /// </summary>
    public static partial class AnimationUtil
    {
        /// <summary>
        /// タイムライン完了イベントハンドラの登録
        /// ・StroyboradやDoubleAnimationの登録に使用する
        /// </summary>
        /// <param name="target"></param>
        /// <param name="timeline"></param>
        /// <param name="completed"></param>
        public static void AddCompletedEventHandler(FrameworkElement target, Timeline timeline, Action<FrameworkElement> completed)
        {
            if (completed == null) return;
            EventHandler handler = null;
            handler = (sender, e) =>
            {
                // senderはAnimationClock
                timeline.Completed -= handler;
                completed(target);
            };
            timeline.Completed += handler;
        }

        /// <summary>
        /// TransformGroupの新規生成
        /// ・子はscale、skew、rotete、translateの順
        /// </summary>
        /// <param name="target"></param>
        public static TransformGroup CreateTransformGroup()
        {
            ScaleTransform scale = new ScaleTransform(1.0, 1.0);
            SkewTransform skew = new SkewTransform(0, 0);
            RotateTransform rotete = new RotateTransform(0);
            TranslateTransform translate = new TranslateTransform(0, 0);

            TransformGroup group = new TransformGroup();
            group.Children.Add(scale);
            group.Children.Add(skew);
            group.Children.Add(rotete);
            group.Children.Add(translate);

            return group;
        }

        /// <summary>
        /// フェードイン
        /// </summary>
        public static void Fadein(FrameworkElement target, double time, Action<FrameworkElement> completed = null)
        {
            DoubleAnimation anim = new DoubleAnimation() { From = 0.0, To = 1.0, Duration = TimeSpan.FromSeconds(time) };
            AddCompletedEventHandler(target, anim, completed);
            target.BeginAnimation(FrameworkElement.OpacityProperty, anim);
        }

        /// <summary>
        /// フェードインストーリーボード作成
        /// </summary>
        public static Storyboard CreateFadein(FrameworkElement target, double time)
        {
            var storyBoard = new Storyboard();
            var animA = new DoubleAnimation { From = 0.0, To = 1.0, Duration = TimeSpan.FromSeconds(time) };
            storyBoard.Children.Add(animA);

            Storyboard.SetTargetProperty(animA, new PropertyPath("Opacity"));
            Storyboard.SetTarget(animA, target);

            return storyBoard;
        }
                
        /// <summary>
        /// フェードアウト
        /// </summary>
        public static void Fadeout(FrameworkElement target, double time, Action<FrameworkElement> completed = null)
        {
            DoubleAnimation anim = new DoubleAnimation() { From = 1.0, To = 0.0, Duration = TimeSpan.FromSeconds(time) };
            AddCompletedEventHandler(target, anim, completed);
            target.BeginAnimation(UIElement.OpacityProperty, anim);
        }

        /// <summary>
        /// フェードアウトストーリーボード作成
        /// </summary>
        public static Storyboard CreateFadeout(FrameworkElement target, double time)
        {
            var storyBoard = new Storyboard();
            var animA = new DoubleAnimation { From = 1.0, To = 0.0, Duration = TimeSpan.FromSeconds(time) };
            storyBoard.Children.Add(animA);

            Storyboard.SetTargetProperty(animA, new PropertyPath("Opacity"));
            Storyboard.SetTarget(animA, target);

            return storyBoard;
        }

        /// <summary>
        /// ポップアップ
        /// </summary>
        public static void Popup(FrameworkElement target, double time, Action<FrameworkElement> completed = null)
        {
            /* この記述は不可。おそらく ScaleTransform.ScaleXProperty が上手く指定できていない
            DoubleAnimation anim = new DoubleAnimation() { From = 0.5, To = 1.0, Duration = TimeSpan.FromSeconds(time) };
            target.RenderTransform = new ScaleTransform(0.5, 0.5);
            AddCompletedEventHandler(target, anim, completed);
            target.BeginAnimation(ScaleTransform.ScaleXProperty, anim);
            */

            //Fadein(target, 1, completed);
            target.RenderTransformOrigin = new Point(0.5, 0.5);
            target.RenderTransform = CreateTransformGroup();

            var storyBoard = new Storyboard();
            var animX = new DoubleAnimation { From = 0.7, To = 1.0, Duration = TimeSpan.FromSeconds(time), EasingFunction = new BackEase() };
            var animY = new DoubleAnimation { From = 0.7, To = 1.0, Duration = TimeSpan.FromSeconds(time), EasingFunction = new BackEase() };
            AddCompletedEventHandler(target, animX, completed); // completedが呼ばれる
            storyBoard.Children.Add(animX);
            storyBoard.Children.Add(animY);

            Storyboard.SetTargetProperty(animX, new PropertyPath("RenderTransform.Children[0].ScaleX"));
            Storyboard.SetTargetProperty(animY, new PropertyPath("RenderTransform.Children[0].ScaleY"));
            Storyboard.SetTarget(animX, target);
            Storyboard.SetTarget(animY, target);
            storyBoard.Begin(target);

            Fadein(target, time * 0.8);
        }

        /// <summary>
        /// ポップアップストーリーボード作成
        /// </summary>
        public static Storyboard CreatePopup(FrameworkElement target, double time)
        {
            target.RenderTransformOrigin = new Point(0.5, 0.5);
            target.RenderTransform = CreateTransformGroup();

            var storyBoard = new Storyboard();
            var animX = new DoubleAnimation { From = 0.7, To = 1.0, Duration = TimeSpan.FromSeconds(time), EasingFunction = new BackEase() };
            var animY = new DoubleAnimation { From = 0.7, To = 1.0, Duration = TimeSpan.FromSeconds(time), EasingFunction = new BackEase() };
            var animA = new DoubleAnimation { From = 0.0, To = 1.0, Duration = TimeSpan.FromSeconds(time * 0.8) };
            storyBoard.Children.Add(animX);
            storyBoard.Children.Add(animY);
            storyBoard.Children.Add(animA);

            Storyboard.SetTargetProperty(animX, new PropertyPath("RenderTransform.Children[0].ScaleX"));
            Storyboard.SetTargetProperty(animY, new PropertyPath("RenderTransform.Children[0].ScaleY"));
            Storyboard.SetTargetProperty(animA, new PropertyPath("Opacity"));
            Storyboard.SetTarget(animX, target);
            Storyboard.SetTarget(animY, target);
            Storyboard.SetTarget(animA, target);

            return storyBoard;
        }
        
        /// <summary>
        /// ポップダウン（ポップアップを閉じる）
        /// </summary>
        public static void Popdown(FrameworkElement target, double time, Action<FrameworkElement> completed = null)
        {
            target.RenderTransform = CreateTransformGroup();

            var storyBoard = new Storyboard();
            var animX = new DoubleAnimation { From = 1.0, To = 0.5, Duration = TimeSpan.FromSeconds(time) };
            var animY = new DoubleAnimation { From = 1.0, To = 0.5, Duration = TimeSpan.FromSeconds(time) };
            AddCompletedEventHandler(target, animX, completed); // completedが呼ばれる
            storyBoard.Children.Add(animX);
            storyBoard.Children.Add(animY);

            Storyboard.SetTargetProperty(animX, new PropertyPath("RenderTransform.Children[0].ScaleX"));
            Storyboard.SetTargetProperty(animY, new PropertyPath("RenderTransform.Children[0].ScaleY"));
            Storyboard.SetTarget(animX, target);
            Storyboard.SetTarget(animY, target);
            storyBoard.Begin(target);

            Fadeout(target, time);
        }

        /// <summary>
        /// ポップダウン（ポップアップを閉じる）ストーリーボード作成
        /// </summary>
        public static Storyboard CreatePopdown(FrameworkElement target, double time)
        {
            target.RenderTransform = CreateTransformGroup();

            var storyBoard = new Storyboard();
            var animX = new DoubleAnimation { From = 1.0, To = 0.5, Duration = TimeSpan.FromSeconds(time) };
            var animY = new DoubleAnimation { From = 1.0, To = 0.5, Duration = TimeSpan.FromSeconds(time) };
            var animA = new DoubleAnimation { From = 1.0, To = 0.0, Duration = TimeSpan.FromSeconds(time) };
            storyBoard.Children.Add(animX);
            storyBoard.Children.Add(animY);
            storyBoard.Children.Add(animA);

            Storyboard.SetTargetProperty(animX, new PropertyPath("RenderTransform.Children[0].ScaleX"));
            Storyboard.SetTargetProperty(animY, new PropertyPath("RenderTransform.Children[0].ScaleY"));
            Storyboard.SetTargetProperty(animA, new PropertyPath("Opacity"));
            Storyboard.SetTarget(animX, target);
            Storyboard.SetTarget(animY, target);
            Storyboard.SetTarget(animA, target);

            return storyBoard;
        }

        /// <summary>
        /// 背景ブラシアニメーションストーリーボード作成
        /// </summary>
        public static Storyboard CreateBackgroundBrush(FrameworkElement target, double time, Color from, Color to)
        {
            var storyBoard = new Storyboard();
            var anim = new ColorAnimation { From = from, To = to, Duration = TimeSpan.FromSeconds(time) };
            storyBoard.Children.Add(anim);

            Storyboard.SetTargetProperty(anim, new PropertyPath("(Background).(SolidColorBrush.Color)"));
            Storyboard.SetTarget(anim, target);

            return storyBoard;
        }
    }
}
