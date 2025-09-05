using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Splines;
using UnityEngine.U2D;
using static UnityEngine.UI.Image;

public struct TileTfData
{
    public Vector3 originW;
    public Vector3 centerW;
    public List<Vector3> cornersFromOrigin;
    public Vector3 forwardFromCenter;
}

public class SplineBoard_Testing : MonoBehaviour
{
    [SerializeField] SplineContainer spline;

    [SerializeField] int tilesCount = 10;
    [SerializeField] float width = 1f;
    [SerializeField] float maxTileLenght = 3;

    List<TileTfData> tilesData = new();
    [SerializeField] GameObject tilePrefab;

    

    private void OnDrawGizmosSelected()
    {
        UpdateStructs();

        foreach(TileTfData info in tilesData)
        {
            Gizmos.color = Color.purple;

            for(int i = 0; i < info.cornersFromOrigin.Count; i++)
            {
                switch(i)
                {
                    case 0: Gizmos.DrawLine(info.originW + info.cornersFromOrigin[0], info.originW + info.cornersFromOrigin[1]); break;
                    case 1: Gizmos.DrawLine(info.originW + info.cornersFromOrigin[1], info.originW + info.cornersFromOrigin[3]); break;
                    case 2: Gizmos.DrawLine(info.originW + info.cornersFromOrigin[2], info.originW + info.cornersFromOrigin[0]); break;
                    case 3: Gizmos.DrawLine(info.originW + info.cornersFromOrigin[3], info.originW + info.cornersFromOrigin[2]); break;
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
        UpdateStructs();
        CreateStartingTiles();
        PlaceTilesToTfData();
    }
    public void UpdateStructs()
    {
        List<Vector3> tilesCornersFromOrigin = new();
        List<Vector3> tilesOriginW = new();
        tilesData = new();
        

        float GetTWithLenght( float lenght)
        {
            float totalLenght = spline.CalculateLength();
            return Mathf.InverseLerp(0, totalLenght, lenght);
        }
        float maxTPerTIle = GetTWithLenght(maxTileLenght);
        float TPerTile = 1f / (float)tilesCount;
        if (maxTileLenght * tilesCount > spline.CalculateLength())
        {
            //dividir normal
            TPerTile = 1f / (float)tilesCount;
        }
        else
        {
            TPerTile = maxTPerTIle;
        }

        //Get corners from all Origins
        for(int i = 0; i < tilesCount +1; i++)
        {
            float t = TPerTile * i;

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
            float centerT = TPerTile * i + (TPerTile/2);
            TileTfData newTileInfo = new();

            newTileInfo.centerW = spline.EvaluatePosition(centerT);
            Vector3 tan = spline.EvaluateTangent(centerT);
            newTileInfo.forwardFromCenter = tan.normalized;
            newTileInfo.originW = tilesOriginW[i];

            Vector3 difBetweenOrigins = tilesOriginW[i + 1] - tilesOriginW[i];

            List<Vector3> VerticesPositionsFromOrigin = new();
            int startingCornerIndex = i * 2;
            for (int j = 0; j < 4; j++)
            {
                //The order of the mesh vertices is:
                //3-----1
                //|     |
                //2-----0
                //But when we created the corners they were in this order:
                //1-----3
                //|     |
                //0-----2
                //So here we are storing them in the proper order in the struct
                switch (j)
                {       
                    case 0: VerticesPositionsFromOrigin.Add(difBetweenOrigins + tilesCornersFromOrigin[startingCornerIndex + 2]); break;
                    case 1: VerticesPositionsFromOrigin.Add(difBetweenOrigins + tilesCornersFromOrigin[startingCornerIndex + 3]); break;
                    case 2: VerticesPositionsFromOrigin.Add(tilesCornersFromOrigin[startingCornerIndex]); break;
                    case 3: VerticesPositionsFromOrigin.Add(tilesCornersFromOrigin[startingCornerIndex+1]); break;
                }
            }
            newTileInfo.cornersFromOrigin = VerticesPositionsFromOrigin;

            tilesData.Add(newTileInfo);
        } 
    }
    List<SplineTile> TilesList = new();
    void CreateStartingTiles()
    {
        for(int i = 0; i < tilesData.Count; i++)
        {
            GameObject newTile = Instantiate(tilePrefab);
            TilesList.Add(newTile.GetComponent<SplineTile>());
        }
    }
    void PlaceTilesToTfData()
    {
        for (int i = 0; i < TilesList.Count; i++)
        {
            StartCoroutine( TilesList[i].C_MoveMeshToStruct(tilesData[i]));
        }
    }

    [SerializeField] int Test_IndexToAdd;
    [ContextMenu("Add New Tile")]
    void AddNewTile()
    {

        if(Test_IndexToAdd >= TilesList.Count)
        {
            Test_IndexToAdd = TilesList.Count-1;
        }
        tilesCount++;
        UpdateStructs();
        GameObject newTile = Instantiate(tilePrefab, transform.position, Quaternion.identity);
        TilesList.Insert(Test_IndexToAdd, newTile.GetComponent<SplineTile>());
        PlaceTilesToTfData();
    }
}
