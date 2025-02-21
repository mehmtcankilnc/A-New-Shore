using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TalkingNpc : MonoBehaviour
{
    public enum NPCType
    {
        WelcomerNpc,
        QuestGiverNpc
    }

    public NPCType npcType;

    //Common things
    public bool isTalkingWithPlayer = false;
    public Animator animator;
    private TextMeshProUGUI dialogText;
    private Button optionBtn;
    private TextMeshProUGUI optionText;

    //Quest Giver NPC things
    public List<Quest> quests;
    public Quest currentActiveQuest = null;
    public int activeQuestIndex = 0;
    public bool firstIntWithQuestGiver = true;
    public int currentQuest;

    //Welcomer NPC things
    public List<Chat> chats;
    public Chat currentActiveChat = null;
    public int currentChatIndex = 0;
    public bool firstIntWithWelcomer = true;
    public int currentChat;

    private AudioSource tempAudioSourceObj;

    private void Start()
    {
        dialogText = DialogSystem.Instance.dialogText;
        optionBtn = DialogSystem.Instance.optionBtn;
        optionText = DialogSystem.Instance.optionBtn.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
        tempAudioSourceObj = gameObject.AddComponent<AudioSource>();
    }

    public void InteractWithNpc()
    {
        isTalkingWithPlayer = true;
        animator.SetTrigger("talk");
        LookAtPlayer();

        if (npcType == NPCType.QuestGiverNpc)
        {
            StartConversationWithQuestGiver();
        }
        else if (npcType == NPCType.WelcomerNpc)
        {
            StartConversationWithWelcomer();
        }
    }

    private void StartConversationWithWelcomer()
    {
        if (firstIntWithWelcomer)
        {
            firstIntWithWelcomer = false;
            currentActiveChat = chats[currentChatIndex];
            StartChatInitialDialog();
            currentChat = 0;
        }
        else
        {
            DialogSystem.Instance.OpenDialogUI();

            dialogText.text = currentActiveChat.comebackChat;
            tempAudioSourceObj.clip = currentActiveChat.comebackAudioClip;
            tempAudioSourceObj.Play();
            optionText.text = "Close";
            optionBtn.onClick.RemoveAllListeners();
            optionBtn.onClick.AddListener(() =>
            {
                if (tempAudioSourceObj != null && tempAudioSourceObj.isPlaying)
                {
                    tempAudioSourceObj.Stop();
                    tempAudioSourceObj.clip = null;
                }
                DialogSystem.Instance.CloseDialogUI();
                isTalkingWithPlayer = false;
            });
        }
    }

    private void StartChatInitialDialog()
    {
        DialogSystem.Instance.OpenDialogUI();

        dialogText.text = currentActiveChat.npcTexts[currentChat];
        tempAudioSourceObj.clip = currentActiveChat.npcAudioClips[currentChat];
        tempAudioSourceObj.Play();
        optionText.text = "Next";
        optionBtn.onClick.RemoveAllListeners();
        optionBtn.onClick.AddListener(() => {
            if (tempAudioSourceObj != null && tempAudioSourceObj.isPlaying)
            {
                tempAudioSourceObj.Stop();
                tempAudioSourceObj.clip = null;
            }
            SetPlayerTexts();
        });
    }

    private void SetPlayerTexts()
    {
        dialogText.text = currentActiveChat.playerTexts[currentChat];
        tempAudioSourceObj.clip = currentActiveChat.playerAudioClips[currentChat];
        tempAudioSourceObj.Play();
        optionText.text = "Next";
        optionBtn.onClick.RemoveAllListeners();
        optionBtn.onClick.AddListener(() =>
        {
            if (tempAudioSourceObj != null && tempAudioSourceObj.isPlaying)
            {
                tempAudioSourceObj.Stop();
                tempAudioSourceObj.clip = null;
            }
            currentChat++;
            animator.SetTrigger("talk");
            CheckIfChatDone();
        });
    }

    private void CheckIfChatDone()
    {
        if (currentChat == currentActiveChat.playerTexts.Count - 1)
        {
            dialogText.text = currentActiveChat.npcTexts[currentChat];
            tempAudioSourceObj.clip = currentActiveChat.npcAudioClips[currentChat];
            tempAudioSourceObj.Play();
            optionText.text = "Next";
            optionBtn.onClick.RemoveAllListeners();
            optionBtn.onClick.AddListener(() => {
                if (tempAudioSourceObj != null && tempAudioSourceObj.isPlaying)
                {
                    tempAudioSourceObj.Stop();
                    tempAudioSourceObj.clip = null;
                }
                SetLastPlayerText();
            });
        }
        else
        {
            dialogText.text = currentActiveChat.npcTexts[currentChat];
            tempAudioSourceObj.clip = currentActiveChat.npcAudioClips[currentChat];
            tempAudioSourceObj.Play();
            optionText.text = "Next";
            optionBtn.onClick.RemoveAllListeners();
            optionBtn.onClick.AddListener(() => {
                if (tempAudioSourceObj != null && tempAudioSourceObj.isPlaying)
                {
                    tempAudioSourceObj.Stop();
                    tempAudioSourceObj.clip = null;
                }
                SetPlayerTexts();
            });
        }
    }

    private void SetLastPlayerText()
    {
        dialogText.text = currentActiveChat.playerTexts[currentChat];
        tempAudioSourceObj.clip = currentActiveChat.playerAudioClips[currentChat];
        tempAudioSourceObj.Play();
        optionText.text = "Close";
        optionBtn.onClick.RemoveAllListeners();
        optionBtn.onClick.AddListener(() =>
        {
            if (tempAudioSourceObj != null && tempAudioSourceObj.isPlaying)
            {
                tempAudioSourceObj.Stop();
                tempAudioSourceObj.clip = null;
            }
            DialogSystem.Instance.CloseDialogUI();
            isTalkingWithPlayer = false;
        });
    }

    private void StartConversationWithQuestGiver()
    {
        if (firstIntWithQuestGiver)
        {
            firstIntWithQuestGiver = false;
            currentActiveQuest = quests[activeQuestIndex];
            StartQuestInitialDialog();
            currentQuest = 0;
        }
        else
        {
            if (currentActiveQuest.accepted && currentActiveQuest.isCompleted == false)
            {
                if (AreQuestRequirementsCompleted())
                {
                    DialogSystem.Instance.OpenDialogUI();

                    dialogText.text = currentActiveQuest.info.comebackCompleted;
                    tempAudioSourceObj.clip = currentActiveQuest.info.comebackCompletedAudioClip;
                    tempAudioSourceObj.Play();
                    optionText.text = "Take Reward";
                    optionBtn.onClick.RemoveAllListeners();
                    optionBtn.onClick.AddListener(() => {
                        if (tempAudioSourceObj != null && tempAudioSourceObj.isPlaying)
                        {
                            tempAudioSourceObj.Stop();
                            tempAudioSourceObj.clip = null;
                        }
                        ReceiveRewardAndCompleteQuest();
                    });
                }
                else
                {
                    DialogSystem.Instance.OpenDialogUI();

                    dialogText.text = currentActiveQuest.info.comebackInProgress;
                    tempAudioSourceObj.clip = currentActiveQuest.info.comebackInProgressAudioClip;
                    tempAudioSourceObj.Play();
                    optionText.text = "Close";
                    optionBtn.onClick.RemoveAllListeners();
                    optionBtn.onClick.AddListener(() => {
                        if (tempAudioSourceObj != null && tempAudioSourceObj.isPlaying)
                        {
                            tempAudioSourceObj.Stop();
                            tempAudioSourceObj.clip = null;
                        }
                        DialogSystem.Instance.CloseDialogUI();
                        isTalkingWithPlayer = false;
                    });
                }
            }

            if (currentActiveQuest.isCompleted == true)
            {
                DialogSystem.Instance.OpenDialogUI();

                dialogText.text = currentActiveQuest.info.finalWords;

                optionText.text = "[Close]";
                optionBtn.onClick.RemoveAllListeners();
                optionBtn.onClick.AddListener(() => {
                    DialogSystem.Instance.CloseDialogUI();
                    isTalkingWithPlayer = false;
                });
            }

            if (currentActiveQuest.initialDialogCompleted == false)
            {
                StartQuestInitialDialog();
            }
        }
    }

    private void ReceiveRewardAndCompleteQuest()
    {
        currentActiveQuest.isCompleted = true;

        var coinsRecieved = currentActiveQuest.info.coinReward;
        print("You recieved " + coinsRecieved + " gold coins");
        OrderUIManager.Instance.IncreaseCoin(coinsRecieved);

        if (currentActiveQuest.info.rewardItem != "")
        {
            QuickSlotSystem.Instance.AddToQuickSlot(currentActiveQuest.info.rewardItem);
            QuickSlotSystem.Instance.SetEquippedModel(currentActiveQuest.info.rewardItem);
        }

        activeQuestIndex++;

        if (activeQuestIndex < quests.Count)
        {
            currentActiveQuest = quests[activeQuestIndex];
            currentQuest = 0;
            DialogSystem.Instance.CloseDialogUI();
            isTalkingWithPlayer = false;
        }
        else
        {
            DialogSystem.Instance.CloseDialogUI();
            isTalkingWithPlayer = false;
        }
    }

    private bool AreQuestRequirementsCompleted()
    {
        int requiredAmount = currentActiveQuest.info.firstRequirementAmount;

        if (PlayerInfo.Instance != null && PlayerInfo.Instance.playerLevel >= requiredAmount)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void StartQuestInitialDialog()
    {
        DialogSystem.Instance.OpenDialogUI();

        dialogText.text = currentActiveQuest.info.initialDialog[currentQuest];
        tempAudioSourceObj.clip = currentActiveQuest.info.dialogAudioClips[currentQuest];
        tempAudioSourceObj.Play();
        optionText.text = "Next";
        optionBtn.onClick.RemoveAllListeners();
        optionBtn.onClick.AddListener(() => {
            if (tempAudioSourceObj != null && tempAudioSourceObj.isPlaying)
            {
                tempAudioSourceObj.Stop();
                tempAudioSourceObj.clip = null;
            }
            currentQuest++;
            animator.SetTrigger("talk");
            CheckIfDialogDone();
        });
    }

    private void CheckIfDialogDone()
    {
        if (currentQuest == currentActiveQuest.info.initialDialog.Count - 1)
        {
            dialogText.text = currentActiveQuest.info.initialDialog[currentQuest];
            tempAudioSourceObj.clip = currentActiveQuest.info.dialogAudioClips[currentQuest];
            tempAudioSourceObj.Play();
            currentActiveQuest.initialDialogCompleted = true;

            optionText.text = currentActiveQuest.info.acceptOption;
            optionBtn.onClick.RemoveAllListeners();
            optionBtn.onClick.AddListener(() => {
                if (tempAudioSourceObj != null && tempAudioSourceObj.isPlaying)
                {
                    tempAudioSourceObj.Stop();
                    tempAudioSourceObj.clip = null;
                }
                currentActiveQuest.accepted = true;

                DialogSystem.Instance.CloseDialogUI();
                isTalkingWithPlayer = false;
            });
        }
        else
        {
            dialogText.text = currentActiveQuest.info.initialDialog[currentQuest];
            tempAudioSourceObj.clip = currentActiveQuest.info.dialogAudioClips[currentQuest];
            tempAudioSourceObj.Play();
            optionText.text = "Next";
            optionBtn.onClick.RemoveAllListeners();
            optionBtn.onClick.AddListener(() => {
                if (tempAudioSourceObj != null && tempAudioSourceObj.isPlaying)
                {
                    tempAudioSourceObj.Stop();
                    tempAudioSourceObj.clip = null;
                }
                currentQuest++;
                animator.SetTrigger("talk");
                CheckIfDialogDone();
            });
        }
    }

    public void LookAtPlayer()
    {
        var player = GameObject.FindWithTag("Player").transform;
        Vector3 direction = player.position - transform.position;
        transform.rotation = Quaternion.LookRotation(direction);

        var yRotation = transform.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}