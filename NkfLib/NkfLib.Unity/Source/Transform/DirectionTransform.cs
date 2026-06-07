using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NkfLib.Unity
{
    /// <summary>
    /// 方向制御のための基底クラス
    /// ・Z成分は意識しない
    /// ・Z軸を中心とした回転のみ有効とする
    /// </summary>
    public class DirectionTransform : MonoBehaviour
    {
        // Z軸を中心とした回転角度(0～360)
        public float Angle()
        {
            return Util.Repeat(Quaternion.Angle(Quaternion.identity, transform.rotation), 360.0f);
        }

        // Z軸を中心とした方向
        public Vector3 Direction()
        {
            return (transform.rotation * Vector3.up).normalized;
        }

        // フリップ（左右反転）状態
        public bool Filpped { get; private set; }

        /// <summary>
        /// ランダムな方向ベクトルを取得
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetRandomDirection()
        {
            return new Vector3(Random.value, Random.value, 0.0f).normalized;
        }

        /// <summary>
        /// fromからtoへの方向ベクトルを取得
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetDirect(Vector3 from, Vector3 to)
        {
            return new Vector3(to.x - from.x, to.y - from.y, 0.0f).normalized;
        }

        /// <summary>
        /// 自身から対象への方向取得
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Vector3 TargetDirect(float x, float y, Vector3 whenEqualReturnValue = default)
        {
            var direction = new Vector3(x - transform.position.x, y - transform.position.y, 0.0f).normalized;
            return direction == Vector3.zero ? whenEqualReturnValue : direction;
        }
        public Vector3 TargetDirectAxis(float x, float y, Vector3 whenEqualReturnValue = default)
        {
            return TargetDirect(x, y, whenEqualReturnValue).ToAxisDirection();
        }
        public Vector3 TargetDirectAxisX(float x, Vector3 whenEqualReturnValue = default)
        {
            if (x > transform.position.x)
            {
                return Vector3.right;
            }
            else if (x < transform.position.x)
            {
                return Vector3.left;
            }

            return whenEqualReturnValue;
        }
        public Vector3 TargetDirectAxisY(float y, Vector3 whenEqualReturnValue = default)
        {
            if (y > transform.position.y)
            {
                return Vector3.forward;
            }
            else if (y < transform.position.y)
            {
                return Vector3.back;
            }

            return whenEqualReturnValue;
        }

        /// <summary>
        /// 右に向く
        /// </summary>
        public void TurnRight()
        {
            Turn(Vector3.right);
        }

        /// <summary>
        /// 左に向く
        /// </summary>
        public void TurnLeft()
        {
            Turn(Vector3.left);
        }

        /// <summary>
        /// 上に向く
        /// </summary>
        public void TurnUp()
        {
            Turn(Vector3.up);
        }

        /// <summary>
        /// 下に向く
        /// </summary>
        public void TurnDown()
        {
            Turn(Vector3.down);
        }

        /// <summary>
        /// 指定の方向に向く
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Turn(Vector3 direction)
        {
            direction.z = 0.0f;
            if (direction == Vector3.zero) return;
            transform.rotation = Quaternion.LookRotation(direction);
        }
        public void Turn(Transform transform)
        {
            Turn(transform.position - this.transform.position);
        }

        /// <summary>
        /// 緩やかに指定の方向に向く
        /// ・繰り返し呼び出しが必要
        /// ・角速度は100程度の大きな値がないと動きが鈍い
        /// </summary>
        /// <param name="direction"></param>
        public void DeltaTurn(Vector3 direction, float angularSpeed)
        {
            direction.y = 0.0f;
            if (direction == Vector3.zero) return;
            var currentDirection = Direction();
            var q = Quaternion.FromToRotation(currentDirection, direction); // 目標とするクォータニオン
            q = Quaternion.RotateTowards(Quaternion.identity, q, angularSpeed * Time.deltaTime); // １フレームのクォータニオン
            Turn(q * currentDirection);
        }
        public void DeltaTurn(Transform transform, float angularSpeed)
        {
            DeltaTurn(transform.position - this.transform.position, angularSpeed);
        }

        /// <summary>
        /// 指定の位置に向く
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Turn(float x, float y)
        {
            Turn(TargetDirect(x, y));
        }

        /// <summary>
        /// 指定の位置の反対方向を向く
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void TurnReverse(float x, float y)
        {
            Turn(-x, -y);
        }

        /// <summary>
        /// フリップ（左右反転）
        /// </summary>
        public bool Flip(bool on)
        {
            Filpped = on;
            transform.localScale = new Vector3((Filpped ? -1 : 1) *transform.localScale.x, transform.localScale.y, transform.localScale.z);
            return Filpped;
        }

        /// <summary>
        /// フリップの切り替え
        /// </summary>
        public bool ToggleFlip()
        {
            return Flip(!Filpped);
        }
    }
}