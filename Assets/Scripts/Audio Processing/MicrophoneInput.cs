using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrophoneInput : MonoBehaviour
{
    public AudioSource source;
    private string mic;
    public Microphone micro;
    public int sampleRate = 48000;
    public int maxDelay = 4800;
    // Start is called before the first frame update
    void Start()
    {

#if UNITY_EDITOR
        mic = Microphone.devices[Microphone.devices.Length - 1];
#elif UNITY_ANDROID
        mic = Microphone.devices[0];     
#else
        mic = Microphone.devices[Microphone.devices.Length - 1];
#endif

    }

    // Update is called once per frame
    void Update()
    {

        if (!Microphone.IsRecording(mic))
        {

            source.clip = Microphone.Start(mic, true, 1, sampleRate);
            //while (!(Microphone.GetPosition(mic) > 0)) { }
            StartCoroutine(MicrophoneSyncCheck());
            source.loop = true;
            source.Play();
        }
        else
        {
            int micSamples = Microphone.GetPosition(mic);
            int delay = Mathf.Min(Mathf.Abs((micSamples - source.timeSamples)), Mathf.Abs(micSamples + sampleRate - source.timeSamples), Mathf.Abs(micSamples - source.timeSamples - sampleRate));
            if (delay > maxDelay)
            {
                Debug.Log("Calculated delay:" + delay + "; playhead time = " + source.time * 48000 + "; recording time = " + Microphone.GetPosition(mic));
                source.timeSamples = micSamples;
            }
        }


        //while (!(Microphone.GetPosition(mic) > 0)) { }
    }

    IEnumerator MicrophoneSyncCheck()
    {

        while (!(Microphone.GetPosition(mic) > 0)) { }
        Debug.Log(mic.ToString() + " position = " + Microphone.GetPosition(mic));
        yield return null;

    }

    void OnDestroy()
    {
        Microphone.End(mic);
        source.Stop();
        StopAllCoroutines();
    }

    void OnDisable()
    {
        Microphone.End(mic);
        source.Stop();
        StopAllCoroutines();
    }
}
