using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ActionDatabase", menuName = "RPG/Action Database")]
public class ActionDatabase : ScriptableObject {
    public List<ActionData> Actions = new();
}