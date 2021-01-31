using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum CatState
{
    Reborn, // 리스폰됐을 때
    Walk,   // 기본 상태, 걸어갈 때
    Full,   // 체력이 다 차서 배부를 때
    Fall,   // 우산있는 경우, 천천히 떨어질 때
}

public class PlayerController : MonoBehaviour
{
    public static CatState Cat = CatState.Walk;
    CatState TempCat = Cat;

    // 고양이 상태 값 관리 변수
    Rigidbody2D rigid2D;
    Animator animator;
    // Fall
    float fallForce = 2.0f;
    // Walk
    float jumpPosBuffer = 0.0f;
    float jumpForce = 680.0f;           // 점프 힘
    float walkForce = 30.0f;            // 걷는 힘
    [HideInInspector] public float maxWalkSpeed = 2.0f;   // 걷는 속도 조정값
    // ReBorn
    float RebornTime = 0.5f;            // 리스폰 연출 시간
    float delta = 0.0f;
    Color32[] RebornColor = { new Color32(0, 0, 0, 255),
        new Color32(255, 255, 255, 255) };  // 리스폰 연출 색RebornColor


    // 라이프 관리 변수
    [Header("Life Management")]
    [HideInInspector] static public int lifeCount = 3;      // 현재 라이프 카운트
    int lifeLimit = 7;                                      // 최대 라이프
    public GameObject ImFullImg = null;                     // 배부를 때 말풍선
    public GameObject LifePannel = null;                    // 라이프 parent
    public GameObject LifeImage = null;                     // 라이프 프리팹

    // 아이템 관리 변수
    [Header("Item Management")]
    public GameObject Umb_Take_Sound = null;
    [HideInInspector] public bool HasUmb = false;

    // 리셋 관리 변수
    StageDirector StageD = null;
    public Button ReplayBtn = null;         // 리플레이 버튼

    GameObject CollObj = null;      // 보상을 두 번 주는 걸 막기위해 사용되는 변수

    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(Cat);
        rigid2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // 처음 위치 받아놓기
        StageDirector.SavePoint = transform.position;

        // 라이프 초기화
        for (int ii = 0; ii < lifeCount; ii++)
        {
            GameObject heart = (GameObject)Instantiate(LifeImage);
            heart.transform.SetParent(LifePannel.transform, false);
        }

