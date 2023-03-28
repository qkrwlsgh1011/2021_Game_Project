using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyPerfect;

[System.Serializable]
public class itemEffect
{
    public string itemName;     // 키값
    [Tooltip("체력회복 또는 펫 먹이주기 가 가능합니다.")]
    public string part;         // 회복 파트
    public float num;             // 회복 수치
}

public class ItemEffectDatabase : MonoBehaviour
{
    public Common_WanderScript comWan;
    public PlayerControl player;
    public GameManager gm;

    [SerializeField]
    private itemEffect[] itemEffects;

    public void UseItem(Item _item)
    {
        if (_item.itemType == Item.ItemType.Used)
        {
            for (int i = 0; i < itemEffects.Length; i++)
            {
                if (itemEffects[i].itemName == _item.itemName)
                {
                    recovery(itemEffects[i].num);
                }
            }
        }
    }
    public void recovery(float num)
    {
        comWan.toughness += num;
        gm.UpdateHPbar();
    }
}