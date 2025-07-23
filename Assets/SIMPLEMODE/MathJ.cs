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

    public static string FloatToString(float value, int maxDecimals)
    {
        string result = value.ToString("F" + maxDecimals);

        if(maxDecimals == 0) { return result; }

        //remove innecessary zeros behind
        for (int i = result.Length - 1; i >= 0; i--)
        {
            char c = result[i];
            if (c == ',' || c== '.') 
            {
                if (i == result.Length - 1)
                {
                   result = result.Remove(i);
                }
                return result;
            }
            //As long as we keep finding 0, remove them, if not zero, stop
            if(c == '0')
            {
                result = result.Remove(i);
            }
            else
            {
                return result;
            }
        }
        return result;
    }
}
