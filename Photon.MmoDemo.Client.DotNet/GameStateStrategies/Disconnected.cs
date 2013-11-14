// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Disconnected.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The dispatcher disconnected.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Client.GameStateStrategies
{
    using System;
    using System.Collections.Generic;

    using ExitGames.Client.Photon;

    using Photon.MmoDemo.Common;

    /// <summary>
    /// The dispatcher disconnected.
    /// </summary>
    [CLSCompliant(false)]
    public class Disconnected : IGameLogicStrategy
    {
        /// <summary>
        /// The instance.
        /// </summary>
        public static readonly IGameLogicStrategy Instance = new Disconnected();

        /// <summary>
        /// Gets State.
        /// </summary>
        public GameState State
        {
            get
            {
                return GameState.Disconnected;
            }
        }

        /// <summary>
        /// The on event receive.
        /// </summary>
        /// <param name="gameLogic">
        /// The game logic.
        /// </param>
        /// <param name="event">
        /// The event.
        /// </param>
        public void OnEventReceive(Game gameLogic, EventData @event)
        {
            gameLogic.OnUnexpectedEventReceive(@event);
        }

        /// <summary>
        /// The on operation return.
        /// </summary>
        /// <param name="gameLogic">
        /// The game logic.
        /// </param>
        /// <param name="operationResponse">
        /// The operation Response.
        /// </param>
        public void OnOperationReturn(Game gameLogic, OperationResponse operationResponse)
        {
            gameLogic.OnUnexpectedPhotonReturn(operationResponse);
        }

        /// <summary>
        /// The send operation.
        /// </summary>
        /// <param name="game">
        /// The mmo game.
        /// </param>
        /// <param name="operationCode">
        /// The operation code.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="sendReliable">
        /// The send reliable.
        /// </param>
        /// <param name="channelId">
        /// the channel id
        /// </param>
        public void SendOperation(Game game, OperationCode operationCode, Dictionary<byte, object> parameter, bool sendReliable, byte channelId)
        {
        }

        #region Implemented Interfaces

        #region IGameLogicStrategy

        /// <summary>
        /// The on peer status callback.
        /// </summary>
        /// <param name="game">
        /// The game.
        /// </param>
        /// <param name="returnCode">
        /// The return code.
        /// </param>
        public void OnPeerStatusCallback(Game game, StatusCode returnCode)
        {
            game.DebugReturn(DebugLevel.ERROR, returnCode.ToString());
        }

        /// <summary>
        /// The on update.
        /// </summary>
        /// <param name="gameLogic">
        /// The game logic.
        /// </param>
        public void OnUpdate(Game gameLogic)
        {
        }

        #endregion

        #endregion
    }
}