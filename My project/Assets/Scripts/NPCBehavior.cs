using System.Collections;
using System.Collections.Generic;
using TMPro;
// using UnityEditor.Callbacks;
using UnityEngine;
using Yarn.Unity;
using System.Linq;

public class NPCBehavior : MonoBehaviour
{
    [System.Serializable]
    public class PotionResponseEntry
    {
        public RecipeSO recipeSO;
        // public PotionQuality quality;
        public PotionType quality;
    }

    public enum PotionQuality { Good, Neutral, Bad }
    public enum PotionType {FursonaManifestation, Enlightenment, FullRecovery, FoodComa, Freedom, Sustenance, Wakefulness, Drowsiness, Arousal, Intelligence, Peacefulness, Constipation, Joy, Furry, Creativity}
    public DialogueRunner dialogueRunner;
    public List<PotionResponseEntry> potionResponses;
    public string endingNodeName = "BandLeader_Ending"; // set per NPC in Inspector
    public string yarnStartNode = "NPC_DefaultNode"; // set per NPC in Inspector
    public string yarnCheckNode = "NPC_CheckNode"; // set per NPC in Inspector
    public GameObject normalBackground;
    public GameObject internalBackground;

    void Start()
    {
        dialogueRunner.AddCommandHandler("lock", () => PlayerMovement.instance.LockMovement());
        dialogueRunner.AddCommandHandler("unlock", () => PlayerMovement.instance.UnlockMovement());
        dialogueRunner.AddCommandHandler("setInternal", () => {
            normalBackground.SetActive(false);
            internalBackground.SetActive(true);
        });
        dialogueRunner.AddCommandHandler("setNormal", () => {
            normalBackground.SetActive(true);
            internalBackground.SetActive(false);
        });
    }
    void OnCollisionEnter(Collision collision)
    {

        Tags tags = collision.gameObject.GetComponent<Tags>();
        if (tags != null)
        {
            if (tags.HasTag("potion")) // doing a rly stupid tag workaround b/c unity doesn't do this natively bruh
            {
                // TextMeshPro text = GetComponentInChildren<TextMeshPro>();
                // text.text = "Wow, thanks dude, that really hits the spot!";
                PotionInstance potionInstance = collision.gameObject.GetComponent<PotionInstance>();
                string quality = GetQuality(potionInstance.recipeSO);
                dialogueRunner.VariableStorage.SetValue("$potion_quality", quality);
                Destroy(collision.gameObject);
                dialogueRunner.StartDialogue(endingNodeName);
                // Rigidbody rb = GetComponent<Rigidbody>();
                // rb.isKinematic = false;
                // rb.AddForce(Vector3.forward * 10f, ForceMode.Impulse);
                // StartCoroutine(Dance());
            }
        }
    }

    private string GetQuality(RecipeSO so)
    {
        var entry = potionResponses.FirstOrDefault(p => p.recipeSO == so);
        if (entry == null) return "neutral";
        return entry.quality.ToString().ToLower();
    }

    public void OnChecked()
    {
        if (dialogueRunner.IsDialogueRunning)
            return;

        dialogueRunner.StartDialogue(yarnCheckNode);
    }

    public void OnClicked()
    {
        if (dialogueRunner.IsDialogueRunning)
        {
            return;
        }
        dialogueRunner.StartDialogue(yarnStartNode);
    }

    /// <summary>
    /// Figure this one out. Supposed to make the sprite dance relative to player.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Dance()
    {
        for (int i = 0; i < 5; i++)
        {
            transform.Rotate(180f, 0f, 0f);
            yield return new WaitForSeconds(1.5f);
        }
        yield return null;
    }
}


