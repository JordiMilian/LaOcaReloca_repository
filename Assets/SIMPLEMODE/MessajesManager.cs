using UnityEngine;

public class MessajesManager : MonoBehaviour
{
    [SerializeField] float durationMultiplier;
    [SerializeField] float minMultiplier = .5f;
    public static MessajesManager instance;
    private void Start()
    {
        if(instance == null) { instance = this; }
        else { Destroy(this); }

        GameController_Simple.Instance.OnRolledDice.AddListener(ResetMultiplier);    
    }
    private void OnDisable()
    {
        GameController_Simple.Instance.OnRolledDice.RemoveListener(ResetMultiplier);
    }
    //Aixo no esta be, 
    public float GetDurationMultiplier()
    {
        durationMultiplier /= 1.2f;
        if(durationMultiplier < minMultiplier) { return minMultiplier; }
        return durationMultiplier * 1.2f;
    }
    void ResetMultiplier() { durationMultiplier = 1.0f; }
}
