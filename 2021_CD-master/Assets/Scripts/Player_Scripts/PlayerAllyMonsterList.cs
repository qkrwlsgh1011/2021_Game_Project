using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAllyMonsterList : MonoBehaviour
{
    public List<string> list;

    private void Start()
    {
        list = new List<string>();
    }

    public void addMonster(string species)
    {
        list.Add(species);
    }

    public void deleteMonster(string species)
    {
        list.Remove(species);
    }
}
