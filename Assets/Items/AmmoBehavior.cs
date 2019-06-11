using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBehavior : ItemBehavior
{

    public float fireRate = 0.1f;
    public float damage = 1f;
    public int crosshairType = -1;

    public void setValues() {
        switch (this.type) {
            case Item.Type.LaserBlue:
                fireRate = 0.1f;
                damage = 1f;
                crosshairType = -1;
                break;

            case Item.Type.LaserRed:
                fireRate = 0.3f;
                damage = 3f;
                crosshairType = 0;
               
                break;
        }
    }

    // Start is called before the first frame update


    // Update is called once per frame

}
