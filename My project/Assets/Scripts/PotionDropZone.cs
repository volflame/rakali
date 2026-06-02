using UnityEngine;
using Yarn.Unity;
using System.Linq;

public class PotionDropZone : MonoBehaviour
{
	public DialogueRunner dialogueRunner;
	public NPCBehavior npc;
	public string giveDialogueNode = "BandLeader_GivePotion";

	private GameObject pendingPotion;

	void Start()
	{
		dialogueRunner.AddCommandHandler("confirmGive", ConfirmGive);
		dialogueRunner.AddCommandHandler("cancelGive", CancelGive);
	}

	private void OnTriggerEnter(Collider other)
	{
		Tags tags = other.GetComponent<Tags>();
		if (tags != null && tags.HasTag("potion"))
		{
			if (dialogueRunner.IsDialogueRunning)
				return;

			PotionInstance potionInstance = other.GetComponent<PotionInstance>();
			if (potionInstance == null) return;

			string quality = npc.GetQualityPublic(potionInstance.recipeSO);
			dialogueRunner.VariableStorage.SetValue("$potion_quality", quality);

			pendingPotion = other.gameObject;

			Rigidbody rb = other.GetComponent<Rigidbody>();
			if (rb != null)
				rb.isKinematic = true;

			dialogueRunner.StartDialogue(giveDialogueNode);
		}
	}

	public void ConfirmGive()
	{
		if (pendingPotion != null)
		{
			Destroy(pendingPotion);
			pendingPotion = null;
		}
	}

	public void CancelGive()
	{
		if (pendingPotion != null)
		{
			Rigidbody rb = pendingPotion.GetComponent<Rigidbody>();
			if (rb != null)
				rb.isKinematic = false;
			pendingPotion = null;
		}
	}
}