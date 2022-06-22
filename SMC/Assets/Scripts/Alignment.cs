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

    void Start()
    {
        isSetting = false;
        alignmentRoot.gameObject.SetActive(isSetting);
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

        Debug.Log("[Alignment] Start Alignment!");

        //(1) pos
        objectRoot.localPosition = center.localPosition;

        //(2) scale
        Vector2 rightPos = new Vector3(right.localPosition.x, right.localPosition.z);
        Vector2 centerPos = new Vector3(center.localPosition.x, center.localPosition.z);
        float scaleZ = (rightPos - centerPos).magnitude * 2.0f;

        Vector2 frontPos = new Vector2(front.localPosition.x, front.localPosition.z);
        float scaleX = (frontPos - centerPos).magnitude * 2.0f;

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

    public void OnRecordCenterPoint()
    {
        if (!isSetting)
            return;
        //record center point
        center.position = RightHandTrans.position;
        Debug.Log("[Alignment] Center Point has been recorded!");
    }

    public void OnRecordRightPoint()
    {
        if (!isSetting)
            return;
        Debug.Log("[Alignment] Right Point has been recorded!");
        //record right point
        right.position = RightHandTrans.position;
    }

    public void OnRecordFrontPoint()
    {
        if (!isSetting)
            return;
        Debug.Log("[Alignment] Front Point has been recorded!");
        //record front point
        front.position = RightHandTrans.position;
    }
}
