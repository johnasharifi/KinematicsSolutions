using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class IKManager : MonoBehaviour
{
    [SerializeField] private BehaviorObjective m_behaviorObjective;

    [SerializeField] private IKNode endNode;
    private readonly HashSet<IKNode> nodes = new HashSet<IKNode>();

    public enum SolveMode { runtime, disabled, step};
    [SerializeField] private SolveMode _solveMode;

    [SerializeField] private bool nodesCanSway = true;

    /// <summary>
    /// Runs after play starts.
    /// Doesn't run in edit mode.
    /// </summary>
    private void Start()
    {
        if (endNode == null || nodes == null || nodes.Count == 0) UpdateNodeSet();

        // have to cache the target because it is re-spawned on init
        Transform tempTarget = null;
        if (m_behaviorObjective != null && m_behaviorObjective.GetTarget() != null)
            tempTarget = m_behaviorObjective.GetTarget();
        
        m_behaviorObjective = BehaviorObjectiveFactory.GetBehaviorObjective(BehaviorObjectiveType.TOUCH);
        m_behaviorObjective.UpdateRoot(this);
        // reset the Target of the new BehaviorObjective
        m_behaviorObjective.UpdateTarget(tempTarget);
        UpdateSolveModeOnNodes();
    }

    private void Update()
    {
        if (m_behaviorObjective != null)
        {
            // when the IKManager updates, the BehaviorObjective should also run its OnUpdate process
            m_behaviorObjective.OnUpdate();
        }
    }

    public void UpdateSolveModeOnNodes()
    {
        if (_solveMode == SolveMode.runtime)
        {
            SetNodesOperational(true);
        }
        else if (_solveMode == SolveMode.disabled)
        {
            SetNodesOperational(false);
        }
        else if (_solveMode == SolveMode.step)
        {
            SetNodesOperational(true);
            foreach (IKNode n in nodes)
            {
                n.Solve();
            }
            SetNodesOperational(false);
        }
    }

    /// <summary>
    /// If end node exists, get its position in space
    /// </summary>
    public Vector3 EndEffector
    {
        get
        {
            if (endNode != null) return endNode.GetTerminus();
            return transform.position;
        }
    }

    /// <summary>
    /// if IKManager has a target Transform, set/get it
    /// </summary>
    public Transform limbTarget
    {
        set
        {
            // propagate targeting info to the BehaviorObjective
            m_behaviorObjective.UpdateTarget(value);

            foreach (IKNode node in nodes)
            {
                node.target = value;
            }

        }
        get
        {
            if (endNode != null)
                return endNode.target;
            return null;
        }
    }

    private void OnTransformChildrenChanged()
    {
        UpdateNodeSet();
    }

    void UpdateNodeSet()
    {
        IKNode[] nodeArr = transform.GetComponentsInChildren<IKNode>();
        nodes.Clear();

        nodes.UnionWith(nodeArr);

        // traverse to end node, record it as end effector node
        if (nodeArr != null && nodeArr.Length > 0)
        {
            IKNode aNode = nodeArr[0];
            while (aNode != null && aNode.childNode != null) aNode = aNode.childNode;
            endNode = aNode;
        }
    }

    void SetNodesOperational(bool status)
    {
        foreach (IKNode n in nodes)
        {
            n.enabled = status;
        }
    }

    public bool NodesCanSway
    {
        get
        {
            return nodesCanSway;
        }
        set
        {
            nodesCanSway = value;
            foreach (IKNode n in nodes)
            {
                n.sway = System.Convert.ToSingle(nodesCanSway) * 1.0f;
            }
        }
    }
}
