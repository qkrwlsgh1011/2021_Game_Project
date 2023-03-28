using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static bool inventoryActivated = false;

    [SerializeField]
    private GameObject invenBase;   // 인벤 이미지
    [SerializeField]
    private GameObject grid_Setting;
    [SerializeField]
    private ItemEffectDatabase itemDB;

    private Slot[] slots;

    public Slot[] GetSlots()
    {
        return slots;
    }
    [SerializeField] private Item[] items;
    public void LoadToInven(int _arryaNum, string _itemName, int _itemNum)
    {
        for (int i = 0; i < items.Length; i++)
            if (items[i].itemName == _itemName)
                slots[_arryaNum].Additem(items[i], _itemNum);
    }

    // Start is called before the first frame update
    void Start()
    {
        slots = grid_Setting.GetComponentsInChildren<Slot>();
    }

    // Update is called once per frame
    void Update()
    {
        TryOpenInventory();
    }

    private void TryOpenInventory()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryActivated = !inventoryActivated;

            if (inventoryActivated)
                OpenInventory();
            else
                CloseInventory();
        }
    }

    private void OpenInventory()
    {
        invenBase.SetActive(true);
    }

    private void CloseInventory()
    {
        invenBase.SetActive(false);
    }

    public void Acquireitem(Item _item, int _count = 1)
    {
        if (_item.itemType != Item.ItemType.Equipment)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item != null)
                {
                    if (slots[i].item.itemName == _item.itemName)
                    {
                        slots[i].SetSlotCount(_count);
                        return;
                    }
                }
            }
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                slots[i].Additem(_item, _count);
                return;
            }
        }
    }

    public bool FindFeedItem()
    {
        bool flag = false;

        Slot[] slots = GetSlots();
        for (int i = 0; i < slots.Length; i++)
            if (slots[i].item != null)
                if (slots[i].item.itemName == "Feed")
                    flag = true;
        return flag;
    }

    public void UseFeedItem()
    {
        Slot[] slots = GetSlots();
        for (int i = 0; i < slots.Length; i++)
            if (slots[i].item != null)
                if (slots[i].item.itemName == "Feed")
                    slots[i].SetSlotCount(-1);
    }
}
