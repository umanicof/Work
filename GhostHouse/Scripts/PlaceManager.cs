using NUnit.Framework;
using Redcode.Extensions;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Rendering.DebugUI;

public class PlaceManager : MonoBehaviour
{
    static public PlaceManager Instance;
    public Transform Destinations;
    public Transform CameraTransforms;
    public Transform Floors;
    public Transform Rooms;
    public Ghost TrackingGhost { get; private set; }
    public bool IsTracking => TrackingGhost != null;

    List<CameraTransform> _cameraTransforms = new List<CameraTransform>();
    Dictionary<string, List<Transform>> _floors = new Dictionary<string, List<Transform>>();
    Dictionary<string, Transform> _rooms = new Dictionary<string, Transform>();

    int _cameraIndex = -1;

    public enum ERoom
    {
        BirdView,
        Entrance,
        Living,
        Hole, // Hallの誤字だが直している暇がないのでこれで
        Dining,
        Kitchen,
        Bar,
        Corridor1,
        Corridor2,
        BedRoom1,
        BedRoom2,
        BedRoom3,
        BedRoom4,
        WashRoom,
        BathRoom,
        Library,
        RecreationRoom,
        Max,
    };
    public static string[] ObjectNames = new string[(int)ERoom.Max]
    {
            "BirdView",
            "Entrance",
            "Living",
            "Hole", // Hallの誤字だが直している暇がないのでこれで
            "Dining",
            "Kitchen",
            "Bar",
            "Corridor1",
            "Corridor2",
            "BedRoom1",
            "BedRoom2",
            "BedRoom3",
            "BedRoom4",
            "WashRoom",
            "BathRoom",
            "Library",
            "RecreationRoom",
    };
    public static string[] RoomNames = new string[(int)ERoom.Max]
    {
        "じょう面図",
        "エントランス",
        "リビング",
        "ホール",
        "食堂",
        "キッチン",
        "バー",
        "廊下１",
        "廊下２",
        "寝室１",
        "寝室２",
        "寝室３",
        "寝室３",
        "洗面所",
        "バスルーム",
        "図書室",
        "娯楽室",
    };

    public static ERoom ObjectNameToRoom(string objectName)
    {
        for (int i = 0; i < ObjectNames.Length; ++i)
        {
            var name = ObjectNames[i];
            if (objectName.Contains(name))
            {
                return (ERoom)i;
            }
        }

        Debug.Assert(false);
        return (ERoom)0;
    }

    public static string ObjectNameToRoomName(string objectName)
    {
        ERoom room = ObjectNameToRoom(objectName);
        return RoomNames[(int)room];
    }

    private void Awake()
    {
        Instance = this;

        foreach (var item in CameraTransforms.GetChilds())
        {
            // 念のため親を除く
            if (CameraTransforms.gameObject == item.gameObject)
            {
                return;
            }
            if (item.gameObject.activeSelf)
            {
                var cameraTransform = item.gameObject.GetComponent<CameraTransform>();
                if (cameraTransform)
                {
                    _cameraTransforms.Add(cameraTransform);
                }
            }
        }
        foreach (var item in Floors.GetChilds())
        {
            // 念のため親を除く
            if (Floors.gameObject == item.gameObject)
            {
                return;
            }
            var name = item.gameObject.name;
            if (item.gameObject.activeSelf)
            {
                if (!_floors.TryGetValue(name, out var list))
                {
                    list = new List<Transform>();
                    _floors[name] = list;
                }
                list.Add(item);
            }
        }
        foreach (var item in Rooms.GetChilds())
        {
            // 念のため親を除く
            if (Rooms.gameObject == item.gameObject)
            {
                return;
            }
            var name = item.gameObject.name;
            if (item.gameObject.activeSelf)
            {
                _rooms[name] = item;
            }
        }


        Debug.Log($"cameraTransformsCount={_cameraTransforms.Count} floorsCount={_floors.Count}");

    }
    void Start()
    {
        NextCameraTransform();

    }

