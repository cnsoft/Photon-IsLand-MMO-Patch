using UnityEngine;
using System.Collections;
using Photon;

/// <summary>
/// created by cnsoft 2013-12-17
/// Room entity network. Used to synchronized local and remote entity
/// You can override it and attached to which entity you wanted.
/// </summary>
public class RoomEntityNetwork : Photon.MonoBehaviour,IPhotonDataListener{

	// Use this for initialization
	public Transform owner;
	public string entityType;
	void Start () {
		//after loaded,we reset local pv observe to player's mount_kernel. 2013-12-18 by cnsoft
		//Only used for PaperDoll.
		DontDestroyOnLoad(this.gameObject);
		
		if (entityType == "Player")
			EventManager.instance.addEventListener("onSceneLoaded",this.gameObject,"doResetLocalObserve");
		else
		{
			//Other Monster ? NPC 
			//if (!PhotonNetwork.isMasterClient)
			//	DestroyImmediate(this.gameObject);
			//later MasterClient will use PhotonNetwork.InstantiateSceneObject to create again.
			if (PhotonNetwork.isMasterClient)
			{
				;//
				//int id1 = PhotonNetwork.AllocateViewID();
				//Use Player's ?
				//photonView.RPC("SpawnOnNetwork", PhotonTargets.AllBuffered, this.gameObject.transform.position, this.gameObject.transform.rotation, id1);
				//EventManager.instance.dispatchEvent(new CustomEvent("QueryPVId"));
			}	
		}
	}
	
	public void setOwner(GameObject g)
	{
		owner = g.transform;
	}
	
	void Awake(){
//Todo!!		
//		//In Room Local we focuse on mount_kernel 2013-12-17
//		if(photonView.isMine)
//		{
//			//one local observer object.
//			GameObject m = GameObject.Find("mount_kernel");
//			//this.setOwner( );
//			owner = m.transform;
//			
//		}
	}
	
	void doResetLocalObserve(){
		//In Room Local we focuse on mount_kernel 2013-12-17
		//if(photonView.isMine)
		//if(PhotonNetwork.isMasterClient)//ghost no need change it.
		if(photonView.isMine) //local photon. now i focus on player's mount kernel transform
		{
			//one local observer object to Player. not safe. if more than one mount_kernel in scene.  2013-12-25 
			//GameObject m = GameObject.Find("mount_kernel");
			GameObject m = GameObject.Find("Player/mount_kernel"); //local Player			
			this.setOwner( m );
			
			//Get RobotClass from Player? 
		 	Robot r = m.transform.parent.gameObject.GetComponent<Robot>(); 	
			
			//find NetRobot and call SetRobot to player. 
			NetRobot nr = this.gameObject.GetComponent<NetRobot>();
			nr.SetRobot( r );
			//HardCode. hide local robotPaperdoll 2013-12-18 moved to ResetToStone. (ignore that for locally)
			//ScenePhotonView.RPC("LoadPlayerData",PhotonTargets.Others,new Hashtable());
			//
			Debug.Log(" reset local observe");
		}
	}
	
	//Basic Serialize function. 
	//Maybe we should let game object do it self. 2013-12-25 since different data is needed by different gameobject
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
		Transform transform = owner;
		if (owner!=null)
			transform = owner;
		else			
			return;
		
        if (stream.isWriting)
        {
            //We own this player: send the others our data
            //stream.SendNext((int)controllerScript._characterState);
			Transform _localTS = this.owner.transform;
            stream.SendNext(_localTS.position);
            stream.SendNext(_localTS.rotation); 
        }
        else
        {
            //Network player, receive data
            //controllerScript._characterState = (CharacterState)(int)stream.ReceiveNext();
            correctPlayerPos = (Vector3)stream.ReceiveNext();
            correctPlayerRot = (Quaternion)stream.ReceiveNext();
        }
		
		//Test Player's PhotonHandle.
		//Todo: support animator also. use rpc method. 2013-12-26 
		if(this.owner.name =="mount_kernel")
		{
			if(this.owner.parent)
			{				
				//Monster?
				//NetRobot r = this.owner.parent.gameObject.GetComponent<NetRobot>();
				
				// Go
				// --NetRobot
				// --RoomEntityNetwork
				// --RoomEntityRPCS
				// NetRobot nr = this.gameObject.GetComponent<NetRobot>(); works under the up tree.
				
				NetRobot nr = this.gameObject.GetComponent<NetRobot>();
				if (nr!=null)
					nr.OnPhotonSerializeView(stream,info);
				//write or load.
				Debug.Log("call back serialize for PhotonHandler");
			}	
		}	
		
		
    }

    private Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this
    private Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this
	
	// Update is called once per frame
    void Update()
    {
        if (!photonView.isMine)
        {
            Debug.Log(" sync position with remote copy");
			//Update remote player (smooth this, this looks good, at the cost of some accuracy)
			Transform _localTS = this.owner.transform;
            _localTS.position = Vector3.Lerp(_localTS.position, correctPlayerPos, Time.deltaTime * 5);
            _localTS.rotation = Quaternion.Lerp(_localTS.rotation, correctPlayerRot, Time.deltaTime * 5);
        }		
    }
	
	
	//[RPC]
	//void SpawnOnNetwork(Vector3 pos, Quaternion rot, int id1)
	//{
	//	//GameObject newPlayer = Instantiate(playerPrefab, pos, rot) as GameObject ;
	//	PhotonView[] nViews = this.gameObject.GetComponentsInChildren<PhotonView>();
	//	nViews[0].viewID = id1;
	//}
}
