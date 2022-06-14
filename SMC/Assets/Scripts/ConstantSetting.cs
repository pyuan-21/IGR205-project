using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantSetting : MonoBehaviour
{
    public bool isRunLoadingEffect = false;


    [Range(0.0f, 0.01f)]
    public float loadingSpeed = 0.007f;
}
