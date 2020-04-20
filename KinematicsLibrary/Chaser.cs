using UnityEngine;
using KinematicsLibrary;

public class Chaser : MonoBehaviour
{
    [SerializeField] private Transform next;

    private Vector3 vr = Vector3.zero;
    private Quaternion qr = Quaternion.identity;

    const float stability = 0.8f;
    const float leak_alpha = 0.8f;
    private VectorReservoir vec_res = new VectorReservoir(stability, leak_alpha);

    const float trailer_distance = 2.0f;
    
    const float speed = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (next != null)
        {
            Vector3 diff = next.position - transform.position - next.forward * trailer_distance;

            vr += diff;
            Vector3 leak = diff * (0.0f + leak_alpha * Time.deltaTime);
            vr -= leak;

            Vector3 vr_leak = vec_res.Exchange(diff, Time.deltaTime);

            // transform.position += speed * leak;
            transform.position += speed * vr_leak;

            Quaternion ideal = Quaternion.LookRotation(next.position - transform.position);
            transform.rotation = ideal;
        }
    }
}
