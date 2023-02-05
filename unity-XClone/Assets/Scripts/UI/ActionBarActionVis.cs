using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionBarActionVis : MonoBehaviour{

    public Button MyButton;
    public TextMeshProUGUI Titletxt;
    public TextMeshProUGUI Keytxt;

    
    public void SetTextAndAction(
        string action_title, 
        System.Action action, 
        int key
    ){
        Titletxt.text = action_title;
        MyButton.onClick.AddListener(() => action?.Invoke());
        Keytxt.text = "(" + key.ToString() + ")";
    }
}
