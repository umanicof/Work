using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NkfLib.Unity
{
    // 破壊を受けるコンポーネント
    // ・ダメージを受ける処理を全てコンポーネントにするかも知れない
    public class CrushReciever : MonoBehaviour
    {
        //[SerializeField] Color _CrushParticleColor = default;   // 破壊時のパーティクルの色

        public Collider MyCollider { get; private set; }

        void Awake()
        {
            // 子のどこかに含まれている前提
            MyCollider = GetComponentInChildren<Collider>();
        }

        /// <summary>
        /// 破壊
        /// </summary>
        public void Crush()
        {
#if false
            // パーティクル生成
            var circle = MyCollider.ToCircleXZ();
            var particleIndex = circle.radious < 0.4f ? 0 : 1; // 半径の大きさから使用するパーティクルを決定
            var position = transform.TransformPoint(MyCollider.GetCenter());
            var particle = Instantiate(SceneGame.Current._CrushParticlePrefabs[particleIndex], position, Quaternion.identity, StageController.Current.transform);
            particle.GetComponent<Renderer>().material.SetColor("_Color", _CrushParticleColor);
            Destroy(particle, 3.0f);
    #endif

            // 破棄
            Destroy(gameObject);
        }
    }
}