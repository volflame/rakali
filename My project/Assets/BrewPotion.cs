using System.Collections.Generic;
using UnityEngine;

public class PotionCrafter : MonoBehaviour
{
    public string ingredient1Id;
    public string ingredient2Id;
    public string ingredient3Id;
    public GameObject potionPrefab;

    private List<GameObject> ingredientsInZone = new List<GameObject>();

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Tags>() != null && other.GetComponent<Tags>().HasTag("mashed")) // TO DO: RYAN MAKE SURE that when you have "unmashable objects" you fix this logic.
        {
            ingredientsInZone.Add(other.gameObject);
            TryBrew();
        }
    }

    void OnTriggerExit(Collider other)
    {
        ingredientsInZone.Remove(other.gameObject);
    }

    void TryBrew()
    {
        // Find each required ingredient in the zone
        GameObject ing1 = ingredientsInZone.Find(o => o.GetComponent<IngredientID>().id == ingredient1Id);
        GameObject ing2 = ingredientsInZone.Find(o => o.GetComponent<IngredientID>().id == ingredient2Id);
        GameObject ing3 = ingredientsInZone.Find(o => o.GetComponent<IngredientID>().id == ingredient3Id);

        if (ing1 != null && ing2 != null && ing3 != null)
        {
            // Spawn potion at center of the zone
            Instantiate(potionPrefab, transform.position, Quaternion.identity);

            // Destroy ingredients
            Destroy(ing1);
            Destroy(ing2);
            Destroy(ing3);

            ingredientsInZone.Remove(ing1);
            ingredientsInZone.Remove(ing2);
            ingredientsInZone.Remove(ing3);
        }
    }
}