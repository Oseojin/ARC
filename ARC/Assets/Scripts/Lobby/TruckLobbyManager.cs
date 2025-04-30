using UnityEngine;
using Photon.Pun;
using System.Collections;

public class TruckLobbyManager : MonoBehaviourPunCallbacks
{
    public Transform[] spawnPoints;

    void Start()
    {
        StartCoroutine(WaitAndSpawn());
    }

    IEnumerator WaitAndSpawn()
    {
        while (!PhotonNetwork.InRoom)
            yield return null;

        yield return new WaitForSeconds(0.1f);
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        if (PhotonNetwork.LocalPlayer.TagObject != null) return;

        int spawnIndex = PhotonNetwork.LocalPlayer.ActorNumber % spawnPoints.Length;
        var spawn = spawnPoints[spawnIndex];
        GameObject player = PhotonNetwork.Instantiate("Player", spawn.position, spawn.rotation);

        // TagObject에 등록해 중복 생성 방지
        PhotonNetwork.LocalPlayer.TagObject = player;
    }
}
