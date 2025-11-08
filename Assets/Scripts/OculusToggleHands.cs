using UnityEngine;
using UnityEngine.VFX;
//using ExitGames.Client.Photon.StructWrapping;

public class OculusToggleHands : MonoBehaviour
{

    public GameObject leftHand;
    public GameObject rightHand;
    public GameObject leftController;
    public GameObject rightController;

    //public VisualEffect galaxyFX;
    public VisualEffect leftFX;
    public VisualEffect rightFX;

    public Transform offsetL;
    public Transform offsetR;

    public bool handsOnly;
    /*
    public VRIK vrik;
    public Transform vrikControllerL;
    public Transform vrikControllerR;
    public Transform vrikHandL;
    public Transform vrikHandR;
    */

    //public Transform targetHandCenterL;
    //public Transform targetHandCenterR;

    private Transform targetL;
    private Transform targetR;

    //public bool testSwitch = false;

    private enum ControllerType
    {
        QuestAndRiftS = 1,
        Rift = 2,
        Quest2 = 3,
    }

    private ControllerType activeControllerType = ControllerType.Rift;

    private bool handTrackingToggle = false;
    // Start is called before the first frame update
    void Start()
    {

        OVRPlugin.SystemHeadset headset = OVRPlugin.GetSystemHeadsetType();

        switch (headset)
        {
            case OVRPlugin.SystemHeadset.Rift_CV1:
                activeControllerType = ControllerType.Rift;
                break;
            case OVRPlugin.SystemHeadset.Oculus_Quest_2:
                activeControllerType = ControllerType.Quest2;
                break;
            default:
                activeControllerType = ControllerType.QuestAndRiftS;
                break;
        }

        bool handTracking = OVRPlugin.GetHandTrackingEnabled();


        if (!handsOnly)
        {
            leftHand.SetActive(handTracking);
            rightHand.SetActive(handTracking);
            leftController.SetActive(!handTracking);
            rightController.SetActive(!handTracking);
        }
            
        

        if (handTracking)
        {
            ActivateHandTracking();
        }
        else
        {
            if(!handsOnly)
                ActivateControllers();
        }

        handTrackingToggle = handTracking;

    }

    // Update is called once per frame
    void Update()
    {
        bool handTracking = OVRPlugin.GetHandTrackingEnabled();

        if (handTracking && !handTrackingToggle)
        {
            ActivateHandTracking();

        }
        else if(!handTracking && handTrackingToggle)
        {
            if(!handsOnly)
                ActivateControllers();
        }


        
        if (handTracking)
        {
            if (leftHand.GetComponent<OVRHand>().IsTracked)
            {
                leftFX.gameObject.SetActive(true);
                //galaxyFX.SetBool("Left Tracking", true);
                UpdateLeftTarget();
            }
            else
            {
                leftFX.gameObject.SetActive(false);
                //galaxyFX.SetBool("Left Tracking", false);
            }

            if (rightHand.GetComponent<OVRHand>().IsTracked)
            {
                rightFX.gameObject.SetActive(true);
                //galaxyFX.SetBool("Right Tracking", true);
                UpdateRightTarget();
            }
            else
            {
                rightFX.gameObject.SetActive(false);
                //galaxyFX.SetBool("Right Tracking", false);
            }
        }
        
        else
        {
            if(!handsOnly)
            {
                UpdateLeftTarget();
                UpdateRightTarget();
            }
            
            //leftFX.gameObject.SetActive(true);
            //rightFX.gameObject.SetActive(true);
        }
        

        handTrackingToggle = handTracking;


    }

    private void UpdateLeftTarget()
    {
        if (targetL)
        {
            offsetL.position = targetL.position;
            offsetL.rotation = targetL.rotation;
            offsetL.localScale = targetL.localScale;
        }

    }

