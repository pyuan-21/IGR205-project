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
    }

    void ResetObjectPos()
    {
        //todo
    }

    void HandleDeviceInput()
    {
        targetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryValue);
        targetDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryValue);
        if (!isGrabing && primaryValue)
        {
            Debug.Log("primaryButton click");
            SwitchNextObject();
        }

        if (!isGrabing && secondaryValue)
        {
            Debug.Log("secondaryButton click");
            ResetObjectPos();
        }
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
