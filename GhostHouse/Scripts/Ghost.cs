using DG.Tweening;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.Services.Analytics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class Ghost : MonoBehaviour
{
    public NavMeshAgent Agent { get; private set; }

    public Animator MyAnimator { get; private set; }
    Transform _destinationTransform;
    Tweener _tweener;
    bool _isReached = false;
    bool _isGoing = false;
    float _defaultAgentSpeed;

    protected virtual void Awake()
    {
        Agent = gameObject.GetComponent<NavMeshAgent>();
        MyAnimator = gameObject.GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        Agent.updatePosition = true;
        _defaultAgentSpeed = Agent.speed;
    }

    protected virtual void Update()
    {
        Agent.speed = _defaultAgentSpeed * GameMain.Instance.GetTimeSpeedScale();

        //Agent.isStopped = GameMain.Instance.IsPause;

        // はみ出し復帰
        //if (!Agent.isOnNavMesh)
        //{
        //    NavMeshHit hit;
        //    if (NavMesh.SamplePosition(transform.position, out hit, 2f, NavMesh.AllAreas))
        //    {
        //        Agent.Warp(hit.position);
        //    }
        //}

        if (!_isGoing)
        {
            return;
        }

        bool agent_reached = (!Agent.pathPending && Agent.remainingDistance <= Agent.stoppingDistance);
        if (agent_reached)
        {
            Agent.enabled = false;
            _isGoing = false;
            //var sec = 50.0f / GameMain.Instance.TimeSpeedScale; // 適当に時間を計算
            var sec = 0.5f; // 適当に時間を計算
            _tweener =  transform.DORotateQuaternion(_destinationTransform.rotation, sec).OnComplete(() => {
                transform.rotation = _destinationTransform.rotation;
                _isReached = true;
            });
            transform.DOMove(_destinationTransform.position, sec).OnComplete(() => {
                transform.position = _destinationTransform.position;
            });
        }
    }

    public void GotoDestination(string name)
    {
        _tweener.Kill();
        Agent.enabled = true;
        _isReached = false;
        _isGoing = true;
        _destinationTransform = PlaceManager.Instance.FindDestination(name);
        Agent.SetDestination(_destinationTransform.position);
    }

    public bool IsReached()
    {
        return _isReached;
    }
    public bool IsGoing()
    {
        return _isGoing;
    }

    protected virtual void OnMouseDown()
    {
        if (!GameMain.Instance.IsInGame)
        {
            return;
        }
        PlaceManager.Instance.LockGhost(this);
    }
}
