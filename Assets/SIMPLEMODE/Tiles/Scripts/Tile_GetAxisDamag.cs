using System.Collections;
using UnityEngine;

public class Tile_GetAxisDamag : Tile_Base
{
    public override IEnumerator OnPlayerLanded()
    {
        yield return base.OnPlayerLanded();
        float totalDamage = 0;
        //tiles under
        for(int i = vectorInBoard.y-1; i >= 0; i--) 
        {
            Tile_Base tile = BoardController.TilesByPosition[new Vector2Int(vectorInBoard.x, i)];
            yield return addTileDamage(tile);
        }
        //tiles to the right
        for (int i = vectorInBoard.x + 1; i < 10; i++)
        {
            if(BoardController.TilesByPosition.ContainsKey(new Vector2Int(i, vectorInBoard.y)))
            {
                Tile_Base tile = BoardController.TilesByPosition[new Vector2Int(i, vectorInBoard.y)];
                yield return addTileDamage(tile);
            }
            else { break; }
            
        }
        //tiles over
        for (int i = vectorInBoard.y+1; i < 10; i++)
        {
            if(BoardController.TilesByPosition.ContainsKey(new Vector2Int(vectorInBoard.x, i)))
            {
                Tile_Base tile = BoardController.TilesByPosition[new Vector2Int(vectorInBoard.x, i)];
                yield return addTileDamage(tile);
            }
            else { break; };
            
        }
        //tiles to the left
        for (int i = vectorInBoard.x - 1; i >= 0; i--)
        {
            Tile_Base tile = BoardController.TilesByPosition[new Vector2Int(i, vectorInBoard.y)];
            yield return addTileDamage(tile);
        }
       
        tileMovement.shakeTile(Intensity.large);
        yield return GameController.C_AddAcumulatedDamage(totalDamage);


        //
        IEnumerator addTileDamage(Tile_Base tile)
        {
            totalDamage += tile.GetCrossedDamageAmount();
            tile.tileMovement.shakeTile(Intensity.mid);
            yield return new WaitForSeconds(0.05f);
        }
    }
    public override string GetTooltipText()
    {
        return "On Landed: Add up the damage of all tiles in the same axis as this tile";
    }


}
