using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System;

public class Script : MonoBehaviour, ISceneLoadDependency
{
	public int cachedScriptTTLSeconds = 10;
	public string scriptURL;
	public string scriptResourcePath;
    public bool loadFromWeb = true;
    public bool loadCachedFile = true;
    public bool loadFromStreamingAssets = true;

	string script;

	public static Script instance;

	bool isReadyForLoad = false;

	Coroutine runningConversation;

	// Conversations that make up the scenario.
	Dictionary<string, Conversation> conversations = new Dictionary<string, Conversation>();

	// State variables
	enum ProcessingState
	{
		Normal,
        CharacterIntroduction,
		EntityIsSpeaking,
		CreatingOptionMenu
	}

	ProcessingState currentState = ProcessingState.Normal;

	string speakingCharacterName = "";
	Conversation currentConversation;
	ShowOptionsConversationAction currentOptionsAction;
    EaseInCharacterConversationAction characterIntroductionAction;

	// PATTERNS
    // Block Pattern
    Regex rScriptBlock = new Regex(@"\s*""\s*([a-zA-Z0-9]+)\s*""\s*\{\s*([\s\S]+?)\}\s*", RegexOptions.IgnoreCase | RegexOptions.Multiline);
    // Comment pattern
    Regex rComment = new Regex(@"^\s*#+.*$", RegexOptions.IgnoreCase);

    // Single-line Patterns
	Regex rBackgroundImageChange = new Regex(@"^\s*""\s*(.*?)\s*""\s*background\s*$", RegexOptions.IgnoreCase);
    Regex rBackgroundMusicChange = new Regex(@"^\s*""\s*(.*?)\s*""\s*music\s*$", RegexOptions.IgnoreCase);
    Regex rConversationLink = new Regex(@"^\s*go\s*to\s*""(.*?)""\s*$", RegexOptions.IgnoreCase);
    Regex rCharacterPoseChange = new Regex(@"^\s*""(.*?)""\s*poses\s*""(.*?)""\s*$", RegexOptions.IgnoreCase);
    Regex rLoadScene = new Regex(@"^\s*load\s+scene\s*""(.*?)""\s*$", RegexOptions.IgnoreCase);
    Regex rCharacterLeaves = new Regex(@"^\s*""(.*?)""\s*exits\s*$", RegexOptions.IgnoreCase);

    // Multi-line Patterns
    // Character introduction patterns:
    Regex rCharacterIntroduction = new Regex(@"^\s*""(.*?)""\s*enters\s*(\s+as\s*"".*?""){0,1}\s*$", RegexOptions.IgnoreCase);
    Regex rCharacterIntroduction_StartPose = new Regex(@"^\s*\+\s*pose\s*""(.*?)""\s*$", RegexOptions.IgnoreCase);
    Regex rCharacterIntroduction_Position = new Regex(@"^\s*\+\s*position\s+(left|right|center)\s*$", RegexOptions.IgnoreCase);
    Regex rCharacterIntroduction_ExtractAlias = new Regex(@"^\s*as\s*""(.*?)""\s*$", RegexOptions.IgnoreCase);

    // Character speaking patterns:
    Regex rCharacterWillSpeak = new Regex(@"^\s*""(.*?)""\s*says\s*$", RegexOptions.IgnoreCase);
    Regex rWriteParagraph = new Regex(@"^\s*\+\s*(.*?)\s*$", RegexOptions.IgnoreCase);

    // Options block patterns:
    Regex rStartOptionsMenu = new Regex(@"^\s*options\s*$", RegexOptions.IgnoreCase);
    Regex rAddOption = new Regex(@"^\s*\+\s*(.*?)\s*=>\s*""(.*?)""\s*$", RegexOptions.IgnoreCase);

    Regex rDialogImageChange = new Regex(@"^\s*~\s*use\s+dialog\s+background\s*""\s*(.*?)\s*""\s*$", RegexOptions.IgnoreCase);

	void Awake()
	{
		instance = this;
		StartCoroutine(SetupScript());
	}

