using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NkfLib.Unity
{
    public static class BoxColliderExtensions
    {
#if false // まとめると逆に面倒くさい
        /// <summary>
        /// 頂点取得
        /// </summary>
        /// <returns></returns>
        static float X1(this BoxCollider self) { return self.center.x - self.size.x / 2.0f; }
        static float Y1(this BoxCollider self) { return self.center.y - self.size.y / 2.0f; }
        static float Z1(this BoxCollider self) { return self.center.z - self.size.z / 2.0f; }
        static float X2(this BoxCollider self) { return self.center.x + self.size.x / 2.0f; }
        static float Y2(this BoxCollider self) { return self.center.y + self.size.y / 2.0f; }
        static float Z2(this BoxCollider self) { return self.center.z + self.size.z / 2.0f; }
        public static Vector3 P1(this BoxCollider self) { return self.transform.TransformPoint(new Vector3(self.X1(), self.Y1(), self.Z1())); }
        public static Vector3 P2(this BoxCollider self) { return self.transform.TransformPoint(new Vector3(self.X1(), self.Y2(), self.Z1())); }
        public static Vector3 P3(this BoxCollider self) { return self.transform.TransformPoint(new Vector3(self.X2(), self.Y2(), self.Z1())); }
        public static Vector3 P4(this BoxCollider self) { return self.transform.TransformPoint(new Vector3(self.X2(), self.Y1(), self.Z1())); }
        public static Vector3 P5(this BoxCollider self) { return self.transform.TransformPoint(new Vector3(self.X1(), self.Y1(), self.Z2())); }
        public static Vector3 P6(this BoxCollider self) { return self.transform.TransformPoint(new Vector3(self.X1(), self.Y2(), self.Z2())); }
        public static Vector3 P7(this BoxCollider self) { return self.transform.TransformPoint(new Vector3(self.X2(), self.Y2(), self.Z2())); }
        public static Vector3 P8(this BoxCollider self) { return self.transform.TransformPoint(new Vector3(self.X2(), self.Y2(), self.Z2())); }
        public static Vector3[] Points(this BoxCollider self)
        {
            var a = new Vector3[8];
            a[0] = self.P1();
            a[1] = self.P2();
            a[2] = self.P3();
            a[3] = self.P4();
            a[4] = self.P5();
            a[5] = self.P6();
            a[6] = self.P7();
            a[7] = self.P8();
            return a;
        }
#endif

        /// <summary>
        /// バウンディングボックスの計算
        /// ・boundsプロパティは度々ぶれるので独自計算
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Bounds CalcBounds(this BoxCollider self)
        {
            float x1 = self.center.x - self.size.x / 2.0f;
            float y1 = self.center.y - self.size.y / 2.0f;
            float z1 = self.center.z - self.size.z / 2.0f;
            float x2 = self.center.x + self.size.x / 2.0f;
            float y2 = self.center.y + self.size.y / 2.0f;
            float z2 = self.center.z + self.size.z / 2.0f;

            Vector3 p1 = self.transform.TransformPoint(new Vector3(x1, y1, z1));
            Vector3 p2 = self.transform.TransformPoint(new Vector3(x1, y1, z2));
            Vector3 p3 = self.transform.TransformPoint(new Vector3(x1, y2, z1));
            Vector3 p4 = self.transform.TransformPoint(new Vector3(x1, y2, z2));
            Vector3 p5 = self.transform.TransformPoint(new Vector3(x2, y1, z1));
            Vector3 p6 = self.transform.TransformPoint(new Vector3(x2, y1, z2));
            Vector3 p7 = self.transform.TransformPoint(new Vector3(x2, y2, z1));
            Vector3 p8 = self.transform.TransformPoint(new Vector3(x2, y2, z2));

            float xMin = Mathf.Min(p1.x, p2.x, p3.x, p4.x, p5.x, p6.x, p7.x, p8.x);
            float xMax = Mathf.Max(p1.x, p2.x, p3.x, p4.x, p5.x, p6.x, p7.x, p8.x);
            float yMin = Mathf.Min(p1.y, p2.y, p3.y, p4.y, p5.y, p6.y, p7.y, p8.y);
            float yMax = Mathf.Max(p1.y, p2.y, p3.y, p4.y, p5.y, p6.y, p7.y, p8.y);
            float zMin = Mathf.Min(p1.z, p2.z, p3.z, p4.z, p5.z, p6.z, p7.z, p8.z);
            float zMax = Mathf.Max(p1.z, p2.z, p3.z, p4.z, p5.z, p6.z, p7.z, p8.z);

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
        public static Rect ToBoundsRectXY(this BoxCollider self)
        {
            // 回転有無で場合分け
            return (self.transform.rotation == Quaternion.identity) ? ToRectXY(self) : ToRotationRectXY(self).BoundsRect();
        }
        public static Rect ToBoundsRectXZ(this BoxCollider self)
        {
            // 回転有無で場合分け
            return (self.transform.rotation == Quaternion.identity) ? ToRectXZ(self) : ToRotationRectXZ(self).BoundsRect();
        }
        public static Rect ToBoundsRectYZ(this BoxCollider self)
        {
            // 回転有無で場合分け
            return (self.transform.rotation == Quaternion.identity) ? ToRectYZ(self) : ToRotationRectYZ(self).BoundsRect();
        }

        /// <summary>
        /// 円の投影
        /// ・垂直の軸にしか回転していない前提で計算を省略している
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Circle ToCircleXY(this BoxCollider self, bool useMinSegment = false)
        {
            Vector3 center = self.transform.TransformPoint(self.center);
            bool useH = useMinSegment ? self.size.x < self.size.y : self.size.x > self.size.y;
            float radious = (useH ? self.size.x : self.size.y) / 2.0f;
            float lossyScale = useH ? self.transform.lossyScale.x : self.transform.lossyScale.y;
            return new Circle(center.x, center.y, lossyScale * radious);
        }
        public static Circle ToCircleXZ(this BoxCollider self, bool useMinSegment = false)
        {
            Vector3 center = self.transform.TransformPoint(self.center);
            bool useH = useMinSegment ? self.size.x < self.size.z : self.size.x > self.size.z;
            float radious = (useH ? self.size.x : self.size.z) / 2.0f;
            float lossyScale = useH ? self.transform.lossyScale.x : self.transform.lossyScale.z;
            return new Circle(center.x, center.z, lossyScale * radious);
        }
        public static Circle ToCircleYZ(this BoxCollider self, bool useMinSegment = false)
        {
            Vector3 center = self.transform.TransformPoint(self.center);
            bool useH = useMinSegment ? self.size.y < self.size.z : self.size.y > self.size.z;
            float radious = (useH ? self.size.y : self.size.z) / 2.0f;
            float lossyScale = useH ? self.transform.lossyScale.y : self.transform.lossyScale.z;
            return new Circle(center.y, center.z, lossyScale * radious);
        }

        /// <summary>
        /// 矩形の投影
        /// ・回転を無視して矩形を取得。内部用。
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        static Rect ToRectXY(this BoxCollider self)
        {
            Vector3 center = self.transform.TransformPoint(self.center);
            float width = self.transform.lossyScale.x * self.size.x;
            float height = self.transform.lossyScale.y * self.size.y;
            return new Rect(center.x - width / 2.0f, center.y - height / 2.0f, width, height);
        }
        static Rect ToRectXZ(this BoxCollider self)
        {
            Vector3 center = self.transform.TransformPoint(self.center);
            float width = self.transform.lossyScale.x * self.size.x;
            float height = self.transform.lossyScale.z * self.size.z;
            return new Rect(center.x - width / 2.0f, center.z - height / 2.0f, width, height);
        }
        static Rect ToRectYZ(this BoxCollider self)
        {
            Vector3 center = self.transform.TransformPoint(self.center);
            float width = self.transform.lossyScale.y * self.size.y;
            float height = self.transform.lossyScale.z * self.size.z;
            return new Rect(center.y - width / 2.0f, center.z - height / 2.0f, width, height);
        }

        /// <summary>
        /// 回転を持つ矩形の投影
        /// ・垂直の軸にしか回転していない前提で計算を省略している
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static RotationRect ToRotationRectXY(this BoxCollider self)
        {
            self.transform.rotation.ToAngleAxis(out float degree, out Vector3 axis);
            return new RotationRect(self.ToRectXY(), Mathf.Deg2Rad * degree);
        }
        public static RotationRect ToRotationRectXZ(this BoxCollider self)
        {
#if false // こちらでも動く
            float xMax = self.center.x + self.size.x / 2.0f;
            float zMax = self.center.z + self.size.z / 2.0f;

            Vector3 center = self.transform.TransformPoint(self.center);
            Vector3 hvec = self.transform.TransformPoint(new Vector3(xMax, self.center.y, self.center.z)) - center;
            Vector3 vvec = self.transform.TransformPoint(new Vector3(self.center.x, self.center.y, zMax)) - center;
            float radian = Mathf.Atan2(hvec.z, hvec.x);
            float width  = hvec.magnitude * 2.0f;
            float height = vvec.magnitude * 2.0f;
            return new RotationRect(new Rect(center.x - width / 2.0f, center.z - height / 2.0f, width, height), radian);
#else
            // ToAngleAxisはやや不安定さがあるとの話。今のところ目に見えておかしなところはない。
            self.transform.rotation.ToAngleAxis(out float degree, out Vector3 axis);
            //degree = float.IsNaN(degree) ? 0.0f : degree;
            //degree = (axis.y > 0) ? degree : 360.0f - degree;
            return new RotationRect(self.ToRectXZ(), Mathf.Deg2Rad * degree);
#endif
        }
        public static RotationRect ToRotationRectYZ(this BoxCollider self)
        {
            self.transform.rotation.ToAngleAxis(out float degree, out Vector3 axis);
            return new RotationRect(self.ToRectYZ(), Mathf.Deg2Rad * degree);
        }

        /// <summary>
        /// 四角形の投影
        /// ・垂直の軸にしか回転していない前提で計算を省略している
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Quadrangle ToQuadXY(this BoxCollider self)
        {
            float x1 = self.center.x - self.size.x / 2.0f;
            float y1 = self.center.y - self.size.y / 2.0f;
            float x2 = self.center.x + self.size.x / 2.0f;
            float y2 = self.center.y + self.size.y / 2.0f;

            Vector3 p1 = self.transform.TransformPoint(new Vector3(x1, y1, 0.0f));
            Vector3 p2 = self.transform.TransformPoint(new Vector3(x1, y2, 0.0f));
            Vector3 p3 = self.transform.TransformPoint(new Vector3(x2, y1, 0.0f));
            Vector3 p4 = self.transform.TransformPoint(new Vector3(x2, y2, 0.0f));

            return new Quadrangle(p1.x, p1.y, p2.x, p2.y, p3.x, p3.y, p4.x, p4.y);
        }
        public static Quadrangle ToQuadXZ(this BoxCollider self)
        {
            float x1 = self.center.x - self.size.x / 2.0f;
            float z1 = self.center.z - self.size.z / 2.0f;
            float x2 = self.center.x + self.size.x / 2.0f;
            float z2 = self.center.z + self.size.z / 2.0f;

            Vector3 p1 = self.transform.TransformPoint(new Vector3(x1, 0.0f, z1));
            Vector3 p2 = self.transform.TransformPoint(new Vector3(x1, 0.0f, z2));
            Vector3 p3 = self.transform.TransformPoint(new Vector3(x2, 0.0f, z1));
            Vector3 p4 = self.transform.TransformPoint(new Vector3(x2, 0.0f, z2));

            return new Quadrangle(p1.x, p1.z, p2.x, p2.z, p3.x, p3.z, p4.x, p4.z);
        }
        public static Quadrangle ToQuadYZ(this BoxCollider self)
        {
            float y1 = self.center.y - self.size.y / 2.0f;
            float z1 = self.center.z - self.size.z / 2.0f;
            float y2 = self.center.y + self.size.y / 2.0f;
            float z2 = self.center.z + self.size.z / 2.0f;

            Vector3 p1 = self.transform.TransformPoint(new Vector3(0.0f, y1, z1));
            Vector3 p2 = self.transform.TransformPoint(new Vector3(0.0f, y1, z2));
            Vector3 p3 = self.transform.TransformPoint(new Vector3(0.0f, y2, z1));
            Vector3 p4 = self.transform.TransformPoint(new Vector3(0.0f, y2, z2));

            return new Quadrangle(p1.y, p1.z, p2.y, p2.z, p3.y, p3.z, p4.y, p4.z);
        }
    }
}