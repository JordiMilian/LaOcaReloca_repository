using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile_Controller : MonoBehaviour
{
    public Tile_Data data { get; private set; }
    public Tile_View view { get; private set; }
    public IEnumerator OnPlayerStepped()
    {
        yield return view.OnPlayerStepped();
        yield return data.OnPlayerStepped();
        
    }
    public IEnumerator OnPlayerLanded()
    {
        yield return view.OnPlayerLanded();
        yield return data.OnPlayerLanded();
        
    }
}
