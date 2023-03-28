using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using PolyPerfect;

[System.Serializable]
public class SaveData
{
    // player ��ġ ����
    public Vector3 playerPos;
    public Vector3 playerRot;

    // player���� ����
    public float playerHunger;
    public float playerFatigue;
    public float playerToughness;
    
    // �¾� �� �ð� ����
    public Vector3 sunRot;
    public float timer;
    public float preTimer;
    public int days;
    
    // �κ��丮 ����
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
        // player ��ġ ����
        thePlayer = FindObjectOfType<PlayerControl>();
        saveData.playerPos = thePlayer.transform.position;
        saveData.playerRot = thePlayer.transform.eulerAngles;

        // player���� ����
        saveData.playerHunger = pm.hunger;
        saveData.playerFatigue = pm.fatigue;
        saveData.playerToughness = comWan.toughness;

        // �¾� �� �ð� ����
        saveData.sunRot = sun.transform.eulerAngles;
        saveData.timer = gm.timer;
        saveData.preTimer = gm.preTimer;
        saveData.days = gm.days;


        // �κ��丮 ����
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

        Debug.Log("���� �Ϸ�");
        Debug.Log(json);
    }

    public void LoadData()
    {
        if (File.Exists(SAVE_DATA_DIRECTORY + SAVE_FILENAME))
        {
            string loadJson = File.ReadAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME);
            saveData = JsonUtility.FromJson<SaveData>(loadJson);

            // player ��ġ �ҷ�����
            thePlayer = FindObjectOfType<PlayerControl>();

            thePlayer.GetComponent<CharacterController>().enabled = false;
            thePlayer.transform.position = saveData.playerPos;
            thePlayer.transform.eulerAngles = saveData.playerRot;            
            thePlayer.GetComponent<CharacterController>().enabled = true;

            // player ���� �ҷ�����
            pm.hunger = saveData.playerHunger;
            pm.fatigue = saveData.playerFatigue;
            comWan.toughness = saveData.playerToughness;

            // �¾� �� �ð� �ҷ�����
            sun.transform.eulerAngles = saveData.sunRot;
            gm.timer = saveData.timer;
            gm.preTimer = saveData.preTimer;
            gm.days = saveData.days;
            gm.ChangeDays();


            // �κ��丮 �ҷ�����
            theInven = FindObjectOfType<Inventory>();
            for (int i = 0; i < saveData.invenItemName.Count; i++)
            {
                theInven.LoadToInven(saveData.invenArrayNum[i], saveData.invenItemName[i], saveData.invenItemNum[i]);
            }

            Debug.Log("�ε� �Ϸ�");
        }
        else
            Debug.Log("���̺� ������ �����ϴ�.");        
    }

}
