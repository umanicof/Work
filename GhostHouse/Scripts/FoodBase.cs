using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class FoodBase : MonoBehaviour
{
    public List<Transform> Contents;

    public string FoodName { get; protected set; }

    public bool CanUse { get; set; }
    public bool IsAte { get; private set; }
    public bool IsUsed { get; private set; }

    public bool IsCanUse => CanUse && !IsAte && !IsUsed;

    protected virtual void Awake()
    {
    }

    protected virtual void Start()
    {
    }

    protected virtual void Update()
    {
    }

    public bool Eat()
    {
        if (IsAte || IsUsed)
        {
            return false;
        }
        IsAte = true;
        foreach (Transform t in Contents)
        {
            t.gameObject.SetActive(false);
        }
        return true;
    }

    public bool Use()
    {
        if (!IsCanUse)
        {
            return false;
        }
        IsUsed = true;
        foreach (Transform t in Contents)
        {
            t.gameObject.SetActive(false);
        }
        return true;
    }
}
