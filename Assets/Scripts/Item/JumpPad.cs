using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public float jumpForce = 15f;

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = collision.collider.GetComponent<Rigidbody>();

        if (rb != null && collision.collider.CompareTag("Player"))
        {
            // 기존 Y 속도 초기화 후 위로 강한 힘
            Vector3 newVelocity = rb.velocity;
            newVelocity.y = 0f;
            rb.velocity = newVelocity;

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}