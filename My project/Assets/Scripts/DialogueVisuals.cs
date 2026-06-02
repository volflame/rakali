using UnityEngine;
using Yarn.Unity;
using System.Collections.Generic;

public class DialogueVisuals : MonoBehaviour
{
	[System.Serializable]
	public class CharacterSprites
	{
		public string characterName;
		public SpriteRenderer portrait;
		public string[] spriteNames;
		public Sprite[] sprites;
	}

	[SerializeField] private CharacterSprites[] characters;

	private DialogueRunner dialogueRunner;

	void Start()
	{
		dialogueRunner = GetComponent<DialogueRunner>();
		dialogueRunner.AddCommandHandler<string, string>("setsprite", SetSprite);
	}

	void SetSprite(string characterName, string spriteName)
	{
		foreach (CharacterSprites character in characters)
		{
			if (character.characterName == characterName)
			{
				for (int i = 0; i < character.spriteNames.Length; i++)
				{
					if (character.spriteNames[i] == spriteName)
					{
						character.portrait.sprite = character.sprites[i];
						return;
					}
				}
				Debug.LogWarning("Sprite not found: " + spriteName + " for character: " + characterName);
				return;
			}
		}
		Debug.LogWarning("Character not found: " + characterName);
	}
}