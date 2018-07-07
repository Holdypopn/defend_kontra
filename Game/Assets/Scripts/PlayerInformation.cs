using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInformation : MonoBehaviour
{
    private static Player current;
    private static Transform t;
    // Use this for initialization
    void Start()
    {
        Player.PlayerSelect += OnPlayerSelect;
        gameObject.SetActive(false);
        t = transform;
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
        gameObject.SetActive(true);
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
        gameObject.SetActive(false);
    }

    public static void UpdateStats()
    {
        //Inventory
        t.Find("Resource").Find("StoneCount").GetComponent<Text>().text = current.Resources.Stone.ToString();
        t.Find("Resource").Find("AmmoCount").GetComponent<Text>().text = current.Resources.Ammo.ToString();

        //PlayerPropertys
        PlayerPropertys props = current.PlayerPropertys;
        SetUiPropertys("MoveSpeed", props.MoveSpeedLevel, props.MoveSpeedCost * props.MoveSpeedLevel, (int)(100 * props.MoveSpeedPercentage));
        SetUiPropertys("Health", props.HealthLevel, props.HealthCost * props.HealthLevel, (int)(100 * props.HealthPercentage));
        SetUiPropertys("FireRate", props.FireRateLevel, props.FireRateCost * props.FireRateLevel, (int)(100 * props.FireRatePercentage));
        SetUiPropertys("Damage", props.DamageLevel, props.DamageCost * props.DamageLevel, (int)(100 * props.DamagePercentage));
        SetUiPropertys("RepairSpeed", props.RepairSpeedLevel, props.RepairSpeedCost * props.RepairSpeedLevel, (int)(100 * props.RepairSpeedPercentage));
        SetUiPropertys("ResourceSpeed", props.ResourceSpeedLevel, props.ResourceSpeedCost * props.ResourceSpeedLevel, (int)(100 * props.ResourceSpeedPercentage));
    }

    private static void SetUiPropertys(string name, int level, int cost, int percentage)
    {
        string l = level < PlayerPropertys.MaxLevel ? level.ToString() : "Max";
        string c = level < PlayerPropertys.MaxLevel ? cost.ToString() : "";
        t.Find("PlayerPropertys").Find(name).Find("Level").GetComponent<Text>().text = l;
        t.Find("PlayerPropertys").Find(name).Find("Cost").GetComponent<Text>().text = c;
        t.Find("PlayerPropertys").Find(name).Find("Upgrade").Find("Text").GetComponent<Text>().text = "+ " + percentage.ToString() + "%";
        t.Find("PlayerPropertys").Find(name).Find("Upgrade").GetComponent<Button>().gameObject.SetActive(level < PlayerPropertys.MaxLevel && Statistics.Credits >= cost);
    }

    public void UpgradeMoveSpeed()
    {
        if (Statistics.Pay(current.PlayerPropertys.MoveSpeedLevel * current.PlayerPropertys.MoveSpeedCost))
        {
            current.UpdateMoveSpeed();
            current.PlayerPropertys.MoveSpeedLevel++;

            UpdateStats();
        }
    }

    public void UpgradeHealth()
    {
        if (Statistics.Pay(current.PlayerPropertys.MoveSpeedLevel * current.PlayerPropertys.MoveSpeedCost))
        {
            current.UpdateHealth();
            current.PlayerPropertys.HealthLevel++;

            UpdateStats();
        }
    }

    public void UpgradeFireRate()
    {
        if (Statistics.Pay(current.PlayerPropertys.FireRateLevel * current.PlayerPropertys.FireRateCost))
        {
            current.UpdateFireRate();
            current.PlayerPropertys.FireRateLevel++;

            UpdateStats();
        }
    }

    public void UpgradeDamage()
    {
        if (Statistics.Pay(current.PlayerPropertys.DamageLevel * current.PlayerPropertys.DamageCost))
        {
            current.UpdateDamage();
            current.PlayerPropertys.DamageLevel++;

            UpdateStats();
        }
    }

    public void UpgradeRepairSpeed()
    {
        if (Statistics.Pay(current.PlayerPropertys.RepairSpeedLevel * current.PlayerPropertys.RepairSpeedCost))
        {
            current.UpdateRepairSpeed();
            current.PlayerPropertys.RepairSpeedLevel++;

            UpdateStats();
        }
    }

    public void UpgradeResourceSpeed()
    {
        if (Statistics.Pay(current.PlayerPropertys.RepairSpeedLevel * current.PlayerPropertys.RepairSpeedCost))
        {
            current.UpdateResourceSpeed();
            current.PlayerPropertys.ResourceSpeedLevel++;

            UpdateStats();
        }
    }
}

public class PlayerPropertys
{
    public const int MaxLevel = 15;

    public float MoveSpeedPercentage = 0.05f;
    public float HealthPercentage = 0.05f;
    public float FireRatePercentage = 0.05f;
    public float DamagePercentage = 0.05f;
    public float RepairSpeedPercentage = 0.05f;
    public float ResourceSpeedPercentage = 0.05f;

    public int MoveSpeedCost = 5;
    public int HealthCost = 5;
    public int FireRateCost = 5;
    public int DamageCost = 5;
    public int RepairSpeedCost = 5;
    public int ResourceSpeedCost = 5;

    public int MoveSpeedLevel = 1;
    public int HealthLevel = 1;
    public int FireRateLevel = 1;
    public int DamageLevel = 1;
    public int RepairSpeedLevel = 1;
    public int ResourceSpeedLevel = 1;
}
