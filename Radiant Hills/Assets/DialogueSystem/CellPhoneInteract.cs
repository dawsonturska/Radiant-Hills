using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;


public class CellPhoneInteract : MonoBehaviour
{
    bool DictionaryOpen = false;

    public GameObject DictionaryUI;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DictionarySwitch()
    {
        if (DictionaryOpen == false)
        {
            DictionaryUI.SetActive(true);
            Time.timeScale = 0f;
            DictionaryOpen = true;
        }
        else
        {
            DictionaryUI.SetActive(false);
            Time.timeScale = 1f;
            DictionaryOpen = false;
        }
    }
}
