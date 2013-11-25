/*
	Event Manager
	Static manager for handling an event driven communication model in Unity.
	This is similar to Adobe Flash's event listener model used in ActionScript.
	
	Copyright © 2012 Dustin Andrew
	dustin.andrew@gmail.com
	http://www.dustinandrew.me/
	
	This program is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or 
	any later version.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	http://www.gnu.org/licenses/
*/

/* 
 *  Setup:
 *  Create an empty GameObject and add the EventManager script to it.
 *  Create custom event classes that extend the CustomEvent.
 * 
 *  Restrictions & Tips:
 *  DO NOT add event listeners in the Awake() method! 
 *  This is used by the EventManager to initialize.
 * 	Change this class' Execution Order to before default time if you need to work around this.
 *  Use the Start() method to setup your events.
 * 	Make event listener callback functions public.
 *  Extend the CustomEvent class when creating your events.
 * 	Use custom variables in your custom events over the arguments hashtable to maintain class abstraction
 *  Clean up and remove event listeners when objects are destroyed.
 * 	Events are not received if the listener gameObject.active is false.
 * 
 *  Examples:
 * 
 * 	// setup event listeners
 * 	void Start() {
 * 		EventManager.instance.addEventListener(CustomEventObj.EVENT_TO_LISTEN_TO, gameObject, "OnSomethingHappened");
 *  }
 *  
 * 	// remove event listeners
 *  void OnDestroy() {
 *  	if (gameObject) {
 * 			// remove a single event
 * 			EventManager.instance.removeEventListener(CustomEventObj.EVENT_TO_LISTEN_TO, gameObject);
 * 			// remove all events
 * 			EventManager.instance.removeAllEventListeners(gameObject);
 * 		}
 * 	}
 * 
 * 	// get values passed by events
 * 	public void OnSomethingHappened(CustomEventObj evt) {
 * 		Debug.Log((datatype)evt.arguments["value"]);
 * 		// or if using custom vars instead of arguments hashtable
 * 		Debug.Log(evt.rockOn);		
 * 	}
 * 
 * 	// dispatch events
 *  void TriggerEvent() {
 *  	CustomEventObj evt = new CustomEventObj(CustomEventObj.EVENT_TO_TRIGGER);
 *  	evt.arguments.Add("value", 3);
 * 		EventManager.instance.dispatchEvent(evt);
 * 	}
 * 
 *  // create custom events
 *  using UnityEngine;
 *  using System.Collections;
 * 
 *  public class CustomEventObj : CustomEvent {
 * 
 * 		// event types
 *  	public static string MY_EVENT_1 = "my_event_1";
 *  	public static string MY_EVENT_2 = "my_event_2";
 * 
 * 		// optionally add custom variables instead of using the arguments hashtable
 * 		public int myCustomEventVar1 = 0;
 * 		public bool rockOn = true;
 * 
 * 		public CustomEventObj(string eventType = "") {
 *         type = eventType;
 *		}
 *  }
 * 
 */

using UnityEngine;
using System.Collections;

// internal event listener model
internal class EventListener {
	public string name;
	public GameObject listener;
	public string function;	
}

// Custom event class, extend when creating custom events
public class CustomEvent {
	
	private string _type;
	private Hashtable _arguments = new Hashtable();
	
	// constructor
	public CustomEvent(string eventType = "") {
		_type = eventType;
	}
	
	// the type of event
	public string type {
		get { return _type; }
		set { _type = value; }
	}
	
	// the arguments to pass with the event
	public Hashtable arguments {
		get { return _arguments; }
		set { _arguments = value; }
	}
}

public class EventManager : MonoBehaviour {
	
	// singleton instance
	public static EventManager instance;
	
	// settings
	public bool allowSingleton = true; // EventManager class will transfer between scene changes.
	public bool allowWarningOutputs = true;
	public bool allowDebugOutputs = true;
	public bool allowAutoCleanUp = true;
	
	private static bool _created = false;	
	private Hashtable _listeners = new Hashtable();	
	
	// setup singleton if allowed
	private void Awake() {
		if (!_created && allowSingleton) {
			DontDestroyOnLoad(this);
			instance = this;
			_created = true;
			Setup();
		} else {
			if (allowSingleton) {
				if (EventManager.instance.allowWarningOutputs) {
					Debug.LogWarning("Only a single instance of " + this.name + " should exists!");
				}
				Destroy(gameObject);
			} else {
				instance = this;
				Setup();
			}
		}
	}
	
	// clear events on quit
	private void OnApplicationQuit() {
		_listeners.Clear();
	}
	
