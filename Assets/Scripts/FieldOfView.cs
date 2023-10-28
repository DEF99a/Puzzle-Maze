
using Assets.Scripts.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] private Material matNormal;
    [SerializeField] private Material matDetected;

    private MeshRenderer meshRenderer;

    private Mesh mesh;
    private float fov;
    private float viewDistance = 3;
    private Vector3 origin;
    private float startAngle;
    private int rayCount;
    Vector3[] vertices;
    int[] triangles;
    //Vector2[] uv;
    bool ignoreLerp = false;
    private Vector3 aimDirection;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        meshRenderer = GetComponent<MeshRenderer>();

        //aimDirection = new Vector3(0, -1);

        fov = 30;
        origin = new Vector3(0, 0, 0);
        rayCount = 100;
        vertices = new Vector3[rayCount + 1 + 1];
        for (int k = 0; k < vertices.Length; k++)
            vertices[k] = GeneralHelper.DefaultVector3;

        //uv = new Vector2[vertices.Length];
        triangles = new int[rayCount * 3];

        //cacheOrigin2 = new List<Vector3>();
    }

    public void SetMaterial(Material material)
    {
        meshRenderer.material = material;
    }   
    
    // Update is called once per frame
    void LateUpdate()
    {
        mesh.Clear();

        int width = 1;
        int height = 1;
        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(0, 0, 0),
            new Vector3(width, 0, 0),
            new Vector3(0, height, 0),
            new Vector3(width, height, 0)
        };
        mesh.vertices = vertices;

        int[] tris = new int[6]
        {
            // lower left triangle
            0, 2, 1,
            // upper right triangle
            2, 3, 1
        };
        mesh.triangles = tris;

        Vector3[] normals = new Vector3[4]
        {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward
        };
        mesh.normals = normals;

        Vector2[] uv = new Vector2[4]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };
        mesh.uv = uv;

        mesh.RecalculateNormals();
    }

    private void OnDrawGizmos()
    {
        //if (showLog)
        //{
        //Gizmos.color = Color.red;
        //for (int k = 0; k < cacheOrigin2.Count; k++)
        //{
        //    Gizmos.DrawSphere(cacheOrigin2[k], 0.1f);
        //}

        //Gizmos.color = Color.yellow;
        //var bs = GetBounds();
        //for (int k = 0; k < bs.Count; k++)
        //{
        //    Gizmos.DrawSphere(bs[k], 0.1f);
        //}
    }

    public Vector3 GetOrigin => this.origin;

    public void SetAimDirection(Vector3 aimDirect, bool atFirst = false)
    {
        if (atFirst) this.aimDirection = aimDirect;
        //if (this.aimDirection == GeneralHelper.DefaultVector3)
        //    this.aimDirection = aimDirect;
        else
        {
            if (!ignoreLerp)
                this.aimDirection = Vector3.Lerp(this.aimDirection, aimDirect, Time.deltaTime * 2f);
            else
                this.aimDirection = Vector3.Lerp(this.aimDirection, aimDirect, Time.deltaTime * 4f);
        }
        startAngle = GeneralHelper.GetAngleFromVectorFloat(this.aimDirection) + fov / 2f;
    }

    public void SetFov(float fov)
    {
        this.fov = fov;
    }

    public void SetViewDistance(float viewDistance)
    {
        this.viewDistance = viewDistance;
    }

    public void SetIgnoreLerp(bool ignoreLerp)
    {
        this.ignoreLerp = ignoreLerp;
    }
}
