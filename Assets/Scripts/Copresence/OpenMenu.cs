using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.Events;
using Unity.XR.Oculus;  // plugin types (if you actually use them)

public class OpenMenu : MonoBehaviour
{
    bool startMenuOpen = false;
    public UnityEvent OpenMenuEvent;
    public UnityEvent CloseMenuEvent;
    public UnityEvent GripPressedWhileMenuOpen;
    public UnityEvent TriggerPressedWhileMenuOpen;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //OVRInput.FixedUpdate();
        //OVRInput.Update();

        if (OVRInput.GetDown(OVRInput.Button.Start))
        {
            if (startMenuOpen)
            {
                CloseMenuEvent.Invoke();
                startMenuOpen = false;
            }
            else
            {
                OpenMenuEvent.Invoke();
                startMenuOpen = true;
            }
        }

        if (startMenuOpen && OVRPlugin.GetHandTrackingEnabled())
        {
            CloseMenuEvent.Invoke();
            startMenuOpen = false;
        }


        //Grip Pressed while menu open
        if (startMenuOpen && (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger) || OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger)))
        {
            GripPressedWhileMenuOpen.Invoke();
        }

        //Trigger Pressed while menu open
        if (startMenuOpen && (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) || OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger)))
        {
            TriggerPressedWhileMenuOpen.Invoke();
        }

    }
}
