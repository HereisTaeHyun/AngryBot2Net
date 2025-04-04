using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class RoomData : MonoBehaviour
{
    private RoomInfo _roomInfo;
    private TMP_Text roomInfoText;
    private PhotonManager photonManager;

    public RoomInfo RoomInfo
    {
        get
        {
            return _roomInfo;
        }
        set
        {
            _roomInfo = value;
            roomInfoText.text = $"{_roomInfo.Name} ({_roomInfo.PlayerCount} / {_roomInfo.MaxPlayers})";
            GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnEnterRoom(_roomInfo.Name));
        }
    }

    void Awake()
    {
        roomInfoText = GetComponentInChildren<TMP_Text>();
        photonManager = GameObject.Find("PhotonManager").GetComponent<PhotonManager>();
    }

    private void OnEnterRoom(string roomName)
    {
        photonManager.SetUserId();

        // 룸 속성 정의
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 20;
        ro.IsOpen = true;
        ro.IsVisible = true;

        PhotonNetwork.JoinOrCreateRoom(roomName, ro, TypedLobby.Default);
    }
}
