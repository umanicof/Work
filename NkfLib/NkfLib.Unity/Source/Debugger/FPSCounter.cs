using UnityEngine;

namespace NkfLib.Unity
{
    public class FPSCounter : MonoBehaviour
    {
        [SerializeField]
        float _updateInterval = 0.5f;

        float _accum;
        int _frames;
        float _timeleft;
        float _fps;

        private void Update()
        {
            _timeleft -= Time.deltaTime;
            _accum += Time.timeScale / Time.deltaTime;
            _frames++;

            if (0 < _timeleft) return;

            _fps = _accum / _frames;
            _timeleft = _updateInterval;
            _accum = 0;
            _frames = 0;
        }

        private void OnGUI()
        {
            //GUILayout.Label("FPS: " + m_fps.ToString("f2"));
            GUI.Label(new Rect(480, 0, 80, 20), "FPS: " + _fps.ToString("f2"));
        }
    }
}