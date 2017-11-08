using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMusicConversationAction : ConversationAction
{
    public string songName;

	public override IEnumerator Execute()
	{
        BackgroundMusicManager.instance.StartPlaying(songName);
		yield break;
	}
}
