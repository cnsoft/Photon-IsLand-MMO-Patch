using UnityEngine;
using System.Collections;

public class CustomEventObj : CustomEvent {
  
   // event types
   public static string MY_EVENT_1 = "my_event_1";
   public static string MY_EVENT_2 = "my_event_2";
  
   // optionally add custom variables instead of using the arguments hashtable
   public int myCustomEventVar1 = 0;
   public bool rockOn = true;
   public CustomEventObj(string eventType = "") {
          type = eventType;
 		}
   }