        // 리플레이버튼 리스너
        StageD = FindObjectOfType<StageDirector>();
        if (ReplayBtn != null)
        {
            ReplayBtn.onClick.AddListener(StageD.ResetGame);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 게임 엔드 화면에서는 못움직이게
        if (StageDirector.StageDir == Stage.GameEnd)
        {
            GetComponent<Animator>().enabled = false;
            return;
        }

        // 플레이어 속도
        float speedx = Mathf.Abs(this.rigid2D.velocity.x);

        // 좌우 이동
        int key = 0;
        if (Input.GetKey(KeyCode.RightArrow)) key = 1;
        if (Input.GetKey(KeyCode.LeftArrow)) key = -1;

        // 움직이는 방향에 따라 이미지 반전
        if (key != 0)
        {
            transform.localScale = new Vector3(key, 1, 1);
        }

        // 점프한다.
        if (Input.GetKeyDown(KeyCode.Space) && rigid2D.velocity.y == 0 && Cat != CatState.Fall)
        {
            animator.SetTrigger("JumpTrigger");
            rigid2D.AddForce(transform.up * jumpForce);
        }

        // 스피드 제한
        if (speedx < maxWalkSpeed)
        {
            rigid2D.AddForce(transform.right * key * walkForce);
        }

        // 화면 벗어나지 못하게 막는 처리
        if (2.5f < transform.position.x)
        {
            transform.position = new Vector3(2.5f, transform.position.y, transform.position.z);
        }
        if (transform.position.x < -2.5f)
        {
            transform.position = new Vector3(-2.5f, transform.position.y, transform.position.z);
        }

        // 플레이어가 아래로 떨어질 때 상태 변화
        if (CameraController.CState == CameraState.InAir || CameraController.CState == CameraState.Top)
        {
            Vector3 CamPos = FindObjectOfType<CameraController>().transform.position;
            Vector3 CatPos = transform.position;
            Vector3 dir = CatPos - CamPos;
            float d = dir.magnitude;
            if (d > 11.3f)      // 화면 밖으로 떨어질 때
            {
                if (HasUmb)     // 우산 있는 경우
                {
                    CameraController.CState = CameraState.Fall;
                    Cat = CatState.Fall;
                }
                else
                {
                    DelLife();
                    transform.position = StageDirector.SavePoint;   // 세이브 포인트로 이동하게 될 부분
                    CameraController.CState = CameraState.Bottom;   // 카메라 다시 세팅
                    Cat = CatState.Reborn;
                }
            } // if (d > 11.0f)      // 화면 밖으로 떨어질 때
        } // if (CameraController.CState == CameraState.InAir)

        // 상태 변화 연출
        if (Cat == CatState.Reborn)
        {
            delta += Time.deltaTime;

            if (delta > (RebornTime / 5.0f) * 4)
                GetComponent<SpriteRenderer>().color = RebornColor[0];
            else if (delta > (RebornTime / 5.0f) * 3)
                GetComponent<SpriteRenderer>().color = RebornColor[1];
            else if (delta > (RebornTime / 5.0f) * 2)
                GetComponent<SpriteRenderer>().color = RebornColor[0];
            else if (delta > (RebornTime / 5.0f) * 1)
                GetComponent<SpriteRenderer>().color = RebornColor[1];
            else if (delta > RebornTime / 5.0f)
                GetComponent<SpriteRenderer>().color = RebornColor[0];

            if (delta > RebornTime)
            {
                GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
                Cat = TempCat;
                delta = 0;
            }
        } // if (Cat == CatState.Reborn)
        else if (Cat == CatState.Walk)
        {
            animator.SetBool("IsFall", false);
            animator.SetBool("IsFull", false);
            ImFullImg.gameObject.SetActive(false);

            // 느리게 연출 해제
            Time.timeScale = 1.0f;
            GetComponent<Rigidbody2D>().gravityScale = 3;
        }
        else if (Cat == CatState.Full)
        {
            ImFullImg.gameObject.SetActive(true);
            animator.SetBool("IsFull", true);
        }
        else if (Cat == CatState.Fall)
        {
            ImFullImg.gameObject.SetActive(false);

            if (rigid2D.velocity.y != 0)    // 발이 허공에 있을 때
            {
                animator.SetBool("IsFall", true);

                // 느리게 연출
                Time.timeScale = 0.3f;
                GetComponent<Rigidbody2D>().gravityScale = 0.001f;
                rigid2D.AddForce(-transform.up * fallForce);
            }
            else
            {
                Cat = TempCat;
                animator.SetBool("IsFall", false);
                CameraController.CState = CameraState.InAir;

                // 느리게 연출 해제
                Time.timeScale = 1.0f;
                GetComponent<Rigidbody2D>().gravityScale = 3;
            }
        } // else if (Cat == CatState.Fall)

        // 플레이어 속도에 맞춰 애니메이션 속도를 바꾼다.
        if (rigid2D.velocity.y == 0)
        {
            animator.speed = speedx / 2.0f;
        }
        else
        {
            animator.speed = 1.0f;
        }
    } // void Update()

    public void AddLife(int amount = 1)
    {
        lifeCount = LifePannel.transform.childCount;
        for (int ii = 0; ii < lifeCount; ii++)
        {
            GameObject heart = LifePannel.transform.GetChild(ii).gameObject;
            Destroy(heart);
        }
        LifePannel.transform.DetachChildren();
        lifeCount = lifeCount + amount;
        for (int ii = 0; ii < lifeCount; ii++)
        {
            GameObject heart = (GameObject)Instantiate(LifeImage);
            heart.transform.SetParent(LifePannel.transform, false);
        }
    }

