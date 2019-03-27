using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;

public class Table : MonoBehaviour {

    protected bool hasSomethingOn;
    protected SpriteRenderer spriteRenderer;
    protected IcecreamMadnessManager manager;
    protected IcecreamChef chef;

    protected GameObject logo;
    protected GameObject trayOn;
    protected GameObject machine;

    protected UnityEngine.Transform trayPositioner;
    protected UnityEngine.Transform machinePositioner;

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
        ChangeTheColor();
    }

	// Update is called once per frame
	void Update () {

	}

    public void ChangeTableColor()
    {
        if (!name.Contains("C"))
        {
            if (int.Parse(name[1].ToString()) % 2 == 0)
            {
                ChangeTheColor("FFFFFF");
            }
            else
            {
                ChangeTheColor("B9B9B9");
            }
        }
        else
        {
            ChangeTheColor("FFFFFF");
        }
    }

    public void ChangeTableSprite(string direction)
    {
        string tableShape;

        if (gameObject.name.Contains("D") || gameObject.name.Contains("U"))
        {
            tableShape = "Center/";
        }
        else if (gameObject.name.Contains("C"))
        {
            tableShape = "Corner/";
            if (gameObject.name.Contains("1") || gameObject.name.Contains("2"))
            {
                tableShape += "Back/";
            }
            else
            {
                tableShape += "Front/";
            }
        }
        else
        {
            tableShape = "Lateral/";
        }


        string pathOfSprite = $"{FoodDicctionary.prefabSpriteDirection}Table/{tableShape}{direction}";
        spriteRenderer.sprite = Resources.Load<Sprite>(pathOfSprite);
    }

    public void CreateALogo(string spriteName)
    {
        logo = new GameObject();
        logo.transform.parent = trayPositioner;
        logo.transform.localScale = sizeOfUpperSprite;
        logo.transform.position = logo.transform.parent.position;
        logo.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        SpriteRenderer spr = logo.AddComponent<SpriteRenderer>();
        
        spr.sprite = LoadSprite.GetSpriteFromSpriteSheet($"{FoodDicctionary.prefabSpriteDirection}Logos/Logos", spriteName);
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

    public void CreateAMachine(string typeOfMachine)
    {
        machine = Instantiate(Resources.Load<GameObject>($"{FoodDicctionary.prefabGameObjectDirection}{FoodDicctionary.machinesDirection}{typeOfMachine}"));
        machine.transform.parent = trayPositioner;
        machine.transform.position = trayPositioner.transform.position;
        machine.transform.localScale = new Vector3(machine.transform.localScale.x * trayPositioner.transform.localScale.x, machine.transform.localScale.y, machine.transform.localScale.z);

        UnityArmatureComponent armature = machine.GetComponentInChildren<UnityArmatureComponent>();
        armature.sortingOrder = spriteRenderer.sortingOrder + 1;
        armature.animation.Play("Idle");
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

    protected void ChangeTheColor()
    {
        spriteRenderer.color = Color.white;
    }

    protected void ChangeTheColor(string colorHtml)
    {
        var colorToShow = Color.white;
        ColorUtility.TryParseHtmlString("#" + colorHtml, out colorToShow);
        spriteRenderer.color = colorToShow;
    }

    /// <summary>
    /// Sets the tray as a game object on the table
    /// </summary>
    /// <param name="tray"></param>
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
                hasSomethingOn = false;
                trayOn = null;
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

    public void RestoreToOriginal()
    {
        if (logo != null)
        {
            Destroy(logo);
            logo = null;
        }
        if (machine != null)
        {
            Destroy(machine);
            machine = null;
        }
        if (trayOn != null)
        {
            Destroy(trayOn);
            trayOn = null;
        }

        hasSomethingOn = false;
    }
}
