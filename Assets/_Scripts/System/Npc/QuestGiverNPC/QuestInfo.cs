using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "QuestInfo", order = 1)]
public class QuestInfo : ScriptableObject
{
    [TextArea(5, 10)]
    public List<string> initialDialog;
    public List<AudioClip> dialogAudioClips;

    [Header("Options")]
    [TextArea(5, 10)]
    public string acceptOption;
    [TextArea(5, 10)]
    public string comebackInProgress;
    public AudioClip comebackInProgressAudioClip;
    [TextArea(5, 10)]
    public string comebackCompleted;
    public AudioClip comebackCompletedAudioClip;
    [TextArea(5, 10)]
    public string finalWords;

    [Header("Rewards")]
    public int coinReward;
    public string rewardItem;

    [Header("Requirements")]
    public string firstRequirementItem;
    public int firstRequirementAmount;
}