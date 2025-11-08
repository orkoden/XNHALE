using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Unity.XR.Oculus;  // plugin types (if you actually use them)

public class OutlineAmplitude : MonoBehaviour
{
 
    public AudioSource source;
    public OVRPassthroughLayer passthroughLayer;
    public float fadeTime = 1f;
    public float gain = 5f;
    public float power = .5f;
    public Gradient amplitudeGradient;
    [Tooltip("Must be a power of 2 (1,2,4,8,16,32,64,128)")] public int sampleSize = 2048;

    public Color output;

    private float smoothedAmplitude = 0;
    private float amplitudeVelocity = 0;
    private float[] samples;

    // Start is called before the first frame update
    void Start()
    {
        samples = new float[sampleSize];
    }

    // Update is called once per frame
    void Update()
    {
        if (source)
        {
            if (source.isPlaying)
            {


                source.GetOutputData(samples, 0);

                float amplitude = 0;

                for (int i = 0; i < samples.Length; i++)
                {
                    amplitude += Mathf.Abs(samples[i]);
                }

                amplitude /= samples.Length;
                amplitude *= gain;

                if (amplitude > smoothedAmplitude)
                {
                    smoothedAmplitude = amplitude;
                    amplitudeVelocity = 0; //reset velocity for smoothing calcualtor;
                }
                else
                {
                    smoothedAmplitude =  Mathf.SmoothDamp(smoothedAmplitude, amplitude, ref amplitudeVelocity, fadeTime);
                }
                               

                if (passthroughLayer)
                {
                    passthroughLayer.edgeColor = amplitudeGradient.Evaluate(Mathf.Pow(Mathf.Clamp01(smoothedAmplitude),power));
                    output = amplitudeGradient.Evaluate(Mathf.Pow(Mathf.Clamp01(smoothedAmplitude),power));
                }
            }
        }
    }


    public void SetAmplitudeGradient(Gradient amplitude_gradient)
    {
        amplitudeGradient = amplitude_gradient;
    }
}
