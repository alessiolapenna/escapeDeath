using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIText : MonoBehaviour
{
    // Start is called before the first frame update
    TextMeshProUGUI label;

    private void Awake()
    {
        label = GetComponent<TextMeshProUGUI>();
        Debug.Log(label);
    }

    void Start()
    {
        if (GlobalGameStatus.shared.userJustDied)
        {
            label.text = "EVERY TIME YOU DIE YOU AND THE ENEMY EXCHANGE PLACE";
            GlobalGameStatus.shared.userJustDied = false;
        }
    }
}
