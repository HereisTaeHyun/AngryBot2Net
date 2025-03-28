using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private CharacterController characterController;
    private new Transform transform;
    private Animator playerAnim;
    private new Camera camera;

    private Plane plane;
    private Ray ray;
    private Vector3 hitPoint;

    public float moveSpeed = 10.0f;
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        transform = GetComponent<Transform>();
        playerAnim = GetComponent<Animator>();
        camera = Camera.main;

        // Player 아래 가상 바닥 생성
        plane = new Plane(transform.up, transform.position);
    }

    void Update()
    {
        Move();
        Turn();
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
}
