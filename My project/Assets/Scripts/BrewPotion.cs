using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using Yarn.Unity;
using System;

public class PotionCrafter : MonoBehaviour
{
    public List<RecipeSO> recipes;
    private List<IngredientStats> ingredientsInZone = new List<IngredientStats>();
    private List<GameObject> objectsInZone = new List<GameObject>();
    public AudioSource microwaveDing;
    // private Dictionary<int, RecipeSO> stats = new Dictionary<int, RecipeSO>();
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
        {
            ingredientsInZone.Remove(pickup.ingredient);
            objectsInZone.Remove(other.gameObject);
        }
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
        // This is kinda weird
        // basically some combined stat potions are tieing with other combined stat potions
        // solution is to choose randomly but to weigh the great outcome potions higher (75%) so they are easier to get
        List<RecipeSO> combinedMatches = recipeMatches.Where(r => r.combined).ToList();

        if (combinedMatches.Count > 0)
        {
            HashSet<string> priorityNames = new HashSet<string> { "FreedomPotion", "FullRecoveryPotion", "FursonaManifestationPotion" };
            
            List<RecipeSO> priorityMatches = combinedMatches.Where(r => priorityNames.Contains(r.potionPrefab.name)).ToList();
            List<RecipeSO> normalMatches = combinedMatches.Where(r => !priorityNames.Contains(r.potionPrefab.name)).ToList();

            RecipeSO chosen;

            if (priorityMatches.Count == 0 || priorityMatches.Count == combinedMatches.Count)
            {
                // All normal, or all priority (priority vs priority) — even odds
                chosen = combinedMatches[UnityEngine.Random.Range(0, combinedMatches.Count)];
            }
            else
            {
                // At least one priority and at least one normal — boost priority to 75% combined
                // e.g. 1 priority + 1 normal: priority=75%, normal=25%
                // e.g. 2 priority + 1 normal: each priority=37.5%, normal=25%
                float priorityShare = 0.75f;
                float normalShare = 0.25f;
                float priorityPerRecipe = priorityShare / priorityMatches.Count;
                float normalPerRecipe = normalShare / normalMatches.Count;

                float roll = UnityEngine.Random.value; // 0.0 to 1.0
                float cursor = 0f;

                chosen = priorityMatches[0];

                bool picked = false;
                foreach (RecipeSO r in priorityMatches)
                {
                    cursor += priorityPerRecipe;
                    if (roll < cursor) { chosen = r; picked = true; break; }
                }
                if (!picked)
                {
                    foreach (RecipeSO r in normalMatches)
                    {
                        cursor += normalPerRecipe;
                        if (roll < cursor) { chosen = r; break; }
                    }
                }
            }

            Brew(chosen);
            return;
        }
        RecipeSO bestRecipe = null;
        int bestTotal = -1;
        List<RecipeSO> tiedRecipes = new List<RecipeSO>();

        foreach (RecipeSO r in recipeMatches)
        {
            if (r.combined) continue;
            int total = Math.Abs(r.requiredStats.Sum(s => GetTotalStat(s.statName)));
            if (total > bestTotal)
            {
                bestTotal = total;
                tiedRecipes.Clear();
                tiedRecipes.Add(r);
            }
            else if (total == bestTotal)
            {
                tiedRecipes.Add(r);
            }
        }

        if (tiedRecipes.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, tiedRecipes.Count);
            Brew(tiedRecipes[index]);
        }
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
        // if recipe is combined, don't find the max, just choose the combined potion
        // if recipe is uncombined, find the max
        foreach (StatRequirement req in recipe.requiredStats)
        {
            int total = GetTotalStat(req.statName);
            // RYAN this will override certain thigns if they have the same stat
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