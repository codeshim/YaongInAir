using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Stage
{
    Stage1,
    Stage2,
    Stage3,
    Stage4,
    GameEnd,
}

public class StageDirector : MonoBehaviour
{
    GameObject player;

    public static Stage StageDir = Stage.Stage1;
    public static Vector3 SavePoint;

    // Stage1
    [Header("Stage1")]
    public GameObject Title = null;                 // 타이틀 화면
    float fades2 = 1.0f;

    // Stage2
    [Header("Stage2")]
    int windKey = 1;                                // 바람부는 방향
    float[] windTime = { 3.0f, 5.0f, 6.0f, 7.0f };  // 바람바뀌는 속도
    int randNum = 0;
    public GameObject[] Wind_Flag = new GameObject[3]; 
    public GameObject Wind_Effect = null;

    // Stage3
    [Header("Stage3")]
    public GameObject Flag_Unplug = null;           // 바람 안부는 풍향계
    public GameObject Fireball = null;              // 파이어볼 프리팹
    float[] FireBallSpot = {-2.5f, -2.0f, -1.5f, -1.0f, -0.5f,
        0.0f, 0.5f, 1.0f, 1.5f, 2.0f, 2.5f};        // 파이어볼 스폰 위치
    float FireBallSpan = 1.0f;                      // 파이어볼 생성 주기

    // Stage4
    [Header("Stage4")]
    int windKey2 = 1;
    int randNum2 = 0;
    public GameObject[] Wind_Flag_S4 = new GameObject[2];

    // GameEnd
    [Header("GameEnd")]
    float fades = 1.0f;                     // 페이드 아웃 변수
    public GameObject Background = null;    // 기존의 배경화면
    public GameObject Sun = null;           // 해 오브젝트
    public GameObject Moon = null;          // 달 오브젝트
    public Button ReplayBtn = null;         // 리플레이 버튼

    // 시간 측정용 델타
    float delta = 0.0f;
    float delta2 = 0.0f;
    float delta3 = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("cat");

        // 바람 초기값 설정
        delta = windTime[randNum];
        delta2 = windTime[randNum2];

        // 리플레이버튼 리스너
        if (ReplayBtn != null)
        {
            ReplayBtn.onClick.AddListener(ResetGame);
        }
    }

    // Update is called once per frame
    void Update()
    {

        //Debug.Log(StageDir);
        if (StageDir == Stage.Stage1)
        {
            // 타이틀 연출
            delta3 += Time.deltaTime;
            if (fades2 > 0.0f && delta3 >= 0.1f)
            {
                delta3 = 0.0f;
                fades2 -= 0.1f;
                Title.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, fades2);
            }
            else if (fades2 <= 0.0f)
            {
                delta3 = 0.0f;
                if (Title.gameObject.activeSelf == true)
                {
                    Title.gameObject.SetActive(false);
                }
            }
        } // if (StageDir == Stage.Stage1)
        else if (StageDir == Stage.Stage2)
        {
            if (player.transform.position.y > 25)
            {
                // 바람 효과 시작
                delta += Time.deltaTime;
                if (delta > windTime[randNum])
                {
                    delta = 0.0f;
                    randNum = Random.Range(0, 4);
                    Wind_Effect.GetComponent<AudioSource>().Play();
                    windKey = windKey * -1;

                    // 풍향계 연출
                    for (int ii = 0; ii < Wind_Flag.Length; ii++)
                    {
                        Wind_Flag[ii].transform.localScale = new Vector3(windKey, 1, 1);
                    }
                }
                float speedx = Mathf.Abs(player.GetComponent<Rigidbody2D>().velocity.x);
                if (speedx < player.GetComponent<PlayerController>().maxWalkSpeed)
                {
                    player.GetComponent<Rigidbody2D>().AddForce(transform.right * windKey * 10.0f);
                }
            } // if (player.transform.position.y > 25)
        } // else if (StageDir == Stage.Stage2)
        else if (StageDir == Stage.Stage3 || StageDir == Stage.Stage4)
        {
            if (player.transform.position.y > 52)
            {
                delta += Time.deltaTime;
                if (delta > FireBallSpan)
                {
                    delta = 0.0f;
                    randNum = Random.Range(0, 11);
                    GameObject go = Instantiate(Fireball) as GameObject;
                    Vector3 CamPos = FindObjectOfType<CameraController>().transform.position;
                    go.transform.position = new Vector3(FireBallSpot[randNum], CamPos.y + 7.0f, 0);
                }
            } // if (player.transform.position.y > 52)
            else
            {
                delta = 0.0f;
                // 풍향계 끄기 연출
                if (Wind_Flag[0].gameObject.activeSelf == true)
                {
                    for (int ii = 0; ii < Wind_Flag.Length; ii++)
                    {
                        Wind_Flag[ii].gameObject.SetActive(false);
                    }
                    Flag_Unplug.gameObject.SetActive(true);
                } // if (Wind_Flag[0].gameObject.activeSelf == true)
            } // else

            if (StageDir == Stage.Stage4)
            {
                if (player.transform.position.y > 80)
                {
                    // 바람 효과 시작
                    delta2 += Time.deltaTime;
                    if (delta2 > windTime[randNum2])
                    {
                        delta2 = 0.0f;
                        randNum2 = Random.Range(0, 4);
                        Wind_Effect.GetComponent<AudioSource>().Play();
                        windKey2 = windKey2 * -1;

                        // 풍향계 연출
                        for (int ii = 0; ii < Wind_Flag_S4.Length; ii++)
                        {
                            Wind_Flag_S4[ii].transform.localScale = new Vector3(windKey2, 1, 1);
                        }
                    }
                    float speedx = Mathf.Abs(player.GetComponent<Rigidbody2D>().velocity.x);
                    if (speedx < player.GetComponent<PlayerController>().maxWalkSpeed)
                    {
                        player.GetComponent<Rigidbody2D>().AddForce(transform.right * windKey2 * 12.0f);
                    }
                } // if (player.transform.position.y > 80)
            } // if (StageDir == Stage.Stage4)
        } // else if (StageDir == Stage.Stage3)
        else if (StageDir == Stage.GameEnd)
        {
            if (Sun.gameObject.activeSelf == true)
            {
                Sun.gameObject.SetActive(false);
                Moon.gameObject.SetActive(true);
            }

            delta3 += Time.deltaTime;
            if (fades > 0.0f && delta3 >= 0.1f)
            {
                delta3 = 0.0f;
                fades -= 0.1f;
                Background.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, fades);
            }
            else if (fades <= 0.0f)
            {
                delta3 = 0.0f;
                // replay 버튼 생성
                ReplayBtn.gameObject.SetActive(true);
            }
        } // else if (StageDir = Stage.GameEnd)
    }

    public void ResetGame()
    {
        // static 변수들 초기화 후 Scene 로드
        StageDir = Stage.Stage1;
        SavePoint = new Vector3(-1.95f, -4.34f, 0);
        CameraController.CState = CameraState.Bottom;
        PlayerController.Cat = CatState.Walk;
        PlayerController.lifeCount = 3;
        UmbrellaDirector.UmbState = Umbrella.None;
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
        player.transform.position = SavePoint;
        Time.timeScale = 1.0f;
    }
}
