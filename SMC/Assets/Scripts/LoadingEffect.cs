using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LoadingEffect : MonoBehaviour
{
    public bool isRunLoadingEffect;

    [Range(0.0f, 1.0f)]
    public float sphereThreshold = 0.0f;
    [Range(0.0f, 1.0f)]
    public float modelThreshold = 1.0f;

    [Range(0.0f, 1.0f)]
    public float loadingSpeed = 0.01f;

    public Transform dissolveSphere;
    public Transform modelRoot;


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
                if (material.name.Contains("DissolveMat"))
                {
                    material.SetFloat("_Threshold", threshold);
                    Debug.Log("threshold: " + threshold);
                }
            }
        }
    }

    private void OnLoadingEffect()
    {
        if (modelThreshold <= 0.0f)
            return;

        //set sphere
        sphereThreshold += loadingSpeed;
        UpdateDissolve(dissolveSphere, sphereThreshold);

        //set model
        modelThreshold -= loadingSpeed;
        UpdateDissolve(modelRoot, modelThreshold);
    }

    private void OnEnable()
    {
        sphereThreshold = 0.0f;
        modelThreshold = 1.0f;
    }

    private void OnDisable()
    {

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isRunLoadingEffect)
        {
            OnLoadingEffect();
        }
    }
}