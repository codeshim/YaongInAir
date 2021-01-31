using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraState
{
    Bottom,     // 시작 상태
    InAir,      // 카메라가 바닥에서 멀어졌을 때
    Fall,       // 고양이가 우산타고 떨어질 때
    Top,        // 종료 상태
}

public class CameraController : MonoBehaviour
{
    public static CameraState CState = CameraState.Bottom;

    GameObject player;
    bool HitTop = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("cat");
        //startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(CState);
        Vector3 playerPos = player.transform.position;
        if (CState == CameraState.Bottom)
        {
            transform.position = new Vector3(transform.position.x, playerPos.y, transform.position.z);

            if (playerPos.y < 0.0f) // 화면이 아래로 내려가지 않도록(스타트지점)
                transform.position = new Vector3(transform.position.x, 0.005f, transform.position.z);
            
            if (player.transform.position.y > 3.0f) // 올라가는 상태로 변경
                CState = CameraState.InAir;
        }
        else if (CState == CameraState.InAir)
        {
            HitTop = false;
            if (transform.position.y < playerPos.y)
                transform.position = new Vector3(transform.position.x, playerPos.y, transform.position.z);

            if (playerPos.y > 97.0f)    // 화면에 더 위로 올라가지 않도록(엔드지점)
                CState = CameraState.Top;
        }
        else if (CState == CameraState.Fall)
        {
            HitTop = false;
            transform.position = new Vector3(transform.position.x, playerPos.y, transform.position.z);

            if (playerPos.y < 0.0f) // 화면이 아래로 내려가지 않도록
                transform.position = new Vector3(transform.position.x, 0.005f, transform.position.z);
        }
        else if (CState == CameraState.Top)
        {
            if (HitTop == false)
            {
                transform.position = new Vector3(transform.position.x, playerPos.y, transform.position.z);

                if (playerPos.y > 100.0f)
                {
                    transform.position = new Vector3(transform.position.x, 100.0f, transform.position.z);
                    HitTop = true;
                }
                else
                    CState = CameraState.InAir;
            } // if (HitTop == false)
            else
            {
                transform.position = new Vector3(transform.position.x, 100.0f, transform.position.z);
            } // else
        } // else if (CState == CameraState.Top)
    }
}
