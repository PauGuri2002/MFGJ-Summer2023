using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Season List")]
public class Seasons : ScriptableObject
{
    public SeasonInfo[] seasons;
}

[Serializable]
public class SeasonInfo
{
    public Season seasonName;
    public string displayName;
    public Color color;
    public Color softColor;
    public string startDate = "MM/DD";
    public string endDate = "MM/DD";
}

public enum Season : int
{
    Winter = 6,
    Spring = 7,
    Summer = 8,
    Autumn = 9
}