using UnityEngine;

public class ManagerData : MonoSingleton<ManagerData> {
    private static DataBundleMaster _master;
    
    [Header("References")]
    public DataBundleMaster masterBundleToUse;
    
    // --- WEAPON DATA ---
    
    private static DataBundleWeapons Weapons => _master.weapons;

    public static DataWeaponTap[] Taps => Weapons.Taps;
    public static DataWeaponSwipe[] Swipes => Weapons.Swipes;
    public static DataWeaponShake[] Shakes => Weapons.Shakes;

    public static DataWeaponTap GetTap(int index) => Taps[index];
    public static DataWeaponSwipe GetSwipe(int index) => Swipes[index];
    public static DataWeaponShake GetShake(int index) => Shakes[index];
    
    public static DataWeaponTap DefaultTap => _master.defaultTap;
    public static DataWeaponSwipe DefaultSwipe => _master.defaultSwipe;
    public static DataWeaponShake DefaultShake => _master.defaultShake;
    
    public static DataWeaponTap FirstTap => Weapons.FirstTap;
    public static DataWeaponSwipe FirstSwipe => Weapons.FirstSwipe;
    public static DataWeaponShake FirstShake => Weapons.FirstShake;
    
    public static bool HasWeaponAtLevel(int level, out DataWeapon weapon) => Weapons.HasWeaponAtLevel(level, out weapon);
    
    // --- ZOMBIE DATA ---
    
    public static DataBundleZombies Zombies => _master.zombies;
    
    public static DataZombie GetZombie(string zombieName) => Zombies.GetZombie(zombieName);
    
    // --- PERK DATA ---
    
    private static DataBundlePerks Perks => _master.perks;
    
    public static DataPerkPlayer[] PlayerPerks => Perks.Perks;
    public static DataPerkPlayer GetPerk(int index) => PlayerPerks[index];
    public static int GetRandomPerkIndex() => Random.Range(0, PlayerPerks.Length);

    // --- AUDIO DATA ---
    
    public static DataBundleAudio DataBundleAudio => _master.dataBundleAudio;

    public static DataAudioGroup GetAudioGroup(SoundType type) => DataBundleAudio.GetGroup(type);
    public static AudioClip GetAudioClip(SoundType type, int index) => DataBundleAudio.GetSound(type, index);
    
    // ------ START METHODS ------
    
    protected override void OnAwake() {
        DontDestroyOnLoad(this);
        _master = masterBundleToUse;
    }
    

    protected override void OnDestroyed(bool isDeletedInstance) { }
}