using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NkfLib.Unity
{
    public static class Vector3Extensions
    {
        /// <summary>
        /// 軸の方向（zero, left, right, up, down, forward, back）のうち、近いものに丸める
        /// </summary>
        /// <returns></returns>
        public static Vector3 ToAxisDirection(this Vector3 self)
        {
            float absX = Mathf.Abs(self.x);
            float absY = Mathf.Abs(self.y);
            float absZ = Mathf.Abs(self.z);
            float absMax = Mathf.Max(absX, absY, absZ);
            if (absMax == 0.0f)
                return Vector3.zero;
            if (absMax == absX)
                return self.x >= 0.0f ? Vector3.right : Vector3.left;
            if (absMax == absY)
                return self.y >= 0.0f ? Vector3.up : Vector3.down;
            return self.z >= 0.0f ? Vector3.forward : Vector3.back;
        }

        /// <summary>
        /// 丸め
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Vector3 Round(this Vector3 self)
        {
            return new Vector3(Mathf.Round(self.x), Mathf.Round(self.y), Mathf.Round(self.z));
        }
        public static Vector3 Floor(this Vector3 self)
        {
            return new Vector3(Mathf.Floor(self.x), Mathf.Floor(self.y), Mathf.Floor(self.z));
        }
        public static Vector3 Ceil(this Vector3 self)
        {
            return new Vector3(Mathf.Ceil(self.x), Mathf.Ceil(self.y), Mathf.Ceil(self.z));
        }
        public static Vector3Int RoundToInt(this Vector3 self)
        {
            return Vector3Int.RoundToInt(self);
        }
        public static Vector3Int FloorToInt(this Vector3 self)
        {
            return Vector3Int.FloorToInt(self);
        }
        public static Vector3Int CeilToInt(this Vector3 self)
        {
            return Vector3Int.CeilToInt(self);
        }

        /// <summary>
        /// Vector2変換
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Vector2 ToXY(this Vector3 self)
        {
            return new Vector2(self.x, self.y);
        }
        public static Vector2 ToXZ(this Vector3 self)
        {
            return new Vector2(self.x, self.z);
        }
        public static Vector2 ToYZ(this Vector3 self)
        {
            return new Vector2(self.y, self.z);
        }

        /// <summary>
        /// Vector2変換丸め
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        //public static Vector2Int RoundToIntXY(this Vector3 self)
        //{
        //    return new Vector2Int(Mathf.RoundToInt(self.x), Mathf.RoundToInt(self.y));
        //}
        public static Vector2Int RoundToIntXZ(this Vector3 self)
        {
            return new Vector2Int(Mathf.RoundToInt(self.x), Mathf.RoundToInt(self.z));
        }
        //public static Vector2Int RoundToIntYZ(this Vector3 self)
        //{
        //    return new Vector2Int(Mathf.RoundToInt(self.y), Mathf.RoundToInt(self.z));
        //}
        //public static Vector2Int FloorToIntXY(this Vector3 self)
        //{
        //    return new Vector2Int(Mathf.FloorToInt(self.x), Mathf.FloorToInt(self.y));
        //}
        public static Vector2Int FloorToIntXZ(this Vector3 self)
        {
            return new Vector2Int(Mathf.FloorToInt(self.x), Mathf.FloorToInt(self.z));
        }
        //public static Vector2Int FloorToIntYZ(this Vector3 self)
        //{
        //    return new Vector2Int(Mathf.FloorToInt(self.y), Mathf.FloorToInt(self.z));
        //}
        //public static Vector2Int CeilToIntXY(this Vector3 self)
        //{
        //    return new Vector2Int(Mathf.CeilToInt(self.x), Mathf.CeilToInt(self.y));
        //}
        public static Vector2Int CeilToIntXZ(this Vector3 self)
        {
            return new Vector2Int(Mathf.CeilToInt(self.x), Mathf.CeilToInt(self.z));
        }
        //public static Vector2Int CeilToIntYZ(this Vector3 self)
        //{
        //    return new Vector2Int(Mathf.CeilToInt(self.y), Mathf.CeilToInt(self.z));
        //}
    }
}