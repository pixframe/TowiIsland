using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour {

    protected bool hasSomethingOn;
    protected SpriteRenderer spriteRenderer;
    protected IcecreamMadnessManager manager;
    protected IcecreamChef chef;

    protected GameObject trayOn;

    protected Transform trayPositioner;
    protected Transform machinePositioner;

    protected Vector3 sizeOfUpperSprite = new Vector3(1f, 1f, 1f);
	// Use this for initialization
	void Start ()
    {
        Initializing();
	}

    public virtual void Initializing()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        manager = FindObjectOfType<IcecreamMadnessManager>();
        chef = FindObjectOfType<IcecreamChef>();
        trayPositioner = transform.GetChild(0);
        machinePositioner = transform.GetChild(1);
    }

	// Update is called once per frame
	void Update () {

	}

    public void ChangeTableSprite(string direction)
    {
        spriteRenderer.sprite = Resources.Load<Sprite>($"{FoodDicctionary.prefabSpriteDirection}Table/{direction}");
    }

    public void CreateALogo(string spriteName)
    {
        GameObject newLogo = new GameObject();
        newLogo.transform.parent = trayPositioner;
        newLogo.transform.localScale = sizeOfUpperSprite;
        newLogo.transform.position = newLogo.transform.parent.position;
        newLogo.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        SpriteRenderer spr = newLogo.AddComponent<SpriteRenderer>();
        Sprite[] sprites = Resources.LoadAll<Sprite>($"{FoodDicctionary.prefabSpriteDirection}Ingredients/Ingredients");
        var dictSprites = new Dictionary<string, Sprite>();

        foreach (Sprite sprite in sprites)
        {
            dictSprites.Add(sprite.name, sprite);
        }

        spr.sprite = dictSprites[spriteName];
        spr.sortingOrder = spriteRenderer.sortingOrder + 1;
    }

    public void CreateAUpperSprite(string KindOfSprite)
    {
        GameObject colorShower = new GameObject();
        colorShower.transform.parent = trayPositioner;   
        colorShower.transform.localScale = sizeOfUpperSprite;
        colorShower.transform.position = colorShower.transform.parent.position;
        SpriteRenderer spr = colorShower.AddComponent<SpriteRenderer>();
        spr.sprite = Resources.Load<Sprite>($"{FoodDicctionary.prefabSpriteDirection}{KindOfSprite}");
        spr.sortingOrder = spriteRenderer.sortingOrder + 1;
    }

    public void CreateAUpperSprite(Color colorOfSprite, string KindOfSprite)
    {
        GameObject colorShower = new GameObject();
        colorShower.transform.parent = transform.GetChild(0);
        colorShower.transform.localScale = sizeOfUpperSprite;
        colorShower.transform.position = colorShower.transform.parent.position;
        SpriteRenderer spr = colorShower.AddComponent<SpriteRenderer>();
        spr.color = colorOfSprite;
        spr.sprite = Resources.Load<Sprite>($"{FoodDicctionary.prefabSpriteDirection}{KindOfSprite}");
        spr.sortingOrder = spriteRenderer.sortingOrder + 1;
        Debug.Log("Finish the creation");
    }

    public bool ItsFill()
    {
        return hasSomethingOn;
    }

    public void MoveToTheTable()
    {
        Debug.Log("its clicked");
        chef.MoveTheChef(this);
    }

    public virtual void DoTheAction()
    {
        if (hasSomethingOn)
        {
            DoTheActionIfTheresSomethingOn();
        }
        else
        {
            DoTheActionIfTheresNothingOn();
        }
    }

    public void OnMouseDown()
    {
        MoveToTheTable();
    }

    protected void ChangeTheColor(string colorHtml)
    {
        var colorToShow = Color.white;
        ColorUtility.TryParseHtmlString("#" + colorHtml, out colorToShow);
        spriteRenderer.color = colorToShow;
    }

    public void SetTray(GameObject tray)
    {
        trayOn = tray;
    }

    public virtual void DoTheActionIfTheresSomethingOn()
    {
        if (!chef.IsHoldingSomething())
        {
            hasSomethingOn = false;
            chef.GrabATray(trayOn);
            trayOn = null;
        }
        else
        {
            if (chef.GetHoldingTray().CanMergeTrays(trayOn.GetComponent<Tray>()))
            {
                Debug.Log("Can merge trays");
                chef.GetHoldingTray().MergeTrays(trayOn.GetComponent<Tray>());
            }
        }
    }

    public virtual void DoTheActionIfTheresNothingOn()
    {
        if (chef.IsHoldingSomething())
        {
            hasSomethingOn = true;
            chef.PutATray(trayPositioner);
        }
    }
}
