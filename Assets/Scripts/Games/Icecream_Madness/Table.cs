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
    }

	// Update is called once per frame
	void Update () {

	}

    public void CreateAUpperSprite(Color colorOfSprite, string KindOfSprite)
    {
        GameObject colorShower = new GameObject();
        colorShower.transform.parent = transform.GetChild(0);
        colorShower.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        colorShower.transform.position = colorShower.transform.parent.position;
        SpriteRenderer spr = colorShower.AddComponent<SpriteRenderer>();
        spr.color = colorOfSprite;
        spr.sprite = Resources.Load<Sprite>("IcecreamMadness/Icons/" + KindOfSprite);
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
