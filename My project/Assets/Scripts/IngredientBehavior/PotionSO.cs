using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Potions/Potion")]
public class PotionSO : ScriptableObject
{
    public string potionName;
    public string flavorText;
    public Sprite potionSprite;
    public GameObject potionPrefab;
}