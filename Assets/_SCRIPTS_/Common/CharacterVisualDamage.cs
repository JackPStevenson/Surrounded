using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterVisualDamage : MonoBehaviour, IUpdateCustom {
    [Serializable]
    public struct Limb{
        // --- LIMBS ---
        public GameObject[] parts;
        public Transform PartTransforms(int index) => parts[index].transform;
        
        public Vector3 Pos { get => PartTransforms(0).position; set => PartTransforms(0).position = value; }
        public Vector3 PosLocal { get => PartTransforms(0).localPosition; set => PartTransforms(0).localPosition = value; }
        public Quaternion Rot { get => PartTransforms(0).rotation; set => PartTransforms(0).rotation = value; }
        public Quaternion RotLocal { get => PartTransforms(0).localRotation; set => PartTransforms(0).localRotation = value; }
        
        public float Size { get => PartTransforms(0).localScale.x; set => PartTransforms(0).localScale = new Vector3(value, value, value); }
    
        // ------ HELPER FUNCTIONS ------
    
        public void Toggle(bool isEnabled) {
            parts[0].SetActive(isEnabled);
            Size = isEnabled ? 1 : 0;
            for (int i = 1; i < parts.Length; i++)
                PartTransforms(i).gameObject.SetActive(isEnabled);
        }
        
        public void Copy(Limb other) {
            Pos = other.Pos;
            Rot = other.Rot;
            for (int i = 1; i < parts.Length; i++)
                PartTransforms(i).localRotation = other.PartTransforms(i).localRotation;
        }
    }
    
    // --- LIMB REFERENCES ---
    public Limb[] limbs;
    public Limb[] gibs;
    private Vector3[] _gibSpeeds;
    private bool[] _gibsEjected;
    
    // --- STATE VARS ---
    protected float currentHealth = 1;

    // ------ START FUNCTIONS ------
    
    private void Start() {
        foreach (var limb in limbs) limb.Toggle(true);
        foreach (var gib in gibs) gib.Toggle(false);
        _gibSpeeds = new Vector3[gibs.Length];
        _gibsEjected = new bool[gibs.Length];
        
        ChangeHealth(1);
    }
    
    // ------ UPDATE FUNCTIONS ------

    private void FixedUpdate() {
        FixedUpdateCustom(Time.fixedDeltaTime, 0);
    }

    public void UpdateCustom(float deltaTime) { }
    
    public void FixedUpdateCustom(float deltaTime, int tick) { 
        for(int i = 0; i < gibs.Length; i++)
            if(_gibsEjected[i]) UpdateGib(i, deltaTime);
    }

    private void UpdateGib(int i, float deltaTime) {
        if (Mathf.Approximately(gibs[i].Size, 0)) return;
        
        // Update pos and size of each gib.
        gibs[i].Pos += _gibSpeeds[i] * deltaTime;
        gibs[i].Size = Mathf.MoveTowards(gibs[i].Size, 0, deltaTime * 0.175f);
        gibs[i].Rot *= Quaternion.Euler(new Vector3(15, 35, -15) * (deltaTime * 3));
        
        // Update gib speed after transforms.
        _gibSpeeds[i] += (Physics.gravity * deltaTime * 0.25f);
    }

    // ------ EVENT FUNCTIONS ------

    public void ChangeHealth(float newHealth) {
        // Update health to given value.
        currentHealth = newHealth;
        Debug.Log(1);
        Debug.Log(gibs.Length);
        
        // Loop through each gib.
        for (int i = 0; i < gibs.Length; i++) {
            // Only continue if gib's state is different from its ideal state.
            bool shouldBeEjected = currentHealth <= ((float)(i) / gibs.Length);
            if (_gibsEjected[i] == shouldBeEjected) continue;
            _gibsEjected[i] = shouldBeEjected;
            
            // Toggle limb and its gib based on its ideal state and copy limb's transforms to gib's.
            gibs[i].Toggle(shouldBeEjected);
            limbs[i].Toggle(!shouldBeEjected);
            gibs[i].Copy(limbs[i]);
            gibs[i].PartTransforms(0).SetParent(shouldBeEjected ? null : transform);
            
            // Set gib's speed based on whether it should be moving.
            if (shouldBeEjected) {
                Vector2 HorizRand = Random.insideUnitCircle * .7f;
                float VertRand = Random.Range(2, 3f);
                _gibSpeeds[i] = new Vector3(HorizRand.x, VertRand, HorizRand.y);
            }
            else {
                _gibSpeeds[i] = Vector3.zero;
            }
        }
    }

    public void OnDestroy() { foreach (var gib in gibs) Destroy(gib.parts[0]); }
}