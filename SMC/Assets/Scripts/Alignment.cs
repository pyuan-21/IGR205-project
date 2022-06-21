using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Alignment : MonoBehaviour
{
    public Transform RightHandTrans;

    public Transform alignmentRoot;
    public Transform center, right, front;
    public Transform objectRoot;
    public Transform simpleTableRoot;
    public Transform simpleTableMain;
    public Transform walls;
    public Transform displayRoot;


    private bool isSetting;
    private int recordIndex;//start from 0: center(0)->right(1)->front(2)
    private Vector3 originalPos;

    void Start()
    {
        isSetting = false;
        alignmentRoot.gameObject.SetActive(isSetting);
        recordIndex = 0;
        originalPos = objectRoot.localPosition;
    }

    public void OnSwitchAlignment()
    {
        isSetting = !isSetting;
        alignmentRoot.gameObject.SetActive(isSetting);
    }

    public void OnTryAlignment()
    {
        if (!isSetting)
            return;

        Debug.Log("[Alignment] Current recrod index is " + recordIndex);

        switch (recordIndex)
        {
            case 0:
                //record center point
                center.position = RightHandTrans.position;
                break;
            case 1:
                //record right point
                right.position = RightHandTrans.position;
                break;
            case 2:
                //record front point
                front.position = RightHandTrans.position;
                break;
            default:
                break;
        }

        // **Alignment**
        if (recordIndex == 2)
        {
            Debug.Log("[Alignment] Start Alignment!");

            //(1) pos
            objectRoot.localPosition = center.localPosition;

            //(2) scale
            Vector2 rightPos = new Vector3(right.localPosition.x, right.localPosition.z);
            Vector2 centerPos = new Vector3(center.localPosition.x, center.localPosition.z);
            float scaleZ = (rightPos-centerPos).magnitude * 2.0f;

            Vector2 frontPos = new Vector2(front.localPosition.x, front.localPosition.z);
            float scaleX = (frontPos-centerPos).magnitude * 2.0f;

            float scaleY = simpleTableMain.localScale.y;
            simpleTableMain.localScale = new Vector3(scaleX, scaleY, scaleZ);

            //(3) rotation
            Vector2 dirVec2 = (frontPos - centerPos).normalized;//totest YUAN replace center to zero!!!YUAN
            Vector3 dir = new Vector3(dirVec2.x, 0, dirVec2.y);
            float angle = Vector3.SignedAngle(Vector3.forward, dir, Vector3.up);
            Debug.Log("[Alignment] angle is " + angle);
            walls.localEulerAngles = new Vector3(0, angle, 0);
            simpleTableRoot.localEulerAngles = new Vector3(0, angle, 0);

            //(4) displayRoot
            displayRoot.localEulerAngles = simpleTableRoot.localEulerAngles;
        }

        recordIndex += 1;
        if (recordIndex > 2)
            recordIndex = 0;
    }
}
