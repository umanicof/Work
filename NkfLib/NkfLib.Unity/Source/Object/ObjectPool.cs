using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
using System.Linq;

namespace NkfLib.Unity
{
    /// <summary>
    /// 拡張オブジェクトプール
    /// ・UniRxのObjectPoolを仕組みを利用
    /// ・TはIPooledObjectを継承しているなら、通知などを行う
    /// </summary>
    [Serializable]
    public class ObjectPoolEx<T>
        : UniRx.Toolkit.ObjectPool<T> where T : Component
    {
        [field: SerializeField] public GameObject Prefab { get; set; }

        // T が IPooledObject かどうか
        public bool TargetIsIPooledObject { get; } = typeof(T).GetInterfaces().Contains(typeof(IPooledObject<T>));

        // 全ての管理オブジェクト
        // ・@note: 起動時にシーンで生成されているものは含まれていない
        public List<T> Objects { get; } = new List<T>();

        // 取得オブジェクト
        // ・Rentされているものはこちら
        // ・@note: 起動時にシーンで生成されているものは含まれていない
        public IEnumerable<T> RentObjects
        {
            get
            {
                return Objects.Where(x => x.gameObject.activeSelf);
            }
        }

        // 取得時イベント
        public Subject<T> OnRent = new Subject<T>();
        // 返却時イベント
        public Subject<T> OnReturn = new Subject<T>();

        /// <summary>
        /// インスタンス生成
        /// </summary>
        /// <returns></returns>
        protected override T CreateInstance()
        {
            //Debug.Log("CreateInstance: " + typeof(T).Name);

            Debug.Assert(Prefab != null, "GameObjectPool: Prefab is null.");
            //Debug.Assert(TargetIsPooledObject);

            var target = GameObject.Instantiate(Prefab).GetComponent<T>();
            if (TargetIsIPooledObject)
            {
                ((IPooledObject<T>)target).PoolRef = this;
            }
            Objects.Add(target);

            Debug.Assert(target != null, "GameObjectPool: The target component is not attached to this game object.");
            return target;
        }

        /// <summary>
        /// OnRentイベント発送
        /// </summary>
        /// <param name="target"></param>
        void RaizeOnRent(T target)
        {
            OnRent.OnNext(target);
            if (TargetIsIPooledObject)
            {
                ((IPooledObject<T>)target).OnRent();
            }
        }
        /// <summary>
        /// OnReturnイベント発送
        /// </summary>
        /// <param name="target"></param>
        void RaizeOnReturn(T target)
        {
            OnReturn.OnNext(target);
            if (TargetIsIPooledObject)
            {
                ((IPooledObject<T>)target).OnReturn();
            }
        }

        /// <summary>
        /// 取得
        /// </summary>
        /// <returns></returns>
        public new T Rent()
        {
            var target = base.Rent();
            RaizeOnRent(target);
            return target;
        }
        public T Rent(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            var target = base.Rent();
            target.transform.position = position;
            target.transform.rotation = rotation;
            target.transform.parent = parent;
            RaizeOnRent(target);
            return target;
        }

        /// <summary>
        /// 返却
        /// </summary>
        /// <returns></returns>
        public new void Return(T target)
        {
            RaizeOnReturn(target);
            target.transform.parent = null;
            base.Return(target);
        }
        public void Return(T target, float seconds)
        {
            Observable.Timer(TimeSpan.FromSeconds(seconds))
                .Subscribe(_ => Return(target));
        }
    }
}