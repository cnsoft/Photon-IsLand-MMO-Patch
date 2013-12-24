using UnityEngine;
using System.Collections;


/// <summary>
/// Room player. implement logic method in Room
/// </summary>

public class RoomEntityRPCS : Photon.MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		GameObject.DontDestroyOnLoad(this);		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(!photonView.isMine)
		{
			Debug.Log("notify remote i am living");
			//photonView.RPC("Polo",PhotonTargets.MasterClient);//e.g: request only. 
		}	
	}
	
#region RPCMethods
	[RPC]
    void Marco()
    {
        if (!this.enabled)
        {
            return;
        }

        Debug.Log("Call Marco");

        //this.audio.clip = marco;
        //this.audio.Play();
    }

    [RPC]
    void Polo()
    {
        if (!this.enabled)
        {
            return;
        }

        Debug.Log("Call Polo");

        //this.audio.clip = polo;
        //this.audio.Play();
    }
	[RPC]
	void ResetToStone()
	{
		if (photonView.isMine)
			//HardCode to hide local room Player. 2013-12-18			
			return;
		//Move Remote Player's Proxy to Stone
		GameObject stoneObject = GameObject.FindGameObjectWithTag( "PlayerStone" );
		if (stoneObject!=null){
			transform.position = stoneObject.transform.position;
			transform.rotation = stoneObject.transform.rotation;
			transform.position[0]+= Random.Range(-50,50);
		}//
		Debug.Log("move player to stone position ");
		//should also notify remote change position later?		
	}
	
	[RPC]
	//ScenePhotonView.RPC("BindRemoteProxy",this.gameObject,_params);
	void BindRemoteProxy(Hashtable _params)
	{
		string tarObjId = (string)_params[0];
		int tarPvid = (int)_params[1];
		
		Debug.Log("To bind pv id "+ tarPvid +" to object by guid " + tarObjId);
		
		//who should bind with which pvid
		GameObject[] coms = GameObject.FindGameObjectsWithTag("Monster");
		foreach(GameObject com in coms)
			{
				GuidProperty guid = (GuidProperty) com.GetComponent<GuidProperty>();
				if (guid ==null)
					continue;
				//if null do nothing
				string objid = guid.objectid;
				if(objid == tarObjId)
				{
					//find local gameobject. next assign pv id to it.
					//manual alloc pvid. 
					PhotonView pv = (PhotonView) com.GetComponent<PhotonView>();
					if (pv==null)
					{
						//attached one by hand
						com.gameObject.AddComponent(typeof(PhotonView));
						pv = (PhotonView) com.GetComponent<PhotonView>();
					}
					pv.viewID = tarPvid;
				 //Do disable monster script locally. since you should be mirror mode.	
				 Monster m = com.GetComponent<Monster>();
				 if (m)
				 	m.enabled = false;
				 //
				 Robot  r = com.GetComponent<Robot>();
				 if (r)
				    r.enabled = false;	
				//
				break;
				}
			}
		//done...
	}
	
	
	[RPC]
	void LoadPlayerData(Hashtable data)
	{
		if (photonView.isMine)
		//Local no need load data,since we use player(in Main prefab) 2013-12-18
		return;
		Debug.Log("To load PlayerData,e.g: Weapon,paperdoll,");
		
	}	
	
	[RPC]
	/// <summary>
	/// Proxies the RPC to owner.
	/// </summary>
	/// <param name='data'>	
	/// Data.
	/// Sender,MethodName,MethodParams
	/// </param>
	void ProxyRPC(Hashtable data)
	{
		object[] args = data[(byte)1] as object[];
		//PhotonStream stream = new PhotonStream(false,contents);	
		//Common RPC Proxy Method. Used to trigger local function by remote. e.g: showModel(xx) walk() ..
		//Todo: convert steam to **args .
		string methodname = (string)data[(byte)0];
		Debug.Log(" proxy remote rpc to local method");
		//Custom call works with hard code (int) since  HealthChange only has 1 paramter 
		//SendMessage(methodname,(int)args[0],SendMessageOptions.DontRequireReceiver);
		//		
		SendMessage(methodname,args,SendMessageOptions.DontRequireReceiver);
		//
	}
	
#endregion	
}

