using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Day7Node : MonoBehaviour
{
    public string id;
    public int depth;
    public List<Day7Node> children;
    public List<Day7Node> parents;
    public TextMeshPro text;

    public GameObject line;
    public List<LineRenderer> lines;
    private Color color1 = new Color(1, 1, 1, 0.1f);
    private Color color2 = new Color(0.1f, 0.9f, 0.1f, 0.8f);

    public void Constructor(string id)
    {
        this.id = id;
        text = GetComponentInChildren<TextMeshPro>();
        HideText();
    }

    public void SetColor(Color color)
    {
        GetComponent<Renderer>().material.color = color;
    }

    public void SetText(string time)
    {
        text.gameObject.SetActive(true);
        text.text = time;
    }

    public void HideText()
    {
        text.gameObject.SetActive(false);
    }
    
    public void OnDrawGizmos()
    {
        //foreach(Day7Node node in parents)
        //{
        //    Gizmos.color = Color.white;
        //    Gizmos.DrawLine(transform.position, node.transform.position);
        //    Gizmos.color = Color.blue;
        //    Gizmos.DrawLine(transform.position, transform.position + (node.transform.position - transform.position).normalized * 2);
        //    Gizmos.color = Color.red;
        //    Gizmos.DrawLine(node.transform.position, node.transform.position + (transform.position - node.transform.position).normalized * 2);
        //}
    }

    public void RequestUpdate()
    {
        foreach (LineRenderer lr in lines)
        {
            Destroy(lr.material);
            Destroy(lr.gameObject);
        }
        lines.Clear();
        foreach (Day7Node node in parents)
        {
            GameObject newLine = Instantiate(line, transform, false);
            LineRenderer lr = newLine.GetComponent<LineRenderer>();
            lr.startColor = color1;
            lr.endColor = color2;
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, node.transform.position);
            lines.Add(lr);
        }
    }
}
