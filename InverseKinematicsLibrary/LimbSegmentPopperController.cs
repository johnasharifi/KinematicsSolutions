using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class LimbSegmentPopperController : MonoBehaviour
{
    private Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        if (cam == null)
            cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        bool lmb = Input.GetMouseButtonDown(0);
        bool rmb = Input.GetMouseButtonDown(1);

        if (lmb || rmb)
        {
            RaycastHit hit;
            if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
            {
                // get the last known node
                IKNode node = hit.transform.GetComponent<IKNode>();
                while (node != null && node.childNode != null)
                    node = node.childNode;

                // if left click on an IKNode, spawn a new node
                if (node != null && lmb)
                {
                    GameObject childNode = GameObject.Instantiate(node.gameObject);
                    childNode.transform.SetParent(node.transform);
                    childNode.transform.localPosition = node.transform.localPosition;
                }
            }
        }
    }
}
