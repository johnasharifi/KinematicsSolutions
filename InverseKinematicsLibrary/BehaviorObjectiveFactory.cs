using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class for generating instances of BehaviorObjective from parameters
/// </summary>
public static class BehaviorObjectiveFactory
{
    public static BehaviorObjective GetBehaviorObjective(BehaviorObjectiveType objectiveCode)
    {
        if (objectiveCode == BehaviorObjectiveType.TOUCH)
        {
            return GetBehaviorObjectiveTouch();
        }
        // dummy code for now
        return GetBehaviorObjectiveTouch();
    }

    public static BehaviorObjective GetBehaviorObjectiveTouch()
    {
        // set up action per frame
        System.Action<IKManager, Transform> touchPerFrame = (IKManager orig, Transform target) =>
        {
            if (target != null)
            {
                orig.limbTarget = target;
            }
        };

        // set up success condition per frame
        System.Func<IKManager, Transform, BehaviorResult> conditionPerFrame = (IKManager orig, Transform target) =>
        {
            if (target == null) return BehaviorResult.FAIL;
            if (target != null)
            {
                Vector3 p0 = orig.EndEffector;
                Vector3 pTarget = target.position;

                const float touchThreshold = 1.0f;

                if (Vector3.Distance(p0, pTarget) < touchThreshold) return BehaviorResult.SUCCESS;
            }
            return BehaviorResult.CONTINUE;
        };

        // instantiate BehaviorObjective object
        BehaviorObjective bObjective = new BehaviorObjective(BehaviorObjectiveType.TOUCH, touchPerFrame, conditionPerFrame);
        
        // set up success event
        bObjective.onSuccess += () =>
        {
            if (bObjective != null && bObjective.GetTarget() != null)
                Object.Destroy(bObjective.GetTarget().gameObject);
            bObjective = null;
        };

        // set up fail event
        bObjective.onFail += () =>
        {
            bObjective = null;
        };

        return bObjective;
    }

}
