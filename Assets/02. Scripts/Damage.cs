using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Damage : MonoBehaviour
{
    private Renderer[] renderers;
    private int initHP = 100;
    public int currHP = 100;
    private Animator playerAnim;
    private CharacterController characterController;

    private readonly int dieHash = Animator.StringToHash("Die");
    private readonly int respawnHash = Animator.StringToHash("Respawn");

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        playerAnim = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        // 체력 초기화
        currHP = initHP;
    }

    void OnCollisionEnter(Collision collision)
    {
        if(currHP > 0 && collision.collider.CompareTag("BULLET"))
        {
            currHP -= 20;
            if(currHP <= 0)
            {
                StartCoroutine(PlayerDie());
            }
        }
    }

    IEnumerator PlayerDie()
    {
        // 컨트롤 권한 박탈 후 사망 애니메이션 재생
        characterController.enabled = false;
        playerAnim.SetBool(respawnHash, false);
        playerAnim.SetTrigger(dieHash);
        yield return new WaitForSeconds(3.0f);

        // 투명 처리
        SetPlayerVisible(false);
        yield return new WaitForSeconds(1.5f);

        // 생성 위치 재조정
        Transform[] spawnPoints = GameObject.Find("SpawnPoints").GetComponentsInChildren<Transform>();
        int idx = Random.Range(1, spawnPoints.Length);
        transform.position = spawnPoints[idx].position;

        // 초기화
        currHP = initHP;
        SetPlayerVisible(true);
        characterController.enabled = true;
    }

    private void SetPlayerVisible(bool isVisible)
    {
        for(int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = isVisible;
        }
    }
}
