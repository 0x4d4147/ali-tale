using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterManager : MonoBehaviour
{
	public static CharacterManager instance;

	public enum RootPosition
	{
		Left,
		Center,
		Right
	}

	public struct CharacterInfo
	{
		public Image charImage;
		public TMP_Text nameLabel;
		public Image characterLabelBg;
		public GameObject characterLabel;
	}

	Dictionary<string, CharacterInfo> activeCharacters = new Dictionary<string, CharacterInfo>();

	void Awake()
	{
		instance = this;
	}

	public void RemoveCharacter(string characterName)
	{
		if (activeCharacters.ContainsKey(characterName)) {
			var charInfo = activeCharacters[characterName];

			charInfo.characterLabelBg.enabled = false;

			charInfo.nameLabel.text = "";
			charInfo.nameLabel.enabled = false;

			//charInfo.characterLabel.SetActive(false);

			charInfo.charImage.sprite = null;
			charInfo.charImage.enabled = false;

			activeCharacters.Remove(characterName);
		}
	}

	public void LoadPoseForExistingCharacter(string characterName, string characterPose)
	{
        var posePath = string.Format("Character Poses/{0}/{1}", characterName, characterPose);
        if (activeCharacters.ContainsKey(characterName))
        {
            CharacterInfo charInfo = activeCharacters[characterName];
            charInfo.charImage.sprite = Resources.Load<Sprite>(posePath);
            Debug.Log("Attempted to load existing pose at path: " + posePath);
        }
        Debug.Log("Attempted to load existing pose at path (and totally failed): " + posePath);
	}

	public void LoadCharacter(RootPosition rootPosition, string characterName, string pose)
	{
		RemoveCharacter(characterName);

		var posePath = string.Format("Character Poses/{0}/{1}", characterName, pose);

		var charInfo = new CharacterInfo();

		// Load character image.
		switch (rootPosition) {
		case RootPosition.Left:
			{
				charInfo.characterLabel = GameObject.Find("Left Character Name");

				charInfo.charImage = GameObject.Find("Image, Left Character").GetComponent<Image>();
				charInfo.characterLabelBg = charInfo.characterLabel.GetComponent<Image>();
				charInfo.characterLabelBg.enabled = true;

				charInfo.nameLabel = charInfo.characterLabel.GetComponentInChildren<TMP_Text>(true);
				charInfo.nameLabel.text = characterName;
				charInfo.nameLabel.enabled = true;

				charInfo.characterLabel.SetActive(true);

				Debug.Log("Found character image location: " + charInfo.charImage);

				charInfo.charImage.sprite = Resources.Load<Sprite>(posePath);
				charInfo.charImage.enabled = true;

				activeCharacters.Add(characterName, charInfo);
				break;
			}
		case RootPosition.Center:
			{
				charInfo.characterLabel = GameObject.Find("Left Character Name");

				charInfo.charImage = GameObject.Find("Image, Center Character").GetComponent<Image>();
				charInfo.characterLabelBg = charInfo.characterLabel.GetComponent<Image>();
				charInfo.characterLabelBg.enabled = true;

				charInfo.nameLabel = charInfo.characterLabel.GetComponentInChildren<TMP_Text>(true);
				charInfo.nameLabel.text = characterName;
				charInfo.nameLabel.enabled = true;

				charInfo.characterLabel.SetActive(true);

				Debug.Log("Found character image location: " + charInfo.charImage);

				charInfo.charImage.sprite = Resources.Load<Sprite>(posePath);
				charInfo.charImage.enabled = true;

				activeCharacters.Add(characterName, charInfo);
				break;
			}
		case RootPosition.Right:
			{
				charInfo.characterLabel = GameObject.Find("Right Character Name");

				charInfo.charImage = GameObject.Find("Image, Right Character").GetComponent<Image>();
				charInfo.characterLabelBg = charInfo.characterLabel.GetComponent<Image>();
				charInfo.characterLabelBg.enabled = true;

				charInfo.nameLabel = charInfo.characterLabel.GetComponentInChildren<TMP_Text>(true);
				charInfo.nameLabel.text = characterName;
				charInfo.nameLabel.enabled = true;

				charInfo.characterLabel.SetActive(true);

				Debug.Log("Found character image location: " + charInfo.charImage);

				charInfo.charImage.sprite = Resources.Load<Sprite>(posePath);
				charInfo.charImage.enabled = true;

				activeCharacters.Add(characterName, charInfo);
				break;
			}
		}

		Debug.Log("Attempted to load: " + posePath);
	}
}
