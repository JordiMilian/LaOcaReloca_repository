using DG.Tweening;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineTile : MonoBehaviour
{
    public MeshFilter meshFilter;
    public void SetMeshToStruct(TileTfData tileInfo)
    {
        transform.position = tileInfo.originW;
        Mesh mesh = meshFilter.mesh;
        mesh.SetVertices(tileInfo.cornersFromOrigin);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }
    public IEnumerator C_MoveMeshToStruct(TileTfData tileInfo)
    {
        //Dotween
        float timer = 0;
        yield return new WaitForSeconds(Random.Range(0,1.2f));
        transform.DOMove(tileInfo.originW, .5f).SetEase(Ease.OutBack);

        while (timer < .5f)
        {
            timer += Time.deltaTime;

            List<Vector3> newVerts = new();
            for (int i = 0; i < meshFilter.mesh.vertexCount; i++)
            {
                Vector3 targetPos = tileInfo.cornersFromOrigin[i];
                Vector3 currentPos = meshFilter.mesh.vertices[i];
                Vector3 lerpedPos = Vector3.Lerp(currentPos, targetPos, timer / 0.5f);
                newVerts.Add(lerpedPos);
            }
            meshFilter.mesh.SetVertices(newVerts);
            yield return null;
        }
    }
    public void SetToDefaultShape()
    {
        Vector3[] defaultPos = new Vector3[4]
        {
            new Vector3(0,0,0),
            new Vector3(0,0,3),
            new Vector3(-3,0,3),
            new Vector3(-3,0,0)
        };
        Mesh mesh = meshFilter.mesh;
        mesh.SetVertices(defaultPos);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }
}
