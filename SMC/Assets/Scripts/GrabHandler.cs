using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabHandler : MonoBehaviour
{
    public Transform pivotLeft;
    public Transform pivotRight;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onGrab()
    {
        var controller = GetComponent<XRGrabInteractable>();
        var node = controller.selectingInteractor.gameObject.GetComponent<XRController>().controllerNode;
        var grabInter = GetComponent<XRGrabInteractable>();
        if(node == XRNode.LeftHand)
        {
            grabInter.attachTransform = pivotLeft;
        }
        else
        {
            grabInter.attachTransform = pivotRight;
        }
        //Debug.Log("node:" + node);
    }

    public void onDetach()
    {
        //Debug.Log("onDetach");
        var grabInter = GetComponent<XRGrabInteractable>();
        grabInter.attachTransform = null;
    }
}
