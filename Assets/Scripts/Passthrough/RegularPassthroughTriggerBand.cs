using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Unity.XR.Oculus;  // plugin types (if you actually use them)

[ExecuteInEditMode]
public class RegularPassthroughTriggerBand : MonoBehaviour
{
    public float interval = 4f;
    public PassthroughControl control;
    private float lastTime;

    void Start()
    {
        lastTime = Time.time;
    }
    // Update is called once per frame
    void Update()
    {
        
        if(Time.time - lastTime >= interval)
        {
            lastTime = Time.time;
            control.GenerateRandomBand();
        }
    }
}
