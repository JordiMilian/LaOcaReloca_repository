
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;


public struct TileTfData
{
    public Vector3 origin; //this is the point in the spline that touches with the previus tile
    public Vector3 center;
    //These lists are sorted like this because that's how the mesh handles it
    //3----1
    //|    |
    //2----0
    public List<Vector3> cornersInWorld;
    public List<Vector3> cornersInLocal;
    public Vector3 forward, up, right;
    public Quaternion rotation;
}

public class SplineBoard_Testing : MonoBehaviour
{
    [SerializeField] SplineContainer spline;

    [SerializeField] int tilesCount = 10;
    [SerializeField] float width = 1f;
    [SerializeField] float maxTileLenght = 3;

    List<TileTfData> tilesData = new();
    [SerializeField] GameObject tilePrefab;

    public List<TileController> TilesList = new();
    [Header("Extra Large tiles")]
    [Range(1,5)]
    [SerializeField] float ExtraLargePercent = 1.5f;

    private void OnDrawGizmosSelected()
    {
        UpdateStructs();

        foreach (TileTfData info in tilesData)
        {
            Gizmos.color = Color.purple;

            for (int i = 0; i < info.cornersInWorld.Count; i++)
            {
                switch (i)
                {
                    case 0: Gizmos.DrawLine( info.cornersInWorld[0],  info.cornersInWorld[1]); break;
                    case 1: Gizmos.DrawLine( info.cornersInWorld[1],  info.cornersInWorld[3]); break;
                    case 2: Gizmos.DrawLine( info.cornersInWorld[2],  info.cornersInWorld[0]); break;
                    case 3: Gizmos.DrawLine( info.cornersInWorld[3],  info.cornersInWorld[2]); break;
                }
            }
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(info.center, info.center + info.forward);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(info.center, info.center + info.up);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(info.center, info.center + info.right);
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
        List<Vector3> tilesOrigins = new();
        tilesData = new();

        float smallT;
        float largeT;

        //if the total max lenght is larger than the whole spline, then divide. Else just use the lenght. 
        //We multiply by 2 because it's only the Start Tile and End Tile
        if ((maxTileLenght* ExtraLargePercent * 2) + maxTileLenght * (tilesCount -2) > spline.CalculateLength() )
        {
            smallT = 1f / (ExtraLargePercent * 2 + (tilesCount - 2));
            largeT = smallT * ExtraLargePercent;
        }
        else
        {
            smallT = GetTWithLenght(maxTileLenght);
            largeT = GetTWithLenght(maxTileLenght * ExtraLargePercent);
        }

        //Get corners from all Origins
        float totalT = 0;
        for (int i = 0; i < tilesCount + 1; i++)
        {
            float thisT = totalT;

            Vector3 tan = spline.EvaluateTangent(thisT);
            Vector3 forward = tan.normalized;
            Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;

            tilesCornersFromOrigin.Add(right * width / 2);
            tilesCornersFromOrigin.Add(-right * width / 2);
            tilesOrigins.Add(spline.EvaluatePosition(thisT));

            float nextT;
            if (i == 0 || i == tilesCount-1) { nextT = largeT; }
            else { nextT = smallT; }
            totalT += nextT;

        }
        totalT = 0;
        //Get the basic info In and add the corners 
        for (int i = 0; i < tilesCount; i++)
        {
            TileTfData newTileInfo = new();

            float thisT;
            if (i == 0 || i == tilesCount-1) { thisT = largeT; }
            else { thisT = smallT; }

            float centerT = totalT + (thisT / 2);

            newTileInfo.center = spline.EvaluatePosition(centerT);

            Vector3 tan = spline.EvaluateTangent(centerT);
            newTileInfo.forward = tan.normalized;
            newTileInfo.up = Vector3.up;
            newTileInfo.right = Vector3.Cross(newTileInfo.up, newTileInfo.forward);

            newTileInfo.origin = tilesOrigins[i];
            newTileInfo.rotation = Quaternion.LookRotation(newTileInfo.forward, Vector3.up);

            //Get the corners from Origin
            Vector3 difBetweenOrigins = tilesOrigins[i + 1] - tilesOrigins[i];
            int startingCornerIndex = i * 2;

            newTileInfo.cornersInWorld = new();
            newTileInfo.cornersInLocal = new();
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
                //So here we are sorting them in the proper order in the struct
                Vector3 sortedWorldPos;
                switch (j)
                {
                    case 0: sortedWorldPos = newTileInfo.origin + difBetweenOrigins + tilesCornersFromOrigin[startingCornerIndex + 2]; break;
                    case 1: sortedWorldPos = newTileInfo.origin + difBetweenOrigins + tilesCornersFromOrigin[startingCornerIndex + 3]; break;
                    case 2: sortedWorldPos = newTileInfo.origin + tilesCornersFromOrigin[startingCornerIndex]; break;
                    case 3: sortedWorldPos = newTileInfo.origin + tilesCornersFromOrigin[startingCornerIndex + 1]; break;
                    default: sortedWorldPos = Vector3.zero; break;
                }
                
                //Now we collect the World and Local positions out of that mess
                newTileInfo.cornersInWorld.Add(sortedWorldPos);

                Vector3 localPos = MathJ.worldToLocal2D(
                    newTileInfo.cornersInWorld[j],
                    newTileInfo.center,
                    newTileInfo.right,
                    newTileInfo.forward);

                newTileInfo.cornersInLocal.Add(localPos);
            }
            totalT += thisT;
            tilesData.Add(newTileInfo);
        }
        //
        float GetTWithLenght(float lenght)
        {
            float totalLenght = spline.CalculateLength();
            return Mathf.InverseLerp(0, totalLenght, lenght);
        }
    }
    
    void CreateStartingTiles()
    {
        for(int i = 0; i < tilesData.Count; i++)
        {
            GameObject newTile = Instantiate(tilePrefab);
            TilesList.Add(newTile.GetComponent<TileController>());
        }
    }
    void PlaceTilesToTfData()
    {
        for (int i = 0; i < TilesList.Count; i++)
        {
            TilesList[i].SetOriginTfData(tilesData[i]);
            TilesList[i].MoveToTfData();
        }
    }

    [Header("Testing")]
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
        TilesList.Insert(Test_IndexToAdd, newTile.GetComponent<TileController>());
        PlaceTilesToTfData();
    }
}
