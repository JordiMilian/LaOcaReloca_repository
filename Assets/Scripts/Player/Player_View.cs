using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Player_View : MonoBehaviour
{
    public IEnumerator MovePlayerTo(Vector3 newPos)
    {
        transform.position = newPos;
        yield return null;
        
    }
    
}
