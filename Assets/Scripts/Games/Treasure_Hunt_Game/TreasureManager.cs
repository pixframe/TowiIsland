using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureManager : MonoBehaviour {

    GameObject[] Variations = new GameObject[3];
    GameObject[] Hider = new GameObject[2];
    TreasureHuntManager manager;

    int indexNumber;
    int typeOfObject;

    void Start()
    {
        manager = FindObjectOfType<TreasureHuntManager>();
        typeOfObject = GetTheTypeOfThe();
        Debug.Log("group type is " + typeOfObject);
    }

    public void Activater(int index)
    {
        indexNumber = index;
        FillTheVariations();
        Variations[indexNumber].SetActive(true);
    }

    //this will return the information to know what is happening
    public int[] GetTheInfo()
    {
        return new int[] { typeOfObject, indexNumber };
    }

    void FillTheVariations()
    {
        for (int i = 0; i < Variations.Length; i++)
        {
            Variations[i] = transform.GetChild(i).gameObject;
            Variations[i].SetActive(false);
        }
        for (int i = 3; i < transform.childCount; i++)
        {
            Hider[i-3] = transform.GetChild(i).gameObject;
            Hider[i-3].SetActive(false);
        }
    }

    public int GetTheTypeOfThe()
    {
        return int.Parse(char.ToString(transform.name[transform.name.Length - 9]));
    }

    void OnCollisionEnter(Collision collision)
    {
        GetComponent<Rigidbody>().isKinematic = true;
        CreateBengal();
    }

    void CreateBengal()
    {
        ParticleSystem bengal = Instantiate(manager.bengal, transform).GetComponent<ParticleSystem>();
        bengal.transform.localPosition = Vector3.zero;
        bengal.Play();
    }
}
