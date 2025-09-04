using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Splines;
using UnityEngine.U2D;
using static UnityEngine.UI.Image;

public class SplineBoard_Testing : MonoBehaviour
{
    [SerializeField] SplineContainer spline;
    [SerializeField] SpriteShapeController shape;

    [SerializeField] int tilesCount = 10;
    [SerializeField] float width = 1f;

    List<Vector3> tilesCornersFromOrigin = new();
    List<Vector3> tilesOriginW = new();
    List<TileInfo> tilesInfo = new();
    [SerializeField] GameObject tilePrefab;

    struct TileInfo
    {
        public Vector3 originW;
        public Vector3 centerW;
        public List<Vector3> cornersFromOrigin;
        public Vector3 forwardFromCenter;
    }

    private void OnDrawGizmosSelected()
    {
        UpdateTiles();

        foreach(TileInfo info in tilesInfo)
        {
            Gizmos.color = Color.purple;

            for(int i = 0; i < info.cornersFromOrigin.Count; i++)
            {
                if(i < info.cornersFromOrigin.Count - 1)
                {
                    Gizmos.DrawLine(info.originW + info.cornersFromOrigin[i], info.originW + info.cornersFromOrigin[i+1]);
                }
                else
                {
                    Gizmos.DrawLine(info.originW + info.cornersFromOrigin[i], info.originW + info.cornersFromOrigin[0]);
                }
            }

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(info.centerW, info.centerW + info.forwardFromCenter);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(info.centerW, info.centerW + Vector3.up);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(info.centerW, info.centerW + Vector3.Cross(Vector3.up, info.forwardFromCenter).normalized);
        }
    }

    private void Start()
    {
        UpdateTiles();
        CreateTIles();
    }
    public void UpdateTiles()
    {
        tilesCornersFromOrigin = new();
        tilesOriginW = new();
        tilesInfo = new();

        float TPerTile = 1f / (float)tilesCount;

        //Get corners from all Origins
        for(int i = 0; i < tilesCount +1; i++)
        {
            float t = 1f / (float)tilesCount * i;

            Vector3 tan = spline.EvaluateTangent(t);
            Vector3 forward = tan.normalized;
            Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;

            tilesCornersFromOrigin.Add(right * width / 2);
            tilesCornersFromOrigin.Add(-right * width / 2);
            tilesOriginW.Add(spline.EvaluatePosition(t));
        }

        //Get all the structs filled
        for (int i = 0; i < tilesCount; i++)
        {
            float centerT = 1f / (float)tilesCount * i + (TPerTile/2);
            TileInfo newTileInfo = new();

            newTileInfo.centerW = spline.EvaluatePosition(centerT);
            Vector3 tan = spline.EvaluateTangent(centerT);
            newTileInfo.forwardFromCenter = tan.normalized;
            newTileInfo.originW = tilesOriginW[i];

            Vector3 difBetweenOrigins = tilesOriginW[i + 1] - tilesOriginW[i];

            List<Vector3> cornersFromOrigin = new();
            int startingCornerIndex = i * 2;
            for (int j = 0; j < 4; j++)
            {
                switch (j)
                {
                    case 0: cornersFromOrigin.Add(tilesCornersFromOrigin[startingCornerIndex]); break;
                    case 1: cornersFromOrigin.Add(tilesCornersFromOrigin[startingCornerIndex +1]); break;
                    case 2: cornersFromOrigin.Add( difBetweenOrigins + tilesCornersFromOrigin[startingCornerIndex + 3]); break;
                    case 3: cornersFromOrigin.Add(difBetweenOrigins + tilesCornersFromOrigin[startingCornerIndex + 2]); break;
                }
            }

            newTileInfo.cornersFromOrigin = cornersFromOrigin;

            tilesInfo.Add(newTileInfo);
        } 
    }
    void CreateTIles()
    {
        for (int i = 0; i < tilesInfo.Count; i++)
        {
            TileInfo info = tilesInfo[i];

            GameObject newTile = Instantiate(tilePrefab, info.originW, Quaternion.identity);
            SplineContainer splineContainer = newTile.GetComponent<SplineContainer>();
            SpriteShapeController shapeController = newTile.GetComponent<SpriteShapeController>();

            for (int j = 0; j < info.cornersFromOrigin.Count; j++)
            {
                BezierKnot knot = splineContainer.Spline[j];
                knot.Position = info.cornersFromOrigin[j];
                splineContainer.Spline[j] = knot;
                shapeController.spline.SetPosition(j, info.cornersFromOrigin[j]);
            }
        }
    }
}
