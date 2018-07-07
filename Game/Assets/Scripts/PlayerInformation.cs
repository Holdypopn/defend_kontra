using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInformation : MonoBehaviour
{
    private Player current;
    // Use this for initialization
    void Start()
    {
        Player.PlayerSelect += OnPlayerSelect;
    }

    private void OnPlayerSelect(Player player)
    {
        if (current != null)
        {
            current.InformationUpdated -= OnInformationUpdated;
            current.PlayerDies -= OnPlayerDies;
            current = null;
        }

        current = player;
        player.InformationUpdated += OnInformationUpdated;
        player.PlayerDies += OnPlayerDies;
        UpdateStats();
    }

    private void OnInformationUpdated(Player p)
    {
        UpdateStats();
    }

    private void OnPlayerDies(Player p)
    {
        p.InformationUpdated -= OnInformationUpdated;
        p.PlayerDies -= OnPlayerDies;
        current = null;
    }

    private void UpdateStats()
    {
        transform.Find("Resource").Find("StoneCount").GetComponent<Text>().text = current.Resources.Stone.ToString();
        transform.Find("Resource").Find("AmmoCount").GetComponent<Text>().text = current.Resources.Ammo.ToString();
    }
}
