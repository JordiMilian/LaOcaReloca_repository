using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public abstract class Tile_Data
{
    public int ShopPrice;
    protected Board_Data Board;
    public Vector2Int positionInBoard; //Not sure why I'd need this
    public int Index;
    public Tile_Data(int index, Board_Data board)
    {
        Index = index;
        Board = board;
    }
    public virtual IEnumerator OnPlayerStepped_logic()
    {
        yield break;
    }
    public virtual IEnumerator OnPlayerLanded_logic()
    {
        yield break;
    }
    public abstract float GetDamageAmount();
    public abstract string GetTooltipText();
}




public class EmptyTile : Tile_Data
{
    public EmptyTile(int index, Board_Data board) : base(index, board) 
    {
        Index = index;
        Board = board;
    }
    public override IEnumerator OnPlayerStepped_logic()
    {
        yield return GameControlle.Instance.AddAcumulatedDamage(1);
    }
    public override float GetDamageAmount()
    {
        return Index;
    }
}
public class StartingTile : Tile_Data
{
    public StartingTile(int index, Board_Data board) : base(index, board) { }

    public override float GetDamageAmount()
    {
        return 0;
    }
}
public class EndTile : Tile_Data
{
    public EndTile(int index, Board_Data board) : base(index, board) { }

    public override IEnumerator OnPlayerLanded_logic()
    {
        GameControlle.Instance.ChangeGameState(GameState.ReachedEnd);
        yield break;
    }
    public override float GetDamageAmount()
    {
        return 200;
    }
}
public class OcaTile : Tile_Data
{
    public OcaTile(int index, Board_Data board) : base(index, board  ) 
    {
        Index = index;
        Board = board;
    }

    public override IEnumerator OnPlayerStepped_logic()
    {
        yield break;
    }

    public override IEnumerator OnPlayerLanded_logic()
    {
        for (int i = Index +1; i < Board.TilesList.Count; i++)
        {
            if (Board.TilesList[i] is OcaTile)
            {
                Board_Controller gameBoard = GameControlle.Instance.BoardController;
                yield return gameBoard.JumpPlayerTo(i,false);
                yield break;
            }
        }
    }
    public override float GetDamageAmount()
    {
        return 1;
    }
}