	// clear null gameobject events on level load
	private void OnLevelWasLoaded() {
		if (allowAutoCleanUp) {
			ArrayList removeList; // create remove list to not break the enumerator
			ArrayList listenerList;
			foreach (DictionaryEntry listenerListObj in _listeners) {
				listenerList = listenerListObj.Value as ArrayList;
				removeList = new ArrayList();
				// find and add to remove list
	 			foreach (EventListener callback in listenerList) {
					if (callback.listener == null) {
						removeList.Add(callback);
					}
				}
				// remove from list
				foreach (EventListener callback in removeList) {
					listenerList.Remove(callback);
				}
			}
		}
	}
	
	// PUBLIC *******************************
	
	// Add event listener
	public bool addEventListener(string eventType, GameObject listener, string function) {
		if (listener == null || eventType == null) {
			if (allowWarningOutputs) {
				Debug.LogWarning("Event Manager: AddListener failed due to no listener or event name specified.");
			}
			return false;
		}
		recordEvent(eventType);
		return recordListener(eventType, listener, function);
	}
	
	// Remove event listener
	public bool removeEventListener(string eventType, GameObject listener) {
		if (!checkForEvent(eventType)) return false;
		
		ArrayList listenerList = _listeners[eventType] as ArrayList;
		foreach (EventListener callback in listenerList) {
			if (callback.name == listener.GetInstanceID().ToString()) {
				listenerList.Remove(callback);
				return true;
			}
		}
		return false;
	}
	
	// Remove all event listeners
	public void removeAllEventListeners(GameObject listener) {
		ArrayList removeList; // create remove list to not break the enumerator
		ArrayList listenerList;
		foreach (DictionaryEntry listenerListObj in _listeners) {
			listenerList = listenerListObj.Value as ArrayList;
			removeList = new ArrayList();
			// find and add to remove list
 			foreach (EventListener callback in listenerList) {
				if (callback.listener != null) {
					if (callback.name == listener.GetInstanceID().ToString()) {
						removeList.Add(callback);
					}
				}
			}
			// remove from list
			foreach (EventListener callback in removeList) {
				listenerList.Remove(callback);
			}
		}
	}
	
	// Dispatch an event
	public bool dispatchEvent(CustomEvent evt) {
		string eventType = evt.type;
		if (!checkForEvent(eventType)) {
			if (allowWarningOutputs) {
				Debug.LogWarning("Event Manager: Event \"" + eventType + "\" triggered has no listeners!");
			}
			return false;
		}
		
		ArrayList listenerList = _listeners[eventType] as ArrayList;
		if (allowDebugOutputs) {
			Debug.Log("Event Manager: Event " + eventType + " dispatched to " + listenerList.Count + ((listenerList.Count == 1) ? " listener." : " listeners."));
		}
		foreach (EventListener callback in listenerList) {
			if (callback.listener && callback.listener.activeSelf) {
				callback.listener.SendMessage(callback.function, evt, SendMessageOptions.DontRequireReceiver);
			}
		}
		return false;
	}
	
	// PRIVATE *******************************
	
	private void Setup() {
		// TO DO: Self create GameObject if not already created
	}
	
	// see if event already exists
	private bool checkForEvent(string eventType) {
		if (_listeners.ContainsKey(eventType)) return true;
		return false;
	}
	
	// record event, if it doesn't already exists
	private bool recordEvent(string eventType) {
		if (!checkForEvent(eventType)) {
			_listeners.Add(eventType, new ArrayList());
		}
		return true;
	}
	
	// delete event, if not already removed
	private bool deleteEvent(string eventType) {
		if (!checkForEvent(eventType)) return false;
		_listeners.Remove(eventType);
		return true;
	}
	
	// check if listener exists
	private bool checkForListener(string eventType, GameObject listener) {
		if (!checkForEvent(eventType)) {
			recordEvent(eventType);
		}
		
		ArrayList listenerList = _listeners[eventType] as ArrayList;
		foreach (EventListener callback in listenerList) {
			if (callback.name == listener.GetInstanceID().ToString()) return true;
		}
		return false;
	}
	
	// record listener, if not already recorded
	private bool recordListener(string eventType, GameObject listener, string function) {
		if (!checkForListener(eventType, listener)) {
			ArrayList listenerList = _listeners[eventType] as ArrayList;
			EventListener callback = new EventListener();
			callback.name = listener.GetInstanceID().ToString();
			callback.listener = listener;
			callback.function = function;
			listenerList.Add(callback);
			return true;
		} else {
			if (allowWarningOutputs) {
				Debug.LogWarning("Event Manager: Listener: " + listener.name + " is already in list for event: " + eventType);
			}
			return false;
		}
	}
}