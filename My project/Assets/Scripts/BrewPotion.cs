using System.Collections.Generic;
using UnityEngine;

public class PotionCrafter : MonoBehaviour
{
    public List<RecipeSO> recipes;
    private List<IngredientStats> ingredientsInZone = new List<IngredientStats>();

    void OnTriggerEnter(Collider other)
    {
        Tags tags = other.GetComponent<Tags>();
        IngredientPickup pickup = other.GetComponent<IngredientPickup>();

        if (tags != null && tags.HasTag("mashed") && pickup != null)
        {
            ingredientsInZone.Add(pickup.ingredient);
            TryBrew();
        }
    }

    void OnTriggerExit(Collider other)
    {
        IngredientPickup pickup = other.GetComponent<IngredientPickup>();
        if (pickup != null)
            ingredientsInZone.Remove(pickup.ingredient);
    }

    void TryBrew()
    {
        foreach (RecipeSO recipe in recipes)
        {
            if (RecipeMatches(recipe))
            {
                Brew(recipe);
                return;
            }
        }
    }

    void Brew(RecipeSO recipe)
    {
        Instantiate(recipe.potionPrefab, transform.position, Quaternion.identity);
        ingredientsInZone.Clear();
    }


    bool RecipeMatches(RecipeSO recipe)
    {
        foreach (StatRequirement req in recipe.requiredStats)
        {
            int total = GetTotalStat(req.statName);
            if (total < req.requiredValue)
                return false;
        }
        return true;
    }

    int GetTotalStat(string statName)
    {
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