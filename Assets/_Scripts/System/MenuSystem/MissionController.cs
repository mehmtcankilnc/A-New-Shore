using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MissionController : MonoBehaviour
{
    public GameObject img;
    public GameObject text;

    private GameObject headMan;
    private GameObject marketChief;
    private void Start()
    {
        headMan = GameObject.FindWithTag("Head-Man");
        marketChief = GameObject.FindWithTag("TalkingNpc");
    }

    private void Update()
    {
        if (headMan != null && marketChief != null)
        {
            if (!headMan.GetComponent<TalkingNpc>().firstIntWithWelcomer && marketChief.GetComponent<TalkingNpc>().firstIntWithQuestGiver)
            {
                text.GetComponent<TextMeshProUGUI>().text = "Talk with the Market Chief under the statue.";
            }
            else if (!marketChief.GetComponent<TalkingNpc>().firstIntWithQuestGiver)
            {
                img.SetActive(false);
                text.SetActive(false);
            }
        }
    }
}
