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
		((PhotonSpace.AvatarPhotonCaster) _watchedRobot.chmGetPhotonHandler() ).setProxy(this);
		//dual bind.
	}
	
	
	// Update is called once per frame
	void Update ()
	{
		//do nothing.
		//if(!photonView || photonView.isMine)
		//	base.Update();
		
		//copy data and check changed or not.		
		/*	matchSpellId_ = new AnimatorParamInt( GlobalTechConfig.ANIMATOR_PARAM_SPELLID );
			matchSpellLife_ = new AnimatorParamInt( GlobalTechConfig.ANIMATOR_PARAM_SPELLLIFE );
			matchActionId_ = new AnimatorParamInt( GlobalTechConfig.ANIMATOR_PARAM_ACTIONID );
			matchDead_ = new AnimatorParamBool( GlobalTechConfig.ANIMATOR_PARAM_DEAD );
			matchSpeed_ = new AnimatorParamSpeed( GlobalTechConfig.ANIMATOR_PARAM_SPEED );
		*/
		if (_watchedRobot)
		{
			//sent dirty animator
/*			Animator animator = _watchedRobot.gameObject.GetComponent<Animator>();
			if (animator==null)
				return;
			stream.SendNext(animator.GetFloat("Speed"));
			stream.SendNext(animator.GetFloat("Direction"));
			stream.SendNext(animator.GetBool("Attack0101"));
			//stream.SendNext(animator.GetBool("Run"));		
			stream.SendNext(animator.GetBool("Hurt"));
			Vector3 _pos = _watchedRobot.chmGetMotorHandler().morGetPosition();
			Vector3 _dir = _watchedRobot.chmGetMotorHandler().morGetDirection();
			this.CallRPC("setAnimatorAttr",PhotonTargets.Others,"position",3,_dir); */
			
			//((PhotonSpace.AvatarPhotonCaster) _watchedRobot.chmGetPhotonHandler()).RPC("setAnimatorAttr",PhotonTargets.Others,"SpellId",matchSpellId_.paramValue_,2);
		}
		
		
	}
	
	
	public void CallRPC(string methodName, PhotonTargets target, params object[] args){
		//robot call this to invoke rpc method.
		photonView.RPC(methodName,target,args);
	}
	
	#region RPC_region
	[RPC]
	void setAnimatorAttrBool(string attr,bool data)
	{
		Animator amr = this.chmGetMountHandler().infGetActiveActObject().GetComponent<Animator>();
		amr.SetBool(attr,(bool)data);		
	}
	[RPC]
	void setAnimatorAttrInt(string attr,int data)
	{
		Animator amr = this.chmGetMountHandler().infGetActiveActObject().GetComponent<Animator>();
		amr.SetInteger(attr,(int)data);		
	}
	[RPC]
	void setAnimatorAttrFloat(string attr,float data)
	{
		Animator amr = this.chmGetMountHandler().infGetActiveActObject().GetComponent<Animator>();
		amr.SetFloat(attr,(float)data);		
	}
	#endregion
	
	void setAnimatorAttrxxxx(string attr,int _type,params object[]datas)
	{
		//used to set local or remote animator property. to make animation works same. 
		Animator amr = this.chmGetMountHandler().infGetActiveActObject().GetComponent<Animator>();
		object data = (object)datas[0]; 
		switch (_type) 
		{
		case 0:
			amr.SetBool(attr,(bool)data);
			break;
		case 1:
			amr.SetFloat(attr,(float)data);
			break;
		case 2:
			amr.SetInteger(attr,(int)data);
			break;
		default:
			;
			break;
		}
		//do nothing.
	}
	
	
	
	//Save and Load Data be called by Photon
	public void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info)
	{
		//if (photonView.isMine){
		//Get Owner's Handler to do serialize.
		if (stream.isWriting)
		{
			((PhotonSpace.AvatarPhotonCaster) _watchedRobot.chmGetPhotonHandler() ).OnPhotonSerializeView(stream,info); //save
			//Amimator synchronized here? or use rpc to set animator
		}	
		//}
		else {
			//we can moved code to PhotonHandler, but have to find paperdoll handle in that class.
			//
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

