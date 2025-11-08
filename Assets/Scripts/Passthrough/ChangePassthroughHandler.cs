using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ChangePassthroughHandler : MonoBehaviour
{
    //public Collider col;
    public Gradient outlineGradient;
    public Gradient baseGradient;
    public Gradient spectralGradient;
    public Gradient spectralGradientMax;
    public Gradient activeGradient;
    public Gradient bandColors;

    private void OnTriggerEnter(Collider other)
    {
        GameObject passthrough = other.GetComponentInChildren<PassthroughControl>().gameObject;
        if(passthrough != null)
        {
            passthrough.GetComponent<PassthroughControl>().SetNewGradients(baseGradient, spectralGradient, spectralGradientMax, activeGradient, bandColors);
            passthrough.GetComponent<OutlineAmplitude>().SetAmplitudeGradient(outlineGradient);
        }
    }
}
