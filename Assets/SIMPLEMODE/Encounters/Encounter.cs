using System.Collections;
using UnityEngine;

public abstract class Encounter : MonoBehaviour, ITooltip
{
    public abstract string GetTooltipDescription();

    [SerializeField] Texture tooltipTexture;
    [SerializeField] string tooltipTitle;

    public Texture GetTooltipTexture() { return tooltipTexture; }
    public string GetTooltipTitle() { return tooltipTitle; }

    public virtual IEnumerator OnEncounterEnter()
    {
        TooltipManager.Instance.RequestTooltip(this);
        yield break;
    }
    public virtual IEnumerator OnEncounterExit()
    {
        TooltipManager.Instance.RemoveRequest(this);
        yield break;
    }
    public abstract bool MeetsRequirementsToSpawn();

}
