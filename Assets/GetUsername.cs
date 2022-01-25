using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GetUsername : MonoBehaviour
{
    public TextMeshProUGUI UsernameField;
    public String Username;
    
    // Update is called once per frame
    void Update()
    {
        Username = UsernameField.text;
    }
}
