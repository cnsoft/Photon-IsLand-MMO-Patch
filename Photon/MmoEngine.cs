// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MmoEngine.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The mmo engine.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

using ExitGames.Client.Photon;

using Photon.MmoDemo.Client;
using Photon.MmoDemo.Common;

using UnityEngine;

using UIPackage; //only for test.

/// <summary>
/// The mmo engine.
/// </summary>
public class MmoEngine : Radar, IGameListener
{
    /// <summary>
    /// EdgeLengthHorizontal / BoxesHorizontal
    /// </summary>
    public const int PositionFactorHorizonal = 1;

    /// <summary>
    /// EdgeLengthVertical / BoxesVertical
    /// </summary>
    public const int PositionFactorVertical = 1;

    /// <summary>
    /// The cam height.
    /// </summary>
    private float camHeight;

    /// <summary>
    /// The engine.
    /// </summary>
    private Game engine;
	
	public const string PlayerAvatarTag = "human_kernel";//"New_Main";
	public string OtherPlayerRes = "RobotPaperDoll";//"New_Main";
	
	//add for switch world and room state
	public bool _isAway = false;
	public bool _isLoading = false;

    /// <summary>
    /// Gets a value indicating whether IsDebugLogEnabled.
    /// </summary>
    public bool IsDebugLogEnabled
    {
        get
        {
            return false;
        }
    }
	
