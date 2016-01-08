using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
        UILabel lab = transform.Find("Find/Label").GetComponent<UILabel>();

        StartCoroutine(Download("http://quotes.money.163.com/service/chddata.html?code=1000001&start=20151215&end=20151221"));

	}
	
	// Update is called once per frame
	void Update () {
        if (queData.Count > 0)
        {
            transform.Find("Find/Label").GetComponent<UILabel>().text = queData.Dequeue();
        }
	}

    Queue<string> queData = new Queue<string>();
    IEnumerator Download(string url)
    {
        WWW www = new WWW(url);

        yield return www;
        if (www.isDone && www.error == null)
        {
            queData.Enqueue(System.Text.Encoding.Default.GetString(www.bytes));
        }
    }

    IEnumerator UpdateLstCode()
    {
        yield return null;
        AllExistStockID aesid = new AllExistStockID();
        aesid.LoadByNet();
    }

    
    
}
