using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachTangentSpheres : MonoBehaviour
{
    /// <summary>
    /// Constant that is used to calculate the probability that the ith tangent sphere will have a tangent sub-sphere generation of its own.
    /// </summary>
    private const float pRecurse_i = 0.6f;
    private const float recurseT = 0.1f;
    private const int recurseMax = 10;
    private const int chainMax = 3 ;

    // Start is called before the first frame update
    void Awake()
    {
        RecursiveTangent(transform, 0);
    }

    void RecursiveTangent(Transform t, int generation)
    {
        float p = Mathf.Pow(pRecurse_i, generation);

        if (p > recurseT)
        {
            for (int i = 0; i < Random.Range(1, recurseMax); i++)
            {
                // determine out vector for this chain
                // approximately same direction as vector from root to current
                System.Func<float> R = () =>
                {
                    return Random.Range(-1.0f, 1.0f);
                };
                Vector3 fwd = (t.position - transform.root.position + new Vector3(R(), R(), R())).normalized;

                // spawn root gameObject
                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go.transform.parent = t;
                go.transform.localPosition = Vector3.zero;
                for (int j = 0; j < chainMax; j++)
                {
                    GameObject next = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    next.transform.parent = go.transform;

                    next.transform.localPosition = fwd;
                    // next.transform.localRotation = Quaternion.LookRotation(fwd);
                    
                    next.AddComponent<Extensor>();

                    // change ptr to next
                    go = next;
                }

                RecursiveTangent(go.transform, generation + 1);

                // go.transform.localRotation = Quaternion.LookRotation(fwd);
                // go.transform.localPosition = fwd;
                // go.AddComponent<Extensor>();

                // recurse
                // RecursiveTangent(go.transform, generation + 1);
            }
        }
    }
    
}
