using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NkfLib.Unity
{
    public static class Vector2Extensions
    {
        /// <summary>
        /// 外積
        /// </summary>
        /// <returns></returns>
        public static float Cross(this Vector2 src, Vector2 dst)
        {
            return src.x * dst.y - dst.x * src.y;
        }

        /// <summary>
        /// 回転
        /// </summary>
        /// <returns></returns>
        public static Vector2 Rotate(this Vector2 self, float radian)
        {
            return new Vector2(self.x * Mathf.Cos(radian) - self.y * Mathf.Sin(radian),
                              (self.x * Mathf.Sin(radian) + self.y * Mathf.Cos(radian)));
        }
        public static Vector2 RotateDeg(this Vector2 self, float degree)
        {
            return self.Rotate(Mathf.Deg2Rad * degree);
        }
        public static Vector2 RotatePoint(this Vector2 self, float radian, Vector2 center)
        {
            return (self - center).Rotate(radian) + center;
        }
        public static Vector2 RotatePointDeg(this Vector2 self, float degree, Vector2 center)
        {
            return (self - center).RotateDeg(degree) + center;
        }

        /// <summary>
        /// Vector3変換
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Vector3 FromXY(this Vector2 self)
        {
            return new Vector3(self.x, self.y, 0.0f);
        }
        public static Vector3 FromXZ(this Vector2 self)
        {
            return new Vector3(self.x, 0.0f, self.y);
        }
        public static Vector3 FromYZ(this Vector2 self)
        {
            return new Vector3(0.0f, self.x, self.y);
        }

        /// <summary>
        /// Vector2Int変換
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Vector2Int RoundToInt(this Vector2 self)
        {
            return new Vector2Int(Mathf.RoundToInt(self.x), Mathf.RoundToInt(self.y));
        }
        public static Vector2Int FloorToInt(this Vector2 self)
        {
            return new Vector2Int(Mathf.FloorToInt(self.x), Mathf.FloorToInt(self.y));
        }
        public static Vector2Int CeilToInt(this Vector2 self)
        {
            return new Vector2Int(Mathf.CeilToInt(self.x), Mathf.CeilToInt(self.y));
        }
    }
}