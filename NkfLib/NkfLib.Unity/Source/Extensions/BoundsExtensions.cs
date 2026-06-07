using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NkfLib.Unity
{
    public static class BoundsExtensions
    {
        /// <summary>
        /// 矩形への投影
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Rect ToRectXY(this Bounds self)
        {
            return new Rect(self.center.x - self.extents.x, self.center.y - self.extents.y, self.size.x, self.size.y);
        }
        public static Rect ToRectXZ(this Bounds self)
        {
            return new Rect(self.center.x - self.extents.x, self.center.z - self.extents.z, self.size.x, self.size.z);
        }
        public static Rect ToRectYZ(this Bounds self)
        {
            return new Rect(self.center.y - self.extents.y, self.center.z - self.extents.z, self.size.y, self.size.z);
        }
    }
}