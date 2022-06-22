using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GameController : MonoBehaviour
{
    private Dictionary<string, bool> keyStateDict;
    private Dictionary<string, Action> keyActionDict;

    private List<int> indexlist;
    private int curIdx;
    private Dictionary<int, List<Vector3>> resetPosDict;

    // Start is called before the first frame update
    void Start()
    {
        keyStateDict = new Dictionary<string, bool>();
        keyStateDict.Add("r", false); // reset object positions
        keyStateDict.Add("s", false); // switch to next object
        keyStateDict.Add("a", false); // enable/disable alignment
        keyStateDict.Add("t", false); // try alignment
        keyStateDict.Add("1", false); // alignment-record center
        keyStateDict.Add("2", false); // alignment-record right
        keyStateDict.Add("3", false); // alignment-record front

        keyActionDict = new Dictionary<string, Action>();
        keyActionDict.Add("r", ResetObjectPos); // bind key 'r' to Function
        keyActionDict.Add("s", SwitchNextObject); // same
        keyActionDict.Add("a", SwitchAlignment);
        keyActionDict.Add("t", tryAlignment);
        keyActionDict.Add("1", recordCenterPoint);
        keyActionDict.Add("2", recordRightPoint);
        keyActionDict.Add("3", recordFrontPoint);


        indexlist = new List<int>();
        var root = GameObject.Find("DisplayRoot");
        for (int i = 0; i < root.transform.childCount; i++)
        {
            var trans = root.transform.GetChild(i);
            trans.gameObject.SetActive(false);
            indexlist.Add(i);
        }
        var rnd = new System.Random();
        indexlist = indexlist.OrderBy(item => rnd.Next()).ToList();
        curIdx = 0;

        // add reset position by hand
        resetPosDict = new Dictionary<int, List<Vector3>>();
        resetPosDict.Add(0, new List<Vector3> { new Vector3(-4.656613e-10f, 0.029f, -0.1170089f), Vector3.zero });
        resetPosDict.Add(1, new List<Vector3> { new Vector3(-2.782326e-08f, -0.0061f, -0.1170088f), new Vector3(0, 90, 0) });
        resetPosDict.Add(2, new List<Vector3> { new Vector3(1.862645e-09f, 1.192093e-07f, -0.1170087f), new Vector3(0, 90, 0) });
        resetPosDict.Add(3, new List<Vector3> { new Vector3(-5.804119e-06f, -0.001913428f, -0.1153618f), new Vector3(0, 90, 0) });
        resetPosDict.Add(4, new List<Vector3> { new Vector3(1.634809e-08f, -0.004999757f, -0.1170086f), new Vector3(0, 90, 0) });
        resetPosDict.Add(5, new List<Vector3> { new Vector3(7.52043e-08f, 1.788139e-07f, -0.1170085f), new Vector3(0, 90, 0) });
        resetPosDict.Add(6, new List<Vector3> { new Vector3(1.117589e-08f, -0.000848949f, -0.05400808f), new Vector3(0, 90, 0) });
        resetPosDict.Add(7, new List<Vector3> { new Vector3(-2.421439e-08f, -0.01739055f, -0.1170088f), new Vector3(0, 90, 0) });
        resetPosDict.Add(8, new List<Vector3> { new Vector3(1.949957e-09f, 0.004601657f, -0.1170087f), new Vector3(0, 90, 0) });
        resetPosDict.Add(9, new List<Vector3> { new Vector3(-1.862645e-08f, -0.006591797f, -0.1170087f), new Vector3(0, 90, 0) });
        resetPosDict.Add(10, new List<Vector3> { new Vector3(0, 0.0119648f, -0.1170087f), new Vector3(0, 90, 0) });
        resetPosDict.Add(11, new List<Vector3> { new Vector3(-5.355105e-09f, -0.00302583f, -0.1170087f), new Vector3(0, 90, 0) });
        resetPosDict.Add(12, new List<Vector3> { new Vector3(1.468121e-09f, 0.02450001f, -0.1170087f), new Vector3(0, 90, 0) });

        ResetObjectPos(); // reset at first
    }

    // Update is called once per frame
    void Update()
    {
        HandleDeviceInput();
    }

    void HandleDeviceInput()
    {
        List<string> keyList = new List<string>();
        foreach(var key in keyStateDict.Keys)
        {
            keyList.Add(key);
        }
        for(int i=0; i<keyList.Count; i++)
        {
            string keyValue = keyList[i];
            bool preClickState = keyStateDict[keyValue];
            bool currentClickState = Input.GetKeyDown(keyValue);
            if (!preClickState && currentClickState)
            {
                // onclick
                Action ac = keyActionDict[keyValue];
                ac();
            }
            keyStateDict[keyValue] = currentClickState;
        }
    }

    void SwitchNextObject()
    {
        Debug.Log("SwitchNextObject");
        int selectIdx = indexlist[curIdx];
        Debug.Log("Select Index:" + selectIdx);
        var root = GameObject.Find("DisplayRoot");
        for (int i = 0; i < root.transform.childCount; i++)
        {
            var trans = root.transform.GetChild(i);
            trans.gameObject.SetActive(trans.name == selectIdx.ToString());
        }
        curIdx += 1;
        curIdx %= indexlist.Count;
    }

    void ResetObjectPos()
    {
        //todo, ajust all virtual objects by hand like rotation/position/scale, and create a empty gameobject name DefaultPosition to get these info.
        Debug.Log("ResetObjectPos");
        var root = GameObject.Find("DisplayRoot");
        for (int i = 0; i < root.transform.childCount; i++)
        {
            var trans = root.transform.GetChild(i);
            int index = -1;
            int.TryParse(trans.name, out index);
            if (resetPosDict.ContainsKey(index))
            {
                trans.localPosition = resetPosDict[index][0];
                trans.localEulerAngles = resetPosDict[index][1];
            }
            else
            {
                trans.localPosition = new Vector3(0.0f, 0.22f, -0.1170087f);
            }
        }
    }

    void SwitchAlignment()
    {
        Alignment alignment = GetComponent<Alignment>();
        alignment.OnSwitchAlignment();
    }

    void tryAlignment()
    {
        Alignment alignment = GetComponent<Alignment>();
        alignment.OnTryAlignment();
    }

    void recordCenterPoint()
    {
        Alignment alignment = GetComponent<Alignment>();
        alignment.OnRecordCenterPoint();
    }

    void recordRightPoint()
    {
        Alignment alignment = GetComponent<Alignment>();
        alignment.OnRecordRightPoint();
    }

    void recordFrontPoint()
    {
        Alignment alignment = GetComponent<Alignment>();
        alignment.OnRecordFrontPoint();
    }
}
