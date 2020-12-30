using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// A class for holding information about a limb's current objective
/// </summary>
/// 
[Serializable]
public class BehaviorObjective
{
    [SerializeField] private BehaviorObjectiveType objectiveType;
    public Action<IKManager, Transform> actionPerFrame { get; set; }
    public Func<IKManager, Transform, BehaviorResult> conditionPerFrame { get; set; }

    // TODO provide method to set root
    [SerializeField] private IKManager root;
    [SerializeField] Transform target;
    
    public OnBehaviorSuccess onSuccess;
    public OnBehaviorFail onFail;

    public BehaviorObjective(BehaviorObjectiveType _type, Action<IKManager, Transform> _actionPerFrame, Func<IKManager, Transform, BehaviorResult> _condition)
    {
        objectiveType = _type;
        actionPerFrame = _actionPerFrame;
        conditionPerFrame = _condition;
    }

    public void UpdateRoot(IKManager _root)
    {
        root = _root;
    }

    public void UpdateTarget(Transform _target)
    {
        target = _target;
    }

    public Transform GetTarget()
    {
        return target;
    }

    public void OnUpdate()
    {
        // on each frame, MonoBehaviour will call OnUpdate
        // actionPerFrame is a-function-of $target
        if (actionPerFrame != null) actionPerFrame.Invoke(root, target);

        // on each frame, MonoBehaviour will evaluate condition
        // condition is a-function-of $origin and $target
        // and sets a flag which indicates whether the behavior objective's state result is success/undetermined so far/fail
        if (conditionPerFrame != null)
        {
            BehaviorResult result = conditionPerFrame.Invoke(root, target);
            if (result == BehaviorResult.SUCCESS)
            {
                onSuccess?.Invoke();
            }
            else if (result == BehaviorResult.FAIL)
            {
                onFail?.Invoke();
            }
        }

    }
}

public delegate void OnBehaviorSuccess();
public delegate void OnBehaviorFail();

public enum BehaviorObjectiveType
{
    TOUCH,
    PASS_THROUGH
}

public enum BehaviorResult
{
    SUCCESS,
    CONTINUE,
    FAIL
}