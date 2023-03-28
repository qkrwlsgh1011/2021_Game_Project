using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using PolyPerfect;

[System.Serializable]
public class SaveData
{
    // player 위치 정보
    public Vector3 playerPos;
    public Vector3 playerRot;

    // player스탯 정보
    public float playerHunger;
    public float playerFatigue;
    public float playerToughness;
    
    // 태양 및 시간 정보
    public Vector3 sunRot;
    public float timer;
    public float preTimer;
    public int days;
    
    // 인벤토리 정보
    public List<int> invenArrayNum = new List<int>();
    public List<int> invenItemNum = new List<int>();
    public List<string> invenItemName = new List<string>();
}
public class SaveNLoad : MonoBehaviour
{
    private SaveData saveData = new SaveData();

    private string SAVE_DATA_DIRECTORY;
    private string SAVE_FILENAME = "/SaveFile.txt";

    private PlayerControl thePlayer;
    private Inventory theInven;
    [SerializeField] private GameObject sun;
    [SerializeField] private GameManager gm;
    [SerializeField] private PlayerControl pm;
    [SerializeField] private Common_WanderScript comWan;

    // Start is called before the first frame update
    void Start()
    {
        SAVE_DATA_DIRECTORY = Application.dataPath + "/Saves/";

        if(!Directory.Exists(SAVE_DATA_DIRECTORY))
            Directory.CreateDirectory(SAVE_DATA_DIRECTORY);
    }

    public void SaveData()
    {
        // player 위치 저장
        thePlayer = FindObjectOfType<PlayerControl>();
        saveData.playerPos = thePlayer.transform.position;
        saveData.playerRot = thePlayer.transform.eulerAngles;

        // player스탯 저장
        saveData.playerHunger = pm.hunger;
        saveData.playerFatigue = pm.fatigue;
        saveData.playerToughness = comWan.toughness;

        // 태양 및 시간 저장
        saveData.sunRot = sun.transform.eulerAngles;
        saveData.timer = gm.timer;
        saveData.preTimer = gm.preTimer;
        saveData.days = gm.days;


        // 인벤토리 저장
        theInven = FindObjectOfType<Inventory>();

        Slot[] slots = theInven.GetSlots();
        for (int i = 0; i < slots.Length; i++) 
        {
            if(slots[i].item != null)
            {
                saveData.invenArrayNum.Add(i);
                saveData.invenItemName.Add(slots[i].item.itemName);
                saveData.invenItemNum.Add(slots[i].itemCount);
            }
        }

        string json = JsonUtility.ToJson(saveData);

        File.WriteAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME, json);

        Debug.Log("저장 완료");
        Debug.Log(json);
    }

    public void LoadData()
    {
        if (File.Exists(SAVE_DATA_DIRECTORY + SAVE_FILENAME))
        {
            string loadJson = File.ReadAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME);
            saveData = JsonUtility.FromJson<SaveData>(loadJson);

            // player 위치 불러오기
            thePlayer = FindObjectOfType<PlayerControl>();

            thePlayer.GetComponent<CharacterController>().enabled = false;
            thePlayer.transform.position = saveData.playerPos;
            thePlayer.transform.eulerAngles = saveData.playerRot;            
            thePlayer.GetComponent<CharacterController>().enabled = true;

            // player 스탯 불러오기
            pm.hunger = saveData.playerHunger;
            pm.fatigue = saveData.playerFatigue;
            comWan.toughness = saveData.playerToughness;

            // 태양 및 시간 불러오기
            sun.transform.eulerAngles = saveData.sunRot;
            gm.timer = saveData.timer;
            gm.preTimer = saveData.preTimer;
            gm.days = saveData.days;
            gm.ChangeDays();


            // 인벤토리 불러오기
            theInven = FindObjectOfType<Inventory>();
            for (int i = 0; i < saveData.invenItemName.Count; i++)
            {
                theInven.LoadToInven(saveData.invenArrayNum[i], saveData.invenItemName[i], saveData.invenItemNum[i]);
            }

            Debug.Log("로드 완료");
        }
        else
            Debug.Log("세이브 파일이 없습니다.");        
    }

}
