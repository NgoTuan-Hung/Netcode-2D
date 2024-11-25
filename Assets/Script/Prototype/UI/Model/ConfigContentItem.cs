using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConfigContentItem", menuName = "ScriptableObjects/ConfigContentItem", order = 1)]
public class ConfigContentItem : ScriptableObject
{
    public enum ItemType {ConfigSlider, ConfigDropdown, ConfigCheckbox}
    public ItemType itemType;
    public string sliderName;
    public string dropdownName;
    public string checkboxName;
    public List<string> dropdownOptions;
}
