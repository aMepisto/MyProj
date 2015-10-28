// UTF8 & LF (유티에프:)

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MakeNumberComma : MonoBehaviour 
{    
    public InputField _inputField;
	// Use this for initialization
	void Start () 
	{
        //_text = this.gameObject.GetComponentInChildren<Text>();
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
