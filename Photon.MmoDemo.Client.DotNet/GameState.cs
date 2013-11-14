// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameState.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The game engine state.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Client
{
    /// <summary>
    /// The game state.
    /// </summary>
    public enum GameState
    {
        /// <summary>
        /// The connected.
        /// </summary>
        Connected, 

        /// <summary>
        /// The disconnected.
        /// </summary>
        Disconnected, 

        /// <summary>
        /// The wait for connect.
        /// </summary>
        WaitForConnect, 

        /// <summary>
        /// The world entered.
        /// </summary>
        WorldEntered
    }
}