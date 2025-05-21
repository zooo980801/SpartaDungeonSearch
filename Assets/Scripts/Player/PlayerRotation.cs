// PlayerRotation.cs
using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    public Transform cameraTransform;

    void Update()
    {
        Vector3 lookDir = cameraTransform.forward;
        lookDir.y = 0f; // 수평 회전만
        if (lookDir != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 10f);
        }
    }
}