	void Update()
	{
		if (   currentConversation != null && 
            (  Input.GetKeyDown(KeyCode.Return)
			|| Input.GetKeyDown(KeyCode.Space)
            || Input.GetKeyDown(KeyCode.E) ) )
		{
            currentConversation.Command("continue");
		}
	}

	public bool IsReadyForLoad()
	{
		return isReadyForLoad;
	}

	public void StartConversation(string startId)
	{
		runningConversation = StartCoroutine(StartScenario(startId));
	}

	public void StopConversation()
	{
		if (runningConversation != null)
			StopCoroutine(runningConversation);
	}

	IEnumerator StartScenario(string startId)
	{
		string nextConversation = startId;
		while (nextConversation != "")
		{
			Debug.Log("<color=green>Running conversation: " + nextConversation + "</color>");
			currentConversation = conversations[nextConversation];
			yield return currentConversation.Execute();
			nextConversation = currentConversation.nextConversation;
		}
	}

	IEnumerator SetupScript()
	{
		// Load the script from web or disk.
		yield return LoadScript();

		// Parse the text.
		HandleFullText(script);

		isReadyForLoad = true;
	}

	IEnumerator LoadScript()
	{
        if (loadFromStreamingAssets)
        {
            script = ReadFromStreamingAssets();
            Debug.Log("Script:: Loaded script from StreamingAssets.");
            yield break;
        }

		string cachedScriptPath = GetCachedScriptPath();
		Debug.LogFormat("Script:: Looking for script at path {0}.", cachedScriptPath);
		bool useCachedFile = false;

        if (loadCachedFile)
        {
    		// First check if we have a cached file.
    		if (File.Exists(cachedScriptPath))
    		{
                Debug.LogFormat("Script:: Found cached script!");
    			script = File.ReadAllText(cachedScriptPath);
    			useCachedFile = true;

    			// Then check if the file is current.
    			FileInfo fi = new FileInfo(cachedScriptPath);
    			if (fi.LastWriteTime.AddSeconds(cachedScriptTTLSeconds) < DateTime.Now)
    			{
                    Debug.LogFormat("Script:: Cached script is older than {0} second(s), will try to re-download!", cachedScriptTTLSeconds);
    			}

    			// If it's current, use this.
    			else
    			{
                    Debug.LogFormat("Script:: Cached script still fresh, will use this version!");
    				yield break;
    			}
    		}
        }
        else
        {
            Debug.LogFormat("Script:: Not looking for cached file, overrideUseCachedFile is false.");
        }

        if (loadFromWeb)
        {
    		// If we don't have a cached file, try to download from the web.
            Debug.LogFormat("Script:: Attempting to download script from {0}.", scriptURL);
    		WWW www = new WWW(scriptURL);

    		yield return www;

    		// If we got an error, or the text is emtpy:
    		if (!string.IsNullOrEmpty(www.error) && string.IsNullOrEmpty(www.text))
    		{
                Debug.LogFormat("Script:: Unable to download script, got error: {0}.", www.error);
    		}
    		else
    		{
                Debug.LogFormat("Script:: Successfully downloaded script from web!");
    			script = www.text;

                Debug.LogFormat("Script:: Attempting to write to the cache file.");
    			File.WriteAllText(cachedScriptPath, script);
    			yield break;
    		}
        }
        else
        {
            Debug.LogFormat("Script:: Will not try to load script from web, overrideLoadFromWeb is false.");
        }

		// If that doesn't work, and we have read in an expired cached script.
		// just use the expired cached script anyway, it's probably newer than the
		// file that's loaded with the build.
		if (useCachedFile)
		{
            Debug.LogFormat("Script:: Using the expired cached script.");
			yield break;
		}
		else
		{
            Debug.LogFormat("Script:: Loaded script from internal resources.");
			script = Resources.Load<TextAsset>(scriptResourcePath).text;
			yield break;
		}
	}

    string ReadFromStreamingAssets()
    {
        var path = GetStreamingAssetsPath();
        Debug.LogFormat("Script:: Attempting to load script from StreamingAssets path {0}", path);
        return File.ReadAllText(path);
    }

