using UnityEngine;
using System.Collections;

public class ListaItem : ScriptableObject {
    public Bestiario[] bestiario;
    public WeaponInfo[] armas;
    public SwordInfo[] espadas;
}

[System.Serializable]
public class Bestiario {
    public string nombreClave;
    public string[] nombres;

    public Color[] colorCuerpo;
    public Sprite[] spriteCara;
    public Sprite[] spriteMascara;

    [Range(0, 10)]
    public int ataque, fortaleza, proteccion, mana, energia, agilidad;
    
    [Range(0, 3)]
    public float porcVida, porcDaño;
}

[System.Serializable]
public class ItemInfoBase {
    public string nombre;
    public bool disponible = true;
    public Sprite icono;
}

[System.Serializable]
public class WeaponInfo : ItemInfoBase {
    public GameObject weaponPrefab;
    
}

[System.Serializable]
public class SwordInfo : ItemInfoBase {
    public Sprite[] spriteArma;
}