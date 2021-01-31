using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballController : MonoBehaviour
{
    GameObject player;
    float DownSpeed = -0.05f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("cat");
    }

    // Update is called once per frame
    void Update()
    {
        // 프레임마다 등속으로 낙하시킨다.
        transform.Translate(0, DownSpeed, 0);

        // 화면 밖으로 떨어지면 소멸
        Vector3 CamPos = FindObjectOfType<CameraController>().transform.position;
        Vector3 dir = transform.position - CamPos;
        float d = dir.magnitude;
        if (d > 15.0f)      // 화면 밖으로 떨어질 때
        {
            Destroy(gameObject);
        }

        // 충돌 판정
        Vector2 p1 = transform.position;        // 파이어볼의 중심 좌표
        Vector2 p2 = player.transform.position; // 플레이어의 중심 좌표
        dir = p1 - p2;
        d = dir.magnitude;
        float r1 = 0.3f;    // 파이어볼 반경
        float r2 = 0.4f;    // 플레이어 반경

        if (d < r1 + r2)    // 충
        {
            Destroy(gameObject);

            // 플레이어 스크립트에 충돌 전달
            player.GetComponent<PlayerController>().DelLife();
            PlayerController.Cat = CatState.Reborn;
        }
    }
}
