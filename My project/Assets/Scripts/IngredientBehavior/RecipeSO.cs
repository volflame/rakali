using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct StatRequirement
{
    public string statName;
    public int requiredValue;
}
// valid stat names: satiation, energy, heroism, horny, wisdom, sexy,
// strength, joy, loud, hot, dehydration, constipation, peace, health,
// furry, bug, drunk, ancient, creativity, glow, clean, claustrophobic
[CreateAssetMenu(menuName = "Potions/Recipe")]
public class RecipeSO : ScriptableObject
{
    public List<StatRequirement> requiredStats;
    public PotionSO result;
    public GameObject potionPrefab;
    public bool combined;
}
