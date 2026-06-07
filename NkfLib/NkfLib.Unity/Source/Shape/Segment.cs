using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NkfLib.Unity
{
    /// <summary>
    /// 線分
    /// </summary>
    public struct Segment
    {
        // 直線どうか、線分かどうか
        // ・isLineがtrueであれば、交差判定などにおいて２点間を通る直線として取り扱う
        public readonly bool isLine;
        public bool isSeqment { get { return !isLine; } }

        public Vector2 p1 { get; private set; }
        public Vector2 p2 { get; private set; }

        public readonly float xMin;
        public readonly float yMin;
        public readonly float xMax;
        public readonly float yMax;
        public float slope { get { return (p2.x == p1.x) ? float.NaN : (p2.y - p1.y) / (p2.x - p1.x); } } // 傾き(y/x) ※0除算であればNaN
        public float slopeX { get { return (p2.y == p1.y) ? float.NaN : (p2.x - p1.x) / (p2.y - p1.y); } } // 傾き(x/y) ※0除算であればNaN
        public float magnitude { get { return vector.magnitude; } }
        public float sqrMagnitude { get { return vector.sqrMagnitude; } }
        public Vector2 origin { get { return p1; } }
        public Vector2 vector { get { return p2 - p1; } }
        public Vector2 direction { get { return vector.normalized; } }
        public Segment negative { get { return new Segment(p2, p1); } }
        public bool isVertical { get { return p1.x == p2.x; } }
        public bool isHorizontal { get { return p1.y == p2.y; } }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        public Segment(Vector2 p1, Vector2 p2, bool isLine = false)
        {
            this.isLine = isLine;
            this.p1 = p1;
            this.p2 = p2;

            // 事前計算してしまう
            xMin = Mathf.Min(p1.x, p2.x);
            yMin = Mathf.Min(p1.y, p2.y);
            xMax = Mathf.Max(p1.x, p2.x);
            yMax = Mathf.Max(p1.y, p2.y);
        }

        public Segment(float x1, float y1, float x2, float y2, bool isLine = false)
            : this(new Vector2(x1, y1), new Vector2(x2, y2), isLine)
        {
        }

        /// <summary>
        /// 頂点リスト取得
        /// </summary>
        /// <returns></returns>
        public Vector2[] GetPoints()
        {
            var points = new Vector2[2];
            points[0] = p1;
            points[1] = p2;
            return points;
        }

        /// <summary>
        /// 頂点設定
        /// </summary>
        /// <returns></returns>
        public void SetPoints(Vector2 p1, Vector2 p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }

        /// <summary>
        /// 外積
        /// </summary>
        /// <returns></returns>
        public float Cross(in Segment dst)
        {
            // Vector2から計算しても良いが、Vector2を生成しない分こちらの方が軽い
            return (p2.x - p1.x) * (dst.p2.y - dst.p1.y) - (p2.y - p1.y) * (dst.p2.x - dst.p1.x);
        }

        /// <summary>
        /// 並行
        /// </summary>
        /// <returns></returns>
        public bool Parallel(in Segment dst)
        {
            //return Vector2.Dot(this.vector, Vector2.Perpendicular(dst.vector)).EqualsByEpsilon(0.0f); // 誤差を考慮
            return Cross(dst).EqualsByEpsilon(0.0f);
        }

        /// <summary>
        /// 交差
        /// ・出典：http://www5d.biglobe.ne.jp/~tomoya03/shtml/algorithm/Intersection.htm
        /// ・直線である場合を考慮している
        /// ・やや軽量
        /// </summary>
        /// <param name="dst"></param>
        /// <param name="containsEnd">接点に端点を含むかどうか</param>
        /// <returns></returns>
        public bool Intersect(in Segment dst, bool containsEnd = false)
        {
            if (!isSeqment && !dst.isSeqment && Parallel(dst)) return false; // 並行

            var p3 = dst.p1;
            var p4 = dst.p2;

            if (isSeqment)
            {
                var tp1 = (p3.x - p4.x) * (p1.y - p3.y) + (p3.y - p4.y) * (p3.x - p1.x);
                var tp2 = (p3.x - p4.x) * (p2.y - p3.y) + (p3.y - p4.y) * (p3.x - p2.x);
                if (containsEnd ? (tp1 * tp2 > 0) : (tp1 * tp2 >= 0)) return false;
            }
            if (dst.isSeqment)
            {
                var tp3 = (p1.x - p2.x) * (p3.y - p1.y) + (p1.y - p2.y) * (p1.x - p3.x);
                var tp4 = (p1.x - p2.x) * (p4.y - p1.y) + (p1.y - p2.y) * (p1.x - p4.x);
                if (containsEnd ? (tp3 * tp4 > 0) : (tp3 * tp4 >= 0)) return false;
            }

            return true;
        }

        /// <summary>
        /// 交差
        /// ・出典：https://www.hiramine.com/programming/graphics/2d_segmentintersection.html
        /// ・直線である場合を考慮している
        /// </summary>
        /// <param name="dst"></param>
        /// <param name="contactPoint">接点</param>
        /// <param name="containsEnd">接点に端点を含むかどうか</param>
        /// <returns></returns>
        public bool Intersect(in Segment dst, out Vector2 contactPoint, bool containsEnd = false)
        {
            contactPoint = default;

            float cross = Cross(dst);
            if (cross.EqualsByEpsilon(0.0f)) return false; // 並行

            var p3 = dst.p1;
            var p4 = dst.p2;

            Vector2 vec13 = p3 - p1;
            var r = ((p4.y - p3.y) * vec13.x - (p4.x - p3.x) * vec13.y) / cross;
            var s = ((p2.y - p1.y) * vec13.x - (p2.x - p1.x) * vec13.y) / cross;

            if (containsEnd)
            {
                if (isSeqment     && (r < 0.0f || r > 1.0f)) return false;
                if (dst.isSeqment && (s < 0.0f || s > 1.0f)) return false;
            }
            else
            {
                if (isSeqment     && (r <= 0.0f || r >= 1.0f)) return false;
                if (dst.isSeqment && (s <= 0.0f || s >= 1.0f)) return false;
            }

            contactPoint = p1 + r * (p2 - p1);

            return true;
        }

        /// <summary>
        /// 回転
        /// </summary>
        /// <param name="dst"></param>
        /// <returns></returns>
        public Segment Rotate(float radian, Vector2 center)
        {
            return new Segment(p1.RotatePoint(radian, center), p2.RotatePoint(radian, center));
        }

        /// <summary>
        /// 直線・線分上の点を求める
        /// ・存在しなければNaN
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public float CalcY(float x)
        {
            if (isSeqment && (x < xMin || x > xMax)) return float.NaN;
            return slope * (x - p1.x) + p1.y;
        }
        public float CalcX(float y)
        {
            if (isSeqment && (y < yMin || y > yMax)) return float.NaN;
            return slopeX * (y - p1.y) + p1.x;
        }

        /// <summary>
        /// ベクトルに沿って指定の長さを進んだ位置
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public Vector2 DistancePoint(float distance)
        {
            return vector * (distance / magnitude) + p1;
        }
    }
}