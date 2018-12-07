using UnityEngine;
using System.Collections;

public class TableTrash : Table
{

    // Use this for initialization
    void Start()
    {
        Initializing();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Initializing()
    {
        base.Initializing();

        spriteRenderer.color = Color.green;
    }
}
