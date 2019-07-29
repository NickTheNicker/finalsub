using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CurrentHealth : MonoBehaviour
{
    // !Cached References!
    // Reference for text that displays the 'Player's current health.
    Text text;
    // The 'Player' object.
    Player player;

    // Start is called before the first frame update.
    void Start()
    {
        // Gets the text component.
        text = GetComponent<Text>();
        // Finds the player object to reference it.
        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame.
    void Update()
    {
        // Finds the player int 'pHealth' and converts it into a string value and displays it on the text component.
        text.text = player.FindHealth().ToString();
    }
}
