using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LoadingEffect : MonoBehaviour
{
    private float sphereThreshold = 0.0f;
    private float modelThreshold = 1.0f;

    public Transform dissolveSphere;
    public Transform modelRoot;
    private Transform constantSettingTrans;
    private Rigidbody rigidbody;

    #region other solution-cube
    // other solution-cube
    //public int maxCubeNum = 10;
    private GameObject cubeRoot;
    private Bounds cubeBounds;

    private List<Vector3> vertices; //relative to cubeRoot

    private void CreateCubeRoot()
    {
        //creat cubeRoot
        cubeRoot = new GameObject();
        cubeRoot.name = "CubeRoot";
        cubeRoot.transform.parent = transform;
        cubeRoot.transform.localPosition = Vector3.zero;
        cubeRoot.transform.localEulerAngles = Vector3.zero;
        cubeRoot.transform.localScale = Vector3.one;

        //cubeBounds = new Bounds();
        vertices = new List<Vector3>();
    }

    void CreateOneCube(Vector3 pos, Vector3 scale, Vector3 eulerAngles)
    {
        //create cube
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.parent = cubeRoot.transform;
        cube.transform.localPosition = pos;
        cube.transform.localScale = scale;
        cube.transform.localEulerAngles = eulerAngles;
        Destroy(cube.GetComponent<BoxCollider>());
    }

    Bounds CalculateLocalBounds()
    {
        //YUAN the below Codes works, but it is too slow. use shader instead!!!!!!!!!
        Bounds bounds = new Bounds();
        vertices.Clear();
        foreach (var filter in GetComponentsInChildren<MeshFilter>())
        {
            for(int i=0; i<filter.mesh.vertices.Length; i++)
            {
                var point = filter.mesh.vertices[i];
                point = filter.transform.TransformPoint(point); // to world
                point = cubeRoot.transform.InverseTransformPoint(point); // world to relative cubeRoot
                vertices.Add(point);
            }

            //var center = render.transform.TransformPoint(render.bounds.center);
            //var size = render.transform.TransformVector(render.bounds.size);
            //center = cubeRoot.transform.InverseTransformPoint(center);
            //size = cubeRoot.transform.InverseTransformVector(size);

            //bounds.Encapsulate(new Bounds(center, size));
        }
        vertices = vertices.Distinct().ToList();

        foreach(var point in vertices)
        {
            bounds.Encapsulate(point);
        }
        return bounds;
    }

    void GenerateCubes()
    {
        BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();//to show it working



        cubeBounds = CalculateLocalBounds();//recursion function, must get applyed to a Bounds
        boxCollider.center = cubeBounds.center;//apply the center and size
        boxCollider.size = cubeBounds.size;

        Debug.Log("Size:" + cubeBounds.size);
        Debug.Log("center:" + cubeBounds.center);

        //CreateOneCube(center, Vector3.one, Vector3.zero);

        //Vector3 interval = size / maxCubeNum;
        //Vector3 startPos = center - size / 2;

        //int cubeIndex = 0;
        //while(cubeIndex < maxCubeNum)
        //{
        //    cubeIndex++;

        //    //calculate data
        //    Vector3 halfInterval = interval * 0.5f;
        //    Vector3 cubePos = startPos + halfInterval * (cubeIndex + 1);
        //    Vector3 cubeScale = halfInterval; // change it later, for test now

        //    CreateOneCube();
        //}
    }
    #endregion

    private void UpdateDissolve(Transform trans, float threshold)
    {
        var renderList = trans.GetComponentsInChildren<MeshRenderer>();
        foreach (var render in renderList)
        {
            foreach (var material in render.materials)
            {
                if (material.shader.name.Contains("Dissolve"))
                {
                    material.SetFloat("_Threshold", threshold);
                    //Debug.Log("threshold: " + threshold);
                }
            }
        }
    }

    private void OnLoadingEffect()
    {
        if (modelThreshold <= 0.0f)
            return;

        float loadingSpeed = constantSettingTrans.GetComponent<ConstantSetting>().loadingSpeed;
        if (sphereThreshold < 1.0f)
        {
            //set sphere
            sphereThreshold += loadingSpeed;
            sphereThreshold = Mathf.Clamp(sphereThreshold, 0, 1);
            dissolveSphere.gameObject.SetActive(true);
            UpdateDissolve(dissolveSphere, sphereThreshold);
        }
        else
        {
            //set model
            modelThreshold -= loadingSpeed;
            modelThreshold = Mathf.Clamp(modelThreshold, 0, 1);
            UpdateDissolve(modelRoot, modelThreshold);
            dissolveSphere.gameObject.SetActive(false);
        }
    }

    private void OnNormalLoad()
    {
        UpdateDissolve(modelRoot, 0);
        dissolveSphere.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        sphereThreshold = 0.0f;
        modelThreshold = 1.0f;
        UpdateDissolve(dissolveSphere, sphereThreshold);
        UpdateDissolve(modelRoot, modelThreshold);
    }

    private void OnDisable()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        constantSettingTrans = GameObject.Find("ObjectRoot").transform;
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        bool isRunLoadingEffect = constantSettingTrans.GetComponent<ConstantSetting>().isRunLoadingEffect;
        if (isRunLoadingEffect)
        {
            OnLoadingEffect();
        }
        else
        {
            OnNormalLoad();
        }
    }

    public void onSelect()
    {
        Debug.Log("onSelect");
        rigidbody.useGravity = false;
        rigidbody.isKinematic = true;
    }

    public void onUnSelect()
    {
        Debug.Log("onUnSelect");
        rigidbody.useGravity = true;
        rigidbody.isKinematic = false;
    }
}
