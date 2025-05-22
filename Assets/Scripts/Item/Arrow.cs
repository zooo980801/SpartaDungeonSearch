using UnityEngine;

[RequireComponent(typeof(ItemObject))]
public class Arrow : MonoBehaviour
{
    public int damage = 10;
    private bool isLaunched = false;

    public void Launch(Vector3 direction, float force)
    {
        isLaunched = true;

        if (TryGetComponent(out Rigidbody rb))
        {
            rb.useGravity = true;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.AddForce(direction * force, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (!isLaunched) return;

        if (collision.collider.GetComponentInParent<IDamagable>() is IDamagable damagable)
        {
            damagable.TakePhysicalDamage(damage);
        }

        // 발사체 기능 종료 → 루팅 전환
        isLaunched = false;

        if (TryGetComponent(out Rigidbody rb)) rb.isKinematic = true;
        if (TryGetComponent(out Collider col)) col.isTrigger = true;
    }
}
