using UnityEngine;

/// <summary>
/// A ScriptableObject that holds an NPC's dialogue lines.
/// Create one via: Assets > Create > Dialogue > Dialogue Data
/// </summary>
[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [Header("NPC Info")]
    public string npcName = "Villager";

    [Header("Dialogue Lines")]
    [TextArea(2, 5)]
    public string[] lines;
}