    public void DelLife(int amount = 1)
    {
        lifeCount = LifePannel.transform.childCount;
        for (int ii = 0; ii < lifeCount; ii++)
        {
            GameObject heart = LifePannel.transform.GetChild(ii).gameObject;
            Destroy(heart);
        }
        LifePannel.transform.DetachChildren();
        lifeCount = lifeCount - amount;

        // GameOver
        if (lifeCount <= 0)
        {
            Time.timeScale = 0.0f;
            GameObject go = GameObject.Find("StageDirector");
            go.transform.GetChild(0).GetComponent<AudioSource>().Stop();
            ReplayBtn.gameObject.SetActive(true);
            return;
        }

        // Full 상태 해제
        Cat = CatState.Walk;
        TempCat = Cat;
        //if (lifeCount < lifeLimit)
        //{
        //    Cat = CatState.Walk;
        //    TempCat = Cat;
        //}
        for (int ii = 0; ii < lifeCount; ii++)
        {
            GameObject heart = (GameObject)Instantiate(LifeImage);
            heart.transform.SetParent(LifePannel.transform, false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 통조림 먹기
        if (other.gameObject.tag.Equals("FishCan"))
        {
            if (Cat != CatState.Full && TempCat != CatState.Full)
            {   // 안배부른 상태일 때(TempCat은 상태가 Fall 상태일 때도 대비하여)
                if (CollObj != other.gameObject)    // 중복되지 않을 때만
                {
                    CollObj = other.gameObject;
                    AddLife();
                    GetComponent<AudioSource>().Play();
                    Destroy(other.gameObject);
                }
                if (lifeCount >= lifeLimit)
                {
                    Cat = CatState.Full;
                    TempCat = Cat;
                }
            } // if (Cat != CatState.Full && TempCat != CatState.Full)
        }

        // 우산 먹기
        if (other.gameObject.tag.Equals("Umbrella"))
        {
            if (CollObj != other.gameObject)
            {
                if (UmbrellaDirector.UmbState == Umbrella.Normal)   // 우산 가지고 있는 경우
                {
                    UmbrellaDirector UmbD = FindObjectOfType<UmbrellaDirector>();
                    UmbD.NewUmbrella();
                }

                CollObj = other.gameObject;
                Umb_Take_Sound.GetComponent<AudioSource>().Play();
                Destroy(other.gameObject);
                HasUmb = true;
                UmbrellaDirector.UmbState = Umbrella.Normal;
            }
        }

        // 세이브 포인트
        if (other.gameObject.tag.Equals("Respawn"))
        {
            if (CollObj != other.gameObject)
            {
                CollObj = other.gameObject;
                StageDirector.SavePoint = other.transform.position;

                // 스테이지
                if (other.gameObject.name.Contains("Stage1"))
                    StageDirector.StageDir = Stage.Stage2;
                else if (other.gameObject.name.Contains("Stage2"))
                    StageDirector.StageDir = Stage.Stage3;
                else if (other.gameObject.name.Contains("Stage3"))
                    StageDirector.StageDir = Stage.Stage4;
                Debug.Log(StageDirector.StageDir);

                // 깃발 연출
                other.GetComponent<SpriteRenderer>().sprite = Resources.Load("flag", typeof(Sprite)) as Sprite;
            }
        }

        // 도착
        if (other.gameObject.tag.Equals("Finish"))
        {
            if (CollObj != other.gameObject)
            {
                CollObj = other.gameObject;
                StageDirector.StageDir = Stage.GameEnd;

                // 스위치 연출
                other.GetComponent<SpriteRenderer>().sprite = Resources.Load("switch", typeof(Sprite)) as Sprite;
            }
        }
    } // void OnTriggerEnter2D(Collider2D other)
}
