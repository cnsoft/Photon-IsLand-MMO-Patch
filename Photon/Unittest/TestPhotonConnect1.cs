using UnityEngine;
using System.Collections;

public class TestPhotonConnect1 : Photon.MonoBehaviour {

	// Use this for initialization
	void Start () {
	
		this.TearUP();
		
	}

	// Update is called once per frame
	public string _lastConnectState="";
	
	void Update () {
		//check status changed time.		
		string _state = PhotonNetwork.connectionStateDetailed.ToString();
		if(_state != _lastConnectState)
		{
			Debug.LogError( Time.deltaTime + " changed from "+_lastConnectState +" to " + _state);
			_lastConnectState = _state;
		}
		// if long time no changed to expected state. do reconnect. 
				
	}
	
	void TearDown(){
		//try to use disconnect directly. works much better. 1.0
		PhotonNetwork.Disconnect();
		return;
		
		
		//how to leave room.
		if(photonView!=null)
			PhotonNetwork.Destroy( photonView);
		if(PhotonNetwork.room!=null)
			PhotonNetwork.LeaveRoom();		
		//clean some stuff
		//PhotonNetwork.Disconnect();
		//PhotonNetwork.DestroyAll();

	}
	
	
	void TearUP(){
		
		
		this.gameObject.AddComponent<PhotonView>();
		PhotonView pv = (PhotonView) this.gameObject.GetComponent<PhotonView>();
		pv.viewID = PhotonNetwork.AllocateViewID();
		//
		if (PhotonNetwork.connectionState.ToString() == "Disconnected")
		{
			PhotonNetwork.ConnectUsingSettings("0.12");//use local server setting
			setExpectedTimer("Joined",20);
			Debug.LogError(" Do reconnect again. ");
		}else 
			Debug.LogError(" you must wait disconnect finished! ");
		
	}
	
	public string  _expectedState="";
	void  setExpectedTimer(string state,float t){
		_expectedState = state;
		Invoke("checkExpected",t);
	}
	
	void checkExpected()
	{
		if(_expectedState != _lastConnectState)
			Debug.LogError(" expected state "+ _expectedState + " is not changed onTime" + "now is "+ _lastConnectState);
		//done. 
		Debug.LogError("works fine");
	}
	
	
	void Test()
	{
		//
		
	}
	
	
	
	void Redo(){
		this.TearDown();
		this.TearUP();		
	}
	
	
	
	float stTime=0;
	
	void OnJoinedLobby()
	{
		//call joinroom or create.  random join or quit ..
		if(PhotonNetwork.countOfRooms > 0)
		{
			PhotonNetwork.JoinRoom("testxxxx");
			return;
		}
		
		PhotonNetwork.CreateRoom("testxxxx");
		stTime  = Time.fixedDeltaTime;
	}
	
	void OnPhotonRandomJoinFailed()
	{
		//
		float costTime =  Time.fixedDeltaTime - stTime;
		Debug.LogError(" changed to onJoinedRoomFailed use "+ costTime);
		
		
	}
		
	void OnJoinedRoom(){
		//it should be called when connect be called?
		float costTime =  Time.fixedDeltaTime - stTime;
		Debug.LogError(" changed to onJoinedRoom use "+ costTime);
		
		//Too frequency? 
		Invoke("Redo",30);
	}
	
	//this interface is not been handled before. so faint.
	 IEnumerator OnLeftRoom()
    {
        //Easy way to reset the level: Otherwise we'd manually reset the camera
        //Wait untill Photon is properly disconnected (empty room, and connected back to main server)
        while(PhotonNetwork.room!=null || PhotonNetwork.connected==false)
            yield return 0;

        //Application.LoadLevel(Application.loadedLevel);
		Debug.LogError("really quit.");
    }
	void OnDisconnectedFromPhoton()
    {
        Debug.LogWarning("OnDisconnectedFromPhoton");
		//we can resetup connect 
		Invoke("Redo",1); //20 works fine.
    }    
}
