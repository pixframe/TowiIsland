using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSelector : MonoBehaviour {

    GameSelectorsController controller;

    public Sprite changeColor;

    SpriteRenderer spriteRenderer;
    Sprite originalSprite;
    bool selected = false;
    bool locked;

	// Use this for initialization
	void Start () {
        controller = transform.parent.GetComponent<GameSelectorsController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalSprite = spriteRenderer.sprite;
        ItsPlayable();
	}

    private void OnMouseEnter()
    {
        spriteRenderer.sprite = changeColor;
    }

    private void OnMouseDown()
    {
        selected = true;
        controller.ShowAGameDescription(this.gameObject);
        controller.CallTheGameByName(gameObject.name);
    }

    private void OnMouseExit()
    {
        if (!selected)
        {
            spriteRenderer.sprite = originalSprite;
        }
    }

    //here will be loking for a demo key active if it is it will be playable
    void ItsPlayable() {
        if (FindObjectOfType<DemoKey>())
        {
            locked = false;
        }
        else
        {
            locked = true;
        }
    }
}
