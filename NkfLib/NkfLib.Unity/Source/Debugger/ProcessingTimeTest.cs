using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace NkfLib.Unity
{
    public class ProcessingTimeTest : MonoBehaviour
    {
        bool _measured;

        void Update()
        {
            if (_measured)
                return;
            var sw = new Stopwatch();
            sw.Start();
            int result = Execute();
            sw.Stop();
            Debug.Log("ProcessingLoadTest : " + sw.ElapsedMilliseconds + "ms" + " result:" + result);

            _measured = true;
        }

        int Execute()
        {
#if true
            Rect rect = new Rect(0.0f, 0.0f, 10.0f, 10.0f);
            Quadrangle quad = new Rect(9.0f, 9.0f, 10.0f, 10.0f).ToQuadrangle();
            int count = 0;
            for (var i = 0; i < 10000000; ++i)
            {
                if (rect.Overlaps(quad))
                {
                    ++count;
                }
            }
            return count;
#else
        int count = 0;
        Rect rect = new Rect(0.0f, 0.0f, 10.0f, 10.0f);
        for (var i = 0; i < 10000000; ++i) {
            if (rect.Edge1().Intersect(rect.Edge2(), out Vector2 p, true)) {
                ++count;
            }
        }
        return count;
#endif
        }
    }
}