using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NkfLib.Unity
{
    public static class CapsuleColliderExtensions
    {
        /// <summary>
        /// バウンディングボックスの計算
        /// ・boundsプロパティは度々ぶれるので独自計算
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Bounds CalcBounds(this CapsuleCollider self)
        {
            // 合ってるか自信ない
            float y1 = self.center.y - (self.height / 2.0f - self.radius);
            float y2 = self.center.y + (self.height / 2.0f - self.radius);

            Vector3 center1 = self.transform.TransformPoint(new Vector3(0.0f, y1, 0.0f));
            Vector3 center2 = self.transform.TransformPoint(new Vector3(0.0f, y2, 0.0f));

            float xMin = Mathf.Min(center1.x - self.radius, center2.x - self.radius);
            float xMax = Mathf.Max(center1.x + self.radius, center2.x + self.radius);
            float yMin = Mathf.Min(center1.y - self.radius, center2.y - self.radius);
            float yMax = Mathf.Max(center1.y + self.radius, center2.y + self.radius);
            float zMin = Mathf.Min(center1.z - self.radius, center2.z - self.radius);
            float zMax = Mathf.Max(center1.z + self.radius, center2.z + self.radius);

            var size = new Vector3(xMax - xMin, yMax - yMin, zMax - zMin);
            var center = new Vector3(xMin + size.x / 2.0f, yMin + size.y / 2.0f, zMin + size.z / 2.0f);

            return new Bounds(center, size);
        }

        /// <summary>
        /// バウンディング矩形の投影
        /// ・バウンディングボックスから計算するのが正確だが、垂直の軸にしか回転していない前提で計算を省略している
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Rect ToBoundsRectXZ(this CapsuleCollider self)
        {
            Vector3 center = self.transform.TransformPoint(self.center);
            float lossyScale = Mathf.Max(self.transform.lossyScale.x, self.transform.lossyScale.z);
            float extents = lossyScale * self.radius;
            return new Rect(center.x - extents, center.z - extents, extents * 2.0f, extents * 2.0f);
        }

        /// <summary>
        /// 円の投影
        /// ・垂直の軸にしか回転していない前提で計算を省略している
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Circle ToCircleXZ(this CapsuleCollider self)
        {
            Vector3 center = self.transform.TransformPoint(self.center);
            float lossyScale = Mathf.Max(self.transform.lossyScale.x, self.transform.lossyScale.z);
            return new Circle(center.x, center.z, lossyScale * self.radius);
        }
    }
}