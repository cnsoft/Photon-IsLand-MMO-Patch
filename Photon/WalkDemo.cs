using Photon.MmoDemo.Client;
using UnityEngine;
using System.Collections;
using System.Text;

public class WalkDemo : MonoBehaviour
{
	private Game engine;
	// Use this for initialization
	void Start ()
	{
		Debug.Log(" walk demo start");	

	}
	
	public void Initialize(Game engine)
	{
		this.engine = engine;
	}
	
	void OnGUI()
	{
		if (GUILayout.Button("SayHi"))
			//call rpc...
		{	//in some on GUI. post event.
			string rpccmd = string.Format("{0},{1},{2}",0,"SayHi",this.name);
			this.engine.Avatar.SetRpc(Encoding.ASCII.GetBytes(rpccmd));//BitConverter.GetBytes(rpccmd));
		}	
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}

