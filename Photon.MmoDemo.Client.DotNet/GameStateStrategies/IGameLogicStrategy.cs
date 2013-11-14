// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGameLogicStrategy.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The i event dispatcher.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Client.GameStateStrategies
{
    using System;
    using System.Collections.Generic;

    using ExitGames.Client.Photon;

    using Photon.MmoDemo.Common;

    /// <summary>
    /// The i event dispatcher.
    /// </summary>
    [CLSCompliant(false)]
    public interface IGameLogicStrategy
    {
        /// <summary>
        /// Gets State.
        /// </summary>
        GameState State { get; }

        /// <summary>
        /// The on event receive.
        /// </summary>
        /// <param name="gameLogic">
        /// The game logic.
        /// </param>
        /// <param name="eventData">
        /// The event data.
        /// </param>
        void OnEventReceive(Game gameLogic, EventData eventData);

        /// <summary>
        /// The on operation return.
        /// </summary>
        /// <param name="gameLogic">
        /// The game logic.
        /// </param>
        /// <param name="operationResponse">
        /// The operation response.
        /// </param>
        void OnOperationReturn(Game gameLogic, OperationResponse operationResponse);

        /// <summary>
        /// The on peer status callback.
        /// </summary>
        /// <param name="gameLogic">
        /// The game logic.
        /// </param>
        /// <param name="returnCode">
        /// The return code.
        /// </param>
        void OnPeerStatusCallback(Game gameLogic, StatusCode returnCode);

        /// <summary>
        /// The on update.
        /// </summary>
        /// <param name="gameLogic">
        /// The game logic.
        /// </param>
        void OnUpdate(Game gameLogic);

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
        /// The send Reliable.
        /// </param>
        /// <param name="channelId">
        /// The channel Id.
        /// </param>
        void SendOperation(Game game, OperationCode operationCode, Dictionary<byte, object> parameter, bool sendReliable, byte channelId);
    }
}