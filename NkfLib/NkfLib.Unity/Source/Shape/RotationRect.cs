using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace NkfLib.Unity
{
    /// <summary>
    /// 回転を持つ矩形
    /// </summary>
    public readonly struct RotationRect
    {
        public readonly Rect rect;
        public readonly float radian;

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
        /// <param name="rect"></param>
        /// <param name="radian"></param>
        public RotationRect(Rect rect, float radian)
        {
            this.rect = rect;
            this.radian = radian;

            // 事前計算してしまう
            p1 = rect.center + new Vector2(-rect.width / 2.0f, -rect.height / 2.0f).Rotate(radian);
            p2 = rect.center + new Vector2(-rect.width / 2.0f, rect.height / 2.0f).Rotate(radian);
            p3 = rect.center + new Vector2(rect.width / 2.0f, rect.height / 2.0f).Rotate(radian);
            p4 = rect.center + new Vector2(rect.width / 2.0f, -rect.height / 2.0f).Rotate(radian);

            xMin = Mathf.Min(p1.x, p2.x, p3.x, p4.x);
            yMin = Mathf.Min(p1.y, p2.y, p3.y, p4.y);
            xMax = Mathf.Max(p1.x, p2.x, p3.x, p4.x);
            yMax = Mathf.Max(p1.y, p2.y, p3.y, p4.y);
        }
        public RotationRect(Vector2 center, Vector2 size, float radian)
            : this(new Rect(center.x - size.x, center.y - size.y, size.x, size.y), radian)
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
        /// <param name="self"></param>
        /// <param name="circle"></param>
        /// <param name="separation">分離ベクトル</param>
        /// <returns></returns>
        public bool Overlaps(Rect rect)
        {
            return rect.Overlaps(this);
        }
        public bool Overlaps(Quadrangle quad)
        {
            if (Overlaps(quad.edge1)) return true;
            if (Overlaps(quad.edge2)) return true;
            if (Overlaps(quad.edge3)) return true;
            if (Overlaps(quad.edge4)) return true;
            return false;
        }
        public bool Overlaps(in Circle circle)
        {
            return Overlaps(circle, out Vector2 _);
        }
        public bool Overlaps(in Circle circle, out Vector2 separation)
        {
            // 円の位置を矩形が無回転の状態に合わせる
            var rotateCircle = circle.Rotate(-radian, rect.center);
            if (!rect.Overlaps(rotateCircle, out separation)) return false;
            separation = separation.Rotate(radian); // 回転を戻す
            return true;
        }
        public bool Overlaps(in Segment segment, out Dictionary<Vector2, Segment> contacts)
        {
            contacts = new Dictionary<Vector2, Segment>();
            // 線分の位置を矩形が無回転の状態に合わせる
            var rotateSegment = segment.Rotate(-radian, rect.center);
            if (!rect.Overlaps(rotateSegment, out Dictionary<Vector2, Segment> contacts2)) return false;
            foreach (var contact in contacts2)
            {
                contacts.Add(contact.Key.RotatePoint(-radian, rect.center), contact.Value.Rotate(-radian, rect.center)); // 回転を戻す
            }
            return true;
        }
        public bool Overlaps(in Segment segment)
        {
            // 線分の位置を矩形が無回転の状態に合わせる
            var rotateSegment = segment.Rotate(-radian, rect.center);
            return rect.Overlaps(rotateSegment);
        }
    }
}