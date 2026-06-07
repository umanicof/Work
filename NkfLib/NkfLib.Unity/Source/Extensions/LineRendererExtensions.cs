using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NkfLib.Unity
{
    public static class LineRendererExtensions
    {
        /// <summary>
        /// バウンディングボックス取得
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Rect GetBoundsRect(this LineRenderer self)
        {
            float xMin = float.MaxValue;
            float xMax = float.MinValue;
            float yMin = float.MaxValue;
            float yMax = float.MinValue;

            if (self.positionCount == 0)
            {
                if (self.useWorldSpace)
                {
                    xMin = self.transform.position.x;
                    xMax = self.transform.position.x;
                    yMin = self.transform.position.y;
                    yMax = self.transform.position.y;
                }
                else
                {
                    xMin = 0.0f;
                    xMax = 0.0f;
                    yMin = 0.0f;
                    yMax = 0.0f;
                }
            }
            else
            {
                for (int i = 0; i < self.positionCount; ++i)
                {
                    var p = self.GetPosition(i);
                    xMin = Math.Min(p.x, xMin);
                    xMax = Math.Max(p.x, xMax);
                    yMin = Math.Min(p.y, yMin);
                    yMax = Math.Max(p.y, yMax);
                }
            }

            if (!self.useWorldSpace)
            {
                xMin += self.transform.position.x;
                xMax += self.transform.position.x;
                yMin += self.transform.position.y;
                yMax += self.transform.position.y;
            }

            return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
        }
    }
}