using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandPresence : MonoBehaviour
{
    public InputDeviceCharacteristics controllerCharacteristics;    
    private InputDevice targetDevice;
    public Animator handAnimator;
    private Rigidbody rb;
    private bool isGrabing;
    private int objectIndex;
    private int objectMaxNum;
    private bool isPrimaryBtnPressed;
    private bool isSecondaryBtnPressed;

    void Start()
    {
        TryInitialize();
    }

    void TryInitialize()
    {
        List<InputDevice> devices = new List<InputDevice>();

        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);
        if (devices.Count > 0)
        {
            targetDevice = devices[0];
        }

        rb = transform.GetComponent<Rigidbody>();

        isGrabing = false;

        objectIndex = 0;
        objectMaxNum = 2;
        isPrimaryBtnPressed = false;
        isSecondaryBtnPressed = false;
    }

    void UpdateHandAnimation()
    {
        if(targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            handAnimator.SetFloat("Trigger", triggerValue);
            //Debug.Log("Trigger: " + triggerValue);
        }
        else
        {
            handAnimator.SetFloat("Trigger", 0);
        }

        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            handAnimator.SetFloat("Grip", gripValue);
        }
        else
        {
            handAnimator.SetFloat("Grip", 0);
        }
    }

    void SwitchNextObject()
    {
        //todo
        var root = GameObject.Find("DisplayRoot");
        for(int i = 0; i < root.transform.childCount; i++)
        {
            var trans = root.transform.GetChild(i);
            trans.gameObject.SetActive(trans.gameObject.name == objectIndex.ToString());
        }
        objectIndex = (objectIndex + 1) % objectMaxNum;
    }

    void ResetObjectPos()
    {
        //todo
    }

    void HandleDeviceInput()
    {
        targetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryValue);
        if (!isPrimaryBtnPressed && primaryValue)
        {
            //handle click!
            Debug.Log("primaryButton click");

            if (!isGrabing)
            {
                SwitchNextObject();
            }
        }
        isPrimaryBtnPressed = primaryValue;

        targetDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryValue);
        if (!isSecondaryBtnPressed && secondaryValue)
        {
            //handle click!
            Debug.Log("secondaryButton click");

            if (!isGrabing)
            {
                ResetObjectPos();
            }
        }
        isSecondaryBtnPressed = secondaryValue;
    }

    // Update is called once per frame
    void Update()
    {
        if(!targetDevice.isValid)
        {
            TryInitialize();
        }
        else
        {
            UpdateHandAnimation();
            HandleDeviceInput();
        }
    }

    public void OnGrabObject()
    {
        isGrabing = true;
        if (rb != null)
        {
            rb.detectCollisions = false;
            Debug.Log("OnGrabObject, rb.detectCollisions: " + rb.detectCollisions);
        }
    }

    public void OnDetachObject()
    {
        isGrabing = false;
        if (rb != null)
        {
            rb.detectCollisions = true;
            Debug.Log("OnDetachObject, rb.detectCollisions: " + rb.detectCollisions);
        }
    }
}
