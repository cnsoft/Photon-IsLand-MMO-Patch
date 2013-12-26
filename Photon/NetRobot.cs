using UnityEngine;
using System.Collections;

public class NetRobot :Robot, IPhotonDataListener
{

	// Use this for initialization
	void Start () {
		
		//Robot _cur = HelperUtils.hlpEngineGetPlayer();
		this.isRealClient = false;
		base.Start();
		//paperdollHandler_ = new PaperdollSpace.AvatarPaperdollCaster( this );
		
		this.chmSetActiveActMountObject();		
		//remove set current player function. 
		//HelperUtils.hlpEngineSetPlayer(_cur);
		//we should not override it.
		
		//camera3rd_.camSetFollowTransform( activeActObject.transform ); 
		//also need restore camera3rd. fuck
		
		
		//Debug.LogWarning(" restore control to" + _cur.name);
	}
	void Awake(){
		if(!this.started)	
		{
			this.isRealClient = false;
			Start();
			//Start();
			//chmOnLevelLoaded(true); //enter fight mode			
		}	
	}
	
	
	Robot _watchedRobot = null;
	public void SetRobot(Robot r)
	{
		//set watched robot instance.
		_watchedRobot = r;
	}
	
	
	// Update is called once per frame
	void Update ()
	{
		//do nothing.
		//if(!photonView || photonView.isMine)
		//	base.Update();
		
	}
	
	//Save and Load Data be called by Photon
	public void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info)
	{
		//if (photonView.isMine){
		//Get Owner's Handler to do serialize.
		if (stream.isWriting)
			((PhotonSpace.AvatarPhotonCaster) _watchedRobot.chmGetPhotonHandler() ).OnPhotonSerializeView(stream,info); //save
		//}
		else {
			PaperdollSpace.PaperdollSettingType humanPd = null;
			int leh = (int)stream.ReceiveNext();
			if (leh==0) //?
				return;
			//
			string[] _paths = new string[leh];			
			for (int i=0;i<_paths.Length;i++)
			{
				string _path = (string) stream.ReceiveNext();
				_paths[i] = _path;
			} 
			
			PaperdollSpace.PaperdollSettingType mountPd = new PaperdollSpace.PaperdollSettingType( _paths );
			// how to load it? 				
			chmGetPaperdollHandler().spdLoadModel(humanPd, mountPd );
			//works or not  */
			Debug.LogWarning("load remote paperdoll");
		}

	}
}

