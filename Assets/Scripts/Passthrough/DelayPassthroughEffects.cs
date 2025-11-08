using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using Unity.XR.Oculus;  // plugin types (if you actually use them)

public class DelayPassthroughEffects : MonoBehaviour
{
    public int delayTime = 7;
    public GameObject patternSphere;
    public Component passthroughControl;
    private bool headsetOn = false;
    private bool delayStarted = false;
    public float incrementDuration = 20f; // Duration in seconds to increment value from 0 to 1
    private float intensityValue = 0f;
    private float elapsedTime = 0f;

    
    // Start is called before the first frame update
    void Start()
    {
        //passthroughControl = gameObject.GetComponent<PassthroughControl>();
        StartCoroutine(DelayEffects());
    }

    private void Update()
    {
        if (OVRPlugin.userPresent)
        {
            //if (!headsetOn)
            //{
            //    // Headset just put on
            //    headsetOn = true;
            //    delayStarted = false;
            //}

            //if (!delayStarted)
            //{
            //    // Start incrementing the value
            //    delayStarted = true;
            //    StartCoroutine(DelayEffects());
            //}
            //else
            //{
                
            //}
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

            //// Headset taken off
            //headsetOn = false;
            //delayStarted = false;
            //gameObject.GetComponent<TriggerBand>().enabled = false;
            //gameObject.GetComponent<OutlineAmplitude>().enabled = false;
            //gameObject.GetComponent<PassthroughControl>().intensity = 0f;
            //gameObject.GetComponent<PassthroughControl>().enabled = false;

            //gameObject.GetComponent<OVRPassthroughLayer>().edgeRenderingEnabled = false;
            //patternSphere.SetActive(false);

            
        }
    }

    void DisableAll()
    {
        gameObject.GetComponent<OVRPassthroughLayer>().edgeRenderingEnabled = false;
        gameObject.GetComponent<PassthroughControl>().enabled = false;
        patternSphere.SetActive(false);
        gameObject.GetComponent<TriggerBand>().enabled = false;
        gameObject.GetComponent<OutlineAmplitude>().enabled = false;
        gameObject.GetComponent<PassthroughControl>().intensity = 0f;
    }

    IEnumerator DelayEffects()
    {
        yield return new WaitForSeconds(4);
        gameObject.GetComponent<OVRPassthroughLayer>().edgeRenderingEnabled = true;
        yield return new WaitForSeconds(4);
        gameObject.GetComponent<PassthroughControl>().enabled = true;
        gameObject.GetComponent<TriggerBand>().enabled = true;
        gameObject.GetComponent<OutlineAmplitude>().enabled = true;
        yield return IncrementValue();

        //yield return new WaitForSeconds(delayTime);
        patternSphere.SetActive(true);
        //yield return new WaitForSeconds(delayTime);

    }

    private IEnumerator IncrementValue()
    {
        elapsedTime = 0f;
        gameObject.GetComponent<PassthroughControl>().intensity = 0f;

        while (elapsedTime < incrementDuration)
        {
            elapsedTime += Time.deltaTime;
            gameObject.GetComponent<PassthroughControl>().intensity = Mathf.Clamp01(elapsedTime / incrementDuration);

            yield return null;
        }
    }
}
