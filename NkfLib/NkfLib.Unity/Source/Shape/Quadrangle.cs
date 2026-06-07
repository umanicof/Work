using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NkfLib.Unity
{
    /// <summary>
    /// 四角形
    /// ・辺が並行でないものも含む全ての四角形
    /// </summary>
    public readonly struct Quadrangle
    {
        public readonly Vector2 p1;
        public readonly Vector2 p2;
        public readonly Vector2 p3;
        public readonly Vector2 p4;

        public Segment edge1 { get { return new Segment(p1, p2); } }
        public Segment edge2 { get { return new Segment(p2, p3); } }
        public Segment edge3 { get { return new Segment(p3, p4); } }
        public Segment edge4 { get { return new Segment(p4, p1); } }

        public readonly float xMin;
        public readonly float yMin;
        public readonly float xMax;
        public readonly float yMax;

        /// <summary>
        /// 頂点リスト取得
        /// </summary>
        /// <returns></returns>
        public Vector2[] GetPoints()
        {
            var points = new Vector2[4];
            points[0] = p1;
            points[1] = p2;
            points[2] = p3;
            points[3] = p4;
            return points;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        public Quadrangle(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
            this.p4 = p4;

            // 事前計算してしまう
            xMin = Mathf.Min(p1.x, p2.x, p3.x, p4.x);
            yMin = Mathf.Min(p1.y, p2.y, p3.y, p4.y);
            xMax = Mathf.Max(p1.x, p2.x, p3.x, p4.x);
            yMax = Mathf.Max(p1.y, p2.y, p3.y, p4.y);
        }

        public Quadrangle(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
            : this(new Vector2(x1, y1), new Vector2(x2, y2), new Vector2(x3, y3), new Vector2(x4, y4))
        {
        }

        public Quadrangle(Rect rect)
            : this(new Vector2(rect.xMin, rect.yMin), new Vector2(rect.xMin, rect.yMax), new Vector2(rect.xMax, rect.yMax), new Vector2(rect.xMax, rect.yMin))
        {
        }

        /// <summary>
        /// バウンディング矩形
        /// </summary>
        /// <param name="point"></param>
        public Rect BoundsRect()
        {
            return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
        }

        /// <summary>
        /// 重なっている
        /// </summary>
        public bool Overlaps(in Quadrangle dst)
        {
            if (Overlaps(dst.edge1)) return true;
            if (Overlaps(dst.edge2)) return true;
            if (Overlaps(dst.edge3)) return true;
            if (Overlaps(dst.edge4)) return true;
            return false;
        }
        public bool Overlaps(Rect dst)
        {
            return dst.Overlaps(this);
        }
        public bool Overlaps(RotationRect dst)
        {
            return dst.Overlaps(this);
        }
        public bool Overlaps(in Segment segment, out Dictionary<Vector2, Segment> contacts)
        {
            const int kMaxPoints = 2;

            // 接点と接点を含む辺をリストで返却
            contacts = new Dictionary<Vector2, Segment>();

            // 内包しているので重なっているとするが、接点は存在しない
            if (Contains(segment)) return true;

#if false
        // 重い
        Vector2 point;
        var edge = self.Edge1();
        if (segment.Intersect(edge, out point, true)) {
            contacts.Add(point, edge);
        }
        edge = self.Edge2();
        if (segment.Intersect(edge, out point, true)) {
            contacts.Add(point, edge);
            if (contacts.Count >= kMaxPoints) return true;
        }
        edge = self.Edge3();
        if (segment.Intersect(edge, out point, true)) {
            contacts.Add(point, edge);
            if (contacts.Count >= kMaxPoints) return true;
        }
        edge = self.Edge4();
        if (segment.Intersect(edge, out point, true)) {
            contacts.Add(point, edge);
        }
#else
            // 軽い
            var y = segment.CalcY(xMin);
            if (!float.IsNaN(y) && y >= yMin && y <= yMax)
            {
                contacts.Add(new Vector2(xMin, y), edge1);
            }
            y = segment.CalcY(xMax);
            if (!float.IsNaN(y) && y >= yMin && y <= yMax)
            {
                contacts.Add(new Vector2(xMax, y), edge3);
                if (contacts.Count >= kMaxPoints) return true;
            }
            var x = segment.CalcX(yMin);
            if (!float.IsNaN(x) && x >= xMin && x <= xMax)
            {
                contacts.Add(new Vector2(x, yMin), edge4);
                if (contacts.Count >= kMaxPoints) return true;
            }
            x = segment.CalcX(yMax);
            if (!float.IsNaN(x) && x >= xMin && x <= xMax)
            {
                contacts.Add(new Vector2(x, yMax), edge2);
            }
#endif

            return contacts.Count > 0;
        }
        public bool Overlaps(in Segment segment)
        {
            // 内包しているので重なっているとするが、接点は存在しない
            if (Contains(segment)) return true;

            var y = segment.CalcY(xMin);
            if (!float.IsNaN(y) && y >= yMin && y <= yMax) return true;
            y = segment.CalcY(xMax);
            if (!float.IsNaN(y) && y >= yMin && y <= yMax) return true;
            var x = segment.CalcX(yMin);
            if (!float.IsNaN(x) && x >= xMin && x <= xMax) return true;
            x = segment.CalcX(yMax);
            if (!float.IsNaN(x) && x >= xMin && x <= xMax) return true;

            return false;
        }


        /// <summary>
        /// 内包している
        /// </summary>
        public bool Contains(Segment segment)
        {
            if (!Contains(segment.p1)) return false;
            if (!Contains(segment.p2)) return false;
            return true;
        }
        public bool Contains(in Quadrangle dst)
        {
            if (!Contains(dst.p1)) return false;
            if (!Contains(dst.p2)) return false;
            if (!Contains(dst.p3)) return false;
            if (!Contains(dst.p4)) return false;
            return true;
        }
        public bool Contains(Rect rect)
        {
            return Contains(rect.ToQuadrangle());
        }

        /// <summary>
        /// 指定の点が含まれている
        /// </summary>
        /// <param name="point"></param>
        public bool Contains(Vector2 point)
        {
            return CalcCrossingNumber(point) % 2 != 0;
        }

        /// <summary>
        /// Crossing Number Algorithm
        /// ・pointから右方向に伸ばした水平線と交差する辺の数を返却
        ///   出典：https://www.nttpc.co.jp/technology/number_algorithm.html
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        int CalcCrossingNumber(Vector2 point)
        {
            var points = GetPoints();

            int count = 0;
            for (int i = 0; i < points.Length; ++i)
            {
                int j = (i + 1) % points.Length;
                // 上向きの辺。点Pがy軸方向について、始点と終点の間にある。ただし、終点は含まない。(ルール1)
                if (((points[i].y <= point.y) && (points[j].y > point.y)) ||
                    // 下向きの辺。点Pがy軸方向について、始点と終点の間にある。ただし、始点は含まない。(ルール2)
                    ((points[i].y > point.y) && (points[j].y <= point.y)))
                {
                    // ルール1,ルール2を確認することで、ルール3も確認できている。
                    // 辺は点pよりも右側にある。ただし、重ならない。(ルール4)
                    // 辺が点pと同じ高さになる位置を特定し、その時のxの値と点pのxの値を比較する。
                    var vt = (point.y - points[i].y) / (points[j].y - points[i].y);
                    if (point.x < (points[i].x + (vt * (points[j].x - points[i].x))))
                    {
                        ++count;
                    }
                }
            }
            return count;
        }

    }
}