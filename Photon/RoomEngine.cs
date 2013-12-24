using UnityEngine;
using System.Collections;

using UIPackage;

/// <summary>
/// Room Engine. used in Cloud Mode. RPC Method should not in this class.
/// 
/// </summary>/
public class RoomEngine : Photon.MonoBehaviour
{
	public static int playerWhoIsIt = 0;
    public static PhotonView ScenePhotonView;
	
	public int _roomStatus = 0;
	private AsyncOperation engineAsync_;
	private string loadingScenePath_ = "";//cur scenename
	// Use this for initialization
	
	public GameObject _locPlayer;
	void Start ()
	{
		//maybe we should use event to control this start.
		//i should be created by application.
		//PhotonNetwork.ConnectUsingSettings("0.1");
		EventManager.instance.addEventListener("LoadScene",this.gameObject,"StartLoad");
	}
	
#region connect&disconnect	
	public void Connect(){
		//setup room connection
		PhotonNetwork.ConnectUsingSettings("0.1");
		Debug.LogWarning("RoomConnect connecting..");
	}
	
	public void DisConnect(){
		PhotonNetwork.Disconnect();
		Debug.LogWarning("Room DisConnecting..");
		
	}
#endregion	
	
	void OnJoinedLobby()
    {
        Debug.LogWarning ("JoinRandom!");
		//it seems we should use customProperties. private_10_scene000_2  私有房间最多10人.场景scene000 难度2.
		//
        PhotonNetwork.JoinRandomRoom();
		_roomStatus = 0;
    }

    void OnPhotonRandomJoinFailed()
    {
        PhotonNetwork.CreateRoom(null);
		//
		Debug.Log("Let me create Room");
		this._roomStatus = 1; //
		
    }

#region  LoadScene and Initlize. 
	
	void onSceneLoaded(CustomEvent evt)
	{
		//
		PhotonNetwork.isMessageQueueRunning = true;
		//open it
	    ScenePhotonView.RPC("ResetToStone",PhotonTargets.All);
		ScenePhotonView.RPC("LoadPlayerData",PhotonTargets.Others,new Hashtable());
		this.doManualPvInit();
	}
	
	
	void doManualPvInit(){
	  if (PhotonNetwork.isMasterClient)
		{
			//Master Client will req server buffer these message to enable other joiner bind proxy correctly. 
			GameObject[] coms = GameObject.FindGameObjectsWithTag("Monster");
			foreach(GameObject com in coms)
			{
				GuidProperty guid = (GuidProperty) com.GetComponent<GuidProperty>();
				if (guid ==null)
					continue;
				//if null do nothing
				string objid = guid.objectid;
				//manual alloc pvid. 
				PhotonView pv = (PhotonView) com.GetComponent<PhotonView>();
				if (pv==null)
				{
					//attached one by hand
					com.gameObject.AddComponent(typeof(PhotonView));
					pv = (PhotonView) com.GetComponent<PhotonView>();
				}
				pv.viewID = PhotonNetwork.AllocateViewID();
				Hashtable _params = new Hashtable();
				_params.Add(0,objid);
				_params.Add(1,pv.viewID);
				//Notify All Remote Client to bind proxy .
				ScenePhotonView.RPC("BindRemoteProxy", PhotonTargets.OthersBuffered,_params);
			}	
		}
	}
	
#endregion	

