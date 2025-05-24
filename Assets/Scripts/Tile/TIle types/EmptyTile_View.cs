using System;
using System.Collections;
using UnityEngine;

public class EmptyTile_View : Tile_View
{
    public override IEnumerator OnPlayerLanded()
    {
        Debug.Log($"player landed in {GetComponent<Tile_Controller>().data.Index} feedback to play");
        yield break;
    }

    public override IEnumerator OnPlayerStepped()
    {
        Debug.Log($"player stepped in {GetComponent<Tile_Controller>().data.Index} feedback to play");
        yield break;
    }
}
