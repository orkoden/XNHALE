using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Unity.XR.Oculus;  // plugin types (if you actually use them)

public class FramerateLimiter : MonoBehaviour
{
    public int targetFramerate = 60;
    public bool setFramerateByVsync = false;
    public bool setFramerateLimit = false;
    public vSync vSyncCount = vSync.off;

    public enum vSync {off,vSync1,vSync2,vSync3,vSync4};
    
    // Start is called before the first frame update
    void Start()
    {

        if (setFramerateByVsync)
            vSyncCount = (vSync)(Screen.currentResolution.refreshRate / targetFramerate);
        else if (setFramerateLimit)
            Application.targetFrameRate = targetFramerate;
        
        QualitySettings.vSyncCount = (int)vSyncCount;      
            
        
    }

    private void OnValidate()
    {
        Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
