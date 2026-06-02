using UnityEngine;
using Yarn.Unity;

public class PotionDropZone : MonoBehaviour
{
	public DialogueRunner dialogueRunner;
	public NPCBehavior npc;
	public string giveDialogueNode = "BandLeader_GivePotion";

	private GameObject pendingPotion;
	private bool dialogueTriggered = false;

	void Start()
	{
		dialogueRunner.AddCommandHandler("confirmGive", ConfirmGive);
		dialogueRunner.AddCommandHandler("cancelGive", CancelGive);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (dialogueTriggered) return;

		PotionInstance potionInstance = other.GetComponent<PotionInstance>();
		if (potionInstance == null) return;

		// Only trigger if potion is NOT being held — check if it's kinematic
		// but NOT because we made it kinematic (pendingPotion would be set)
		Rigidbody rb = other.GetComponent<Rigidbody>();
		if (rb == null) return;

		// If it's kinematic and not our pending potion, it's being held — ignore
		if (rb.isKinematic && pendingPotion == null) return;

		if (dialogueRunner.IsDialogueRunning) return;

		string quality = npc.GetQualityPublic(potionInstance.recipeSO);
		dialogueRunner.VariableStorage.SetValue("$potion_quality", quality);

		pendingPotion = other.gameObject;
		rb.isKinematic = true;

		dialogueTriggered = true;
		dialogueRunner.StartDialogue(giveDialogueNode);
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.GetComponent<PotionInstance>() != null)
		{
			dialogueTriggered = false;
			if (pendingPotion == other.gameObject)
				pendingPotion = null;
		}
	}

	public void ConfirmGive()
	{
		if (pendingPotion != null)
		{
			Destroy(pendingPotion);
			pendingPotion = null;
		}
		dialogueTriggered = false;
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
		// Don't reset dialogueTriggered here — wait for OnTriggerExit
		// so the player must physically remove the potion first
	}
}