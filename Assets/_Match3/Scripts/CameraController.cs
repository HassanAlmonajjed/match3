using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float unitsToShowHorizontally = 12;

    void Update()
    {
        float screenWidth = unitsToShowHorizontally;

        float screenHeight = screenWidth * Screen.height / Screen.width;

        float orthographicSize = screenHeight / 2f;

        Camera.main.orthographicSize = orthographicSize;

        Camera.main.aspect = screenWidth / screenHeight;
    }
}
