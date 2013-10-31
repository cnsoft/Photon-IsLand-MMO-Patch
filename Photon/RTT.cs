// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RTT.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The rtt.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Photon.MmoDemo.Client;

using UnityEngine;

/// <summary>
/// The rtt.
/// </summary>
public class RTT : MonoBehaviour
{
    /// <summary>
    /// The engine.
    /// </summary>
    private Game engine;

    /// <summary>
    /// The initialize.
    /// </summary>
    /// <param name="game">
    /// The game.
    /// </param>
    public void Initialize(Game game)
    {
        this.engine = game;
    }

    /// <summary>
    /// The start.
    /// </summary>
    public void Start()
    {
        if (!this.guiText)
        {
            Debug.Log("UtilityRoundTripTime needs a GUIText component!");
            this.enabled = false;
        }
    }

    /// <summary>
    /// The update.
    /// </summary>
    public void Update()
    {
        this.guiText.text = string.Format("{0}/{1}", this.engine.Peer.RoundTripTime, this.engine.Peer.RoundTripTimeVariance);
    }
}