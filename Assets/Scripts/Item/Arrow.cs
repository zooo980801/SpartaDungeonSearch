using UnityEngine;

[RequireComponent(typeof(ItemObject))]
public class Arrow : MonoBehaviour
{
    public int damage = 10;
    private bool isLaunched = false;

    public void Launch(Vector3 direction, float force)
    {
        isLaunched = true;
        Debug.Log("[Arrow] Launch 호출됨");

        if (TryGetComponent(out Rigidbody rb))
        {
            rb.useGravity = true;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.AddForce(direction * force, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"[Arrow] 충돌 감지: {collision.collider.name}");

        if (!isLaunched) return;

        if (collision.collider.GetComponentInParent<IDamagable>() is IDamagable damagable)
        {
            Debug.Log("[Arrow] IDamagable 감지됨 → 데미지 적용");
            damagable.TakePhysicalDamage(damage);
        }
        else
        {
            Debug.Log("[Arrow] IDamagable 못 찾음");
        }

        // 발사체 기능 종료 → 루팅 전환
        isLaunched = false;

        if (TryGetComponent(out Rigidbody rb)) rb.isKinematic = true;
        if (TryGetComponent(out Collider col)) col.isTrigger = true;
    }
}
