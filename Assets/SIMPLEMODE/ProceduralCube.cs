using UnityEngine;

public class ProceduralCube : MonoBehaviour
{
    [SerializeField] float sideSize = 1;
    [SerializeField] float randomRadius = .1f;
    [SerializeField] float randomUpdateInterval = 0.1f;
    Mesh mesh;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;

    private void Start()
    {
        meshFilter = gameObject.GetComponent<MeshFilter>();
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        mesh = new Mesh();

        UpdateVertices();

        Vector2[] uvs =
        {
            new Vector2(0, 0),
            new Vector2(0,1),
            new Vector2(1,1),
            new Vector2(1,0),

            new Vector2(0, 0),
            new Vector2(0,1),
            new Vector2(1,1),
            new Vector2(1,0)
        };

        int[] triangles = {
            0,1,2,2,3,0, //up

            3,2,6,6,7,3,

            0,3,7,7,4,0,

            5,1,0,0,4,5,

            6,2,1,1,5,6,

            6,5,4,4,7,6 //down
        };

        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }


    float timer = 0;
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > randomUpdateInterval)
        {
            timer = 0;
            UpdateVertices();
        }
    }
    void UpdateVertices()
    {
        Vector3[] vertices = {
            new Vector3(-sideSize/2, sideSize/2, -sideSize/2) + (Random.insideUnitSphere * randomRadius),    //0 esquerra baix dalt
            new Vector3(-sideSize/2, sideSize/2, sideSize/2) + (Random.insideUnitSphere * randomRadius),     //1 davant esquerra dalt
            new Vector3( sideSize/2, sideSize/2, sideSize/2) + (Random.insideUnitSphere * randomRadius),     //2 davant dreta dalt 
            new Vector3( sideSize/2, sideSize/2, -sideSize/2f) + (Random.insideUnitSphere * randomRadius),   //3 darrere dreta dalt
            new Vector3(-sideSize/2, -sideSize/2, -sideSize/2) + (Random.insideUnitSphere * randomRadius),   //4 esquerra baix baix
            new Vector3(-sideSize/2, -sideSize/2, sideSize/2) + (Random.insideUnitSphere * randomRadius),    //5 davant esquerra baix
            new Vector3( sideSize/2, -sideSize/2, sideSize/2) + (Random.insideUnitSphere * randomRadius),    //6 davant dreta baix 
            new Vector3( sideSize/2, -sideSize/2, -sideSize/2f) + (Random.insideUnitSphere * randomRadius),  //7 darrere dreta baix
        };

        mesh.vertices = vertices;
    }
}
