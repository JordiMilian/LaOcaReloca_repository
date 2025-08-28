using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CamerasManager : MonoBehaviour
{
    Dictionary<string, CinemachineCamera> cameras = new();

    public static CamerasManager instance;
    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        CinemachineCamera[] cams = GetComponentsInChildren<CinemachineCamera>(true);
        foreach (CinemachineCamera cam in cams)
        {
            cameras.Add(cam.gameObject.name, cam);
        }
    }

    public void SetCameraPriority(string camName, int priority)
    {
        if (cameras.ContainsKey(camName))
        {
            cameras[camName].Priority = priority;
        }
        else
        {
            Debug.LogError($"Camera {camName} not found!");
        }
    }
}
