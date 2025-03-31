using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.Demo.Cockpit;
using UnityEngine;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    // 버전, 유저 ID
    private readonly string version = "1.0";
    private string userId = "KTH";
    void Awake()
    {
        // 마스터 클라이언트 씬 자동 동기화 옵션
        PhotonNetwork.AutomaticallySyncScene = true;

        // 버전 및 접속자 ID 설정
        PhotonNetwork.GameVersion = version;
        PhotonNetwork.NickName = userId;

        // 포톤 서버와의 데이터 초당 전송 횟수
        Debug.Log(PhotonNetwork.SendRate);

        // 포톤 서버 접속
        PhotonNetwork.ConnectUsingSettings();
    }

    // 포톤 서버 접속 후 가장 먼저 호출되는 콜백
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected To Master");

        // 자동으로 로비 접속은 안되기에 Log는 False. JoinLobby();해야 함
        Debug.Log($"PhotonNetwork.InLobby : {PhotonNetwork.InLobby}");
        PhotonNetwork.JoinLobby();
    }

    // 로비 접속 후 호출되는 콜백
    public override void OnJoinedLobby()
    {
        Debug.Log($"PhotonNetwork.InLobby : {PhotonNetwork.InLobby}");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"Join Random Filed {returnCode} : {message}");

        // 룸 속성 정의 및 생성
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 20;
        ro.IsOpen = true;
        ro.IsVisible = true;
        PhotonNetwork.CreateRoom("New Room", ro);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room");
        Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom.Name}");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
        Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");

        foreach(var player in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log($"{player.Value.NickName} , {player.Value.ActorNumber}");
        }
    }
}
