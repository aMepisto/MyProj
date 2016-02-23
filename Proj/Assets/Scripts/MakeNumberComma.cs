// UTF8 & LF (유티에프:)

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MakeNumberComma : MonoBehaviour 
{    
    public InputField _inputField;
	// Use this for initialization

    int[] test = new int[2]{1,2};
	void Start () 
	{
        //_text = this.gameObject.GetComponentInChildren<Text>();
        foreach (var a in test)
        {
            Debug.LogError("a : " + a);
        }
	}

    void OnGUI()
    {
        if (GUILayout.Button("test"))
        {
            var a = 1234.567;
            var data = a.ToString("N");
            _inputField.text = data;
        }
    }

	// Update is called once per frame
	void Update () 
	{

        
	}
}
