using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerController controller;
    public PlayerCondition condition;
    public Equipment equip;

    public ItemData itemData;
    public Action addItem;

    public Transform dropPosition;


    private void Awake()
    {
        CharacterManager.Instance.Player = this;
        controller = GetComponent<PlayerController>();
        condition = GetComponent<PlayerCondition>();
        equip = GetComponent<Equipment>();
    }
    public void UseConsumable(ItemData data)
    {
        switch (data.effectType)
        {
            case ItemEffectType.Poison:
                StartCoroutine(condition.ApplyPoison(data.effectValue));
                StartCoroutine(condition.ApplyPoisonVisual(data.effectValue));
                break;

            case ItemEffectType.RandomEffect:
                int rand = UnityEngine.Random.Range(0, 3);
                switch (rand)
                {
                    case 0:
                        controller.moveSpeed *= 2f;
                        break;
                    case 1:
                        condition.SetInvincible(5f); // 체력/배고픔 감소 무시
                        break;
                    case 2:
                        condition.Die(); // 즉사
                        break;
                }
                StartCoroutine(condition.ApplyRainbowEffect(3f));
                break;

            case ItemEffectType.SpeedUp:
                controller.moveSpeed *= data.effectValue;
                break;

            case ItemEffectType.Enlarge:
                transform.localScale *= data.effectValue;
                break;

            case ItemEffectType.FullHeal:
                condition.Heal(condition.uiCondition.health.maxValue);
                break;
        }
    }

}