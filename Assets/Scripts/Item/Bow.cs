using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : Equip
{
    public float useStamina;
    public GameObject arrowPrefab;
    public Transform arrowSpawnPoint;
    public float shootForce;
    public float arrowCooldown = 1f;
    private bool canShoot = true;
    public Animator animator;

    public override void OnAttackInput()
    {
        if (canShoot && CharacterManager.Instance.Player.condition.UseStamina(useStamina))
        {
            animator.SetTrigger("Attack");
            StartCoroutine(ShootArrowRoutine());
        }
    }

    IEnumerator ShootArrowRoutine()
    {
        canShoot = false;

        yield return new WaitForSeconds(0.3f); // 애니메이션 동기화 타이밍

        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
        Rigidbody rb = arrow.GetComponent<Rigidbody>();
        rb.velocity = arrowSpawnPoint.forward * shootForce;

        yield return new WaitForSeconds(arrowCooldown);
        canShoot = true;
    }
}
