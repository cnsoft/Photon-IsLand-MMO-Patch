// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Connected.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The dispatcher connected.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Client.GameStateStrategies
{
    using System;
    using System.Collections.Generic;

    using ExitGames.Client.Photon;

    using Photon.MmoDemo.Common;

    /// <summary>
    /// The dispatcher connected.
    /// </summary>
    [CLSCompliant(false)]
    public class Connected : IGameLogicStrategy
    {
        /// <summary>
        /// The instance.
        /// </summary>
        public static readonly IGameLogicStrategy Instance = new Connected();

        /// <summary>
        /// Gets State.
        /// </summary>
        public GameState State
        {
            get
            {
                return GameState.Connected;
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
        /// <param name="eventData">
        /// The event data.
        /// </param>
        public void OnEventReceive(Game game, EventData eventData)
        {
            game.OnUnexpectedEventReceive(eventData);
        }

        /// <summary>
        /// The on operation return.
        /// </summary>
        /// <param name="game">
        /// The game logic.
        /// </param>
        /// <param name="response">
        /// The response.
        /// </param>
        public void OnOperationReturn(Game game, OperationResponse response)
        {
            // by default, a return of 0 is "successfully done"
            if (response.ReturnCode == 0)
            {
                switch ((OperationCode)response.OperationCode)
                {
                    case OperationCode.CreateWorld:
                        {
                            game.Avatar.EnterWorld();
                            return;
                        }

                    case OperationCode.EnterWorld:
                        {
                            var worldData = new WorldData
                                {
                                    Name = (string)response.Parameters[(byte)ParameterCode.WorldName],
                                    BottomRightCorner = (float[])response.Parameters[(byte)ParameterCode.BottomRightCorner],
                                    TopLeftCorner = (float[])response.Parameters[(byte)ParameterCode.TopLeftCorner],
                                    TileDimensions = (float[])response.Parameters[(byte)ParameterCode.TileDimensions]
                                };
                            game.SetStateWorldEntered(worldData);
                            return;
                        }
                }
            }
            else
            {
                switch ((OperationCode)response.OperationCode)
                {
                    case OperationCode.EnterWorld:
                        {
                            Operations.CreateWorld(
                                game, game.WorldData.Name, game.WorldData.TopLeftCorner, game.WorldData.BottomRightCorner, game.WorldData.TileDimensions);
                            return;
                        }
                }
            }

            game.OnUnexpectedOperationError(response);
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
            game.Peer.OpCustom((byte)operationCode, parameter, sendReliable, channelId);
        }

        #endregion

        #endregion
    }
}