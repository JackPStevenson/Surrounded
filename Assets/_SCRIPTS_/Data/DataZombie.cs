using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu(fileName = "D_Zombie", menuName = "Data/Zombie")]
public class DataZombie : ScriptableObject {
    [Header("References")]
    public GameObject visualPrefab;

    [Header("General")]
    public new string name;
    
    [Header("Body")]
    public float health;
    public float speed;

    [Header("Attacking")]
    public float attackDamage = 1;
    public float attackRange = 1;
    public float attackSpeed = 1.2f;

    [Header("Spawning")]
    public int minimumSpawnWave;
    public float spawnWeight;

    [Header("SFX")]
    public string hurtSound;
    public string attackSound;
    public string deathSound;

    [Header("Death")]
    public int bloodOnDeath;
}
