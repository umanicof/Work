//  http://kan-kikuchi.hatenablog.com/entry/ManagerSceneAutoLoader
//  Created by kan.kikuchi on 2016.08.04.
//#define INITIALIZE_SCENE_AUTO_UNLOAD

using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using Cysharp.Threading.Tasks;

namespace NkfLib.Unity
{
    /// <summary>
    /// Awake前にInitializeシーンを自動でロード・アンロードするクラス
    /// </summary>
    public class InitializeSceneAutoLoader
    {
        const string kSceneName = "";
        //const string kSceneName = "Initialize";

        // ゲーム開始時(シーン読み込み前)に実行される
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        async static void LoadInitializeScene()
        {
            if (!kSceneName.IsNullOrEmpty() && !SceneManager.GetSceneByName(kSceneName).IsValid())
            {
                SceneManager.LoadScene(kSceneName, LoadSceneMode.Additive);
                await UniTask.Yield(PlayerLoopTiming.Initialization);
#if INITIALIZE_SCENE_AUTO_UNLOAD
            var _ = SceneManager.UnloadSceneAsync(kSceneName); // 警告を消すために変数に入れている
#endif
            }
        }
    }
}