using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class GoalManager : MonoBehaviour
{
    private Color color;
    private Text text;

    // Start is called before the first frame update
    void Start()
    {
        text = GameObject.FindGameObjectWithTag("UIText").GetComponent<Text>();
    
        float rand = Random.Range(0, 3);
        int x = Mathf.FloorToInt(rand);

        if(x == 0)
        {
            color = Color.red;
        }
        else if (x == 1)
        {
            color = Color.blue;
        }
        else if (x == 2)
        {
            color = Color.green;
        }

        this.gameObject.GetComponent<Renderer>().material.color = color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.GetComponent<Renderer>().material.color == this.GetComponent<Renderer>().material.color)
        {
            text.text = "You Won!";
            
            collision.collider.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
    }
}
