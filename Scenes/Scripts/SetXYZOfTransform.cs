using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetXYZOfTransform : MonoBehaviour
{
    [SerializeField] private Transform target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public float SetX
    {
        set
        {
            if (target != null)
            {
                target.position = new Vector3(value, target.position.y, target.position.z);
            }
        }
    }

    public float SetY
    {
        set
        {
            if (target != null)
            {
                target.position = new Vector3(target.position.x, value, target.position.z);
            }
        }
    }

    public float SetZ
    {
        set
        {
            if (target != null)
            {
                target.position = new Vector3(target.position.x, target.position.y, value);
            }
        }
    }
}
