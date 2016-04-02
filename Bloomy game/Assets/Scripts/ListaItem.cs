using UnityEngine;
using System.Collections;

public class ListaItem : ScriptableObject {

    public WeaponInfo[] armas;
}

[System.Serializable]
public class WeaponInfo {
    public string nombre;
    public GameObject weaponPrefab;
    public Sprite weaponIcon;
}