    private void UpdateRightTarget()
    {

        if (targetR)
        {
            offsetR.position = targetR.position;
            offsetR.rotation = targetR.rotation;
            offsetR.localScale = targetR.localScale;
        }
    }

private void ActivateHandTracking()
    {
        SkinnedMeshRenderer leftMesh = leftHand.GetComponentInChildren<SkinnedMeshRenderer>();
        SkinnedMeshRenderer rightMesh = rightHand.GetComponentInChildren<SkinnedMeshRenderer>();

        /*
        if(vrik)
        {
            vrik.solver.leftArm.target = vrikHandL;
            vrik.solver.rightArm.target = vrikHandR;
        }
        */


        leftHand.SetActive(true);
        rightHand.SetActive(true);
        leftController.SetActive(false);
        rightController.SetActive(false);

        
        if (leftMesh)
        {

            leftFX.SetSkinnedMeshRenderer("Mesh",leftMesh);
        }

        if (rightMesh)
        {

            rightFX.SetSkinnedMeshRenderer("Mesh", rightMesh);
        }
        

        targetL = leftMesh.rootBone;
        targetR = rightMesh.rootBone;

    }
    
    private void ActivateControllers()
    {

        leftHand.SetActive(false);
        rightHand.SetActive(false);
        leftController.SetActive(true);
        rightController.SetActive(true);

        //otherwise set the VFX to one of the controllers
        leftFX.gameObject.SetActive(true);
        rightFX.gameObject.SetActive(true);

        /*
        if(vrik)
        {
            vrik.solver.leftArm.target = vrikControllerL;
            vrik.solver.rightArm.target = vrikControllerR;
        }
        */

        //code for multiple controller support used in Unity 20221
        
        OVRControllerHelper leftOVR = leftController.GetComponent<OVRControllerHelper>();
        if (activeControllerType == ControllerType.QuestAndRiftS)
        {

            leftFX.SetSkinnedMeshRenderer("SkinnedMeshRenderer", leftOVR.m_modelOculusTouchQuestAndRiftSLeftController.gameObject.GetComponentInChildren<SkinnedMeshRenderer>());
            targetL = leftOVR.m_modelOculusTouchQuestAndRiftSLeftController.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().rootBone;
        }
        else if (activeControllerType == ControllerType.Rift)
        {
            leftFX.SetSkinnedMeshRenderer("SkinnedMeshRenderer", leftOVR.m_modelOculusTouchRiftLeftController.gameObject.GetComponentInChildren<SkinnedMeshRenderer>());
            targetL = leftOVR.m_modelOculusTouchRiftLeftController.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().rootBone;
        }
        else if (activeControllerType == ControllerType.Quest2)
        {
            leftFX.SetSkinnedMeshRenderer("SkinnedMeshRenderer", leftOVR.m_modelOculusTouchQuest2LeftController.gameObject.GetComponentInChildren<SkinnedMeshRenderer>());
            targetL = leftOVR.m_modelOculusTouchQuest2LeftController.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().rootBone;
        }


        OVRControllerHelper rightOVR = rightController.GetComponent<OVRControllerHelper>();
        if (activeControllerType == ControllerType.QuestAndRiftS)
        {
            rightFX.SetSkinnedMeshRenderer("SkinnedMeshRenderer", rightOVR.m_modelOculusTouchQuestAndRiftSRightController.gameObject.GetComponentInChildren<SkinnedMeshRenderer>());
            targetR = rightOVR.m_modelOculusTouchQuestAndRiftSRightController.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().rootBone;
        }
        else if (activeControllerType == ControllerType.Rift)
        {
            rightFX.SetSkinnedMeshRenderer("SkinnedMeshRenderer", rightOVR.m_modelOculusTouchRiftRightController.gameObject.GetComponentInChildren<SkinnedMeshRenderer>());
            targetR = rightOVR.m_modelOculusTouchRiftRightController.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().rootBone;
        }
        else if (activeControllerType == ControllerType.Quest2)
        {
            rightFX.SetSkinnedMeshRenderer("SkinnedMeshRenderer", rightOVR.m_modelOculusTouchQuest2RightController.gameObject.GetComponentInChildren<SkinnedMeshRenderer>());
            targetR = rightOVR.m_modelOculusTouchQuest2RightController.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().rootBone;
        }
        

    }

}
