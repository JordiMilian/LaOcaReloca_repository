using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player_Controller : MonoBehaviour
{
    Player_View view;
    Player_Data data;
    public IEnumerator MovePlayerTo(Tile_Controller tile)
    {
        yield return StartCoroutine(view.MovePlayerTo(tile.view.positionForPlayer.position));
    }
   
}
