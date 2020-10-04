using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using KinematicsLibrary;

public class IKNode : MonoBehaviour
{
    private IKNode parentNode;
    public IKNode childNode { get; private set; }

    public Transform target;

    [SerializeField] private bool solving = false;
    public bool isSolving
    {
        get
        {
            return solving;
        }
        set
        {
            solving = value;
        }
    }

    [SerializeField] private Vector3 minSpread;
    [SerializeField] private Vector3 maxSpread;
    
    private QuaternionReservoir qres = new QuaternionReservoir(1.0f);
    const float maxDampDistance = 15f;
    const float echoRate = 0.1f;
    const float rotationOverTimePenalty = 0.5f;

    private void Update()
    {
        Solve();
    }
    private void OnEnable()
    {
        parentNode = transform.parent.GetComponent<IKNode>();
        childNode = GetChildNodeOrNull();
    }

    private void OnTransformChildrenChanged()
    {
        // when new limbs are deleted or spawned, reacquire child node
        childNode = GetChildNodeOrNull();
    }

    private IKNode GetChildNodeOrNull()
    {
        return transform.GetComponentsInChildren<IKNode>().Except(new[] { this }).FirstOrDefault();
    }

    public void Solve()
    {
        // update and Editor loop.update both call Solve

        // TODO in editor, check if editor is paused
        if (solving)
        {
            if (target != null)
            {
                Vector3 intervention = (target.position - GetTerminus());
                Vector3 maxIntervention = intervention;

                float distanceDampener = Mathf.Pow(Mathf.Clamp(intervention.sqrMagnitude, 0.01f, maxDampDistance) / maxDampDistance, 0.5f);
                
                if (maxIntervention.sqrMagnitude < 1E-4)
                {
                    // generate a nonsense vector when at zero-magnitude error
                    // because we cannot calculate a Quaternion.LookRotation for a zero-magnitude vector
                    maxIntervention = Vector3.one;
                }
                maxIntervention.Normalize();
                
                Quaternion rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(maxIntervention), distanceDampener * Mathf.PI * rotationOverTimePenalty);
                Quaternion localRotation = Quaternion.Inverse(transform.parent.rotation) * rotation;
                
                // extent to which child segments retrieve error from parents. 0 to ignore parent localRotation
                localRotation = Quaternion.Lerp(localRotation, transform.parent.localRotation, echoRate * Time.deltaTime);

                System.Func<float, float, float, float> eulerSpread = (float v, float bound1, float bound2) =>
                {
                    return Mathf.Clamp(v > 180f? v - 360f: v, Mathf.Min(bound1, bound2), Mathf.Max(bound1, bound2));
                };
                
                Vector3 clampedEuler = new Vector3(
                    eulerSpread(localRotation.eulerAngles.x, minSpread.x, maxSpread.x),
                    eulerSpread(localRotation.eulerAngles.y, minSpread.y, maxSpread.y),
                    eulerSpread(localRotation.eulerAngles.z, minSpread.z, maxSpread.z)
                    );
                
                localRotation = Quaternion.Euler(clampedEuler);

                // leak rate is HZ at an instant when playing / when FPS is calculable, 1 / 60 otherwise
                float dt = (Application.isPlaying ? Time.deltaTime : 0.016f);
                Quaternion leak = qres.Exchange(localRotation, dt);
                transform.localRotation = leak;
            }
        }
    }

    Vector3 GetTerminus()
    {
        if (childNode != null)
        {
            return childNode.GetTerminus();
        }
        else
        {
            // else this is a terminal node
            return (transform.position + transform.forward * GetMagnitude());
        }
    }

    private float GetMagnitude()
    {
        // hardwire to create a vector that spans to next node
        if (childNode != null)
        {
            return childNode.transform.localPosition.z;
        }
        return Mathf.Max(transform.localScale.z, transform.localPosition.z);
    }
    
    [ContextMenu("Rebuild mesh")]
    private void RebuildMesh()
    {
        Mesh m = BoneDrawLibrary.GetBone(1, GetMagnitude());
        transform.GetComponent<MeshFilter>().mesh = m;
    }
}

