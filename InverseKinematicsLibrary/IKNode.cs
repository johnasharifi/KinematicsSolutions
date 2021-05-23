using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using KinematicsLibrary;

public class IKNode : MonoBehaviour
{
    public IKNode childNode { get; private set; }

    public Transform target;
    
    [SerializeField] private Vector3 minSpread;
    [SerializeField] private Vector3 maxSpread;
    
    private QuaternionReservoir qres = new QuaternionReservoir(1.0f);
    const float maxDampDistance = 15f;
    const float rotationSpeed = 120.0f;
    const int rotationStickiness = 64;

    private void Update()
    {
        Solve();
    }
    private void OnEnable()
    {
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
                
            Quaternion rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(maxIntervention), distanceDampener * rotationSpeed * Time.deltaTime);
            Quaternion localRotation = Quaternion.Inverse(transform.parent.rotation) * rotation;
                
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
			
            float exchangeRate = Mathf.Clamp(Mathf.Pow(1.0f - Time.deltaTime, rotationStickiness), 1E-5f, 1.0f);
            Quaternion leak = qres.Exchange(localRotation, exchangeRate * distanceDampener * rotationSpeed * Time.deltaTime);
            transform.localRotation = leak;
        }
    }

    public Vector3 GetTerminus()
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

	/// <summary>
	/// Update the node's min and max angule values
	/// </summary>
	/// <param name="minAngles">Min angle rotation about XYZ axes that this node can take while trying to touch the target</param>
	/// <param name="maxAngles">Max angle rotation about XYZ axes that this node can take while trying to touch the target</param>
	public void SetMinMaxAngles(Vector3 minAngles, Vector3 maxAngles) 
	{
		minSpread = minAngles;
		maxSpread = maxAngles;
	}
    
    [ContextMenu("Rebuild mesh")]
    private void RebuildMesh()
    {
        Mesh m = BoneDrawLibrary.GetBone(1, GetMagnitude());
        transform.GetComponent<MeshFilter>().mesh = m;
    }
}

