using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using PolyPerfect;

public class DeleteMonster : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Decay());
    }

    IEnumerator Decay()
    {
        yield return new WaitForSeconds(10f);
        gameObject.SetActive(false);
    }
}
