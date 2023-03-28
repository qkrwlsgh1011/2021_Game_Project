using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyPerfect;

[System.Serializable]
public class itemEffect
{
    public string itemName;     // Ű��
    [Tooltip("ü��ȸ�� �Ǵ� �� �����ֱ� �� �����մϴ�.")]
    public string part;         // ȸ�� ��Ʈ
    public float num;             // ȸ�� ��ġ
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