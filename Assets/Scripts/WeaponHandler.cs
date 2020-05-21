using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    public enum Action { Equip, Unequip};

    public Transform weapon;
    public Transform weaponHandle;
    public Transform weaponRestPose;

    public void ResetWeapon(Action action)
    {
        if(action == Action.Equip)
        {
            weapon.SetParent(weaponHandle);
        } 
        else
        {
            weapon.SetParent(weaponRestPose);
        }
        weapon.localRotation = Quaternion.identity;
        weapon.localPosition = Vector3.zero;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
