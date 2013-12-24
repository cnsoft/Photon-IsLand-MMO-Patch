using UnityEngine;
using System.Collections;
using Photon;

public interface IPhotonDataListener {
	/// <summary>
	/// Raises the photon serialize view event.
	/// Used for sync data between local and remote	
	/// </summary>
	/// <param name='stream'>
	/// Stream.
	/// </param>
	/// <param name='info'>
	/// Info.
	/// </param>
	//
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info);
}