    void OnJoinedRoom()
    {
		Debug.LogWarning("RoomPhotonView onJoinedRoom");		
		EventManager.instance.dispatchEvent(new CustomEvent("onTeleportTo"));
		//should changed to ui or other handler not here.
		int sceneId = 6; //not same with buildings. 
		UIHelper.getMaster.chmGetPhysicsHandler().pcsTeleportTo( sceneId );
		Debug.LogWarning("will moved to scene 3");
		//
		EventManager.instance.dispatchEvent(new CustomEvent("JoinedRoom"));		
		EventManager.instance.addEventListener("onSceneLoaded",this.gameObject,"onSceneLoaded");
		
		//
		//Manual close
		//PhotonNetwork.LoadLevel();
		PhotonNetwork.isMessageQueueRunning = false;
     	//after loaded open it again.
		
		//RoomEntity need PhotonNetwork		
		//Notice: we should call PhotonNetwork.Instantiate to make we can see each other in room.
		//Or PhotonInstantiate scensor object, observe local realobject. 
		//create the proxy object?
		_locPlayer = PhotonNetwork.Instantiate("network/NetRobotPaperDoll", Vector3.zero, Quaternion.identity,0);//(int) PhotonTargets.Others);
		GameObject.DontDestroyOnLoad(_locPlayer);		
        ScenePhotonView = _locPlayer.GetComponent<PhotonView>();
		
		//only local interface can be used.
		Debug.Log(" ScenePV is used locally." + ScenePhotonView);
		
		//TestNetBox.. 
	
		//		
		//PhotonNetwork.LoadLevel(4);
		
		//Todo: Spawn Level Game Object by level file. instead of .unity. 
		//e.g: create a monster.
		//PhotonNetwork.InstantiateSceneObject("network/NetRobotPaperDoll",Vector3.zero,Quaternion.identity,0);//
		
		//Component[] coms = GameObject.FindGameObjectsWithTag("monster");
		//int id = coms[0].GetInstanceID;
		
		return;
		
		//ScenePhotonView = this.gameObject.GetComponent<PhotonView>();
		//To add method call and convert local exist object to network object.
		//To check transfer position to remote. 2013-12-17 

		
		//Notify Application to initlize Player?
		//PhotonNetwork error: Could not Instantiate the prefab [network/RobotPaperDoll] as it has no PhotonView attached to the root.
		//To fix it,we attached a photonView to prefabs.		
        _locPlayer = PhotonNetwork.Instantiate("network/NetRobotPaperDoll", Vector3.zero, Quaternion.identity, 0);
		GameObject.DontDestroyOnLoad(_locPlayer);		
        ScenePhotonView = _locPlayer.GetComponent<PhotonView>();
		EventManager.instance.dispatchEvent(new CustomEvent("JoinedRoom"));
		//EntityManager should initlize it.
		
		if (_roomStatus ==1)
		{
			//you enter room first.
			Debug.LogWarning("i am loading donot destroy me");
			//Scene_000_01
			engineAsync_ = Application.LoadLevelAsync(3);
		}else {
			//get roominfo. and property. use scenename to load local scene.
			engineAsync_ = Application.LoadLevelAsync(3);
			
		}
		
		
    }
	
	
	bool _loadSceneUpdate(){
		if( engineAsync_ == null || loadingScenePath_ == null )
				return true;
		int progress = System.Convert.ToInt32( engineAsync_.progress * 100 );		
		EventManager.instance.dispatchEvent(new CustomEventObj("onSceneLoading"));
		if( engineAsync_.isDone )
			{
				// clear done things here.
				engineAsync_ = null;
				EventManager.instance.dispatchEvent(new CustomEventObj("onSceneLoaded"));
			}
			// done.
		return engineAsync_ == null;
		
	}
	public void Update()
	{
		//_loadSceneUpdate();			
	}
	
    void OnGUI()		
    {
		//Debug.Log("on RoomPV gui callback");
		
        GUI.Label(new Rect(100,160,100,40),PhotonNetwork.connectionStateDetailed.ToString());

     	//if (PhotonNetwork.room == null)
		return;
		
		if (PhotonNetwork.connectionStateDetailed == PeerState.Joined)
        {
            bool shoutMarco = PhotonNetwork.isMasterClient;//judge with master GameLogic.playerWhoIsIt == PhotonNetwork.player.ID;

            if (shoutMarco && GUI.Button(new Rect(100,180,100,30),"Marco!"))
            {
				Debug.Log("req Marco method be called");
				ScenePhotonView.RPC("Marco", PhotonTargets.All);
            }
            if (!shoutMarco && GUI.Button(new Rect(100,180,100,30),"Polo!"))
            {
                ScenePhotonView.RPC("Polo", PhotonTargets.All);
            }
        }
    }
	
	#region RoomEvent Region	
	public void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		Debug.Log("OnPhotonPlayerConnected: " + player);
	}
	public void OnPhotonPlayerDisconnected(PhotonPlayer player)
	{
		Debug.Log("OnPhotonPlayerDisConnected: " + player);
		
	}
	
	public void onPhotonInstantiate(PhotonMessageInfo info)
	{
		Debug.Log(" on Instantiate be called " + info.sender);		
		//object[] objs = photonView.instantiationData; //The instantiate data..
        //bool[] mybools = (bool[])objs[0];   //Our bools!		
		GameObject stoneObject = GameObject.FindGameObjectWithTag( "PlayerStone" );
		if (stoneObject!=null){
			GameObject _netbox = GameObject.FindGameObjectWithTag("NetBox");
			_netbox.transform.position = stoneObject.transform.position;
			_netbox.transform.rotation = stoneObject.transform.rotation;
			_netbox.transform.position[0]+= Random.Range(-50,50);
		}	
	}
	
	public void onFailedToConnecToPhoton(){
		Debug.Log ("fail connect to photon");
	}
	
	#endregion
	

}

