using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    public Button continueButton;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.playerData.CanContinue())
        {
            continueButton.interactable = true;
        }
    }

    public void SetCubeSizeMode(Dropdown mode)
    {
        GameManager.Instance.SetCubeSizeMode(mode);
    }   
}
