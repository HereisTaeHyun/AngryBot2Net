using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.Demo.Cockpit;
using UnityEngine;
using TMPro;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    // 버전, 유저 ID
    private readonly string version = "1.0";
    private string userId = "KTH";

    public TMP_InputField userIF;
    public TMP_InputField roomNameIF;

    private Dictionary<string, GameObject> rooms = new Dictionary<string, GameObject>();
    private GameObject roomItemPrefab;
    public Transform scrollContent;

    void Awake()
    {
        // 마스터 클라이언트 씬 자동 동기화 옵션
        PhotonNetwork.AutomaticallySyncScene = true;

        // 버전 및 접속자 ID 설정
        PhotonNetwork.GameVersion = version;
        PhotonNetwork.NickName = userId;

        // 포톤 서버와의 데이터 초당 전송 횟수
        Debug.Log(PhotonNetwork.SendRate);
        
        roomItemPrefab = Resources.Load<GameObject>("RoomItem");

        // 포톤 서버 접속
        if(PhotonNetwork.IsConnected == false)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    void Start()
    {
        userId = PlayerPrefs.GetString("USER_ID", $"USER_{Random.Range(1, 21):0}");
        userIF.text = userId;
        PhotonNetwork.NickName = userId;
    }

    public void SetUserId()
    {
        if(string.IsNullOrEmpty(userIF.text))
        {
            userId = $"USER_{Random.Range(1, 21):0}";
        }
        else
        {
            userId = userIF.text;
        }
        PlayerPrefs.SetString("USER_ID", userId);
        PhotonNetwork.NickName = userId;
    }

    private string SetRoomName()
    {
        if(string.IsNullOrEmpty(roomNameIF.text))
        {
            roomNameIF.text = $"ROOM_{Random.Range(1, 101):000}";
        }
        return roomNameIF.text;
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
        // PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"Join Random Filed {returnCode} : {message}");
        OnMakeRoomClick();

        // 룸 속성 정의 및 생성
        // RoomOptions ro = new RoomOptions();
        // ro.MaxPlayers = 20;
        // ro.IsOpen = true;
        // ro.IsVisible = true;
        // PhotonNetwork.CreateRoom("New Room", ro);
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

        // PhotonNetword 동기화 상태로 Player 생성
        // Transform[] spawnPoints = GameObject.Find("SpawnPoints").GetComponentsInChildren<Transform>();
        // int idx = Random.Range(1, spawnPoints.Length);
        // PhotonNetwork.Instantiate("Player", spawnPoints[idx].position, spawnPoints[idx].rotation, 0);

        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("BattleField");
        }
    }
#region UI Button Event
    public void OnLoginClick()
    {
        SetUserId();
        PhotonNetwork.JoinRandomRoom();
    }
    public void OnMakeRoomClick()
    {
        SetUserId();

        // 룸 속성 정의 및 생성
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 20;
        ro.IsOpen = true;
        ro.IsVisible = true;
        PhotonNetwork.CreateRoom(SetRoomName(), ro);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // 삭제룸 프리펩 저장 변수
        GameObject tempRoom = null;
        foreach(var roomInfo in roomList)
        {
            // 룸이 삭제된 경우
            if(roomInfo.RemovedFromList == true)
            {
                // 룸 이름 검색하여 저장된 프리펩 추출   
                rooms.TryGetValue(roomInfo.Name, out tempRoom);
                Destroy(tempRoom);
                rooms.Remove(roomInfo.Name);
            }
            else
            {
                // 룸이 딕셔너리에 없으면 추가
                if(rooms.ContainsKey(roomInfo.Name) == false)
                {
                    // roomInfo를 scroll 하위에 생성
                    GameObject roomPrefab = Instantiate(roomItemPrefab, scrollContent);
                    roomPrefab.GetComponent<RoomData>().RoomInfo = roomInfo;
                    rooms.Add(roomInfo.Name, roomPrefab);
                }
                else
                {
                    rooms.TryGetValue(roomInfo.Name, out tempRoom);
                    tempRoom.GetComponent<RoomData>().RoomInfo = roomInfo;
                }
            }
        }
    }
    #endregion
}
