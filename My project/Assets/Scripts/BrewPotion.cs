using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using Yarn.Unity;

public class PotionCrafter : MonoBehaviour
{
    public List<RecipeSO> recipes;
    private List<IngredientStats> ingredientsInZone = new List<IngredientStats>();
    private List<GameObject> objectsInZone = new List<GameObject>();
    public AudioSource microwaveDing;
    private Dictionary<int, RecipeSO> stats = new Dictionary<int, RecipeSO>();
    private List<RecipeSO> recipeMatches = new List<RecipeSO>();
    public DialogueRunner dialogueRunner;
    private bool ratwurstTutorialDone = false;

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
            if (!ratwurstTutorialDone && pickup.ingredient.ingredientName == "Ratwurst")
            {
                Debug.Log("Ratwurst detected, runner null: " + (dialogueRunner == null) + " | already done: " + ratwurstTutorialDone);
                ratwurstTutorialDone = true;
                dialogueRunner.StartDialogue("TutorialManager_PostRatwurst");
            }
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
        recipeMatches.Clear();
        foreach (RecipeSO recipe in recipes)
        {
            if (RecipeMatches(recipe))
            {
                recipeMatches.Add(recipe);
                // foreach (StatRequirement requirement in recipe.requiredStats)
                // {
                //     Debug.Log(requirement.statName + "" + requirement.requiredValue);
                // }
            }
        }
        
        foreach (RecipeSO recipe in recipeMatches)
        {
            if (recipe.combined)
            {
                Brew(recipe);
            }
        }
        RecipeSO maxRecipe = stats[stats.Keys.Max()];
        Brew(maxRecipe);
    }

    void Brew(RecipeSO recipe)
    {
        GameObject potion = Instantiate(recipe.potionPrefab, transform.position, Quaternion.identity);
        potion.GetComponent<PotionInstance>().recipeSO = recipe;
        foreach (GameObject obj in objectsInZone)
            Destroy(obj);
        microwaveDing.Play();
        ingredientsInZone.Clear();
        objectsInZone.Clear();
    }

    // change logic; creates a stat type; a combined state will take precedent over a normal stat no matter what; fursona 2 > horny 3
    // if there is no combined stat, then take the max value and make a potion based off of that
    bool RecipeMatches(RecipeSO recipe)
    {
        stats.Clear();
        // if recipe is combined, don't find the max, just choose the combined potion
        // if recipe is uncombined, find the max
        foreach (StatRequirement req in recipe.requiredStats)
        {
            int total = GetTotalStat(req.statName);
            // RYAN this will override certain thigns if they have the same stat
            stats[total] = recipe;
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