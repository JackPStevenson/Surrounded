using TMPro;
using UnityEngine;

public struct UIStat {
    public string Name;
    public int Amount, Blood, Exp;
    
    public UIStat(string name, int amount, int blood, int exp) { Name = name; Amount = amount; Blood = blood; Exp = exp; }
}

public class UIPerformanceStatDisplay : UIDisplayItem {
    private TMP_Text _bloodText;
    protected TMP_Text Blood => ChildAutoFetch(_bloodText, "Blood");
    
    private TMP_Text _experienceText;
    protected TMP_Text Experience => ChildAutoFetch(_experienceText, "Exp");
    
    // ------ SETUP METHODS ------

    public void SetInfo(UIStat stat) => SetInfo(stat.Name, stat.Amount, stat.Blood, stat.Exp);
    public void SetInfo(string statName, int statAmount, int blood, int experience) { gameObject.SetActive(true); SetInfo(statName, statAmount.ToString()); SetBlood(blood); SetExperience(experience); }
    
    
    // ------ SINGLE SETUP METHODS ------

    public void SetBlood(int amount) => Blood?.SetText(amount.ToString());
    public void SetExperience(int amount) => Experience?.SetText(amount.ToString());
}