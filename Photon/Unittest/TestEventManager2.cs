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
	public class TestEventManager2 : MonoBehaviour
	{
		public void TearUp()
		{
			EventManager.instance.addEventListener("Event1",this.gameObject,"Test");
			EventManager.instance.dispatchEvent(new CustomEvent("Event1"));
		}
		
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
