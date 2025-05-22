using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target; // CameraTarget
    public Vector2 sensitivity = new Vector2(3f, 1.5f);
    public float distance = 4f;
    public float height = 2f;
    public float minX = -60f;
    public float maxX = 75f;

    private Vector2 currentRotation;

    void LateUpdate()
    {
        if (CharacterManager.Instance?.Player?.controller?.isFrozen ?? true) return;

        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        currentRotation.x += mouseDelta.x * sensitivity.x;
        currentRotation.y -= mouseDelta.y * sensitivity.y;
        currentRotation.y = Mathf.Clamp(currentRotation.y, minX, maxX);

        Quaternion rotation = Quaternion.Euler(currentRotation.y, currentRotation.x, 0);
        Vector3 cameraPos = target.position - rotation * Vector3.forward * distance + Vector3.up * height;

        transform.position = cameraPos;
        transform.LookAt(target.position + Vector3.up * 1.5f); // 카메라가 플레이어 머리쯤 바라보게
    }
}
