using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NkfLib.Unity
{
    public static class ColliderExtensions
    {
        /// <summary>
        /// 中心の取得
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Vector3 GetCenter(this Collider self)
        {
            return (self as CapsuleCollider)?.center ??
                   (self as BoxCollider)?.center ??
                   (self as SphereCollider).center;
        }

        /// <summary>
        /// バウンディングボックスの計算
        /// ・boundsプロパティは度々ぶれるので独自計算
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Bounds CalcBounds(this Collider self)
        {
            return (self as CapsuleCollider)?.CalcBounds() ??
                   (self as BoxCollider)?.CalcBounds() ??
                   (self as SphereCollider).CalcBounds();
        }

        /// <summary>
        /// バウンディング矩形の投影
        /// ・バウンディングボックスから計算するのが正確だが、垂直の軸にしか回転していない前提で計算を省略している
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Rect ToBoundsRectXZ(this Collider self)
        {
            return (self as CapsuleCollider)?.ToBoundsRectXZ() ??
                   (self as BoxCollider)?.ToBoundsRectXZ() ??
                   (self as SphereCollider).ToBoundsRectXZ();
        }

        /// <summary>
        /// 円の投影
        /// ・垂直の軸にしか回転していない前提で計算を省略している
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Circle ToCircleXZ(this Collider self)
        {
            return (self as CapsuleCollider)?.ToCircleXZ() ??
                   (self as BoxCollider)?.ToCircleXZ() ??
                   (self as SphereCollider).ToCircleXZ();
        }

#if false // １対多の判定を行う場合に無駄が多いので却下
        /// <summary>
        /// 重なっている
        /// ・垂直の軸にしか回転していない前提で計算を省略している
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static bool OverlapsXZ(this Collider src, Collider dst)
        {
            // as を多用してしまったが、さほど重くはないとの話
            var dst_capsule = dst as CapsuleCollider;
            if (dst_capsule != null) {
                var dst_circle = dst_capsule.ToCircleXZ();
                return (src as CapsuleCollider)?.ToCircleXZ().Overlaps(dst_circle) ??
                       (src as BoxCollider)?.ToRotationRectXZ().Overlaps(dst_circle) ??
                       (src as SphereCollider).ToCircleXZ().Overlaps(dst_circle);
            }
            var dst_box = dst as BoxCollider;
            if (dst_box != null) {
                var dst_rect = dst_box.ToRotationRectXZ();
                return (src as CapsuleCollider)?.ToCircleXZ().Overlaps(dst_rect) ??
                       //(src as BoxCollider)?.ToRotationRectXZ().Overlaps(dst_rect) ?? // 未実装
                       (src as SphereCollider).ToCircleXZ().Overlaps(dst_rect);
            }
            var dst_sphere = dst as SphereCollider;
            if (dst_sphere != null) {
                var dst_circle = dst_sphere.ToCircleXZ();
                return (src as CapsuleCollider)?.ToCircleXZ().Overlaps(dst_circle) ??
                       (src as BoxCollider)?.ToRotationRectXZ().Overlaps(dst_circle) ??
                       (src as SphereCollider).ToCircleXZ().Overlaps(dst_circle);
            }

            throw new System.ArgumentException();
        }
#endif
    }
}