	string GetCachedScriptPath()
	{
		return Application.persistentDataPath + Path.AltDirectorySeparatorChar + "CachedScript.dat";
	}

    string GetStreamingAssetsPath()
    {
        return Application.dataPath + "/StreamingAssets/" + scriptResourcePath + ".txt";
    }

	void HandleFullText(string text)
	{
        
        Match m = rScriptBlock.Match(text);
        // Handle each script block (conversation):
		while (m.Success) 
		{
			string conversationId = m.Groups[1].Value;
			string conversationContent = m.Groups[2].Value;
			Debug_ParseResult("Parsing conversation: " + conversationId);
			HandleConversation(conversationContent, conversationId);
			m = m.NextMatch();
		}
	}

	void HandleConversation(string body, string id)
	{
		Conversation conversation = new Conversation();

        // Make sure we start in the Normal state.
        if (currentState != ProcessingState.Normal)
        {
            ReturnToNormalState("Conversation ended, returning to Normal state.");
            currentState = ProcessingState.Normal;
        }

		// Handle each line:
		string[] lines = body.Split('\n');
		for (int i = 0; i < lines.Length; ++i)
		{
			string line = lines[i];
			if (line.Length == 0 || line.Trim().Length == 0) continue;
			Debug.Log("<color=red>Processing line: " + line + "</color>");

			switch (currentState)
			{
    			case ProcessingState.Normal:
    				{
    					// Note: use of 'if' this way is to take advantage of short-circuit
    					// evaluation so it doesn't evaluate every single pattern if an earlier
    					// pattern matches a line. Organized by estimated order of frequency.
    					if ( false
    						|| Handle_CharacterWillSpeak(line, conversation)    // Block start
    						|| Handle_CharacterPoseChange(line, conversation)
    						|| Handle_StartOptionsMenu(line, conversation)      // Block start
    						|| Handle_CharacterWillLeave(line, conversation)
    						|| Handle_ConversationLink(line, conversation)
    						|| Handle_BackgroundImageChange(line, conversation)
    						|| Handle_BackgroundMusicChange(line, conversation)
                            || Handle_CharacterIntroduction(line, conversation) // Block start
    						//|| Handle_DialogImageChange(line, conversation)
    						|| Handle_LoadScene(line, conversation)
    					) {}
                        else if (!IsComment(line))
                        {
                            Debug.LogError("Script:: Invalid syntax at line: " + line);
                        }
    					break;
    				}
                case ProcessingState.CharacterIntroduction:
                    {
                        if ( false
                            || Handle_CharacterIntroduction_StartPose(line, conversation)
                            || Handle_CharacterIntroduction_Position(line, conversation)
                        ) {}
                        else
                        {
                            ReturnToNormalState("Did not understand line while in CharacterIntroduction state. " + line);
                            --i; // Redo line.
                        }
                        break;
                    }
    			case ProcessingState.EntityIsSpeaking:
    				{
    					if (Handle_WriteParagraph(line, conversation)) {}
                        else
                        {
                            ReturnToNormalState("Did not understand line while in EntityIsSpeaking state. " + line);
                            --i; // Redo line.
                        }
    					break;
    				}
    			case ProcessingState.CreatingOptionMenu:
    				{
    					if (Handle_AddOption(line, conversation)) {}
                        else
                        {
                            ReturnToNormalState("Did not understand line while in CreatingOptionMenu state. " + line);
                            --i; // Redo line.
                        }
    					break;
    				}
			}
		}

		conversations.Add(id, conversation);
	}

    bool IsComment(string line)
    {
        return rComment.IsMatch(line);
    }

    #region SINGLE_LINE_PATTERN_HANDLERS

	bool Handle_BackgroundImageChange(string line, Conversation conversation)
	{
		Match m = rBackgroundImageChange.Match(line);
		if (m.Success)
		{
			SetBackgroundConversationAction action = new SetBackgroundConversationAction();
			action.spriteToSet = Resources.Load<Sprite>("Scenario Backgrounds/" + m.Groups[1].Value);
			conversation.AddAction(action);
			Debug_ParseResult(string.Format("Background will change to {0}", m.Groups[1].Value));
			return true;
		}
		return false;
	}

