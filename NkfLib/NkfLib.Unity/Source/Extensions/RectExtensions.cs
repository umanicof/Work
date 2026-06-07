using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NkfLib.Unity
{
    public static class RectExtensions
    {
        /// <summary>
        /// 辺を取得
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Segment Edge1(this Rect self)
        {
            return new Segment(self.x, self.y, self.x, self.y + self.height);
        }
        public static Segment Edge2(this Rect self)
        {
            return new Segment(self.x, self.y + self.height, self.x + self.width, self.y + self.height);
        }
        public static Segment Edge3(this Rect self)
        {
            return new Segment(self.x + self.width, self.y + self.height, self.x + self.width, self.y);
        }
        public static Segment Edge4(this Rect self)
        {
            return new Segment(self.x + self.width, self.y, self.x, self.y);
        }

        /// <summary>
        /// 有効無効
        /// ・現状は縦横の両方が0のものを無効としている（面積が0のものではない）
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsValid(this Rect self)
        {
            //return self.Area() != 0.0f;
            return self.IsZero();
        }

        /// <summary>
        /// 縦横が0
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool IsZero(this Rect self)
        {
            return self.width == 0.0f && self.height == 0.0f;
        }

        /// <summary>
        /// 面積
        /// </summary>
        /// <returns></returns>
        public static float Area(this Rect self)
        {
            return self.width * self.height;
        }

        /// <summary>
        /// クリア ※参照渡し
        /// </summary>
        /// <returns></returns>
        public static void Clear(ref this Rect self)
        {
            self.x = 0.0f;
            self.y = 0.0f;
            self.width = 0.0f;
            self.height = 0.0f;
        }

        /// <summary>
        /// 中心位置をそのままに縦横を伸ばす
        /// </summary>
        /// <returns></returns>
        public static Rect Streach(this Rect self, float addWidth, float addHeight)
        {
            return new Rect(self.x - addWidth / 2.0f, self.y - addHeight / 2.0f, self.width + addWidth, self.height + addHeight);
        }

        /// <summary>
        /// 頂点を整数に丸めた矩形を取得
        /// </summary>
        /// <returns></returns>
        public static Rect RoundToInt(this Rect self)
        {
            return new Rect(Mathf.RoundToInt(self.x), Mathf.RoundToInt(self.y), Mathf.RoundToInt(self.width), Mathf.RoundToInt(self.height));
        }

        /// <summary>
        /// 四角形に変換
        /// </summary>
        /// <returns></returns>
        public static Quadrangle ToQuadrangle(this Rect self)
        {
            return new Quadrangle(self);
        }

        /// <summary>
        /// 円に変換
        /// </summary>
        /// <returns></returns>
        public static Circle ToCircle(this Rect self, bool useMinSegment = false)
        {
            var radious = (useMinSegment ? Mathf.Min(self.width, self.height) : Mathf.Max(self.width, self.height)) / 2.0f;
            return new Circle(self.center.x, self.center.y, radious);
        }

        /// <summary>
        /// 重なっている
        /// </summary>
        /// <param name="separation">分離ベクトル</param>
        /// <returns></returns>
        public static bool Overlaps(this Rect self, in Quadrangle quad)
        {
            if (self.Overlaps(quad.edge1))
                return true;
            if (self.Overlaps(quad.edge2))
                return true;
            if (self.Overlaps(quad.edge3))
                return true;
            if (self.Overlaps(quad.edge4))
                return true;
            return false;
        }
        public static bool Overlaps(this Rect self, in RotationRect rotationRect)
        {
            if (self.Overlaps(rotationRect.edge1))
                return true;
            if (self.Overlaps(rotationRect.edge2))
                return true;
            if (self.Overlaps(rotationRect.edge3))
                return true;
            if (self.Overlaps(rotationRect.edge4))
                return true;
            return false;
        }
        public static bool Overlaps(this Rect self, in Circle circle)
        {
            return Overlaps(self, circle, out Vector2 _);
        }
        public static bool Overlaps(this Rect self, in Circle circle, out Vector2 separation)
        {
            separation = default;

            // 円から一番近い矩形上の点
            float x = circle.center.x < self.xMin ? self.xMin
                    : circle.center.x > self.xMax ? self.xMax
                    : circle.center.x;
            float y = circle.center.y < self.yMin ? self.yMin
                    : circle.center.y > self.yMax ? self.yMax
                    : circle.center.y;
            // 円への方向ベクトル
            var direction = new Vector2(circle.center.x - x, circle.center.y - y);
            var distance = direction.magnitude;
            if (distance >= circle.radious)
                return false;
            separation = direction * (circle.radious - distance) / distance; // distanceは0ではないだろう
            return true;
        }
        public static bool Overlaps(this Rect self, in Segment segment, out Dictionary<Vector2, Segment> contacts)
        {
            const int kMaxPoints = 2;

            // 接点と接点を含む辺をリストで返却
            contacts = new Dictionary<Vector2, Segment>();

            // 内包しているので重なっているとするが、接点は存在しない
            if (self.Contains(segment))
                return true;

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
                if (contacts.Count >= kMaxPoints)
                    return true;
            }
            edge = self.Edge3();
            if (segment.Intersect(edge, out point, true)) {
                contacts.Add(point, edge);
                if (contacts.Count >= kMaxPoints)
                    return true;
            }
            edge = self.Edge4();
            if (segment.Intersect(edge, out point, true)) {
                contacts.Add(point, edge);
            }
#else
            // 軽い
            var y = segment.CalcY(self.xMin);
            if (!float.IsNaN(y) && y >= self.yMin && y <= self.yMax)
            {
                contacts.Add(new Vector2(self.xMin, y), self.Edge1());
            }
            y = segment.CalcY(self.xMax);
            if (!float.IsNaN(y) && y >= self.yMin && y <= self.yMax)
            {
                contacts.Add(new Vector2(self.xMax, y), self.Edge3());
                if (contacts.Count >= kMaxPoints) return true;
            }
            var x = segment.CalcX(self.yMin);
            if (!float.IsNaN(x) && x >= self.xMin && x <= self.xMax)
            {
                contacts.Add(new Vector2(x, self.yMin), self.Edge4());
                if (contacts.Count >= kMaxPoints) return true;
            }
            x = segment.CalcX(self.yMax);
            if (!float.IsNaN(x) && x >= self.xMin && x <= self.xMax)
            {
                contacts.Add(new Vector2(x, self.yMax), self.Edge2());
            }
#endif

            return contacts.Count > 0;
        }
        public static bool Overlaps(this Rect self, in Segment segment)
        {
            // 内包しているので重なっているとするが、接点は存在しない
            if (self.Contains(segment))
                return true;

            var y = segment.CalcY(self.xMin);
            if (!float.IsNaN(y) && y >= self.yMin && y <= self.yMax)
                return true;
            y = segment.CalcY(self.xMax);
            if (!float.IsNaN(y) && y >= self.yMin && y <= self.yMax)
                return true;
            var x = segment.CalcX(self.yMin);
            if (!float.IsNaN(x) && x >= self.xMin && x <= self.xMax)
                return true;
            x = segment.CalcX(self.yMax);
            if (!float.IsNaN(x) && x >= self.xMin && x <= self.xMax)
                return true;

            return false;
        }

        /// <summary>
        /// 内包している
        /// </summary>
        /// <param name="point"></param>
        public static bool Contains(this Rect src, Rect dst)
        {
            if (!src.Contains(new Vector2(dst.xMin, dst.yMin)))
                return false;
            if (!src.Contains(new Vector2(dst.xMin, dst.yMax)))
                return false;
            if (!src.Contains(new Vector2(dst.xMax, dst.yMax)))
                return false;
            if (!src.Contains(new Vector2(dst.xMax, dst.yMin)))
                return false;

            return true;
        }
        public static bool Contains(this Rect src, in Quadrangle dst)
        {
            if (!src.Contains(dst.p1))
                return false;
            if (!src.Contains(dst.p2))
                return false;
            if (!src.Contains(dst.p3))
                return false;
            if (!src.Contains(dst.p4))
                return false;

            return true;
        }
        public static bool Contains(this Rect src, in Segment segment)
        {
            if (segment.isLine)
                return false; // 内包できない
            if (!src.Contains(segment.p1))
                return false;
            if (!src.Contains(segment.p2))
                return false;
            return true;
        }

        /// <summary>
        /// 結合
        /// ・サイズが0の矩形は無視されるものとする
        /// </summary>
        /// <returns></returns>
        public static Rect Combine(this Rect src, Rect dst)
        {
            if (src.IsZero())
                return dst;
            if (dst.IsZero())
                return src;

            var x = Mathf.Min(src.xMin, dst.xMin);
            var y = Mathf.Min(src.yMin, dst.yMin);
            var xMax = Mathf.Max(src.xMax, dst.xMax);
            var yMax = Mathf.Max(src.yMax, dst.yMax);
            return new Rect(x, y, xMax - x, yMax - y);
        }
        public static Rect Combine(IEnumerable<Rect> rects)
        {
            var rect = new Rect();
            foreach (var r in rects)
            {
                rect = rect.Combine(r);
            }
            return rect;
        }

        /// <summary>
        /// 共通部分
        /// ・サイズが0の矩形は無視されるものとする
        /// ・共通部分がなければRect.zeroを返却することに注意
        ///   ※折り返し入力とすると無視されるので、もう一方の矩形がそのまま出力される
        /// </summary>
        /// <returns></returns>
        public static Rect Intersection(this Rect src, Rect dst)
        {
            if (src.IsZero())
                return dst;
            if (dst.IsZero())
                return src;

            var x = Mathf.Max(src.xMin, dst.xMin);
            var y = Mathf.Max(src.yMin, dst.yMin);
            var xMax = Mathf.Min(src.xMax, dst.xMax);
            var yMax = Mathf.Min(src.yMax, dst.yMax);
            if (x > xMax || y > xMax)
                return Rect.zero;
            return new Rect(x, y, xMax - x, yMax - y);
        }
        public static Rect Intersection(IEnumerable<Rect> rects)
        {
            var rect = new Rect();
            foreach (var r in rects)
            {
                rect = rect.Intersection(r);
                if (rect == Rect.zero)
                    break;
            }
            return rect;
        }

        /// <summary>
        /// 分離方向の計算
        /// ・重なっていなければ機能しない
        /// ・directionは分離方向。right, left, forward, backのみ。
        /// ・distanceは分離距離
        /// ┌───────┐             
        /// │        ┌──┼──────┐  
        /// │   src  │    │  dst       │
        /// │        │    │            │
        /// │        └──┼──────┘
        /// └───────┘ 
        /// 上記の状態であれば左方向に分離と判定
        /// ┌───────┐             
        /// │              ├─────────┐  
        /// │   src        │        dst       │
        /// │              │                  │
        /// │              ├─────────┘
        /// └───────┘ 
        /// </summary>
        /// <returns></returns>
        public static bool ComputeSeparation(this Rect src, Rect dst, out Vector3 direction, out float distance)
        {
            direction = Vector3.zero;
            distance = Mathf.Infinity;
            if (src.IsZero() || dst.IsZero())
                return false;

            bool dstAwayRight = src.xMax <= dst.xMin; // 右に抜けているか
            bool dstAwayLeft = src.xMin >= dst.xMax; // 左に抜けているか
            bool dstAwayForward = src.yMax <= dst.yMin; // 奥に抜けているか
            bool dstAwayBack = src.yMin >= dst.yMax; // 手前に抜けているか
            if (dstAwayRight || dstAwayLeft || dstAwayForward || dstAwayBack)
                return false;

            // 分離方向
            // ・中心間の座標が離れている方を分離方向とみなす
            var distanceCenterX = dst.center.x - src.center.x;
            var distanceCenterY = dst.center.y - src.center.y;
            if (Mathf.Abs(distanceCenterX) > Mathf.Abs(distanceCenterY))
            {
                if (distanceCenterX > 0)
                {
                    direction = Vector3.left;
                    distance = (src.width + dst.width) / 2.0f - distanceCenterX;
                }
                else
                {
                    direction = Vector3.right;
                    distance = (src.width + dst.width) / 2.0f + distanceCenterX;
                }
            }
            else
            {
                if (distanceCenterY > 0)
                {
                    direction = Vector3.back;
                    distance = (src.height + dst.height) / 2.0f - distanceCenterY;
                }
                else
                {
                    direction = Vector3.forward;
                    distance = (src.height + dst.height) / 2.0f + distanceCenterY;
                }
            }

            return true;
        }

        /// <summary>
        /// 含有方向の計算
        /// ・重なっていなければ機能しない
        /// ・directionは含有方向。right, left, forward, backのみ。
        /// ・distanceは含有距離
        /// ┌───────┐
        /// │        ┌──┼──────┐
        /// │   src  │    │  dst       │
        /// │        │    │            │
        /// │        └──┼──────┘
        /// └───────┘ 
        /// 上記の状態であれば右方向に含有と判定
        /// ┌───────┐
        /// ├───────┼─┐
        /// │  src   dst   │  │
        /// │              │  │
        /// ├───────┼─┘
        /// └───────┘ 
        /// </summary>
        /// <returns></returns>
        public static bool ComputeContent(this Rect src, Rect dst, out Vector3 direction, out float distance)
        {
            direction = Vector3.zero;
            distance = Mathf.Infinity;
            if (src.IsZero() || dst.IsZero())
                return false;
            //bool dstAwayRight   = src.xMax <= dst.xMin; // 右に抜けているか
            //bool dstAwayLeft    = src.xMin >= dst.xMax; // 左に抜けているか
            //bool dstAwayForward = src.yMax <= dst.yMin; // 奥に抜けているか
            //bool dstAwayBack    = src.yMin >= dst.yMax; // 手前に抜けているか
            //if (dstAwayRight || dstAwayLeft || dstAwayForward || dstAwayBack) return false;

            // 揃える方向
            // ・中心間の座標が離れている方を含有方向とみなす
            var distanceCenterX = dst.center.x - src.center.x;
            var distanceCenterY = dst.center.y - src.center.y;
            if (Mathf.Abs(distanceCenterX) > Mathf.Abs(distanceCenterY))
            {
                if (distanceCenterX > 0)
                {
                    if (src.xMin >= dst.xMin)
                        return false; // 含有済み
                    direction = Vector3.right;
                    distance = dst.xMin - src.xMin;
                }
                else
                {
                    if (dst.xMax >= src.xMax)
                        return false; // 含有済み
                    direction = Vector3.left;
                    distance = src.xMax - dst.xMax;
                }
            }
            else
            {
                if (distanceCenterY > 0)
                {
                    if (src.yMin >= dst.yMin)
                        return false; // 含有済み
                    direction = Vector3.forward;
                    distance = dst.yMin - src.yMin;
                }
                else
                {
                    if (dst.yMax >= src.yMax)
                        return false; // 含有済み
                    direction = Vector3.back;
                    distance = src.yMax - dst.yMax;
                }
            }

            return true;
        }
    }
}