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

    [SerializeField] private GameObject invenBase;  // �κ� �̹���
    [SerializeField] private Image sleepImage;      // ���� �̹���

    [SerializeField] private GameObject thePlayer;
    [SerializeField] private Image hpBar;
    [SerializeField] private Image hungerBar;
    [SerializeField] private Image fatigueBar;
    [SerializeField] private Common_WanderScript comWan;
    [SerializeField] private PlayerControl pm;
    [SerializeField] private RandomCharacterPlacer rcp;

    public GameObject sunLight;
    public float sunRotSpeed; // 1�ʿ� 180��

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
        // �Ͻ�����
        if (Input.GetKeyDown(KeyCode.P))
        {
            pauseImage.SetActive(true);
            Time.timeScale = 0;
        }

        // ���� ȸ��
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

        // �ð� ����
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
        // player��ġ �ʱ�ȭ
        thePlayer.GetComponent<CharacterController>().enabled = false;
        thePlayer.transform.position = new Vector3(1.98f, 11.709f, 80.08f);
        thePlayer.transform.eulerAngles = new Vector3(0, 0, 0);
        thePlayer.GetComponent<CharacterController>().enabled = true;

        // player���� �ʱ�ȭ
        pm.HungerIncrease(100f);
        pm.fatigue = 0f;
        comWan.toughness = 100f;

        // player ��� �ʱ�ȭ
        pm.isEquipSword = false;
        pm.isEquipBow = false;

        // �¾� �� �ð� �ʱ�ȭ
        preTimer = 0;
        days = 1;

        // UI �ʱ�ȭ
        coverImage.SetActive(true);
        pauseImage.SetActive(false);
        askSaveImage.SetActive(false);
        invenBase.SetActive(false);
        overImage.SetActive(false);
    }
    // ���� ��ư
    public void OnClickStartButton()
    {
        newGame();

        coverImage.SetActive(false);
        willExit = false;
        Time.timeScale = 1;
    }
    // ���� ��ư
    public void OnClickSaveButton()
    {
        theSaveNLoad.SaveData();
        pauseImage.SetActive(false);        
        Time.timeScale = 1;
    }
    // �ҷ����� ��ư
    public void OnClickLoadButton()
    {                
        theSaveNLoad.LoadData();
        coverImage.SetActive(false);
        Time.timeScale = 1;
    }
    // ���� ��ư
    public void OnClickExitButton(bool inGame)
    {
        // �ΰ��ӿ��� �θ��Ÿ� ���忩�� �����
        if (inGame)
        {
            willExit = true;
            askSaveImage.SetActive(true);
        }
        else
            Application.Quit();
    }
    // �޴� �ݱ� ��ư
    public void OnClickCloseButton()
    {
        pauseImage.SetActive(false);
        Time.timeScale = 1;
    }
    // �������� ���ư��� ��ư
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
    // ���忩�� ����� ��ư
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
        daysText.text = days + "����";
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
