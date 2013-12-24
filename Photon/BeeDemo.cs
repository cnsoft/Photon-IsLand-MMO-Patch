using UnityEngine;
using System.Collections;

public class BeeDemo : Photon.MonoBehaviour {

	// Use this for initialization
	void Start () {	
		if (!PhotonNetwork.isMasterClient)
			return;
		EventManager.instance.addEventListener("HealthChange",this.gameObject,"onHealthChangedEvt");		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	//Handle evt with arguments.
	void onHealthChangedEvt(CustomEvent evt)
	{
		this.onHealthChanged((int) evt.arguments[0]);		
	}
	
	[RPC]
	void onHealthChanged(params object[] args)
	{
		int nvalue = (int)args[0];
		//Notify other clients healthchanged.
		if (PhotonNetwork.isMasterClient)
		{
			//GetComponent<PhotonView>().RPC("onHealthChanged",PhotonTargets.Others, nvalue);
			//ProxyRPC build params[] to stream.
			PhotonStream stream = new PhotonStream(true,null);
			stream.SendNext(nvalue);
			//Use HashTable
			object[] objs = stream.ToArray();
			Hashtable _params = new Hashtable();
			_params[(byte)0] = "onHealthChanged";
			_params[(byte)1] = objs;			
			GetComponent<PhotonView>().RPC("ProxyRPC",PhotonTargets.Others,_params);
			//offical : hot to pass more parameters?
			//testing...
		}
		else 
		{
			Debug.Log(" got remote notify health changed "+ nvalue);
			//got remote health change notify. forward event to local.
			EventManager.instance.dispatchEvent(new CustomEvent("HealthChange"));
		}	
	}
	
	[RPC]
	void onDied()
	{
		//
		
	}
	
}
