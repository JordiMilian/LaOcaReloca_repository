using UnityEngine;

public class Tile_Monobehaviour : MonoBehaviour
{
}
public abstract class Tile
{
    public int Value;
    public Board Board;
    public Vector3 worldPosition;
    public Vector2Int positionInBoard; //Not sure why I'd need this
    public int Index;
    public Tile(int index, Board board) 
    {
        Index = index;
        Board = board;
    }
    public abstract void OnPlayerStepped();
    public abstract void OnPlayerLanded();
}
public class EmptyTile : Tile
{
    public EmptyTile(int index, Board board) : base(index, board) { }

    public override void OnPlayerStepped()
    {
    }

    public override void OnPlayerLanded()
    {
    }
}
public class StartingTile : Tile
{
    public StartingTile(int index, Board board) : base(index, board) { }

    public override void OnPlayerStepped()
    {
    }

    public override void OnPlayerLanded()
    {
    }
}
public class EndTile : Tile
{
    public EndTile(int index, Board board) : base(index, board) { }

    public override void OnPlayerStepped()
    {
       
    }

    public override void OnPlayerLanded()
    {
        GameController.Instance.ChangeGameState(GameState.ReachedEnd);
    }
}
public class OcaTile : Tile
{
    public OcaTile(int index, Board board) : base(index, board  ) 
    {
        Index = index;
        Board = board;
    }

    public override void OnPlayerStepped()
    {

    }

    public override void OnPlayerLanded()
    {
        Board gameBoard = GameController.Instance.GameBoard;
        for (int i = Index +1; i < gameBoard.TilesList.Count; i++)
        {
            if (gameBoard.TilesList[i] is OcaTile)
            {
                gameBoard.MovePlayerTo(i);
            }
        }
    }
}
