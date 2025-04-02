using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Movement : MonoBehaviourPunCallbacks, IPunObservable
{
    private CharacterController characterController;
    private new Transform transform;
    private Animator playerAnim;
    private new Camera camera;

    private Plane plane;
    private Ray ray;
    private Vector3 hitPoint;

    // 포톤뷰 컴포넌트 캐시 처리
    private PhotonView pv;

    private CinemachineVirtualCamera cinemachineVirtualCamera;

    public float moveSpeed = 10.0f;

    private Vector3 receivePos;
    private Quaternion receiveRot;
    public float damping = 10.0f;
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        transform = GetComponent<Transform>();
        playerAnim = GetComponent<Animator>();
        camera = Camera.main;

        pv = GetComponent<PhotonView>();
        cinemachineVirtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        if(pv.IsMine)
        {
            cinemachineVirtualCamera.Follow = transform;
            cinemachineVirtualCamera.LookAt = transform;
        }

        // Player 아래 가상 바닥 생성
        plane = new Plane(transform.up, transform.position);
    }

    void Update()
    {
        if(pv.IsMine)
        {
            Move();
            Turn();
        }
        // 타인이라면 수신 처리
        else if (!photonView.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, receivePos, Time.deltaTime * 10f);
            transform.rotation = Quaternion.Lerp(transform.rotation, receiveRot, Time.deltaTime * 10f);
        }
    }

    float h => Input.GetAxis("Horizontal");
    float v => Input.GetAxis("Vertical");

    private void Move()
    {
        Vector3 cameraForward = camera.transform.forward;
        Vector3 cameraRight = camera.transform.right;
        cameraForward.y = 0.0f;
        cameraRight.y = 0.0f;

        // 이동 방향 벡터 계산
        Vector3 moveDir = (cameraForward * v) + (cameraRight * h);
        moveDir.Set(moveDir.x, 0.0f, moveDir.z);

        // Player 이동 처리
        characterController.SimpleMove(moveDir * moveSpeed);

        // 애니메이션 처리
        float forward = Vector3.Dot(moveDir, transform.forward);
        float strafe = Vector3.Dot(moveDir, transform.right);

        playerAnim.SetFloat("Forward", forward);
        playerAnim.SetFloat("Strafe", strafe);
    }

    private void Turn()
    {
        ray = camera.ScreenPointToRay(Input.mousePosition);

        // 가상 바닥과의 충돌 계산
        float enter = 0.0f;
        plane.Raycast(ray, out enter);
        hitPoint = ray.GetPoint(enter);

        // 회전 벡터 계산 후 적용
        Vector3 lookDir = hitPoint - transform.position;
        lookDir.y = 0;
        transform.localRotation = Quaternion.LookRotation(lookDir);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 로컬 캐릭터라면 송신
        if(stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        // 아니라면 수신
        else
        {
            receivePos = (Vector3)stream.ReceiveNext();
            receiveRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
