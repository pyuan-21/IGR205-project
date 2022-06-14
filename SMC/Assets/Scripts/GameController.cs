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

    // Start is called before the first frame update
    void Start()
    {
        keyStateDict = new Dictionary<string, bool>();
        keyStateDict.Add("r", false); // reset object positions
        keyStateDict.Add("s", false); // switch to next object

        keyActionDict = new Dictionary<string, Action>();
        keyActionDict.Add("r", ResetObjectPos); // bind key 'r' to Function
        keyActionDict.Add("s", SwitchNextObject); // same

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
        Vector3 pos = new Vector3(-6.07f, 0.8f, 0);

        int selectIdx = indexlist[curIdx];
        var root = GameObject.Find("DisplayRoot");
        for (int i = 0; i < root.transform.childCount; i++)
        {
            var trans = root.transform.GetChild(i);
            if(trans.name == selectIdx.ToString())
            {
                trans.localPosition = pos;
            }
        }
    }

}
