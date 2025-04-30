using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenuManager : MonoBehaviourPunCallbacks
{
    private const string GAME_VERSION = "1.0";

    [Header("UI")]
    public TMP_InputField roomCodeInput;
    public TMP_InputField nicknameInput;
    public GameObject connectingText;
    private bool hasPendingJoinRequest = false;

    private bool isHosting;
    private string currentRoomCode;

    public void OnClickHost()
    {
        currentRoomCode = RoomCodeGenerator.GenerateCode();
        StartConnection(hosting: true);
    }

    public void OnClickJoin()
    {
        currentRoomCode = roomCodeInput.text.Trim().ToUpper();
        if(currentRoomCode == "")
        {
            return;
        }
        StartConnection(hosting: false);
    }

    private void StartConnection(bool hosting)
    {
        isHosting = hosting;
        hasPendingJoinRequest = true;
        connectingText.SetActive(true);

        // 닉네임 설정
        string name = nicknameInput.text.Trim();
        PhotonNetwork.NickName = string.IsNullOrEmpty(name) ? "Player_" + Random.Range(1000, 9999) : name;

        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = GAME_VERSION;

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            TryEnterRoom();
        }
    }

    public override void OnConnectedToMaster()
    {
        if (hasPendingJoinRequest)
        {
            TryEnterRoom();
        }
    }

    private void TryEnterRoom()
    {
        if (isHosting)
        {
            RoomOptions options = new RoomOptions
            {
                MaxPlayers = 4,
                IsOpen = true,
                IsVisible = true
            };

            PhotonNetwork.CreateRoom(currentRoomCode, options);
        }
        else
        {
            PhotonNetwork.JoinRoom(currentRoomCode);
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"JoinRoom Failed: {message}");

        // 방이 없으면 사용자에게 재입력 유도 or 알림 필요
        connectingText.SetActive(false);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"CreateRoom Failed: {message}");
        connectingText.SetActive(false);
    }

    public override void OnJoinedRoom()
    {
        SceneManager.LoadScene("TruckLobbyScene");
    }

    public void OnClickQuit()
    {
        Application.Quit();
    }
}
