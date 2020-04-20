using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Extensor : MonoBehaviour
{
    Quaternion localLookAway;
    const float lookK = 5.0f;

    private void Start()
    {
        StartCoroutine(RandomLookAway());
    }

    IEnumerator RandomLookAway()
    {
        for (; ;)
        {
            localLookAway = Quaternion.LookRotation(transform.localPosition) * Quaternion.Euler(Random.Range(-lookK, lookK), Random.Range(-lookK, lookK), Random.Range(-lookK, lookK));
            yield return new WaitForSeconds(Random.value * 1.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.localRotation = Quaternion.Lerp(transform.localRotation, localLookAway, 0.9f * Time.deltaTime);
    }
}
