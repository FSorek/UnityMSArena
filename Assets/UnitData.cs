using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit Data", menuName = "Units/New Unit")]
public class UnitData : ScriptableObject {

    public enum ArmorType
    {
        Biological,
        Mechanical,
        Magical,
        Metal,
        Mutated
    }
    public enum ArmorWeight
    {
        Unarmored,
        Light,
        Heavy,
        Medium
    }

    //Unit's info:
    public string Name; //the name of the unit that will be displayd when it is selected.
    public string Code; //unique code for each unit that is used to identify it in the system.
    public string Category; //the category that this unit belongs to.
    public ArmorType UnitArmorType;
    public ArmorWeight UnitArmorWeight;
    public int UnitArmorPoints;
    public string Description; //the description of the unit that will be displayed when it is selected.
    public Sprite Icon; //the icon that will be displayed when the unit is selected.

    public float Speed = 10.0f; //The unit's movement speed.
    public bool CanRotate = false; //can the unit rotate? 
    public float UnitHeight = 1.0f; // This will always be the position on the y axis for this unit.
    public bool FlyingUnit = false; //is the unit flying or walking on the normal terrain? 
}
