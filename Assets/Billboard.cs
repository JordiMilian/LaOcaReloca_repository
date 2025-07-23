using UnityEngine;

public class Billboard : MonoBehaviour
{
    Camera main;
    private void Awake()
    {
        main = Camera.main;
    }

    void Update()
    {
        transform.forward = -main.transform.forward;
    }
}
