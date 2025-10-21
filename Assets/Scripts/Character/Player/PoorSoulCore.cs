using UnityEngine;

[RequireComponent(typeof(Health))] [RequireComponent(typeof(StatusHandler))]
public class PoorSoulCore : MonoBehaviour {
    public static PoorSoulCore Instance;
    
    public Health Health { get; private set; }
    public StatusHandler Status { get; private set; }
    public Vector3 Position => Health.Position;
    
    // ------ START METHODS ------
    
    void Awake() {
        Instance = this;
        if(TryGetComponent(out Health h)) Health = h;
        if(TryGetComponent(out StatusHandler s)) Status = s;

        if (!h || !s) {
            enabled = false;
            return;
        }

        Health.OnDeath += Disable;
    }
    
    // ------ UPDATE METHODS ------

    void FixedUpdate() {
        Status.FixedUpdateCustom(Time.fixedDeltaTime);
    }

    public void Disable() {
        gameObject.SetActive(false);
    }
    
    public void Reset() {
        Health.Reset();
        Status.Reset();
    }
    
    public void AddPerk(DataPerkPlayer dataPerkPlayer) {
        print(dataPerkPlayer.perkPrefab);
        Instantiate(dataPerkPlayer.perkPrefab, transform);
    }
}
