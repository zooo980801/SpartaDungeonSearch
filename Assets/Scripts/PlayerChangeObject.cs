using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChangeObject : MonoBehaviour, IInteractable
{
    public GameObject controlUI;

    public string GetInteractPrompt()
    {
        return "E 키를 눌러 플레이어 설정 열기";
    }

    public void OnInteract()
    {
        Time.timeScale = 0f; // 시간 멈춤
        controlUI.SetActive(true);

        var player = CharacterManager.Instance.Player;
        player.controller.ToggleCursor(true); // 마우스 커서 보이게

        // UI에 플레이어 정보 전달
        controlUI.GetComponent<PlayerChangeUI>().Init(player);
    }
}