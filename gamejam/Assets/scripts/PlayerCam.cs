using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sensX = 200f;
    public float sensY = 200f;

    public Transform orientation;

    float xRotation;
    float yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update() // must be uppercase
    {
        // Correct axis names
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Rotate the camera (pitch + yaw)
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);

        // Rotate the orientation object (yaw only, for player movement)
        if (orientation != null)
        {
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        }
    }
}
