﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{
    int selectedWeapon = 0;

    void Start() {
        selectWeapon();
    }

    void Update() {
        int previousSelectedWeapon = selectedWeapon;
        if (Input.GetAxis("Mouse ScrollWheel") > 0.0f) {
            if (selectedWeapon >= transform.childCount - 1)
                selectedWeapon = 0;
            else
                selectedWeapon++;
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0.0f) {
            if (selectedWeapon <= 0)
                selectedWeapon = transform.childCount - 1;
            else
                selectedWeapon--;
        }
        if (previousSelectedWeapon != selectedWeapon) {
            selectWeapon();
        }

    }



    void selectWeapon() {
        int i = 0;
        foreach (Transform weapon in transform) {
            weapon.gameObject.SetActive(i == selectedWeapon);
            i++;
        }
    }
}
