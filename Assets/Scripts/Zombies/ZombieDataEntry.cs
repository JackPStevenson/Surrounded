using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Zombie Data Entry")]
public class ZombieDataEntry : ScriptableObject {
    [Header("References")]
    public GameObject visualPrefab;

    [Header("General")]
    public float maxHealth;
    public float speed;
    public float damage;
    
    [Header("Spawning")]
    public int minimumSpawnWave;
    public float spawnWeight;
}
