using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

using ERoom = PlaceManager.ERoom;

public class SoundManager : MonoBehaviour
{
    static public SoundManager Instance;
    public AudioSource _wallClock1;
    public AudioSource _wallClock2;
    public AudioSource _alarmClock1; // 鳴らない仕様になった
    public AudioSource _alarmClock2;
    public Piano _piano;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
    }
    void Update()
    {
    }

    public void PauseBGM()
    {
        _piano.PauseBGM();
    }
    public void UnPauseBGM()
    {
        _piano.UnPauseBGM();
    }

    public async UniTask PlayGameEnd()
    {
        //_wallClock1.volume = 0.8f;
        _wallClock2.volume = 0.8f;
        _alarmClock1.volume = 0.0f;
        _alarmClock2.volume = 0.0f;
        _piano.SetBGMsVolume(0.0f);
        _piano.SetSEsVolume(0.0f);

        _wallClock2.Play();
        await UniTask.Delay(2000);
        _wallClock2.Play();
        await UniTask.Delay(2000);

        _wallClock1.volume = 0.0f;
        _wallClock2.volume = 0.0f;
    }

    public void PlayEndingBGM()
    {
        _piano.SetBGMsVolume(0.6f);
        _piano.PlayEndingBGM();
    }

    public void SetRoomVolume(ERoom room)
    {
        switch (room)
        {
            case ERoom.BirdView:
                _wallClock1.volume = 0.0f;
                _wallClock2.volume = 0.0f;
                _alarmClock1.volume = 0.0f;
                _alarmClock2.volume = 0.0f;
                _piano.SetBGMsVolume(0.0f);
                _piano.SetSEsVolume(0.0f);
                break;
            case ERoom.Entrance:
                _wallClock1.volume = 0.8f;
                _wallClock2.volume = 0.8f;
                _alarmClock1.volume = 0.1f;
                _alarmClock2.volume = 0.0f;
                _piano.SetBGMsVolume(0.4f);
                _piano.SetSEsVolume(0.2f);
                break;
            case ERoom.Living:
                _wallClock1.volume = 0.2f;
                _wallClock2.volume = 0.2f;
                _alarmClock1.volume = 0.0f;
                _alarmClock2.volume = 0.0f;
                _piano.SetBGMsVolume(0.3f);
                _piano.SetSEsVolume(0.15f);
                break;
            case ERoom.Hole:
                _wallClock1.volume = 0.2f;
                _wallClock2.volume = 0.2f;
                _alarmClock1.volume = 0.15f;
                _alarmClock2.volume = 0.0f;
                _piano.SetBGMsVolume(1.0f);
                _piano.SetSEsVolume(0.5f);
                break;
            case ERoom.Dining:
                _wallClock1.volume = 0.0f;
                _wallClock2.volume = 0.0f;
                _alarmClock1.volume = 0.0f;
                _alarmClock2.volume = 0.2f;
                _piano.SetBGMsVolume(0.1f);
                _piano.SetSEsVolume(0.05f);
                break;
            case ERoom.Kitchen:
                _wallClock1.volume = 0.0f;
                _wallClock2.volume = 0.0f;
                _alarmClock1.volume = 0.0f;
                _alarmClock2.volume = 0.15f;
                _piano.SetBGMsVolume(0.1f);
                _piano.SetSEsVolume(0.05f);
                break;
            case ERoom.Bar:
                _wallClock1.volume = 0.0f;
                _wallClock2.volume = 0.0f;
                _alarmClock1.volume = 0.2f;
                _alarmClock2.volume = 0.0f;
                _piano.SetBGMsVolume(0.3f);
                _piano.SetSEsVolume(0.15f);
                break;
            case ERoom.Corridor1:
                _wallClock1.volume = 0.25f;
                _wallClock2.volume = 0.25f;
                _alarmClock1.volume = 0.25f;
                _alarmClock2.volume = 0.0f;
                _piano.SetBGMsVolume(0.4f);
                _piano.SetSEsVolume(0.2f);
                break;
            case ERoom.Corridor2:
                _wallClock1.volume = 0.0f;
                _wallClock2.volume = 0.0f;
                _alarmClock1.volume = 0.15f;
                _alarmClock2.volume = 0.15f;
                _piano.SetBGMsVolume(0.2f);
                _piano.SetSEsVolume(0.1f);
                break;
            case ERoom.BedRoom1:
                _wallClock1.volume = 0.0f;
                _wallClock2.volume = 0.0f;
                _alarmClock1.volume = 1.0f;
                _alarmClock2.volume = 0.0f;
                _piano.SetBGMsVolume(0.1f);
                _piano.SetSEsVolume(0.05f);
                break;
            case ERoom.BedRoom2:
                _wallClock1.volume = 0.0f;
                _wallClock2.volume = 0.0f;
                _alarmClock1.volume = 0.2f;
                _alarmClock2.volume = 0.0f;
                _piano.SetBGMsVolume(0.0f);
                _piano.SetSEsVolume(0.0f);
                break;
            case ERoom.BedRoom3:
                _wallClock1.volume = 0.0f;
                _wallClock2.volume = 0.0f;
                _alarmClock1.volume = 0.0f;
                _alarmClock2.volume = 0.25f;
                _piano.SetBGMsVolume(0.0f);
                _piano.SetSEsVolume(0.0f);
                break;
            case ERoom.BedRoom4:
                _wallClock1.volume = 0.0f;
                _wallClock2.volume = 0.0f;
                _alarmClock1.volume = 0.0f;
                _alarmClock2.volume = 1.0f;
                _piano.SetBGMsVolume(0.0f);
                _piano.SetSEsVolume(0.0f);
                break;
            case ERoom.WashRoom:
                _wallClock1.volume = 0.0f;
                _wallClock2.volume = 0.0f;
                _alarmClock1.volume = 0.0f;
                _alarmClock2.volume = 0.1f;
                _piano.SetBGMsVolume(0.0f);
                _piano.SetSEsVolume(0.0f);
                break;
            case ERoom.BathRoom:
                _wallClock1.volume = 0.0f;
                _wallClock2.volume = 0.0f;
                _alarmClock1.volume = 0.0f;
                _alarmClock2.volume = 0.0f;
                _piano.SetBGMsVolume(0.0f);
                _piano.SetSEsVolume(0.0f);
                break;
            case ERoom.Library:
                _wallClock1.volume = 0.0f;
                _wallClock2.volume = 0.0f;
                _alarmClock1.volume = 0.1f;
                _alarmClock2.volume = 0.0f;
                _piano.SetBGMsVolume(0.0f);
                _piano.SetSEsVolume(0.0f);
                break;
            case ERoom.RecreationRoom:
                _wallClock1.volume = 0.1f;
                _wallClock2.volume = 0.1f;
                _alarmClock1.volume = 0.2f;
                _alarmClock2.volume = 0.0f;
                _piano.SetBGMsVolume(0.1f);
                _piano.SetSEsVolume(0.05f);
                break;
        }
    }
}
