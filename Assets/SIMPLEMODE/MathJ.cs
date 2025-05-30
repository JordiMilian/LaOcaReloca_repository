using UnityEngine;

public static class MathJ 
{
    public static Vector2Int rotateVectorClockwise90Degrees(Vector2Int VectorToRotate)
    {
        return new Vector2Int(VectorToRotate.y, -VectorToRotate.x);
    }
    public static Vector2Int rotateVectorUnclockwise90Degrees(Vector2Int VectorToRotate)
    {
        return new Vector2Int(-VectorToRotate.y, VectorToRotate.x);
    }

    public static int SignZero(int amount) //The same as Mathf.Sign but returns a zero if given a 0 and it works with INTs
    {
        if (amount == 0) { return 0; }
        return (int)Mathf.Sign(amount);
    }
}
