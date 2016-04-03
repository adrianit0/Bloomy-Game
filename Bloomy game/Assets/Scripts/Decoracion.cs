using UnityEngine;
using System.Collections;

public class Decoracion : MonoBehaviour {

    public Sprite[] sprites = new Sprite[2];
    public int framePerSecond = 5;
    int actualFrame = 0;

    SpriteRenderer _render;

    void Awake () {
        _render = GetComponent<SpriteRenderer>();
    }

    void Start () {
        if(framePerSecond > 0 && sprites.Length > 0)
            InvokeRepeating("SpriteChange", 0, 1 / (float)framePerSecond);
    }

    void SpriteChange() {
        actualFrame++;
        if(actualFrame >= sprites.Length)
            actualFrame = 0;

        _render.sprite = sprites[actualFrame];
    }
}
