using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Chat")]
public class Chat : ScriptableObject
{
    [TextArea(5, 10)]
    public List<string> npcTexts;
    public List<AudioClip> npcAudioClips;

    [TextArea(5, 10)]
    public List<string> playerTexts;
    public List<AudioClip> playerAudioClips;

    [TextArea(5, 10)]
    public string comebackChat;
    public AudioClip comebackAudioClip;
}
