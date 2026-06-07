using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using UnityEngine;

namespace NkfLib.Unity
{
    public static class Util
    {
        /// <summary>
        /// Assert
        /// </summary>
        /// <param name="self">Debug 型のインスタンス</param>
        public static void Assert(bool condition)
        {
            Debug.Assert(condition);
            if (!condition)
            {
                System.Diagnostics.Debugger.Break();
            }
        }

        /// <summary>
        /// 範囲内
        /// </summary>
        /// <returns></returns>
        public static bool InRange(float min, float max, float value)
        {
            return min <= value && value <= max;
        }

        /// <summary>
        /// Repeat
        /// </summary>
        public static int Repeat(int value, int max)
        {
            if (max == 0) throw new System.ArgumentException();

            while (value >= max)
            {
                value = value - max;
            }
            while (value < 0)
            {
                value = value + max;
            }
            return value;
        }
        public static float Repeat(float value, float max)
        {
            if (max == 0) throw new System.ArgumentException();

            while (value >= max)
            {
                value = value - max;
            }
            while (value < 0)
            {
                value = value + max;
            }
            return value;
        }

#if false // 実装中
        /// <summary>
        /// ゲームコントローラーが接続されているかどうか
        /// </summary>
        /// <returns></returns>
        bool ConnectedGameController()
        {
            while (true)
            {
                var controllers = Input.GetJoystickNames();

                if (!connected && controllers.Length > 0)
                {
                    connected = true;
                    Debug.Log("Connected");

                }
                else if (connected && controllers.Length == 0)
                {
                    connected = false;
                    Debug.Log("Disconnected");
                }

                yield return new WaitForSeconds(1f);
            }
        }
#endif

        /// <summary>
        /// DOTween - DelayするTween取得
        /// </summary>
        /// <param name="dulation">sec</param>
        /// <returns></returns>
        public static Tween DelayTween(float dulation)
        {
            //float a = 0;
            //return DOTween.To(() => a, x => { a = x; }, 1.0f, dulation);
            return DOVirtual.DelayedCall(dulation, () => { });
        }

        /// <summary>
        /// DOTween - 待ちを行うTween ※動作未確認
        /// </summary>
        /// <param name="getter"></param>
        /// <returns></returns>
        public static Tween WaitUntilTween(Func<bool> getter)
        {
            var t = DOVirtual.DelayedCall(0.0f, async () =>
            {
                while (!getter())
                {
                    await UniTask.Yield();
                }
            });
            return t;
        }

        /// <summary>
        /// 文字列の数式を計算
        /// </summary>
        /// <param name="formula"></param>
        /// <returns></returns>
        static DataTable _calculator = new DataTable();
        public static double CalcFormula(string formula)
        {
            try
            {
                string s = _calculator.Compute(formula, "").ToString();
                return double.Parse(s.ToString());
            }
            catch
            {
                return double.NaN;
            }
        }

        /// <summary>
        /// 直線と無限遠の平面の交点の取得
        /// 出典：https://qiita.com/edo_m18/items/c8808f318f5abfa8af1e
        /// </summary>
        /// <param name="linePoint">直線の通る位置</param>
        /// <param name="lineVector">直線の方向ベクトル</param>
        /// <param name="planePoint">面を通る位置</param>
        /// <param name="planeNormal">面の法線ベクトル</param>
        /// <returns>交点</returns>
        public static Vector3 IntersectLineAndPlane(in Vector3 linePoint, in Vector3 lineDirect, in Vector3 planePoint, in Vector3 planeNormal)
        {
            return linePoint + ((Vector3.Dot(planeNormal, planePoint) - Vector3.Dot(planeNormal, linePoint)) / (Vector3.Dot(planeNormal, lineDirect))) * lineDirect;
        }

