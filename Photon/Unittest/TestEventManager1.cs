/****
 * 
 * Test EventManager Class
 * 
 * ****/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Unittest
{	
	public class TestEventManager1 : MonoBehaviour
	{
		public void TearUp()
		{
			EventManager.instance.addEventListener("Event1",this.gameObject,"Test");
			EventManager.instance.dispatchEvent(new CustomEvent("Event1"));
		}
		/// <summary>
		/// InvalidOperationException: List has changed.
		///System.Collections.ArrayList+SimpleEnumerator.MoveNext () (at /Applications/buildAgent/work/b59ae78cff80e584/mcs/class/corlib/System.Collections/ArrayList.cs:141)
		///EventManager.dispatchEvent (.CustomEvent evt) (at Assets/Photon/EventManager.cs:250)
		///Unittest.TestEventManager1.TearUp () (at Assets/Photon/Unittest/TestEventManager1.cs:17)
		///Unittest.TestEventManager1.Start () (at Assets/Photon/Unittest/TestEventManager1.cs:26)
		/// </summary>
		
		public void Test()
		{
			EventManager.instance.removeEventListener("Event1",this.gameObject);			
		}
		
		void Start(){
			this.TearUp();
			this.Test();
		}
		
	}
}
