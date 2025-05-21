using System;
using System.Collections;
using UnityEngine;

public interface IDamagable
{
    void TakePhysicalDamage(int damage);
}

public class PlayerCondition : MonoBehaviour, IDamagable
{
    [Header("UI 상태 참조")]
    public UICondition uiCondition;

    [Header("기본 수치")]
    public float noHungerHealthDecay;

    [Header("피해 감소 / 무적")]
    public float damageReductionPercent = 0f;
    private bool isInvincible = false;

    public event Action onTakeDamage;

    // 내부 상태 접근
    private Condition health => uiCondition.health;
    private Condition hunger => uiCondition.hunger;
    private Condition stamina => uiCondition.stamina;

    private void Update()
    {
        var smr = GetComponentInChildren<SkinnedMeshRenderer>();
        Debug.Log("현재 색상: " + smr.material.color);
        if (!isInvincible)
        {
            hunger.Subtract(hunger.passiveValue * Time.deltaTime);

            if (hunger.curValue <= 0f)
            {
                health.Subtract(noHungerHealthDecay * Time.deltaTime);
            }
        }

        stamina.Add(stamina.passiveValue * Time.deltaTime);

        if (health.curValue <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        health.Add(amount);
    }

    public void Eat(float amount)
    {
        hunger.Add(amount);
    }

    public void Die()
    {
        Debug.Log("플레이어가 죽었다.");
        // TODO: 죽음 연출 추가 (애니메이션, UI 등)
    }
    public IEnumerator ApplyRainbowEffect(float duration)
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Color randomColor = UnityEngine.Random.ColorHSV(); // 무지개색 랜덤

            foreach (var r in renderers)
            {
                if (r.material.HasProperty("_Color"))
                    r.material.color = randomColor;
            }

            yield return new WaitForSeconds(0.1f); // 깜빡이는 속도
            elapsed += 0.1f;
        }

        ResetPlayerColor(renderers);
    }

    public IEnumerator ApplyPoisonVisual(float duration)
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Color poisonColor = new Color(0.4f, 1f, 0.4f); // 연두색
            foreach (var r in renderers)
            {
                if (r.material.HasProperty("_Color"))
                    r.material.color = poisonColor;
            }

            yield return new WaitForSeconds(0.2f);

            ResetPlayerColor(renderers);
            yield return new WaitForSeconds(0.2f);

            elapsed += 0.4f;
        }
    }
    private void ResetPlayerColor(Renderer[] renderers)
    {
        foreach (var r in renderers)
        {
            if (r.material.HasProperty("_Color"))
                r.material.color = Color.white; // 기본색으로 복원
        }
    }

    public void TakePhysicalDamage(int damageAmount)
    {
        if (isInvincible) return;

        float reduced = damageAmount * (1f - damageReductionPercent);
        health.Subtract(reduced);
        onTakeDamage?.Invoke();
    }

    public bool UseStamina(float amount)
    {
        if (stamina.curValue - amount < 0f)
        {
            return false;
        }

        stamina.Subtract(amount);
        return true;
    }

    public IEnumerator ApplyPoison(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            health.Subtract(5f * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    public void SetInvincible(float duration)
    {
        StartCoroutine(InvincibleCoroutine(duration));
    }

    private IEnumerator InvincibleCoroutine(float duration)
    {
        isInvincible = true;
        yield return new WaitForSeconds(duration);
        isInvincible = false;
    }
}
