using UnityEngine;
using System.Collections;
using Photon;

/// <summary>
/// created by cnsoft 2013-12-17
/// Room entity network. Used to synchronized local and remote entity
/// You can override it and attached to which entity you wanted.
/// </summary>
public class RoomEntityNetwork : Photon.MonoBehaviour{

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
		if(photonView.isMine)
		{
			//one local observer object.
			GameObject m = GameObject.Find("mount_kernel");
			this.setOwner( m);
			//HardCode. hide local robotPaperdoll 2013-12-18 moved to ResetToStone. (ignore that for locally)
			//ScenePhotonView.RPC("LoadPlayerData",PhotonTargets.Others,new Hashtable());
			//
		}
	}
	
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
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
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation); 
        }
        else
        {
            //Network player, receive data
            //controllerScript._characterState = (CharacterState)(int)stream.ReceiveNext();
            correctPlayerPos = (Vector3)stream.ReceiveNext();
            correctPlayerRot = (Quaternion)stream.ReceiveNext();
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
            transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot, Time.deltaTime * 5);
        }		
    }
	
	
	[RPC]
	void SpawnOnNetwork(Vector3 pos, Quaternion rot, int id1)
	{
		//GameObject newPlayer = Instantiate(playerPrefab, pos, rot) as GameObject ;
		PhotonView[] nViews = this.gameObject.GetComponentsInChildren<PhotonView>();
		nViews[0].viewID = id1;
	}
}
