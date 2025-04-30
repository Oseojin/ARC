using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.Collections;

public class TruckLobbyUI : MonoBehaviourPunCallbacks
{
    [Header("UI")]
    public GameObject menuPanel;
    public GameObject copyButton;

    private bool isMenuOpen = false;
    private PlayerController playerController;

    void Start()
    {
        menuPanel.SetActive(false);
        SetCursor(false);
        StartCoroutine(WaitForLocalPlayer());
    }

    IEnumerator WaitForLocalPlayer()
    {
        GameObject localPlayer = null;

        while (localPlayer == null)
        {
            localPlayer = FindLocalPlayer();
            yield return null;
        }

        playerController = localPlayer.GetComponent<PlayerController>();

        // 호스트만 복사 버튼 활성화
        copyButton.SetActive(PhotonNetwork.IsMasterClient);
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;
        menuPanel.SetActive(isMenuOpen);

        // 플레이어 이동 제어
        if (playerController != null)
        {
            playerController.enabled = !isMenuOpen;
        }

        // 마우스 커서 상태 제어
        SetCursor(isMenuOpen);
    }

    void SetCursor(bool visible)
    {
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = visible;
    }

    GameObject FindLocalPlayer()
    {
        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            var view = player.GetComponent<PhotonView>();
            if (view != null && view.IsMine)
                return player;
        }
        return null;
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void OnClickExit()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LocalPlayer.TagObject = null;
    }

    public void OnClickCopy()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GUIUtility.systemCopyBuffer = PhotonNetwork.CurrentRoom.Name;
        }
    }
}
