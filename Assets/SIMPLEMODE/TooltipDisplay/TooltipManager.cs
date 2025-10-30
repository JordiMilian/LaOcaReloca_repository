using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TooltipManager : MonoBehaviour
{
    List<ITooltip> tooltipRequests = new();
    ITooltip forcedTooltip;
    [SerializeField] TextMeshProUGUI TMP_Description, TMP_Title;
    [SerializeField] Material imageMaterial;
    [SerializeField] Texture defaultTexture;
    public static TooltipManager Instance;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    public void RequestTooltip(ITooltip tooltipRequested)
    {
        if (!tooltipRequests.Contains(tooltipRequested))
        {
            tooltipRequests.Add(tooltipRequested);
        }
        UpdateTooltipTexts();
    }
    public void RemoveRequest(ITooltip tooltipRequested)
    {
        if (tooltipRequests.Contains(tooltipRequested))
        {
            tooltipRequests.Remove(tooltipRequested);
        }
        UpdateTooltipTexts();
    }
    public void ForceTooltip(ITooltip tooltipToForce)
    {
        if(forcedTooltip == null)
        {
            forcedTooltip = tooltipToForce;
            UpdateTooltipTexts();
        }
        
    }
    public void StopForcingThisTooltip(ITooltip tooltipToStop)
    {
        if(forcedTooltip == tooltipToStop)
        {
            forcedTooltip = null;
            UpdateTooltipTexts();
        }
    }
    void UpdateTooltipTexts()
    {
        if(forcedTooltip != null)
        {
            TMP_Description.text = forcedTooltip.GetTooltipDescription();
            TMP_Title.text = forcedTooltip.GetTooltipTitle();
            imageMaterial.SetTexture("_mainTexture", forcedTooltip.GetTooltipTexture());
        }
        else if(tooltipRequests.Count > 0)
        {
            ITooltip tooltipToDisplay = tooltipRequests[tooltipRequests.Count - 1];//Display the last requested tooltip
            if(tooltipToDisplay == null)
            {
                TMP_Description.text = "";
                TMP_Title.text = "";
                imageMaterial.SetTexture("_mainTexture", defaultTexture);
                return;
            }

            TMP_Description.text = tooltipToDisplay.GetTooltipDescription();
            TMP_Title.text = tooltipToDisplay.GetTooltipTitle();
            imageMaterial.SetTexture("_mainTexture", tooltipToDisplay.GetTooltipTexture());
        }
        else
        {
            TMP_Description.text = "";
            TMP_Title.text = "";
            imageMaterial.SetTexture("_mainTexture", defaultTexture);
        }
    }
}
