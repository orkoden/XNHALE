using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Unity.XR.Oculus;  // plugin types (if you actually use them)

public class TriggerBand : MonoBehaviour
{
    public PassthroughControl control;
    public AudioSource source;
    public float triggerDelay = 0.1f;
    public float triggerThreshold = 2f;
    public float triggerMin = 0.2f;
    public int triggerRange = 400;

    private bool trigger = false;
    private float triggerTime = 0;
    private float average = 0f;
    private float[] samples;

    // Start is called before the first frame update
    void Start()
    {
        triggerTime = Time.time;
        samples = new float[2048];
    }

    //could be improved by focusing on specific frequencies and setting the trigger threshold more relative to the volume

    void Update()
    {
        
            if (source.isPlaying)
            {


                //source.clip.GetData(samples, 0);

                source.GetOutputData(samples, 0);

                float triggerAverage = 0;

                for (int i = 0; i < samples.Length; i++)
                {


                    average += Mathf.Abs(samples[i]);

                    if (i > samples.Length - triggerRange)
                    {
                        triggerAverage += Mathf.Abs(samples[i]);
                    }
                }
                average = average / samples.Length;
                triggerAverage = triggerAverage / triggerRange;



            //activate when the trigger average is twice as high as the base and is a value of at least 0.2f
            if (trigger)
            {
                if (Time.time > triggerTime + triggerDelay)
                {
                    trigger = false;
                }
            }
            else
            {
                if ((triggerAverage > triggerThreshold * average) && triggerAverage > triggerMin)
                {
                    Debug.Log("Triggered, average = " + average + "; trigger average = " + triggerAverage);
                    control.triggerBand = true;
                    trigger = true;
                    triggerTime = Time.time;
                }
            }

        }

        
    }
}