	bool Handle_DialogImageChange(string line, Conversation conversation)
	{
		Match m = rDialogImageChange.Match(line);
		if (m.Success)
		{
			SetDialogBackgroundConversationAction action = new SetDialogBackgroundConversationAction();
			action.spriteToSet = Resources.Load<Sprite>("Dialog Backgrounds/" + m.Groups[1].Value);
			conversation.AddAction(action);
			Debug_ParseResult(string.Format("Dialog background will change to {0}", m.Groups[1].Value));
			return true;
		}
		return false;
	}

	bool Handle_BackgroundMusicChange(string line, Conversation conversation)
	{
		Match m = rBackgroundMusicChange.Match(line);
		if (m.Success)
		{
			SetMusicConversationAction action = new SetMusicConversationAction();
			action.songName = m.Groups[1].Value;
			conversation.AddAction(action);
			Debug_ParseResult(string.Format("Background music will change to {0}", m.Groups[1].Value));
			return true;
		}
		return false;
	}

	bool Handle_ConversationLink(string line, Conversation conversation)
	{
		Match m = rConversationLink.Match(line);
		if (m.Success)
		{
			conversation.nextConversation = m.Groups[1].Value;
			Debug_ParseResult(string.Format("Next conversation set to {0}", m.Groups[1].Value));
			return true;
		}
		return false;
	}

	bool Handle_CharacterPoseChange(string line, Conversation conversation)
	{
		Match m = rCharacterPoseChange.Match(line);
		if (m.Success)
		{
			string characterName = m.Groups[1].Value;
			string characterPose = m.Groups[2].Value;
			ChangeCharacterPoseConversationAction action = new ChangeCharacterPoseConversationAction();
			action.characterName = characterName;
			action.characterPose = characterPose;
			conversation.AddAction(action);
			Debug_ParseResult(string.Format("{0} will change pose to {1}", characterName, characterPose));
			return true;
		}
		return false;
	}

    bool Handle_CharacterWillLeave(string line, Conversation conversation)
    {
        Match m = rCharacterLeaves.Match(line);
        if (m.Success)
        {
            EaseOutCharacterConversationAction action = new EaseOutCharacterConversationAction();
            action.characterAlias = m.Groups[1].Value;
            conversation.AddAction(action);

            Debug_ParseResult(string.Format("Will remove character {0}.", action.characterAlias));

            return true;
        }
        return false;
    }

    bool Handle_LoadScene(string line, Conversation conversation)
    {
        Match m = rLoadScene.Match(line);
        if (m.Success)
        {
            LoadSceneConversationAction action = new LoadSceneConversationAction();
            action.sceneName = m.Groups[1].Value;
            conversation.AddAction(action);

            Debug_ParseResult(string.Format("Setting scene '{0}' to load in future.", action.sceneName));

            return true;
        }
        return false;
    }

    #endregion

    #region CHARACTER_INTRODUCTION_PATTERN
    bool Handle_CharacterIntroduction(string line, Conversation conversation)
    {
        Match m = rCharacterIntroduction.Match(line);
        if (m.Success)
        {
            characterIntroductionAction = new EaseInCharacterConversationAction();
            characterIntroductionAction.characterName = m.Groups[1].Value;
            characterIntroductionAction.characterAlias = characterIntroductionAction.characterName;

            if (m.Groups[2].Success)
            {
                Debug_ParseResult("Looks like character introduction is using alias, attempting to extract. (" + m.Groups[2] + ")");
                Handle_CharacterIntroduction_ExtractAlias(m.Groups[2].Value);
            }

            conversation.AddAction(characterIntroductionAction);
            SwitchState(ProcessingState.CharacterIntroduction);
            Debug_ParseResult(string.Format("{0} will be introduced", characterIntroductionAction.characterName));

            return true;
        }
        return false;
    }

