using UnityEngine;

[CreateAssetMenu(fileName = "SpawnEnemyTable", menuName = "Scriptable Objects/SpawnEnemyTable")]
public class SpawnEnemyTable : ScriptableObject
{
    public SpawnTable[] Table;
}

[System.Serializable]
public class SpawnTable
{
    public EnemyEnums.EnemyID[] Table;
}
