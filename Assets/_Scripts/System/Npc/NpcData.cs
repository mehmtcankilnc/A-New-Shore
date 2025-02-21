using UnityEngine;

[CreateAssetMenu(fileName = "NewNPC", menuName = "NPC Data")]
public class NpcData : ScriptableObject
{
    public string charName;
    public float moveSpeed;
    public float wanderRadius;
    public float wanderTimer;
    public RuntimeAnimatorController animatorController;
}
