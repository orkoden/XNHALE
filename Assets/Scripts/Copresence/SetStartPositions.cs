using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
public class SetStartPositions : MonoBehaviour
{
    public Transform[] origins;
    public Transform orientationIndicator;
    //public Transform VRTrackingSpace;
    public Transform transformSettingTarget;

    public GameObject[] indicators;

    //public OVRManager oculusManager;
    public OVRPassthroughLayer passthrough;
    public UnityEvent HideDuringCalibration;
    public UnityEvent ShowAfterCalibration;

    public Vector3 CombinedRelativePositionAndAngle { get; private set; }
    //public Camera disableSkyboxCamera;

    //public TextMeshPro text;
    //public string[] textDisplays = { "Set Target Origin", "Set Local Origin", "Set Origins"};

    //public Transform targetOrigin;

    private string[] transformNames = { "360Video", "CGhug"};
    private string[] transformParts = { "Position", "Rotation" };
    private string[] euclideanCoords = { "x", "y", "z" };
        
    private bool calibrationToggle = false;

    private void Awake()
    {
        
        if (LoadTransforms())
        {            

        }
        else
        {
            Debug.Log("no player prefs found, origins set to default values");
            
        }

        //SetSkyboxAngle(skyboxOffset + origins[0].eulerAngles.y);

    }


    void Update()
    {

        //updates the current transform while it's being set

        Vector3 position = Vector3.zero;
        position.x = transformSettingTarget.position.x;
        position.z = transformSettingTarget.position.z;

        Vector3 angles = Vector3.zero;
        angles.y = transformSettingTarget.eulerAngles.y;
        orientationIndicator.position = position;
        orientationIndicator.eulerAngles = angles;

        Vector3 combine = origins[1].InverseTransformPoint(transformSettingTarget.position);
        combine.y = angles.y - origins[1].eulerAngles.y;
        CombinedRelativePositionAndAngle = combine;


    }

    public void ToggleMenu(bool active)
    {
        Debug.Log("menu open: " + active);
        calibrationToggle = active;

        foreach (GameObject indicator in indicators)
        {
            indicator.SetActive(active);
        }

        if(active)
        {
            HideDuringCalibration.Invoke();
        }
        else
        {
            ShowAfterCalibration.Invoke();
        }

        SetPassthroughActive(active);
    }


    public void SetOriginPosition(int n)
    {
        if(n<origins.Length)
        {
            origins[n].position = orientationIndicator.position;
            origins[n].rotation = orientationIndicator.rotation;
        }
        SaveTransforms();
    }

    public void SetSkyboxAngle(float y)
    {
        while(y>360)
        {
            y -= 360;
        }

        while(y<0)
        {
            y += 360;
        }

        RenderSettings.skybox.SetFloat("_Rotation", y);
    }

    private void SetPassthroughActive(bool active)
    {
        //oculusManager.isInsightPassthroughEnabled = active;
        if(passthrough)
            passthrough.enabled = active;

        Debug.Log("passthrough toggled by calibration, current setting:" + active);
    }

    private bool LoadTransforms()
    {
        bool success = true;

        Vector3[] positions = new Vector3[origins.Length];
        Vector3[] angles = new Vector3[origins.Length];

        for (int i = 0; i < origins.Length; i++)
        {
            for (int j = 0; j < transformParts.Length; j++)
            {
                for (int k = 0; k < euclideanCoords.Length; k++)
                {
                    string name = transformNames[i] + transformParts[j] + euclideanCoords[k];
                    if (PlayerPrefs.HasKey(name))
                    {

                        if (j == 0)
                        {
                            SetFloatToVector(k, ref positions[i], name);
                        }
                        else
                        {
                            SetFloatToVector(k, ref angles[i], name);
                        }
                    }
                    else
                    {
                        success &= false;
                    }

                }

            }
            origins[i].position = positions[i];
            origins[i].eulerAngles = angles[i];

        }

        return success;

    }



    private void SaveTransforms()
    {

        for (int i = 0; i < origins.Length; i++)
        {
            for (int j = 0; j < transformParts.Length; j++)
            {
                for (int k = 0; k < euclideanCoords.Length; k++)
                {
                    string name = transformNames[i] + transformParts[j] + euclideanCoords[k];
                    float set;
                    if (j == 0)
                    {
                        set = GetFloatFromVector(k, origins[i].position);
                    }
                    else
                    {
                        set = GetFloatFromVector(k, origins[i].eulerAngles);
                    }
                    PlayerPrefs.SetFloat(name, set);

                }
            }
        }
        PlayerPrefs.Save();
    }

    float GetFloatFromVector(int index, Vector3 vector)
    {
        float f = 0;
        switch (index)
        {
            case 0:
                f = vector.x;
                break;
            case 1:
                f = vector.y;
                break;
            case 2:
                f = vector.z;
                break;
        }

        return f;
    }

    void SetFloatToVector(int index, ref Vector3 vector, string key)
    {

        switch (index)
        {
            case 0:
                vector.x = PlayerPrefs.GetFloat(key);
                break;
            case 1:
                vector.y = PlayerPrefs.GetFloat(key);
                break;
            case 2:
                vector.z = PlayerPrefs.GetFloat(key);
                break;

        }
    }

}
