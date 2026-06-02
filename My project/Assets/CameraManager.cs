using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public CinemachineVirtualCamera[] virtualCameras;
    
    public void EnableCamera(int cameraIndex)
    {
        // Set all cameras to low priority
        foreach (var cam in virtualCameras)
        {
            cam.Priority = 0;
        }
        
        // Set the desired camera to high priority
        if (cameraIndex >= 0 && cameraIndex < virtualCameras.Length)
        {
            virtualCameras[cameraIndex].Priority = 10;
        }
    }
    
    // Example usage
    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Alpha1)) EnableCamera(0);
    //     if (Input.GetKeyDown(KeyCode.Alpha2)) EnableCamera(1);
    //     if (Input.GetKeyDown(KeyCode.Alpha3)) EnableCamera(2);
    //     if (Input.GetKeyDown(KeyCode.Alpha4)) EnableCamera(3);
    // }
}