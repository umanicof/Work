using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NkfLib.Unity
{
    public readonly struct Circle
    {
        public readonly float radious;
        public readonly Vector2 center;

        public readonly float xMin;
        public readonly float yMin;
        public readonly float xMax;
        public readonly float yMax;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Circle(Vector2 center, float radious)
        {
            this.center = center;
            this.radious = radious;

            // 事前計算してしまう
            xMin = center.x - radious;
            yMin = center.y - radious;
            xMax = center.x + radious;
            yMax = center.y + radious;
        }
        public Circle(float centerX, float centerY, float radious)
            : this(new Vector2(centerX, centerY), radious)
        {
        }

        /// <summary>
        /// 指定の点が含まれている
        /// </summary>
        /// <param name="point"></param>
        public bool Contains(Vector2 point)
        {
            return point.sqrMagnitude < radious * radious;
        }

        /// <summary>
        /// 重なっている
        /// </summary>
        /// <param name="separation">分離ベクトル</param>
        public bool Overlaps(in Circle dst)
        {
            return (dst.center - center).sqrMagnitude < Mathf.Pow(radious + dst.radious, 2);
        }
        public bool Overlaps(in Circle dst, out Vector2 separation)
        {
            separation = default;
            var direction = dst.center - center;
            var distance = direction.magnitude;
            if (distance < radious + dst.radious) return false;
            separation = direction * (radious + dst.radious - distance) / distance; // distanceは0ではないだろう
            return true;
        }
        public bool Overlaps(Rect rect)
        {
            return rect.Overlaps(this);
        }
        public bool Overlaps(Rect rect, out Vector2 separation)
        {
            if (!rect.Overlaps(this, out separation)) return false;
            separation = -separation; // 逆ベクトル
            return true;
        }
        public bool Overlaps(in RotationRect rect)
        {
            return rect.Overlaps(this);
        }
        public bool Overlaps(in RotationRect rect, out Vector2 separation)
        {
            if (!rect.Overlaps(this, out separation)) return false;
            separation = -separation; // 逆ベクトル
            return true;
        }

        /// <summary>
        /// 回転
        /// </summary>
        /// <returns></returns>
        public Circle Rotate(float radian, Vector2 center)
        {
            return new Circle(this.center.RotatePoint(radian, center), radious);
        }
    }
}