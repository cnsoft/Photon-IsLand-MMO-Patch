// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGameListener.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The i game logic listener.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Client
{
    using System;

    using ExitGames.Client.Photon;

    /// <summary>
    /// The i game logic listener.
    /// </summary>
    [CLSCompliant(false)]
    public interface IGameListener
    {
        /// <summary>
        /// Gets a value indicating whether IsDebugLogEnabled.
        /// </summary>
        bool IsDebugLogEnabled { get; }

        /// <summary>
        /// The log debug.
        /// </summary>
        /// <param name="game">
        /// The source game.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        void LogDebug(Game game, string message);

        /// <summary>
        /// The log error.
        /// </summary>
        /// <param name="game">
        /// The source game.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        void LogError(Game game, string message);

        /////// <summary>
        /////// The log error
        /////// </summary>
        /////// <param name="game">
        /////// The source game.
        /////// </param>
        /////// <param name="errorCode">
        /////// The error code.
        /////// </param>
        /////// <param name="debugMessage">
        /////// The debug message.
        /////// </param>
        /////// <param name="operationCode">
        /////// The operation code.
        /////// </param>
        ////void LogError(Game game, ReturnCode errorCode, string debugMessage, OperationCode operationCode);

        /// <summary>
        /// The log error.
        /// </summary>
        /// <param name="game">
        /// The source game.
        /// </param>
        /// <param name="exception">
        /// The exception.
        /// </param>
        void LogError(Game game, Exception exception);

        /// <summary>
        /// The log info.
        /// </summary>
        /// <param name="game">
        /// The source game.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        void LogInfo(Game game, string message);

        /// <summary>
        /// The on camera attached.
        /// </summary>
        /// <param name="itemId">
        /// The item Id.
        /// </param>
        /// <param name="itemType">
        /// The item Type.
        /// </param>
        void OnCameraAttached(string itemId, byte itemType);

        /// <summary>
        /// The on camera detached.
        /// </summary>
        void OnCameraDetached();

        /// <summary>
        /// The on connect.
        /// </summary>
        /// <param name="game">
        /// The source game.
        /// </param>
        void OnConnect(Game game);

        /// <summary>
        /// The on disconnect.
        /// </summary>
        /// <param name="game">
        /// The source game.
        /// </param>
        /// <param name="returnCode">
        /// The return code.
        /// </param>
        void OnDisconnect(Game game, StatusCode returnCode);

        /// <summary>
        /// The on item added.
        /// </summary>
        /// <param name="game">
        /// The mmo game.
        /// </param>
        /// <param name="item">
        /// The mmo item.
        /// </param>
        void OnItemAdded(Game game, Item item);

        /// <summary>
        /// The on item removed.
        /// </summary>
        /// <param name="game">
        /// The mmo game.
        /// </param>
        /// <param name="item">
        /// The mmo item.
        /// </param>
        void OnItemRemoved(Game game, Item item);

        /// <summary>
        /// The on item spawned.
        /// </summary>
        /// <param name="itemType">
        /// The item type.
        /// </param>
        /// <param name="itemId">
        /// The item id.
        /// </param>
        void OnItemSpawned(byte itemType, string itemId);

        /// <summary>
        /// The on radar update.
        /// </summary>
        /// <param name="itemId">
        /// The item id.
        /// </param>
        /// <param name="itemType">
        /// The item type.
        /// </param>
        /// <param name="position">
        /// The position.
        /// </param>
        void OnRadarUpdate(string itemId, byte itemType, float[] position);

        /// <summary>
        /// The on world entered.
        /// </summary>
        /// <param name="game">
        /// The source game.
        /// </param>
        void OnWorldEntered(Game game);
    }
}