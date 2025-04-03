using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviour
{
    void Awake()
    {
        CreatePlayer();
    }

    private void CreatePlayer()
    {
        // PhotonNetword 동기화 상태로 Player 생성
        Transform[] spawnPoints = GameObject.Find("SpawnPoints").GetComponentsInChildren<Transform>();
        int idx = Random.Range(1, spawnPoints.Length);
        PhotonNetwork.Instantiate("Player", spawnPoints[idx].position, spawnPoints[idx].rotation, 0);
    }
}
