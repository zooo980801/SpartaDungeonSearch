using System.Collections;
using UnityEngine;

public class Bow : Equip
{
    [Header("Bow Settings")]
    public float useStamina = 10f;
    public GameObject arrowPrefab;
    public Transform arrowSpawnPoint; // 화살 생성 위치
    public float shootForce = 20f;
    public float arrowCooldown = 1f;

    [SerializeField] private ItemData arrowItemData;
    [SerializeField] private UIInventory inventory;

    private bool canShoot = true;

    public Transform rayOrigin;
    private void Awake()
    {
        if (inventory == null)
        {
            inventory = FindObjectOfType<UIInventory>();
            if (inventory == null)
                Debug.LogError("[Bow] UIInventory를 찾을 수 없습니다.");
        }
    }

    public override void OnAttackInput()
    {
        if (!canShoot) return;

        if (CharacterManager.Instance.Player.condition.UseStamina(useStamina))
        {
            StartCoroutine(ShootArrowRoutine());
        }
    }

    private IEnumerator ShootArrowRoutine()
    {
        canShoot = false;

        yield return new WaitForSeconds(0.3f);

        if (inventory == null || !inventory.HasItem(arrowItemData, 1))
        {
            Debug.Log("화살 없음! 발사 불가");
            canShoot = true;
            yield break;
        }

        inventory.UseItem(arrowItemData, 1);

        Vector3 shootDirection = rayOrigin.forward.normalized;
        Vector3 offsetPosition = arrowSpawnPoint.position + shootDirection * 1.0f;

        GameObject arrow = Instantiate(arrowPrefab, offsetPosition, Quaternion.LookRotation(shootDirection));
        Debug.Log($"[Bow] 화살 생성됨 at {offsetPosition}");

        if (arrow.TryGetComponent(out Arrow arrowScript))
        {
            arrowScript.Launch(shootDirection, shootForce);
        }

        yield return new WaitForSeconds(arrowCooldown);
        canShoot = true;
    }



}
