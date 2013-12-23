using UnityEngine;
using System.Collections;

/// <summary>
/// GUID property. Used to handle networked multiply game object instranite. 
/// by cnsoft  2013-12
/// </summary>/
public class GuidProperty : MonoBehaviour {

	// Use this for initialization
	public string objectid = GetUniqueID();
	void Start () {
#if UNITY_EDITOR
		if (objectid=="")
		{
			//generate it. so it will be stored. can be used to loadLevel with network supported.
			 //GetUniqueID();
			objectid =  GetUniqueID();
			Debug.Log(objectid);
		}
#endif
		ReadUUID();
	}
	
	public string ReadUUID(){
		Debug.Log("Your uuid is "+ objectid);
		return objectid;
	}
	
	
	public static string GetUniqueID(){
 /*      string key = "ID";
 
       var random = new System.Random();              
       DateTime epochStart = new System.DateTime(1970, 1, 1, 8, 0, 0, System.DateTimeKind.Utc);
       double timestamp = (System.DateTime.UtcNow - epochStart).TotalSeconds;
 
       string uniqueID = Application.systemLanguage                 //Language
          +"-"+GetPlatform()                           //Device   
          +"-"+String.Format("{0:X}", Convert.ToInt32(timestamp))          //Time
          +"-"+String.Format("{0:X}", Convert.ToInt32(Time.time*1000000))     //Time in game
          +"-"+String.Format("{0:X}", random.Next(1000000000));          //random number
 
       Debug.Log("Generated Unique ID: "+uniqueID);
 
       if(PlayerPrefs.HasKey(key)){
         uniqueID = PlayerPrefs.GetString(key);      
       } else {       
         PlayerPrefs.SetString(key, uniqueID);
         PlayerPrefs.Save();  
       }  */
		

	   string uniqueID = "";
#if UNITY_EDITOR		
 	   uniqueID =  System.Guid.NewGuid().ToString();	
#endif
		Debug.Log(" uuid=" + uniqueID);
		return uniqueID;
		
    }
	
	// Update is called once per frame
	public string doUpdate (bool forced) {
#if UNITY_EDITOR
		if (forced || objectid=="")
		{
			//generate it. so it will be stored. can be used to loadLevel with network supported.
			 //GetUniqueID();
			string _objectid =  GetUniqueID();
			Debug.Log(objectid);
			objectid = _objectid;
			return _objectid;
		}
#endif
		return "";
	
	}
}
