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

using System.Diagnostics;

namespace NkfLib
{
    /// <summary>
    /// タッチスクロール可能なスクロールビューワ
    /// </summary>
    public class TouchScrollViewer : ScrollViewer
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TouchScrollViewer()
        {
            PanningMode = PanningMode.VerticalOnly;

            ManipulationBoundaryFeedback += OnManipulationBoundaryFeedback;
        }

        /// <summary>
        /// ManipulationBoundaryFeedback イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            // スワイプ時にメインウィンドウが動かないようにする対応
            e.Handled = true;
        }
    }
}
