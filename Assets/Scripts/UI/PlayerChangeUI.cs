using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerChangeUI : MonoBehaviour
{
    public Slider sizeSlider;
    public Slider speedSlider;
    public Toggle invincibleToggle;
    public Button closeButton;


    private Player player;

    public void Init(Player player)
    {
        this.player = player;

        sizeSlider.minValue = 0.5f;
        sizeSlider.maxValue = 3.0f;
        sizeSlider.value = player.transform.localScale.x;

        // ✅ 스피드 슬라이더 범위와 값 설정
        speedSlider.minValue = 1f;
        speedSlider.maxValue = 20f;
        speedSlider.value = player.controller.moveSpeed;

        invincibleToggle.isOn = false;


        // ✅ 캐릭터 정지
        Time.timeScale = 0f;
        player.controller.ToggleCursor(true);
        player.controller.isFrozen = true;

        // 리스너 연결
        sizeSlider.onValueChanged.AddListener(ChangeSize);
        speedSlider.onValueChanged.AddListener(ChangeSpeed);
        invincibleToggle.onValueChanged.AddListener(ChangeInvincibility);
        closeButton.onClick.AddListener(CloseUI);
    }

    void ChangeSize(float scale)
    {
        player.transform.localScale = Vector3.one * scale;
    }

    void ChangeSpeed(float speed)
    {
        player.controller.moveSpeed = speed;
    }

    void ChangeInvincibility(bool value)
    {
        if (value)
        {
            player.condition.SetInvincible(9999f); // 사실상 무제한
        }
    }

    void CloseUI()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
        player.controller.ToggleCursor(false);
        player.controller.isFrozen = false;
    }
}