    bool Handle_CharacterIntroduction_ExtractAlias(string line)
    {
        Match m = rCharacterIntroduction_ExtractAlias.Match(line);
        if (m.Success)
        {
            characterIntroductionAction.characterAlias = m.Groups[1].Value;
            Debug_ParseResult("Extracted alias as: (" + characterIntroductionAction.characterAlias + ")");
            return true;
        }
        else
        {
            Debug.LogErrorFormat("Script:: Attempted to extract alias from {0}, but syntax was incorrect.", line);
        }
        return false;
    }

    bool Handle_CharacterIntroduction_StartPose(string line, Conversation conversation)
    {
        Match m = rCharacterIntroduction_StartPose.Match(line);
        if (m.Success)
        {
            characterIntroductionAction.startPose = m.Groups[1].Value;
            Debug_ParseResult(string.Format("> with pose {0}", characterIntroductionAction.startPose));
            return true;
        }
        return false;
    }

    bool Handle_CharacterIntroduction_Position(string line, Conversation conversation)
    {
        Match m = rCharacterIntroduction_Position.Match(line);
        if (m.Success)
        {
            characterIntroductionAction.SetRootPosition(m.Groups[1].Value);
            Debug_ParseResult(string.Format("> at position {0}", characterIntroductionAction.rootPosition.ToString()));
            return true;
        }
        return false;
    }

    #endregion

    #region CHARACTER_SPEAKS_PATTERN

    bool Handle_CharacterWillSpeak(string line, Conversation conversation)
    {
        Match m = rCharacterWillSpeak.Match(line);
        if (m.Success)
        {
            SwitchCharacterConversationAction action = new SwitchCharacterConversationAction();
            action.characterName = speakingCharacterName = m.Groups[1].Value;
            conversation.AddAction(action);
            SwitchState(ProcessingState.EntityIsSpeaking);
            Debug_ParseResult(string.Format("Character {0} will speak.", action.characterName));
            return true;
        }
        return false;
    }

	bool Handle_WriteParagraph(string line, Conversation conversation)
	{
		Match m = rWriteParagraph.Match(line);
		if (m.Success)
		{
			WriteParagraphConversationAction action = new WriteParagraphConversationAction();
			action.message = m.Groups[1].Value;
			action.speakingCharacter = speakingCharacterName;
			conversation.AddAction(action);
			Debug_ParseResult(string.Format("> {0}", action.message));
			return true;
		}
		return false;
	}

    #endregion

    #region OPTIONS_BLOCK_PATTERNS

    bool Handle_StartOptionsMenu(string line, Conversation conversation)
    {
        Match m = rStartOptionsMenu.Match(line);
        if (m.Success)
        {
            currentOptionsAction = new ShowOptionsConversationAction();
            conversation.AddAction(currentOptionsAction);
            SwitchState(ProcessingState.CreatingOptionMenu);
            Debug_ParseResult("Creating options dialog.");
            return true;
        }
        return false;
    }

    bool Handle_AddOption(string line, Conversation conversation)
    {
        Match m = rAddOption.Match(line);
        if (m.Success)
        {
            string optionLabel = m.Groups[1].Value;
            string optionResultConversation = m.Groups[2].Value;
            currentOptionsAction.AddOption(optionLabel, optionResultConversation, conversation);
            Debug_ParseResult(string.Format("Added option {0} => {1}.", optionLabel, optionResultConversation));
            return true;
        }
        return false;
    }

    #endregion

    void SwitchState(ProcessingState nextState)
    {
        Debug_StateChange(currentState, nextState);
        currentState = nextState;
    }

    void ReturnToNormalState(string reason)
    {
        Debug_ParseResult("Switching to normal state because " + reason);
        SwitchState(ProcessingState.Normal);
    }

	void Debug_ParseResult(string response)
	{
		Debug.LogFormat("Script:: <color=orange>{0}</color>", response);
	}

	void Debug_StateChange(ProcessingState current, ProcessingState next)
	{
        Debug.LogFormat("Script:: <color=cyan>State change {0} => {1}</color>", current, next);
	}
}
