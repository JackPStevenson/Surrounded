using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu(fileName = "Data Zombie", menuName = "Data/Zombie")]
public class DataZombie : ScriptableObject {
    [Header("References")]
    public GameObject visualPrefab;

    [Header("Body")]
    public float health;
    
    [Header("Movement")]
    public float speed;
    
    [Header("Attacking")]
    public float attackDamage = 1;
    public float attackRange = 1;
    [FormerlySerializedAs("attackInterval")] public float attackRate = 1.2f;
    
    [Header("Spawning")]
    public int minimumSpawnWave;
    public float spawnWeight;
}
