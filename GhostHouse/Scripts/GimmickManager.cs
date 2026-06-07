using UnityEngine;
using System;
using System.Collections.Generic;

using ERoom = PlaceManager.ERoom;
using Cysharp.Threading.Tasks;
using System.Linq;

public class GimmickManager : MonoBehaviour
{
    public List<FoodBase> Foods = new List<FoodBase>();
    public GameObject Particle;    
    public AudioSource ParticleSound;

    static public GimmickManager Instance;
    bool _inEvent = false;

    List<FoodBase> ExchangeFoods;
    void Awake()
    {
        Instance = this;
        ExchangeFoods = Foods.ToList();
    }
    void Start()
    {

    }

    void Update()
    {
        
    }

    public async void ExchangeFoodsPosition(Vector3 particlePosition)
    {
        _inEvent = true;
        if (ExchangeFoods.Count >= 2)
        {
            // パーティクル生成
            var go = Instantiate(Particle, particlePosition, Quaternion.identity);
            go.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            ParticleSound.Play();
        }

        await UniTask.Delay(100);

        for (int i = 0; i < ExchangeFoods.Count - 1; ++i)
        {
            int j = i + 1;
            if (j >= ExchangeFoods.Count)
            {
                j = 0;
            }
            var temp_position = Foods[i].transform.position;
            ExchangeFoods[i].transform.position =  ExchangeFoods[j].transform.position;
            ExchangeFoods[j].transform.position = temp_position;
        }
        _inEvent = false;
    }

    public FoodBase FindFood(ERoom room)
    {
        foreach (var food in Foods)
        {
            var food_room = PlaceManager.Instance.GetRoom(food.transform.position);
            if (room == food_room)
            {
                return food;
            }
        }
        return null;
    }

    public async UniTask<bool> EatFood(ERoom room)
    {
        await UniTask.WaitWhile(() => _inEvent);

        var food = FindFood(room);
        if (!food)
        {
            return false;
        }
        if (!food.Eat())
        {
            return false;
        }
        ExchangeFoods.Remove(food);
        return true;
    }

    public bool IsCanUseFood(ERoom room)
    {
        var food = FindFood(room);
        if (!food)
        {
            return false;
        }
        return food.IsCanUse;
    }

    public async UniTask<bool> UseFood(ERoom room)
    {
        await UniTask.WaitWhile(() => _inEvent);

        var food = FindFood(room);
        if (!food)
        {
            return false;
        }
        if (!food.Use())
        {
            return false;
        }
        ExchangeFoods.Remove(food);

        return true;
    }
}

