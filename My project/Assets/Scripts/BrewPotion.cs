using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class PotionCrafter : MonoBehaviour
{
    public List<RecipeSO> recipes;
    private List<IngredientStats> ingredientsInZone = new List<IngredientStats>();
    private List<GameObject> objectsInZone = new List<GameObject>();
    public AudioSource microwaveDing;

    void OnTriggerEnter(Collider other)
    {
        Tags tags = other.GetComponent<Tags>();
        IngredientPickup pickup = other.GetComponent<IngredientPickup>();

        // Debug.Log("hit: " + other.name + " | pickup: " + pickup + " | ingredient: " + pickup?.ingredient);
        if (tags != null && tags.HasTag("mashed") && pickup != null)
        {
            ingredientsInZone.Add(pickup.ingredient);
            objectsInZone.Add(other.gameObject);
            TryBrew();
        }
    }

    void OnTriggerExit(Collider other)
    {
        IngredientPickup pickup = other.GetComponent<IngredientPickup>();
        if (pickup != null)
            ingredientsInZone.Remove(pickup.ingredient);
            objectsInZone.Remove(other.gameObject);
    }

    void TryBrew()
    {
        if (ingredientsInZone.Count <= 2) return; // don't even try with 1 ingredient

        foreach (RecipeSO recipe in recipes)
        {
            if (RecipeMatches(recipe))
            {
                // foreach (StatRequirement requirement in recipe.requiredStats)
                // {
                //     Debug.Log(requirement.statName + "" + requirement.requiredValue);
                // }

                Brew(recipe);
                microwaveDing.Play();
                return;
            }
        }
    }

    void Brew(RecipeSO recipe)
    {
        GameObject potion = Instantiate(recipe.potionPrefab, transform.position, Quaternion.identity);
        potion.GetComponent<PotionInstance>().recipeSO = recipe;
        foreach (GameObject obj in objectsInZone)
            Destroy(obj);
        ingredientsInZone.Clear();
        objectsInZone.Clear();
    }


    bool RecipeMatches(RecipeSO recipe)
    {
        foreach (StatRequirement req in recipe.requiredStats)
        {
            int total = GetTotalStat(req.statName);
            // Debug.Log(req.statName + "" + total);
            // keep into account negative states; RYAN make sure this logic is sound
            if (total < req.requiredValue && req.requiredValue > 0)
                return false;
            else if (total > req.requiredValue && req.requiredValue < 0)
                return false;
        }
        return true;
    }

    int GetTotalStat(string statName)
    {
        statName = statName.ToLower();
        int total = 0;
        foreach (IngredientStats ingredient in ingredientsInZone)
        {
            total += statName switch
            {
                "wisdom" => ingredient.wisdom,
                "creativity" => ingredient.creativity,
                "energy" => ingredient.energy,
                "health" => ingredient.health,
                "satiation" => ingredient.satiation,
                "heroism" => ingredient.heroism,
                "horny" => ingredient.horny,
                "sexy" => ingredient.sexy,
                "strength" => ingredient.strength,
                "joy" => ingredient.joy,
                "loud" => ingredient.loud,
                "hot" => ingredient.hot,
                "dehydration" => ingredient.dehydration,
                "constipation" => ingredient.constipation,
                "peace" => ingredient.peace,
                "furry" => ingredient.furry,
                "bug" => ingredient.bug,
                "drunk" => ingredient.drunk,
                "ancient" => ingredient.ancient,
                "glow" => ingredient.glow,
                "clean" => ingredient.clean,
                "claustrophobic" => ingredient.claustrophobic,
                _ => 0
            };
        }
        return total;
    }
}