using UnityEngine;

public class SplineTile : MonoBehaviour
{
    public MeshFilter meshFilter;
    public void SetMeshToStruct(TileInfo tileInfo)
    {
        transform.position = tileInfo.originW;
        Mesh mesh = meshFilter.mesh;
        mesh.SetVertices(tileInfo.cornersFromOrigin);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }
}
