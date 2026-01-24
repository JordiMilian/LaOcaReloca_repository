using UnityEngine;

public static class StringTools 
{
    public static string FloatToString(float value, int maxDecimals)
    {
        string result = value.ToString("F" + maxDecimals);

        if (maxDecimals == 0) { return result; }

        //remove innecessary zeros behind
        for (int i = result.Length - 1; i >= 0; i--)
        {
            char c = result[i];
            if (c == ',' || c == '.')
            {
                if (i == result.Length - 1)
                {
                    result = result.Remove(i);
                }
                return result;
            }
            //As long as we keep finding 0, remove them, if not zero, stop
            if (c == '0')
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
    public static string BoldText(string text)
    {
        return $"<b>{text}</b>";
    }
    public static string ColorText(string text ,string colorName)
    {
        return $"<color={colorName}>{text}<color=black>";
    }
    public static string AddDamage(float damage) { return $"<color=blue>+{FloatToString(damage, 1)}DMG<color=black>"; }
    public static string AddMultiplier(float damage) { return $"<color=red>+{FloatToString(damage, 1)}mult<color=black>"; }

    #region TOOLTIP INTRO
    public static string OnCrossed = $"{OnCustomMessaje("ON CROSSED")}";
    public static string OnLanded = "<b>- ON LANDED:</b>";
    public static string OnRolledDice = "<b>- ON ROLLED DICES:</b>";
    public static string OnReachedEnd = "<b>- ON REACHED END TILE:</b>";
    public static string OnReached = "<b>- ON REACHED:</b>";
    public static string OnAddedDamage = "<b>- ON ADDED DAMAGE TO THIS TILE:</b>";
    public static string OnAddedNewTileToBoard = "<b>- ON ADDED A NEW TILE TO BOARD:</b>";
    public static string OnRemovedTileFromBoard = OnCustomMessaje("ON REMOVED A TILE FROM BOARD");
    public static string OnEnterInBoard = "<b>- ON ENTER BOARD:</b>";
    public static string OnEaten = $"{OnCustomMessaje("ON EATEN")}";
    public static string OnRotten = $"{OnCustomMessaje("ON ROTTEN")}";
    public static string OnCustomMessaje(string message) { return $"{BoldText($"- {message}:")}"; }
    public static string OnLandedOnTag(TileTags tag) { return $"<b>- ON LANDED ON AN {tag.ToString().ToUpper()} TILE:</b>"; }
    public static string OnCrossedOnTag(TileTags tag) { return $"<b>- ON CROSSED A {tag.ToString().ToUpper()} TILE:</b>"; }
    #endregion
    public static string ExtraDiceRoll = $"{BoldText(ColorText("+DiceRoll","blue"))}"; //On Landed get an extra dice roll
    public static string Fragile = $"{BoldText(ColorText("Fragile","blue"))}"; //Destroy after crossed
    public static string NoStep = $"{BoldText(ColorText("NoStep","blue"))}"; //Takes 0 steps to cross
    public static string Peaceful = $"{BoldText(ColorText("Peaceful", "blue"))}"; //No damage
    public static string Unmovable = $"{BoldText(ColorText("Unmovable", "blue"))}"; //Unmovable tag
    public static string CustomSkill(string s) { return $"{BoldText(ColorText(s, "blue"))}"; }
    public static string Growth(float amount) { return $"{BoldText(ColorText($"Growth(+{FloatToString(amount,1)})", "blue"))}"; }  //Plants tag
    public static string Rot(int remaining) { return BoldText(ColorText($"Rot({remaining})", "blue")); }
    
}
