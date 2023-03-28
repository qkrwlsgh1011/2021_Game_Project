using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Item", menuName = "New Item/item", order = 1)]
// ScriptableObject : Object�� �Ⱥٿ��� �۵���
public class Item : ScriptableObject
{
    public string itemName;
    public ItemType itemType;
    public Sprite itemImage;
    public GameObject itemPrefab;

    public string weaponType;

    public enum ItemType
    {
        Equipment,  // ���
        Used,       // �Һ�
        ingredient, // ���
        ETC         // ��Ÿ
    }
}