        /// <summary>
        /// 直線の指定要素の値の位置の取得
        /// 出典：https://manabitimes.jp/math/998
        /// </summary>
        /// <param name="linePoint">直線の通る位置</param>
        /// <param name="lineVector">直線の方向ベクトル</param>
        /// <param name="x,y,z">各要素</param>
        /// <returns>位置</returns>
        public static Vector3 LinePointFromX(in Vector3 linePoint, in Vector3 lineDirect, float x)
        {
            return new Vector3(x,
                               lineDirect.y * (x - linePoint.x) / lineDirect.x + linePoint.y,
                               lineDirect.z * (x - linePoint.x) / lineDirect.x + linePoint.z);
        }
        public static Vector3 LinePointFromY(in Vector3 linePoint, in Vector3 lineDirect, float y)
        {
            return new Vector3(lineDirect.x * (y - linePoint.y) / lineDirect.y + linePoint.x,
                               y,
                               lineDirect.z * (y - linePoint.y) / lineDirect.y + linePoint.z);
        }
        public static Vector3 LinePointFromZ(in Vector3 linePoint, in Vector3 lineDirect, float z)
        {
            return new Vector3(lineDirect.x * (z - linePoint.z) / lineDirect.z + linePoint.x,
                               lineDirect.y * (z - linePoint.z) / lineDirect.z + linePoint.y,
                               z);
        }

        /// <summary>
        /// 等加速度直線運動から各パラメータを算出
        /// ・公式：v = Vt + at
        ///         d = Vt + 1/2 * at^2
        /// 　　　　v^2 - V^2 = 2ad
        /// 　d:移動距離、V:初速度、v:速度、a:加速度、t:時間
        /// ・ジェネリック引数（T）はfloat, Vector3を想定。
        ///   現状は処理負荷を考慮しジェネリックを使わず、型の数だけメソッドを用意している
        /// </summary>
        /// <param name="time"></param>
        /// <param name="distance"></param>
        /// <param name="acceleration"></param>
        /// <param name="startSpeed"></param>
        /// <returns></returns>
        // 移動距離： d = Vt + 1/2 * at^2
        //public static T DistanceFromLinearMotion<T>(float time, T acceleration, T startVelocity)
        //{
        //    return Operations.Add(Operations.Multiply(startVelocity, time), Operations.Multiply(acceleration, time * time / 2.0f));
        //}
        public static float DistanceFromLinearMotion(float time, float acceleration, float startVelocity)
        {
            return startVelocity * time + (acceleration * time * time / 2.0f);
        }
        public static Vector3 DistanceFromLinearMotion(float time, Vector3 acceleration, Vector3 startVelocity)
        {
            return startVelocity * time + (acceleration * time * time / 2.0f);
        }

        // 時間： t = (-V + √(V^2 + 2a + 4d)) / a  ※解の公式で時間の式にしたもの（プラスの解のみ）
        // 割り算が絡むので、ジェネリックで計算するのは困難。Vector3であればスカラー値に直して計算する。
        public static float TimeFromLinearMotion(float distance, float acceleration, float startVelocity)
        {
            return (-startVelocity + Mathf.Sqrt(startVelocity * startVelocity + 2 * acceleration + 4 * distance)) / acceleration;
        }
        public static float TimeFromLinearMotion(float distance, Vector3 acceleration, Vector3 startVelocity)
        {
            var startVelocityM = startVelocity.magnitude;
            var accelerationM = acceleration.magnitude;
            return (-startVelocityM + Mathf.Sqrt(startVelocityM * startVelocityM + 2 * accelerationM + 4 * distance)) / accelerationM;
        }

        // 加速度： a = 2(d - Vt) / t^2
        //public static T AccerationFromLinearMotion<T>(float time, T distance, T startVelocity)
        //{
        //    return Operations.Multiply(Operations.Subtract(distance, Operations.Multiply(startVelocity, time)), 2.0f / (time * time));
        //}
        public static float AccerationFromLinearMotion(float time, float distance, float startVelocity)
        {
            return 2.0f * (distance - startVelocity * time) / (time * time);
        }
        public static Vector3 AccerationFromLinearMotion(float time, Vector3 distance, Vector3 startVelocity)
        {
            return 2.0f * (distance - startVelocity * time) / (time * time);
        }

        // 初速度： V = d/t - at/2
        //public static T StartVelocityFromLinearMotion<T>(float time, T distance, T acceleration)
        //{
        //    return Operations.Subtract(Operations.Divide(distance, time), Operations.Multiply(acceleration, time / 2.0f));
        //}
        public static float StartVelocityFromLinearMotion(float time, float distance, float acceleration)
        {
            return distance / time - acceleration * time / 2.0f;
        }
        public static Vector3 StartVelocityFromLinearMotion(float time, Vector3 distance, Vector3 acceleration)
        {
            return distance / time - acceleration * time / 2.0f;
        }

