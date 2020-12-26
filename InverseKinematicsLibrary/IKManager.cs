﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class IKManager : MonoBehaviour
{
    private HashSet<IKNode> nodes = new HashSet<IKNode>();

    public enum SolveMode { runtime, disabled, step};
    [SerializeField] private SolveMode _solveMode;

    /// <summary>
    /// Runs after play starts.
    /// Doesn't run in edit mode.
    /// </summary>
    private void Start()
    {
        UpdateSolveModeOnNodes();
    }

    public void UpdateSolveModeOnNodes()
    {
        UpdateNodeSet();
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
    /// Dirty implementation of target overriding for each IKNode which this IKManager might manage.
    /// </summary>
    public Transform limbTarget
    {
        set
        {
            UpdateNodeSet();
            foreach (IKNode node in nodes)
            {
                node.target = value;
            }
        }
        get
        {
            if (nodes != null && nodes.Count > 0)
                return nodes.FirstOrDefault().target;
            return null;
        }
    }

    void UpdateNodeSet()
    {
        nodes = new HashSet<IKNode>(transform.GetComponentsInChildren<IKNode>());
    }

    void SetNodesOperational(bool status)
    {
        foreach (IKNode n in nodes)
        {
            n.enabled = status;
        }
    }


}
