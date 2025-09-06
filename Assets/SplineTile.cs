using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SplineTile : MonoBehaviour, IPointerClickHandler
{
    public MeshFilter meshFilter;
    [SerializeField] MeshCollider meshCollider;
    TileTfData TfData;
    bool isDataSet = false;//this is for gizmo drawing for now
    public Vector3 cornerPos;
    [Range(0,1)]
    [SerializeField] float PercentageOfCorner = .25f;
    [SerializeField] float sizeOfCorner = 1;
    public void SetOriginTfData(TileTfData tileData)
    {
        TfData = tileData;
        isDataSet = true;
    }
    public void SetAtTfData()
    {
        transform.position = TfData.center;
        transform.rotation = TfData.rotation;

        Mesh mesh = meshFilter.mesh;
        mesh.SetVertices(TfData.cornersInLocal);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        meshCollider.sharedMesh = meshFilter.mesh;
    }
    private void OnDrawGizmos()
    {
        if(!isDataSet ) { return; }
        //Draw the corner
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.TransformPoint(GetLocalCornerPos()), sizeOfCorner);

    }
    public void MoveToOrigin()
    {
        StartCoroutine(moving());

        IEnumerator moving()
        {
            float timer = 0;
            const float movingTime = .5f;
            yield return new WaitForSeconds(Random.Range(0, movingTime));
            transform.DOMove(TfData.center, movingTime).SetEase(Ease.OutBack);

            while (timer < movingTime)
            {
                timer += Time.deltaTime;

                List<Vector3> newVerts = new();
                for (int i = 0; i < meshFilter.mesh.vertexCount; i++)
                {
                    Vector3 targetPos = TfData.cornersInLocal[i];
                    Vector3 currentPos = meshFilter.mesh.vertices[i];
                    Vector3 lerpedPos = Vector3.Lerp(currentPos, targetPos, timer / movingTime);
                    newVerts.Add(lerpedPos);
                }
                meshFilter.mesh.SetVertices(newVerts);

                Quaternion targetRot = TfData.rotation;
                Quaternion currentRot = transform.rotation;
                Quaternion lerpedRot = Quaternion.Lerp(currentRot, targetRot, timer / movingTime);
                transform.rotation = lerpedRot;
                yield return null;
            }

            SetAtTfData();
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
    
    Vector3 GetLocalCornerPos()
    {
        int TopCornerIndex = GetMostTopCornerIndexW();
        Vector3 topCornerL = TfData.cornersInLocal[TopCornerIndex];
        Vector3 opositeCorner = TfData.cornersInLocal[GetOpositeCornerIndex(TopCornerIndex)];;
        return ((opositeCorner - topCornerL) * PercentageOfCorner) + topCornerL;

        //
        int GetMostTopCornerIndexW()
        {
            float maxValue = -999999999;
            int maxIndex = -1;
            for (int i = 0; i < TfData.cornersInWorld.Count; i++)
            {
                Vector3 thisCornerPos = TfData.cornersInWorld[i];
                if (thisCornerPos.z > maxValue)
                {
                    maxValue = thisCornerPos.z;
                    maxIndex = i;
                }
            }
            return maxIndex;
        }
    }
    static int GetOpositeCornerIndex(int thisIndex)
    {
        switch(thisIndex)
        {
            case 0:return 3;
            case 1: return 2;
            case 2: return 1;
            case 3: return 0;
            default: return -1;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(gameObject.name);
    }
}
