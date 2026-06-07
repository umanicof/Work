using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace NkfLib.Unity
{
    /// <summary>
    /// シーン制御の基底クラス
    /// ・ここでのOpen、Closeとはユーザーによる明示的なシーンのロード、アンロードのこと。
    ///   Close呼び出しで自身を破棄しようと思ったが、Unloadが呼び出された時点でUpdateなど呼ばれないようになっているようなので、破棄は行っていない。
    /// ・同じシーンを複数同時にロードすることは考慮していない
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SceneBase<T> : SingleMonoBehaviour<SceneBase<T>>
        where T : class, new()
    {
        public static readonly string SceneName = typeof(T).Name.Substring(5); // クラス名から"Scene"を取り除いたものがシーン名

        public static Subject<T> Opened;  // オープン終了イベント
        public static Subject<T> Closing; // クローズ開始イベント

        /// <summary>
        /// キャスト
        /// </summary>
        /// <param name="holder"></param>
        public static implicit operator T(SceneBase<T> sceneBase)
        {
            return sceneBase;
        }

        /// <summary>
        /// オープン
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static async UniTask OpenAsync(LoadSceneMode mode = LoadSceneMode.Additive)
        {
            Debug.Assert(Current == null);
            await SceneManager.LoadSceneAsync(SceneName, mode);
            Opened.OnNext(Current);
        }

        /// <summary>
        /// クローズ
        /// </summary>
        /// <returns></returns>
        public static async UniTask CloseAsync()
        {
            Debug.Assert(Current != null);
            Closing.OnNext(Current);
            await SceneManager.UnloadSceneAsync(SceneName);
        }
    }
}