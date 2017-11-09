using UnityEngine;

public class GameData : MonoBehaviour
{
	public string startAtConversation = "1";
	public static GameData instance;

	void Awake()
	{
		instance = this;
	}
}
