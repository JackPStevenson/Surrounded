using TMPro;
using UnityEngine;


public class UIPerformanceStatDisplay : UIDisplayItem {
    private TMP_Text _bloodText;
    protected TMP_Text Blood => ChildAutoFetch(_bloodText, "Blood");
    
    private TMP_Text _experienceText;
    protected TMP_Text Experience => ChildAutoFetch(_experienceText, "Exp");
    
    // ------ SETUP METHODS ------

    public void SetInfo(PlayerStat stat) => SetInfo(stat.Name, stat.Amount, stat.Blood, stat.Exp);
    public void SetInfo(string statName, int statAmount, int blood, int experience) { gameObject.SetActive(true); SetInfo(statName, statAmount.ToString("N0")); SetBlood(blood); SetExperience(experience); }
    
    
    // ------ SINGLE SETUP METHODS ------

    public void SetBlood(int amount) => Blood?.SetText(amount.ToString("N0"));
    public void SetExperience(int amount) => Experience?.SetText(amount.ToString("N0"));
}