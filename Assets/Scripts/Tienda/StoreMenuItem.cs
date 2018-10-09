using UnityEngine;
using System.Collections;

public class StoreMenuItem : MonoBehaviour {
	public enum SlotSize {Small, Big};
	public enum MenuType {Item, Category,Empty};
	public string title;
	public string menuId;
	public string tag;
	public Sprite hoverSprite;
	public SlotSize size;
	public MenuType type;
	public float xOffset;
	public float yOffset;
	public int price;
	public bool owned=false;
	Sprite normalSprite;
	SpriteRenderer spriteRend;
	SpriteRenderer backColor;
	bool hover=false;
	bool active=false;
	public bool ignore=false;
	Vector3 originalPos;
	Vector3 lastPos;
	float distance=0;
	TextMesh text;
	MenuControl menuRef;
	// Use this for initialization
	void Awake () {
		spriteRend = GetComponent<SpriteRenderer> ();
		normalSprite = spriteRend.sprite;
		text = transform.Find ("Title").GetComponent<TextMesh> ();
		text.text = "";
		backColor = transform.Find ("BackColor").GetComponent<SpriteRenderer> ();
	}

	void Start()
	{
		GameObject temp = GameObject.Find ("Menu");
		menuRef = temp.GetComponent<MenuControl> ();
	}
	// Update is called once per frame
	void Update () {
		if(hover)
		{
			distance=Vector3.Distance(originalPos,transform.position);
			if(distance<1)
			{
				transform.Translate(Vector3.back*Time.deltaTime*4,Space.World);
				lastPos=transform.position;
			}
			if(Input.GetMouseButtonDown(0))
			{
				switch (type)
				{
					case MenuType.Category:
						menuRef.LoadCategoryItems(title,menuId,size);
					break;
					case MenuType.Item:
						if(owned)
							menuRef.CreateClothing(menuId,tag);
						else
							menuRef.ShowConfirmation(menuId,tag,price,this);
					break;
					case MenuType.Empty:
						menuRef.DeleteClothing(tag);
					break;
				}
			}
		}else{
			if(active)
			{
				if(Vector3.Distance(lastPos,transform.position)<distance)
				{
					transform.Translate(Vector3.forward*Time.deltaTime*4,Space.World);
				}else
				{
					transform.position=new Vector3(originalPos.x,transform.position.y,originalPos.z);
					active=false;
				}
			}
		}
	}

	void OnMouseOver()
	{
		if(!menuRef.showConfirmation&&!menuRef.itemsTransition&&!active)
		{
			if (!ignore && !hover) {
				if(!active)
				{
					originalPos = transform.position;
				}
				hover = true;
				active = true;
				spriteRend.sprite = hoverSprite;
				if(type!=MenuType.Empty&&!owned)
				{
					backColor.enabled = true;
					if(type==MenuType.Item)
						text.text = price.ToString();
					else
						text.text=title;
				}
			}
		}
	}

//	void OnMouseEnter()
//	{
//		if(!ignore)
//		{
//			if(!active)
//			{
//				originalPos = transform.position;
//			}
//			hover = true;
//			active = true;
//			spriteRend.sprite = hoverSprite;
//			backColor.enabled = true;
//			text.text = title;
//		}
//	}

	public void DeactivateObject()
	{
		ignore = true;
		hover = false;
		spriteRend.sprite = normalSprite;
		backColor.enabled = false;
		text.text = "";
	}

	public void HideKiwi()
	{
		transform.Find ("Kiwi").gameObject.SetActive (false);
	}

	void OnMouseExit()
	{	
		if(!ignore)
		{
			hover = false;
			spriteRend.sprite = normalSprite;
			backColor.enabled = false;
			text.text = "";
		}
	}
}
