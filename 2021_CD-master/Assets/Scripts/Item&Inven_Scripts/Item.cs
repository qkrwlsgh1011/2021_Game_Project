using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Item", menuName = "New Item/item", order = 1)]
// ScriptableObject : Object에 안붙여도 작동됨
public class Item : ScriptableObject
{
    public string itemName;
    public ItemType itemType;
    public Sprite itemImage;
    public GameObject itemPrefab;

    public string weaponType;

    public enum ItemType
    {
        Equipment,  // 장비
        Used,       // 소비
        ingredient, // 재료
        ETC         // 기타
    }
}
