using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSceneConversationAction : ConversationAction
{
    public string sceneName;

    public override IEnumerator Execute()
    {
		Script.instance.StopConversation();
        SceneLoader.instance.LoadSceneAsync(sceneName);
        yield break;
    }
}