    void Update()
    {
        if (TrackingGhost)
        {
            // ゴーストの位置からカメラを設定
            var room = GetRoom(TrackingGhost.transform.position);
            if (room == ERoom.BirdView)
            { // 不正な値として戻ってくるパターンあり
                return;
            }
            SetCamera(GetCameraTransform(room));
        }
    }

    CameraTransform GetCameraTransform(ERoom room)
    {
        var object_name = ObjectNames[(int)room];
        for (int i = 0; i < _cameraTransforms.Count; ++i)
        {
            if (_cameraTransforms[i].gameObject.name.Contains(object_name))
            {
                _cameraIndex = i;
            }
        }
        return _cameraTransforms[_cameraIndex];
    }


    public Transform FindDestination(string name)
    {
        var destination = Destinations.Find(name);
        Debug.Assert(destination, name);
        return destination.transform;
    }

    public void NextCameraTransform()
    {
        TrackingGhost = null; // ロック解除
        if (++_cameraIndex >= _cameraTransforms.Count)
        {
            _cameraIndex = 0;
        }
        SetCamera(_cameraTransforms[_cameraIndex]);
    }
    public void PrevCameraTransform()
    {
        TrackingGhost = null; // ロック解除
        if (--_cameraIndex < 0)
        {
            _cameraIndex = _cameraTransforms.Count - 1;
        }
        SetCamera(_cameraTransforms[_cameraIndex]);
    }

    void SetCamera(CameraTransform cameraTransform)
    {
        cameraTransform.SetMainCamea();
        var roomName = cameraTransform.gameObject.name;
        ERoom room = ObjectNameToRoom(roomName);
        SoundManager.Instance.SetRoomVolume(room);
        GameUI.Instance.SetRoomText(RoomNames[(int)room]);

        // 部屋の有効無効
        if (roomName == "Corridor1" || roomName == "Corridor2")
        {
            roomName = "Corridor";
        }
        foreach (var pair in _rooms)
        {
            if (pair.Key == roomName)
            {
                pair.Value.gameObject.SetActive(true);
            }
            else
            {
                pair.Value.gameObject.SetActive(false);
            }
        }
    }

    public void SetCameraRoom(ERoom room)
    {
        TrackingGhost = null; // ロック解除
        SetCamera(GetCameraTransform(room));
    }

    public bool InFloor(string floorName, Vector3 pos)
    {
        if (!_floors.TryGetValue(floorName, out var list))
        {
            return false;
        }
        foreach (Transform t in list)
        {
            var bounds = t.GetComponent<MeshCollider>().bounds;
            if (bounds.min.x > pos.x || bounds.max.x < pos.x || bounds.min.z > pos.z || bounds.max.z < pos.z)
            {
                return false;
            }
        }

        return true;
    }

    public ERoom GetRoom(Vector3 pos)
    {
        foreach (var pair in _floors)
        {
            foreach (var t in pair.Value)
            {
                var bounds = t.GetComponent<MeshCollider>().bounds;
                if (bounds.min.x <= pos.x && bounds.max.x >= pos.x && bounds.min.z <= pos.z && bounds.max.z >= pos.z)
                {
                    return ObjectNameToRoom(pair.Key);
                }

            }
        }

        // 変な位置で呼ぶと判定できない場合はある
        // ・ゴーストロックで呼ばれるパターンがある
        return (ERoom)0;
    }

    public CameraTransform GetCurrentCameraTransform()
    {
        if (_cameraIndex < 0)
        {
            return null;
        }
        return _cameraTransforms[_cameraIndex];
    }

    public void LockGhost(Ghost ghost)
    {

        if (TrackingGhost == ghost)
        {
            TrackingGhost = null;
            GameUI.Instance.ShowMessageUI("ゴーストの追跡を終わった", ghost.transform.position);
        }
        else
        {
            TrackingGhost = ghost;
            GameUI.Instance.ShowMessageUI("ゴーストの追跡を始めた", ghost.transform.position);
        }

    }
}
