// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorldEntered.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The dispatcher world entered.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Client.GameStateStrategies
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using ExitGames.Client.Photon;

    using Photon.MmoDemo.Common;

    /// <summary>
    /// The dispatcher world entered.
    /// </summary>
    [CLSCompliant(false)]
    public class WorldEntered : IGameLogicStrategy
    {
        /// <summary>
        /// The instance.
        /// </summary>
        public static readonly IGameLogicStrategy Instance = new WorldEntered();

        /// <summary>
        /// Gets State.
        /// </summary>
        public GameState State
        {
            get
            {
                return GameState.WorldEntered;
            }
        }

        #region Implemented Interfaces

        #region IGameLogicStrategy

        /// <summary>
        /// The on event receive.
        /// </summary>
        /// <param name="game">
        /// The mmo game.
        /// </param>
        /// <param name="eventData">
        /// The event data.
        /// </param>
        public void OnEventReceive(Game game, EventData eventData)
        {
            switch ((EventCode)eventData.Code)
            {
                case EventCode.RadarUpdate:
                    {
                        HandleEventRadarUpdate(eventData.Parameters, game);
                        return;
                    }

                case EventCode.ItemMoved:
                    {
                        HandleEventItemMoved(game, eventData.Parameters);
                        return;
                    }

                case EventCode.ItemDestroyed:
                    {
                        HandleEventItemDestroyed(game, eventData.Parameters);
                        return;
                    }

                case EventCode.ItemProperties:
                    {
                        HandleEventItemProperties(game, eventData.Parameters);
                        return;
                    }

                case EventCode.ItemPropertiesSet:
                    {
                        HandleEventItemPropertiesSet(game, eventData.Parameters);
                        return;
                    }

                case EventCode.ItemSubscribed:
                    {
                        HandleEventItemSubscribed(game, eventData.Parameters);
                        return;
                    }

                case EventCode.ItemUnsubscribed:
                    {
                        HandleEventItemUnsubscribed(game, eventData.Parameters);
                        return;
                    }

                case EventCode.WorldExited:
                    {
                        game.SetConnected();
                        return;
                    }
            }

            game.OnUnexpectedEventReceive(eventData);
        }

        /// <summary>
        /// The on operation return.
        /// </summary>
        /// <param name="game">
        /// The mmo game.
        /// </param>
        /// <param name="response">
        /// The operation response.
        /// </param>
        public void OnOperationReturn(Game game, OperationResponse response)
        {
            if (response.ReturnCode == 0)
            {
                switch ((OperationCode)response.OperationCode)
                {
                    case OperationCode.RemoveInterestArea:
                    case OperationCode.AddInterestArea:
                        {
                            return;
                        }

                    case OperationCode.AttachInterestArea:
                        {
                            HandleEventInterestAreaAttached(game, response.Parameters);
                            return;
                        }

                    case OperationCode.DetachInterestArea:
                        {
                            HandleEventInterestAreaDetached(game);
                            return;
                        }

                    case OperationCode.SpawnItem:
                        {
                            HandleEventItemSpawned(game, response.Parameters);
                            return;
                        }

                    case OperationCode.RadarSubscribe:
                        {
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

        /// <summary>
        /// The handle event interest area attached.
        /// </summary>
        /// <param name="game">
        /// The mmo game.
        /// </param>
        /// <param name="eventData">
        /// The event data.
        /// </param>
        private static void HandleEventInterestAreaAttached(Game game, IDictionary eventData)
        {
            var itemType = (byte)eventData[(byte)ParameterCode.ItemType];
            var itemId = (string)eventData[(byte)ParameterCode.ItemId];

            game.OnCameraAttached(itemId, itemType);
        }

        /// <summary>
        /// The handle event interest area detached.
        /// </summary>
        /// <param name="game">
        /// The mmo game.
        /// </param>
        private static void HandleEventInterestAreaDetached(Game game)
        {
            game.OnCameraDetached();
        }

        /// <summary>
        /// The handle event item destroyed.
        /// </summary>
        /// <param name="game">
        /// The mmo game.
        /// </param>
        /// <param name="eventData">
        /// The event data.
        /// </param>
        private static void HandleEventItemDestroyed(Game game, IDictionary eventData)
        {
            var itemType = (byte)eventData[(byte)ParameterCode.ItemType];
            var itemId = (string)eventData[(byte)ParameterCode.ItemId];

            Item item;
            if (game.TryGetItem(itemType, itemId, out item))
            {
                item.IsDestroyed = game.RemoveItem(item);
            }
        }

        /// <summary>
        /// The handle event item moved.
        /// </summary>
        /// <param name="game">
        /// The mmo game.
        /// </param>
        /// <param name="eventData">
        /// The event data.
        /// </param>
        private static void HandleEventItemMoved(Game game, IDictionary eventData)
        {
            var itemType = (byte)eventData[(byte)ParameterCode.ItemType];
            var itemId = (string)eventData[(byte)ParameterCode.ItemId];
            Item item;
            if (game.TryGetItem(itemType, itemId, out item))
            {
                if (item.IsMine == false)
                {
                    var position = (float[])eventData[(byte)ParameterCode.Position];
                    var oldPosition = (float[])eventData[(byte)ParameterCode.OldPosition];
                    float[] rotation = eventData.Contains((byte)ParameterCode.Rotation) ? (float[])eventData[(byte)ParameterCode.Rotation] : null;
                    float[] oldRotation = eventData.Contains((byte)ParameterCode.OldRotation) ? (float[])eventData[(byte)ParameterCode.OldRotation] : null;
                    item.SetPositions(position, oldPosition, rotation, oldRotation);
                }
            }
        }

        /// <summary>
        /// The handle event item properties.
        /// </summary>
        /// <param name="game">
        /// The mmo game.
        /// </param>
        /// <param name="eventData">
        /// The event data.
        /// </param>
        private static void HandleEventItemProperties(Game game, IDictionary eventData)
        {
            var itemType = (byte)eventData[(byte)ParameterCode.ItemType];
            var itemId = (string)eventData[(byte)ParameterCode.ItemId];

            Item item;
            if (game.TryGetItem(itemType, itemId, out item))
            {
                item.PropertyRevision = (int)eventData[(byte)ParameterCode.PropertiesRevision];

                if (item.IsMine == false)
                {
                    var propertiesSet = (Hashtable)eventData[(byte)ParameterCode.PropertiesSet];

                    item.SetColor((int)propertiesSet[Item.PropertyKeyColor]);
                    item.SetText((string)propertiesSet[Item.PropertyKeyText]);
                    item.SetInterestAreaAttached((bool)propertiesSet[Item.PropertyKeyInterestAreaAttached]);
                    item.SetInterestAreaViewDistance(
                        (float[])propertiesSet[Item.PropertyKeyViewDistanceEnter], (float[])propertiesSet[Item.PropertyKeyViewDistanceExit]);

                    item.MakeVisibleToSubscribedInterestAreas();
                }
            }
        }

        /// <summary>
        /// The handle event item properties set.
        /// </summary>
        /// <param name="game">
        /// The mmo game.
        /// </param>
        /// <param name="eventData">
        /// The event data.
        /// </param>
        private static void HandleEventItemPropertiesSet(Game game, IDictionary eventData)
        {
            var itemType = (byte)eventData[(byte)ParameterCode.ItemType];
            var itemId = (string)eventData[(byte)ParameterCode.ItemId];
            Item item;
            if (game.TryGetItem(itemType, itemId, out item))
            {
                item.PropertyRevision = (int)eventData[(byte)ParameterCode.PropertiesRevision];

                if (item.IsMine == false)
                {
                    var propertiesSet = (Hashtable)eventData[(byte)ParameterCode.PropertiesSet];

                    if (propertiesSet.ContainsKey(Item.PropertyKeyColor))
                    {
                        item.SetColor((int)propertiesSet[Item.PropertyKeyColor]);
                    }

                    if (propertiesSet.ContainsKey(Item.PropertyKeyText))
                    {
                        item.SetText((string)propertiesSet[Item.PropertyKeyText]);
                    }

                    if (propertiesSet.ContainsKey(Item.PropertyKeyViewDistanceEnter))
                    {
                        var viewDistanceEnter = (float[])propertiesSet[Item.PropertyKeyViewDistanceEnter];
                        item.SetInterestAreaViewDistance(viewDistanceEnter, (float[])propertiesSet[Item.PropertyKeyViewDistanceExit]);
                    }

                    if (propertiesSet.ContainsKey(Item.PropertyKeyInterestAreaAttached))
                    {
                        item.SetInterestAreaAttached((bool)propertiesSet[Item.PropertyKeyInterestAreaAttached]);
                    }
                }
            }
        }

        /// <summary>
        /// The handle event item spawned.
        /// </summary>
        /// <param name="game">
        /// The mmo game.
        /// </param>
        /// <param name="eventData">
        /// The event data.
        /// </param>
        private static void HandleEventItemSpawned(Game game, IDictionary eventData)
        {
            var itemType = (byte)eventData[(byte)ParameterCode.ItemType];
            var itemId = (string)eventData[(byte)ParameterCode.ItemId];

            game.OnItemSpawned(itemType, itemId);
        }

        /// <summary>
        /// The handle event item subscribed.
        /// </summary>
        /// <param name="game">
        /// The mmo game.
        /// </param>
        /// <param name="eventData">
        /// The event data.
        /// </param>
        private static void HandleEventItemSubscribed(Game game, IDictionary eventData)
        {
            var itemType = (byte)eventData[(byte)ParameterCode.ItemType];
            var itemId = (string)eventData[(byte)ParameterCode.ItemId];
            var position = (float[])eventData[(byte)ParameterCode.Position];
            var cameraId = (byte)eventData[(byte)ParameterCode.InterestAreaId];
            float[] rotation = eventData.Contains((byte)ParameterCode.Rotation) ? (float[])eventData[(byte)ParameterCode.Rotation] : null;

            Item item;
            if (game.TryGetItem(itemType, itemId, out item))
            {
                if (item.IsMine)
                {
                    item.AddSubscribedInterestArea(cameraId);
                    item.AddVisibleInterestArea(cameraId);
                }
                else
                {
                    var revision = (int)eventData[(byte)ParameterCode.PropertiesRevision];
                    if (revision == item.PropertyRevision)
                    {
                        item.AddSubscribedInterestArea(cameraId);
                        item.AddVisibleInterestArea(cameraId);
                    }
                    else
                    {
                        item.AddSubscribedInterestArea(cameraId);
                        item.GetProperties();
                    }

                    item.SetPositions(position, position, rotation, rotation);
                }
            }
            else
            {
                item = new ForeignItem(itemId, itemType, game);
                item.SetPositions(position, position, rotation, rotation);
                game.AddItem(item);

                item.AddSubscribedInterestArea(cameraId);
                item.GetProperties();
            }
        }

        /// <summary>
        /// The handle event item unsubscribed.
        /// </summary>
        /// <param name="game">
        /// The mmo game.
        /// </param>
        /// <param name="eventData">
        /// The event data.
        /// </param>
        private static void HandleEventItemUnsubscribed(Game game, IDictionary eventData)
        {
            var itemType = (byte)eventData[(byte)ParameterCode.ItemType];
            var itemId = (string)eventData[(byte)ParameterCode.ItemId];
            var cameraId = (byte)eventData[(byte)ParameterCode.InterestAreaId];

            Item item;
            if (game.TryGetItem(itemType, itemId, out item))
            {
                if (item.RemoveSubscribedInterestArea(cameraId))
                {
                    item.RemoveVisibleInterestArea(cameraId);
                }
            }
        }

        /// <summary>
        /// The handle event radar update.
        /// </summary>
        /// <param name="eventData">
        /// The event data.
        /// </param>
        /// <param name="game">
        /// The mmo game.
        /// </param>
        private static void HandleEventRadarUpdate(IDictionary eventData, Game game)
        {
            var itemId = (string)eventData[(byte)ParameterCode.ItemId];
            var itemType = (byte)eventData[(byte)ParameterCode.ItemType];
            var position = (float[])eventData[(byte)ParameterCode.Position];
            game.Listener.OnRadarUpdate(itemId, itemType, position);
        }
    }
}