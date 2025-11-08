using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Unity.XR.Oculus;  // plugin types (if you actually use them)

public class HandFollow : MonoBehaviour
{
    public Transform controllerTransform;
    //public Transform trackedHand;
    public GameObject follower;

    // Start is called before the first frame update
    void Start()
    {
        if(follower == null)
        {
            follower = GetComponentInChildren<GameObject>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!OVRPlugin.GetHandTrackingEnabled())
        {
            transform.position = controllerTransform.position;
            transform.rotation = controllerTransform.rotation;
            follower.SetActive(true);
        }
        else
        {
            follower.SetActive(false);
        }
    }
}