	public Game XEngine
	{
		get
		{
			return engine;
		}
	}
    /// <summary>
    /// The on application quit.
    /// </summary>
    public void OnApplicationQuit()
    {
        try
        {
            this.engine.Disconnect();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    /// <summary>
    /// The on gui.
    /// </summary>
    public override void OnGUI()
    {
        base.OnGUI();

        if (Event.current.type == EventType.ScrollWheel)
        {
            if (Event.current.delta.y < 0)
            {
                this.IncreaseViewDistance();
            }
            else if (Event.current.delta.y > 0)
            {
                this.DecreaseViewDistance();
            }
        }
        else if (Event.current.type == EventType.MouseDown)
        {
            if (Event.current.button == 2)
            {
                InterestArea cam;
                this.engine.TryGetCamera(0, out cam);
                cam.ResetViewDistance();
            }
        }
		//_isLoading
		if (this._isLoading)
			GUI.Label(new Rect(100,200,100,40),"Scene is Loading");
		//add RoomButton.
		if (true || !this._isAway)
			if ( GUI.Button(new Rect(100,120,100,40),"Click2Room") )
			{
				//
				if (this.gameObject.GetComponent<RoomEngine>()==null)
					EventManager.instance.dispatchEvent(new CustomEventObj("onReqEnterRoom"));
				//trigger request
			} //
		
		if (PhotonNetwork.connectionStateDetailed == PeerState.Joined)
        {
			if (PhotonNetwork.room == null) return;
            bool shoutMarco = PhotonNetwork.isMasterClient;//judge with master GameLogic.playerWhoIsIt == PhotonNetwork.player.ID;
			//PhotonView ScenePhotonView = this.gameObject.GetComponent<PhotonView>();
			//RoomPhotonView roomPV = (RoomPhotonView) this.gameObject.GetComponent<RoomPhotonView>();
            if (shoutMarco && GUI.Button(new Rect(100,180,100,30),"Marco!"))
            {
				Debug.Log("req Marco method be called");
				RoomEngine.ScenePhotonView.RPC("Marco", PhotonTargets.All);
            }
            if (!shoutMarco && GUI.Button(new Rect(100,180,100,30),"Polo!"))
            {
                RoomEngine.ScenePhotonView.RPC("Polo", PhotonTargets.All);
            }
        }
		
		
    }

    /// <summary>
    /// The start.
    /// </summary>
    public void Start()
    {
        try
        {
            this.style.normal.textColor = Color.white;

            // Make the game run even when in background
            Application.runInBackground = true;

            if (this.IsDebugLogEnabled)
            {
                Debug.Log("Start");
            }

            Settings settings = Settings.GetDefaultSettings();
            this.engine = new Game(this, settings, "Unity");
            this.engine.Avatar.SetText("Unity");
            GameObject player = GameObject.Find(PlayerAvatarTag);//GameObject.Find("First Person Controller Prefab");			
			if (player == null)
			{
				Debug.Log("not find player prefabs");
				return;
			}
			//
            this.engine.Avatar.MoveAbsolute(Player.GetPosition(player.transform.position), Player.GetRotation(player.transform.rotation.eulerAngles));
            this.engine.Avatar.ResetPreviousPosition();

            Photon.MmoDemo.Client.PhotonPeer peer = new Photon.MmoDemo.Client.PhotonPeer(this.engine, settings.UseTcp);
            this.engine.Initialize(peer);

            RTT rttBehaviour = (RTT)this.gameObject.AddComponent("RTT");//typeof(RTT));
            rttBehaviour.Initialize(this.engine);
			
			if (Terrain.activeTerrain)
            	this.camHeight = Camera.main.transform.position.y - Terrain.activeTerrain.SampleHeight(Camera.main.transform.position);
			
			//add test code..
			//This script should be attached with LocalActor.
			WalkDemo demoBehaviour = (WalkDemo) this.gameObject.AddComponent("WalkDemo");//typeof(WalkDemo));
			demoBehaviour.Initialize(this.engine);
			demoBehaviour.LocPlayer = player;
			
			//Hardcode.
			Invoke("setAOI",1);
			//handle joindRoom
			EventManager.instance.addEventListener("JoinedRoom",this.gameObject,"onJoinedRoom");
			EventManager.instance.addEventListener("onReqEnterRoom",this.gameObject,"onReqEnterRoomMode");
			
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
	
	
#region SceneRegion

	
	//clicked to trigger this
	//is there some decroator to make state check easier.
	public void onReqEnterRoomMode(){
		//attached an RoomPhotonView .and switch to room mode. 
		RoomEngine roomPV = (RoomEngine) this.gameObject.AddComponent("RoomEngine");
		//when it start, createRoom. leaveWorld.	
		roomPV.Connect();		
		
	}
	
	
	/**  Notified by RoomLogic **/
	public void onJoinedRoom(CustomEvent evt)
	{
		this.LogDebug(this.engine,"to handle onJoindRoom, quit world mode");
		//this.doWorldLeaved();
		//Notify UI load scene with name: 
		//WaitLoaded,PreInitLize.  
		//before send event, get scene name from RoomPV .
		EventManager.instance.dispatchEvent(new CustomEventObj("LoadScene"));
		//
		EventManager.instance.addEventListener("onSceneLoaded",this.gameObject,"onSceneLoaded");
		//E.g: manual create Player or loaded from level data after 		
	}
	
	public void onSceneLoaded(CustomEvent evt)
	{
		LogDebug(this.engine," todo: loaded custom level data here.");
		this.doWorldLeaved();
		//todo disable walkdemo.
		//WalkDemo w = this.gameObject.GetComponent<WalkDemo>();
		//w.enabled = false;
		//_isAway = true;
		//this.LogInfo(this.engine," world enter away mode.");
		
		//now we can disable player in Main. since in each scene one Main is placed.
		//RoomPhotonView roomPV = (RoomPhotonView) this.gameObject.GetComponent<RoomPhotonView>();
		/* GameObject stoneObject = GameObject.FindGameObjectWithTag( "PlayerStone" );
		if (stoneObject!=null){
			//roomPV.gameObject.transform.position = stoneObject.transform.position;
			//roomPV.gameObject.transform.rotation = stoneObject.transform.rotation;
			GameObject player = GameObject.FindGameObjectWithTag("human_kernel");
			player.transform.position = stoneObject.transform.position;
			player.transform.rotation = stoneObject.transform.rotation;
		}*/ 
		//move player to stone
		//EvHandle
		//int sceneId = 3;
		//UIHelper.getMaster.chmGetPhysicsHandler().pcsTeleportTo( sceneId );
		//Debug.LogWarning("client moved to scene3");
		//to invoke offical interface
	}
	
	public void doWorldLeaved()
	{
		WalkDemo w = this.gameObject.GetComponent<WalkDemo>();
		w.enabled = false;
		_isAway = true;
		this.LogInfo(this.engine," world enter away mode.");
		//not controlled by player anymore. not update anymore
	}
	
	public void onSceneLoading(CustomEvent evt)
	{
		_isLoading = true;
	}
	
#endregion	
	/** **/
	public void setAOI(){
			InterestArea cam;
            this.engine.TryGetCamera(0, out cam);
			float[] viewDistance = new float[2]; //(float[])this.engine.Avatar.ViewDistanceEnter.Clone();
			viewDistance[0] = 70.0f;
			viewDistance[1] = 70.0f;
            cam.SetViewDistance(viewDistance);
	}
	

	
    /// <summary>
    /// The update.
    /// </summary>
    public void Update()
    {
		if( this._isAway)
			return;
		//
        try
        {
            this.engine.Update();
            if (this.engine.State == GameState.WorldEntered)
            {
                this.OnRadarUpdate(this.engine.Avatar.Id, this.engine.Avatar.Type, this.engine.Avatar.Position);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    #region Implemented Interfaces

    #region IGameListener

    /// <summary>
    /// The log debug.
    /// </summary>
    /// <param name="game">
    /// The game.
    /// </param>
    /// <param name="message">
    /// The message.
    /// </param>
    public void LogDebug(Game game, string message)
    {
        Debug.Log(message);
    }

    /// <summary>
    /// The log error.
    /// </summary>
    /// <param name="game">
    /// The game.
    /// </param>
    /// <param name="message">
    /// The message.
    /// </param>
    public void LogError(Game game, string message)
    {
        Debug.Log(message);
    }

    /// <summary>
    /// The log error.
    /// </summary>
    /// <param name="game">
    /// The game.
    /// </param>
    /// <param name="errorCode">
    /// The error code.
    /// </param>
    /// <param name="debugMessage">
    /// The debug message.
    /// </param>
    /// <param name="operationCode">
    /// The operation code.
    /// </param>
    public void LogError(Game game, ReturnCode errorCode, string debugMessage, OperationCode operationCode)
    {
        Debug.Log(debugMessage);
    }

    /// <summary>
    /// The log error.
    /// </summary>
    /// <param name="game">
    /// The game.
    /// </param>
    /// <param name="exception">
    /// The exception.
    /// </param>
    public void LogError(Game game, Exception exception)
    {
        Debug.Log(exception.ToString());
    }

    /// <summary>
    /// The log info.
    /// </summary>
    /// <param name="game">
    /// The game.
    /// </param>
    /// <param name="message">
    /// The message.
    /// </param>
    public void LogInfo(Game game, string message)
    {
        Debug.Log(message);
    }

    /// <summary>
    /// The on camera attached.
    /// </summary>
    /// <exception cref="NotImplementedException">
    /// </exception>
    public void OnCameraAttached(string itemId, byte itemType)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// The on camera detached.
    /// </summary>
    /// <exception cref="NotImplementedException">
    /// </exception>
    public void OnCameraDetached()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// The on connect.
    /// </summary>
    /// <param name="game">
    /// The game.
    /// </param>
    public void OnConnect(Game game)
    {
        Debug.Log("connected");
    }

    /// <summary>
    /// The on disconnect.
    /// </summary>
    /// <param name="game">
    /// The game.
    /// </param>
    /// <param name="returnCode">
    /// The return code.
    /// </param>
    public void OnDisconnect(Game game, StatusCode returnCode)
    {
        Debug.Log("disconnected");
		this.doWorldLeaved();
    }
	

    /// <summary>
    /// on item added
    /// </summary>
    /// <param name="game">
    /// The game.
    /// </param>
    /// <param name="item">
    /// The item.
    /// </param>
    public void OnItemAdded(Game game, Item item)
    {
        if (this.engine != null)
        {
            if (this.IsDebugLogEnabled)
            {
                Debug.Log("add item " + item.Id);
            }

            this.CreateActor(game, item);
		
        }
    }

    /// <summary>
    /// The on item removed.
    /// </summary>
    /// <param name="game">
    /// The game.
    /// </param>
    /// <param name="item">
    /// The item.
    /// </param>
    public void OnItemRemoved(Game game, Item item)
    {
        GameObject actor = GameObject.Find("Item" + item.Id);
        if (actor != null)
        {
            Actor behaviour = (Actor)actor.GetComponent(typeof(Actor));
            behaviour.Destroy();
            if (this.IsDebugLogEnabled)
            {
                Debug.Log("destroy item " + item.Id);
            }
        }
        else
        {
            if (this.IsDebugLogEnabled)
            {
                Debug.Log("destroy item NOT FOUND " + item.Id);
            }
        }
    }

    /// <summary>
    /// The on item spawned.
    /// </summary>
    /// <param name="itemType">
    /// The item type.
    /// </param>
    /// <param name="itemId">
    /// The item id.
    /// </param>
    public void OnItemSpawned(byte itemType, string itemId)
    {
		Debug.Log(string.Format("item {0} type {1} on spawned",itemId,itemType));
    }

    /// <summary>
    /// The on world entered.
    /// </summary>
    /// <param name="game">
    /// The game.
    /// </param>
    public void OnWorldEntered(Game game)
    {
        Debug.Log("entered world " + game.WorldData.Name);
        GameObject player = GameObject.Find(PlayerAvatarTag);//GameObject.Find("First Person Controller Prefab");		
        Player playerBehaviour = (Player)player.AddComponent(typeof(Player));
        playerBehaviour.Initialize(this.engine);
        this.world = game.WorldData;
        this.selfId = game.Avatar.Id + game.Avatar.Type;
        Operations.RadarSubscribe(game.Peer, game.WorldData.Name);
		//GameClient.
    }

    #endregion

    #endregion

    /// <summary>
    /// The create actor.
    /// </summary>
    /// <param name="engine">
    /// The engine.
    /// </param>
    /// <param name="actor">
    /// The actor.
    /// </param>
    private void CreateActor(Game engine, Item actor)
    {
		//Todo: how to extend Item to support RPC call? if so. we can call PhotonClient.callRpc(ItemId,others,{"iGotYou","a":1,"b":2})
		// when player health change > dispatch it.  PhotonClient.callRpc(893,others,{"healthChange","1900"});
		// seems like: broadcast in room. 
		// in here: we have to  PhotonClient.callRpc(actor.id,"getProperties");
		// do not show local actor  
        if (actor != engine.Avatar)
        {
			//Todo: inilized entity with entityInfo. 
			//e.g: initialize a monster. 
            //GameObject actorCube = actor.Rotation != null ? GameObject.CreatePrimitive(PrimitiveType.Cube) : GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			var guai2 = Resources.Load("network/"+ OtherPlayerRes);
			//Debug.Log(guai2);
			GameObject actorCube = Instantiate(guai2, new Vector3(0,0,0), Quaternion.Euler(0, 0, 0)) as GameObject;
            Actor actorBehaviour = (Actor)actorCube.AddComponent(typeof(Actor));			
            actorBehaviour.Initialize(actor, this.camHeight);
			//hard code.
			Debug.Log("CreateActor..");
			Debug.Log("To get properties");
			actor.GetProperties();	
			//when be pushed. local function be called.
						//add test code..
			WalkDemo demoBehaviour = (WalkDemo)actorCube.AddComponent(typeof(WalkDemo));
			demoBehaviour.Initialize(this.engine);
			
			//Ensure. RobotScript be killed.
			Robot robot = (Robot)actorCube.GetComponent(typeof(Robot));
			if ( robot !=null )
				robot.enabled = false;
			//done
			
        }
    }

    /// <summary>
    /// The decrease view distance.
    /// </summary>
    private void DecreaseViewDistance()
    {
        InterestArea cam;
        this.engine.TryGetCamera(0, out cam);
		if (cam == null || cam.ViewDistanceEnter ==null )return;
        float[] viewDistance = (float[])cam.ViewDistanceEnter.Clone();
        viewDistance[0] = Math.Max(1, viewDistance[0] - (this.engine.WorldData.TileDimensions[0] / 2));
        viewDistance[1] = Math.Max(1, viewDistance[1] - (this.engine.WorldData.TileDimensions[1] / 2));
        cam.SetViewDistance(viewDistance);
    }
   
    /// <summary>
    /// The increase view distance.
    /// </summary>
    private void IncreaseViewDistance()
    {
        InterestArea cam;
        this.engine.TryGetCamera(0, out cam);
		if (cam == null || cam.ViewDistanceEnter ==null)return;
        float[] viewDistance = (float[])cam.ViewDistanceEnter.Clone();
        viewDistance[0] = Math.Min(this.engine.WorldData.Width, viewDistance[0] + (this.engine.WorldData.TileDimensions[0] / 2));
        viewDistance[1] = Math.Min(this.engine.WorldData.Height, viewDistance[1] + (this.engine.WorldData.TileDimensions[1] / 2));
        cam.SetViewDistance(viewDistance);
    }
}