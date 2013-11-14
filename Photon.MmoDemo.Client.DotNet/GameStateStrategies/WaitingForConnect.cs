// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WaitingForConnect.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The waiting for connect.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Client.GameStateStrategies
{
    using System;
    using System.Collections.Generic;

    using ExitGames.Client.Photon;

    using Photon.MmoDemo.Common;

    /// <summary>
    /// The waiting for connect.
    /// </summary>
    [CLSCompliant(false)]
    public class WaitingForConnect : IGameLogicStrategy
    {
        /// <summary>
        /// The instance.
        /// </summary>
        public static readonly IGameLogicStrategy Instance = new WaitingForConnect();

        /// <summary>
        /// Gets State.
        /// </summary>
        public GameState State
        {
            get
            {
                return GameState.WaitForConnect;
            }
        }

        #region Implemented Interfaces

        #region IGameLogicStrategy

        /// <summary>
        /// The on event receive.
        /// </summary>
        /// <param name="game">
        /// The game logic.
        /// </param>
        /// <param name="event">
        /// The event.
        /// </param>
        public void OnEventReceive(Game game, EventData @event)
        {
            game.OnUnexpectedEventReceive(@event);
        }

        /// <summary>
        /// The on operation return.
        /// </summary>
        /// <param name="game">
        /// The game logic.
        /// </param>
        /// <param name="operationResponse">
        /// The operation Response.
        /// </param>
        public void OnOperationReturn(Game game, OperationResponse operationResponse)
        {
            game.OnUnexpectedPhotonReturn(operationResponse);
        }

        /// <summary>
        /// The on peer status callback.
        /// </summary>
        /// <param name="game">
        /// The mmo game.
        /// </param>
        /// <param name="returnCode">
        /// The return code.
        /// </param>
        public void OnPeerStatusCallback(Game game, StatusCode returnCode)
        {
            switch (returnCode)
            {
                case StatusCode.Connect:
                    {
                        InterestArea camera;
                        game.TryGetCamera(0, out camera);
                        camera.ResetViewDistance();
                        game.Avatar.SetInterestAreaAttached(true);
                        game.SetConnected();
                        game.Avatar.EnterWorld();
                        break;
                    }

                case StatusCode.Disconnect:
                case StatusCode.DisconnectByServer:
                case StatusCode.DisconnectByServerLogic:
                case StatusCode.DisconnectByServerUserLimit:
                case StatusCode.TimeoutDisconnect:
                    {
                        game.SetDisconnected(returnCode);
                        break;
                    }

                default:
                    {
                        game.DebugReturn(DebugLevel.ERROR, returnCode.ToString());
                        break;
                    }
            }
        }

        /// <summary>
        /// The on update.
        /// </summary>
        /// <param name="game">
        /// The game logic.
        /// </param>
        public void OnUpdate(Game game)
        {
            game.Peer.Service();
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
        /// The channel Id.
        /// </param>
        public void SendOperation(Game game, OperationCode operationCode, Dictionary<byte, object> parameter, bool sendReliable, byte channelId)
        {
        }

        #endregion

        #endregion
    }
}