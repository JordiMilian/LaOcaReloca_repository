using UnityEngine;

public class FPS_limiter : MonoBehaviour
{
    [SerializeField] int FPSLimit = 60;
    private void Awake()
    {
        Application.targetFrameRate = FPSLimit;
    }
}