        /// <summary>
        /// 二次方程式の解の公式
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <returns></returns>
        public static List<float> QuadraticFormula(float A, float B, float C)
        {
            List<float> Solutions = new List<float>(); // 解
            if (Mathf.Approximately(A, 0.0f)) // 0による除算の回避
            {
                return Solutions; // 解無し
            }

            float D = QuadraticDiscriminant(A, B, C);
            if (D < 0)
            {
                return Solutions; // 解無し
            }

            float SqrtD = Mathf.Sqrt(D);
            Solutions.Add((-B + SqrtD) / (2 * A));
            if (D == 0)
            {
                return Solutions; // 解１
            }

            Solutions.Add((-B - SqrtD) / (2 * A));
            return Solutions; // 解２        
        }

        /// <summary>
        /// 二次方程式の判別式
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <returns></returns>
        public static float QuadraticDiscriminant(float A, float B, float C)
        {
            // b^2 - 4ac
            return B * B - 4 * A * C;
        }

        /// <summary>
        /// 加速度(a)の計算
        /// </summary>
        /// <param name="v0">初速度</param>
        /// <param name="v">速度</param>
        /// <param name="s">距離</param>
        /// <returns></returns>
        public static float CalcAcceleration(float v0, float v, float s)
        {
            // v^2 - v0^2 = 2as → a = (v^2-v0^2)/2s
            return (v * v - v0 * v0) / (2.0f * s);
        }

        /// <summary>
        /// 初速度(v0)を計算
        /// </summary>
        /// <param name="v">速度</param>
        /// <param name="a">加速度</param>
        /// <param name="s">距離</param>
        /// <returns></returns>
        public static float CalcInitialVelocity(float v, float a, float s)
        {
            // v^2 - v0^2 = 2as → v0 = √(v^2 - 2as)
            return Mathf.Sqrt(v * v - 2 * a * s);
        }

        /// <summary>
        /// 速度(v)を計算
        /// </summary>
        /// <param name="v0">初速度</param>
        /// <param name="a">加速度</param>
        /// <param name="s">距離</param>
        /// <returns></returns>
        public static float CalcVelocity(float v0, float a, float s)
        {
            // v^2 - v0^2 = 2as → v = √(2as + v0^2)
            return Mathf.Sqrt(2 * a * s + v0 * v0);
        }

        /// <summary>
        /// InterpTo ※UEのUKismetMathLibrary::TInterpTo()を参考にしたもの
        /// </summary>O
        /// <returns></returns>
        public static float InterpTo(float current, float target, float deltaTime, float interpSpeed, Ease easeTime = Ease.Linear)
        {
            if (interpSpeed <= 0.0f)
            {
                return target;
            }

            float alpha = Mathf.Clamp(deltaTime * interpSpeed, 0.0f, 1.0f);

            return DOVirtual.EasedValue(current, target, alpha, easeTime);
        }
        public static Vector3 InterpTo(Vector3 current, Vector3 target, float deltaTime, float interpSpeed, Ease easeTime = Ease.Linear)
        {
            if (interpSpeed <= 0.0f)
            {
                return target;
            }

            float alpha = Mathf.Clamp(deltaTime * interpSpeed, 0.0f, 1.0f);

            return DOVirtual.EasedValue(current, target, alpha, easeTime);
        }
        public static Quaternion InterpTo(Quaternion current, Quaternion target, float deltaTime, float interpSpeed, Ease easeTime = Ease.Linear)
        {
            if (interpSpeed <= 0.0f)
            {
                return target;
            }

            float alpha = Mathf.Clamp(deltaTime * interpSpeed, 0.0f, 1.0f);

            Vector3 currentXyz = new Vector3(current.x, current.y, current.z);
            Vector3 targetXyz = new Vector3(target.x, target.y, target.z);
            var xyz = DOVirtual.EasedValue(currentXyz, targetXyz, alpha, easeTime);
            var w = DOVirtual.EasedValue(current.w, target.w, alpha, easeTime);
            return new Quaternion(xyz.x, xyz.y, xyz.z, w);
        }
    }
}