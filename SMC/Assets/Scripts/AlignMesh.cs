using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class AlignMesh : MonoBehaviour
{
    public Transform HandTransform;
    public float scaleSpeed = 0.01f;

    public Transform targetTrans;
    public Transform startCube;
    public Transform endCube;

    private bool lastPrimaryButtonValue;
    private bool lastSecondaryButtonValue;
    private Vector3 startPos;
    private Vector3 endPos;
    private bool isIncreasingScale = false;

    public enum AligmentState
    {
        None,
        StartPos,
        EndPos,
        Scale
    }

    public AligmentState alignmentState = AligmentState.None;

    public InputDeviceCharacteristics controllerCharacteristics;
    private InputDevice targetDevice;

    void TryInitialize()
    {
        List<InputDevice> devices = new List<InputDevice>();

        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);
        if (devices.Count > 0)
        {
            targetDevice = devices[0];
        }
        lastPrimaryButtonValue = false;
        lastSecondaryButtonValue = false;
    }

    void Start()
    {
        TryInitialize();

        startCube.gameObject.SetActive(false);
        endCube.gameObject.SetActive(false);
    }

    private void OnButtonClick(bool primaryButtonValue, bool secondaryButtonValue)
    {
        switch (alignmentState)
        {
            case AligmentState.None:
                if (primaryButtonValue && !lastPrimaryButtonValue)
                {
                    alignmentState = AligmentState.StartPos; // just enter StartPos
                    Debug.Log("Enter record StartPos!");
                }

                if (secondaryButtonValue && !lastSecondaryButtonValue)
                {
                    isIncreasingScale = !isIncreasingScale;
                    Debug.Log("Change increasingScale: " + isIncreasingScale);
                }
                break;

            case AligmentState.StartPos:
                if(primaryButtonValue && !lastPrimaryButtonValue)
                {
                    alignmentState = AligmentState.EndPos;
                    Debug.Log("Enter record EndPos!");
                }

                if(secondaryButtonValue && !lastSecondaryButtonValue)
                {
                    startPos = HandTransform.position;
                    startCube.gameObject.SetActive(true);
                    startCube.position = startPos;
                    Debug.Log("StartPos has been recorded!");
                }
                break;

            case AligmentState.EndPos:
                if (primaryButtonValue && !lastPrimaryButtonValue)
                {
                    alignmentState = AligmentState.Scale;
                    Debug.Log("Enter Scale state!");
                    //change mesh position
                    targetTrans.position = (startPos + endPos) / 2; // how about the Y
                }

                if (secondaryButtonValue && !lastSecondaryButtonValue)
                {
                    endPos = HandTransform.position;
                    endCube.gameObject.SetActive(true);
                    endCube.position = endPos;
                    Debug.Log("EndPos has been recorded!");
                }
                break;

            case AligmentState.Scale:
                if (primaryButtonValue && !lastPrimaryButtonValue)
                {
                    alignmentState = AligmentState.None;
                    Debug.Log("Back to None!");
                    startCube.gameObject.SetActive(false);
                    endCube.gameObject.SetActive(false);
                }

                if (secondaryButtonValue && !lastSecondaryButtonValue)
                {
                    //change mesh position
                    if (isIncreasingScale)
                        targetTrans.localScale += Vector3.one * scaleSpeed;
                    else
                        targetTrans.localScale -= Vector3.one * scaleSpeed;

                    Debug.Log("Changing Scale!");
                }
                break;
        }
    }

    private void Update()
    {
        if (!targetDevice.isValid)
        {
            TryInitialize();
        }

        targetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonValue);
        targetDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryButtonValue);
        OnButtonClick(primaryButtonValue, secondaryButtonValue);
        lastPrimaryButtonValue = primaryButtonValue;
        lastSecondaryButtonValue = secondaryButtonValue;
    }
}
