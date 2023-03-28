using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PolyPerfect;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject coverImage;
    public GameObject pauseImage;
    public GameObject askSaveImage;
    public GameObject overImage;

    [SerializeField] private GameObject invenBase;  // 인벤 이미지
    [SerializeField] private Image sleepImage;      // 수면 이미지

    [SerializeField] private GameObject thePlayer;
    [SerializeField] private Image hpBar;
    [SerializeField] private Image hungerBar;
    [SerializeField] private Image fatigueBar;
    [SerializeField] private Common_WanderScript comWan;
    [SerializeField] private PlayerControl pm;
    [SerializeField] private RandomCharacterPlacer rcp;

    public GameObject sunLight;
    public float sunRotSpeed; // 1초에 180도

    public int days = 1;
    public Text daysText;

    public float timer;
    public float preTimer;
    public int endDay;
    private int sleepingTimer;
    public float spawnTimer;
    public float spawnTimerValue;

    private bool willExit;

    [SerializeField] private SaveNLoad theSaveNLoad;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0;
        preTimer = 0;
        days = 1;
        sleepingTimer = 1;
        spawnTimer = spawnTimerValue;

        coverImage.SetActive(true);
        pauseImage.SetActive(false);
        askSaveImage.SetActive(false);
        invenBase.SetActive(false);
        overImage.SetActive(false);        
    }
    // Update is called once per frame
    void Update()
    {
        // 일시정지
        if (Input.GetKeyDown(KeyCode.P))
        {
            pauseImage.SetActive(true);
            Time.timeScale = 0;
        }

        // 빛의 회전
        sunLight.transform.Rotate(new Vector3(1, 0, 0) * Time.deltaTime * sunRotSpeed);
        
        timer += Time.deltaTime * sleepingTimer;
        if ((int)(timer - preTimer) == 1)
        {
            if ((int)timer % 360 == 0)
            {
                days++;
                ChangeDays();
            }
                
            preTimer = timer;
        }

        // 시간 종료
        if (days == endDay)
        {
            GameOver();
        }

        if (sleepImage.color.a > 0)
        {
            // sleeping
            sunRotSpeed = 36;
            sleepingTimer = (int)sunRotSpeed;
        }

        if (sleepImage.color.a == 0)
        {
            // awake
            sunRotSpeed = 1;
            sleepingTimer = (int)sunRotSpeed;
        }

        UpdateHPbar();
        updateHungerbar();
        updateFatiguebar();

        spawnTimer = Mathf.MoveTowards(spawnTimer, 0f, Time.deltaTime);
        if(spawnTimer == 0f)
        {
            rcp.SpawnAnimals();
            spawnTimer = spawnTimerValue;
        }

    }

    public void newGame()
    {
        // player위치 초기화
        thePlayer.GetComponent<CharacterController>().enabled = false;
        thePlayer.transform.position = new Vector3(1.98f, 11.709f, 80.08f);
        thePlayer.transform.eulerAngles = new Vector3(0, 0, 0);
        thePlayer.GetComponent<CharacterController>().enabled = true;

        // player스탯 초기화
        pm.HungerIncrease(100f);
        pm.fatigue = 0f;
        comWan.toughness = 100f;

        // player 장비 초기화
        pm.isEquipSword = false;
        pm.isEquipBow = false;

        // 태양 및 시간 초기화
        preTimer = 0;
        days = 1;

        // UI 초기화
        coverImage.SetActive(true);
        pauseImage.SetActive(false);
        askSaveImage.SetActive(false);
        invenBase.SetActive(false);
        overImage.SetActive(false);
    }
    // 시작 버튼
    public void OnClickStartButton()
    {
        newGame();

        coverImage.SetActive(false);
        willExit = false;
        Time.timeScale = 1;
    }
    // 저장 버튼
    public void OnClickSaveButton()
    {
        theSaveNLoad.SaveData();
        pauseImage.SetActive(false);        
        Time.timeScale = 1;
    }
    // 불러오기 버튼
    public void OnClickLoadButton()
    {                
        theSaveNLoad.LoadData();
        coverImage.SetActive(false);
        Time.timeScale = 1;
    }
    // 종료 버튼
    public void OnClickExitButton(bool inGame)
    {
        // 인게임에서 부른거면 저장여부 물어보기
        if (inGame)
        {
            willExit = true;
            askSaveImage.SetActive(true);
        }
        else
            Application.Quit();
    }
    // 메뉴 닫기 버튼
    public void OnClickCloseButton()
    {
        pauseImage.SetActive(false);
        Time.timeScale = 1;
    }
    // 메인으로 돌아가기 버튼
    public void OnClinckMainButton(bool Dead)
    {
        if (Dead)
        {
            overImage.SetActive(false);
            coverImage.SetActive(true);           
        }
        else
            askSaveImage.SetActive(true);
    }
    // 저장여부 물어보기 버튼
    public void AskSave(bool willSave)
    {
        if (willSave)
            theSaveNLoad.SaveData();
        if (willExit)
            Application.Quit();
        else
        {
            askSaveImage.SetActive(false);
            pauseImage.SetActive(false);
            coverImage.SetActive(true);
        }
    }

    public void ChangeDays()
    {
        daysText.text = days + "일차";
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        overImage.SetActive(true);
    }

    public void UpdateHPbar()
    {
        hpBar.fillAmount = comWan.toughness / 100;
    }

    public void updateHungerbar() {
        
        hungerBar.fillAmount = pm.hunger / 100;
    }

    public void updateFatiguebar() {
        fatigueBar.fillAmount = pm.fatigue / 100;
    }

}
