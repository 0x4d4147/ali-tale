using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScenarioManager : MonoBehaviour
{
    [Header("Object Dependencies")]
	public Image backgroundImage;
	public TMP_Text dialogText;
	public Image dialogBackground;

	public static ScenarioManager instance;

	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		Script.instance.StartConversation(GameData.instance.startAtConversation);
	}
}
