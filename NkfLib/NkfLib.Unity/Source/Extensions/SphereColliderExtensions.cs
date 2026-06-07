using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NkfLib.Unity
{
    public static class SphereColliderExtensions
    {
        /// <summary>
        /// バウンディングボックスの計算
        /// ・boundsプロパティは度々ぶれるので独自計算
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Bounds CalcBounds(this SphereCollider self)
        {
            Vector3 center = self.transform.TransformPoint(self.center);

            var diameter = self.radius * 2.0f;
            var size = new Vector3(diameter, diameter, diameter);

            return new Bounds(center, size);
        }

        /// <summary>
        /// バウンディング矩形の投影
        /// ・バウンディングボックスから計算するのが正確だが、垂直の軸にしか回転していない前提で計算を省略している
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Rect ToBoundsRectXZ(this SphereCollider self)
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
        public static Circle ToCircleXY(this SphereCollider self)
        {
            Vector3 center = self.transform.TransformPoint(self.center);
            float lossyScale = Mathf.Max(self.transform.lossyScale.x, self.transform.lossyScale.y);
            return new Circle(center.x, center.y, lossyScale * self.radius);
        }
        public static Circle ToCircleXZ(this SphereCollider self)
        {
            Vector3 center = self.transform.TransformPoint(self.center);
            float lossyScale = Mathf.Max(self.transform.lossyScale.x, self.transform.lossyScale.z);
            return new Circle(center.x, center.z, lossyScale * self.radius);
        }
        public static Circle ToCircleYZ(this SphereCollider self)
        {
            Vector3 center = self.transform.TransformPoint(self.center);
            float lossyScale = Mathf.Max(self.transform.lossyScale.y, self.transform.lossyScale.z);
            return new Circle(center.y, center.z, lossyScale * self.radius);
        }
    }
}