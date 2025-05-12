using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public abstract class Tile_Data
{
    public int Value;
    public Board_Data Board;
    public Vector2Int positionInBoard; //Not sure why I'd need this
    public int Index;
    public Tile_Data(int index, Board_Data board) 
    {
        Index = index;
        Board = board;
    }
    public abstract IEnumerator OnPlayerStepped();
    public abstract IEnumerator OnPlayerLanded();
    public abstract float GetDamageAmount();
}
public class EmptyTile : Tile_Data
{
    public EmptyTile(int index, Board_Data board) : base(index, board) { }

    public override void OnPlayerStepped()
    {
    }

    public override void OnPlayerLanded()
    {
    }
    public override float GetDamageAmount()
    {
        return Index;
    }
}
public class StartingTile : Tile_Data
{
    public StartingTile(int index, Board_Data board) : base(index, board) { }

    public override void OnPlayerStepped()
    {
    }

    public override void OnPlayerLanded()
    {
    }
    public override float GetDamageAmount()
    {
        return 0;
    }
}
public class EndTile : Tile_Data
{
    public EndTile(int index, Board_Data board) : base(index, board) { }

    public override void OnPlayerStepped()
    {
       
    }

    public override void OnPlayerLanded()
    {
        GameController_C.Instance.ChangeGameState(GameState.ReachedEnd);
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

    public override void OnPlayerStepped()
    {

    }

    public override IEnumerator OnPlayerLanded()
    {
        Board_Controller gameBoard = GameController_C.Instance.BoardController;
        for (int i = Index +1; i < gameBoard.Data.TilesList.Count; i++)
        {
            if (gameBoard.Data.TilesList[i] is OcaTile)
            {
                yield return gameBoard.JumpPlayerTo(i);
                yield break;
            }
        }
    }
    public override float GetDamageAmount()
    {
        return Index;
    }
}
