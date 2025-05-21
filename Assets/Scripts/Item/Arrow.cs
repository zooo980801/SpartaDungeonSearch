using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public int damage;
    public float destroyAfter = 5f;

    private void Start()
    {
        Destroy(gameObject, destroyAfter);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent(out PlayerCondition player))
        {
            Vector3 dir = (player.transform.position - transform.position).normalized;
            player.TakePhysicalDamage(damage); // 방향 포함된 오버로드 호출
        }
        else if (collision.collider.TryGetComponent(out IDamagable damagable))
        {
            damagable.TakePhysicalDamage(damage);
        }

        Destroy(gameObject);
    }

}
