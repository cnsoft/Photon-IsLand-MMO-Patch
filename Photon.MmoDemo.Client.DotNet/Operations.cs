// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Operations.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The operations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Client
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Photon.MmoDemo.Common;

    /// <summary>
    /// The operations.
    /// </summary>
    [CLSCompliant(false)]
    public static class Operations
    {
        /// <summary>
        /// The add interest area.
        /// </summary>
        /// <param name="game">
        /// The mmo game.
        /// </param>
        /// <param name="cameraId">
        /// The camera id.
        /// </param>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <param name="viewDistanceEnter">
        /// The view distance enter.
        /// </param>
        /// <param name="viewDistanceExit">
        /// The view distance exit.
        /// </param>
        public static void AddInterestArea(Game game, byte cameraId, float[] position, float[] viewDistanceEnter, float[] viewDistanceExit)
        {
            var data = new Dictionary<byte, object>
                {
                    { (byte)ParameterCode.InterestAreaId, cameraId }, 
                    { (byte)ParameterCode.ViewDistanceEnter, viewDistanceEnter }, 
                    { (byte)ParameterCode.ViewDistanceExit, viewDistanceExit }, 
                    { (byte)ParameterCode.Position, position }
                };

            game.SendOperation(OperationCode.AddInterestArea, data, true, Settings.ItemChannel);
        }

        /// <summary>
        /// The attach camera.
        /// </summary>
        /// <param name="game">
        /// The mmo game.
        /// </param>
        /// <param name="itemId">
        /// The item id.
        /// </param>
        /// <param name="itemType">
        /// The item type.
        /// </param>
        public static void AttachInterestArea(Game game, string itemId, byte? itemType)
        {
            var data = new Dictionary<byte, object>();

            if (!string.IsNullOrEmpty(itemId))
            {
                data.Add((byte)ParameterCode.ItemId, itemId);
            }

            if (itemType.HasValue)
            {
                data.Add((byte)ParameterCode.ItemType, itemType.Value);
            }

            game.SendOperation(OperationCode.AttachInterestArea, data, true, Settings.ItemChannel);
        }

        /// <summary>
        /// The counter subscribe.
        /// </summary>
        /// <param name="peer">
        /// The photon peer.
        /// </param>
        /// <param name="receiveInterval">
        /// The receive interval.
        /// </param>
        public static void CounterSubscribe(PhotonPeer peer, int receiveInterval)
        {
            var data = new Dictionary<byte, object> { { (byte)ParameterCode.CounterReceiveInterval, receiveInterval } };
            peer.OpCustom((byte)OperationCode.SubscribeCounter, data, true, Settings.DiagnosticsChannel);
        }

        /// <summary>
        /// The create world.
        /// </summary>
        /// <param name="game">
        /// The mmo game.
        /// </param>
        /// <param name="worldName">
        /// The world name.
        /// </param>
        /// <param name="topLeftCorner">
        /// The top left corner.
        /// </param>
        /// <param name="bottomRightCorner">
        /// The bottom right corner.
        /// </param>
        /// <param name="tileDimensions">
        /// The tile dimensions.
        /// </param>
        public static void CreateWorld(Game game, string worldName, float[] topLeftCorner, float[] bottomRightCorner, float[] tileDimensions)
        {
            var data = new Dictionary<byte, object>
                {
                    { (byte)ParameterCode.WorldName, worldName }, 
                    { (byte)ParameterCode.TopLeftCorner, topLeftCorner }, 
                    { (byte)ParameterCode.BottomRightCorner, bottomRightCorner }, 
                    { (byte)ParameterCode.TileDimensions, tileDimensions }
                };
            game.SendOperation(OperationCode.CreateWorld, data, true, Settings.OperationChannel);
        }

        /// <summary>
        /// The destroy item.
        /// </summary>
        /// <param name="game">
        /// The mmo game.
        /// </param>
        /// <param name="itemId">
        /// The item id.
        /// </param>
        /// <param name="itemType">
        /// The item type.
        /// </param>
        public static void DestroyItem(Game game, string itemId, byte itemType)
        {
            var data = new Dictionary<byte, object> { { (byte)ParameterCode.ItemId, itemId }, { (byte)ParameterCode.ItemType, itemType } };
            game.SendOperation(OperationCode.DestroyItem, data, true, Settings.ItemChannel);
        }

        /// <summary>
        /// The detach camera.
        /// </summary>
        /// <param name="game">
        /// The mmo game.
        /// </param>
        public static void DetachInterestArea(Game game)
        {
            game.SendOperation(OperationCode.DetachInterestArea, new Dictionary<byte, object>(), true, Settings.ItemChannel);
        }

        /// <summary>
        /// The enter world.
        /// </summary>
        /// <param name="game">
        /// The mmo game.
        /// </param>
        /// <param name="worldName">
        /// The world name.
        /// </param>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="properties">
        /// The properties.
        /// </param>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <param name="rotation">
        /// The rotation.
        /// </param>
        /// <param name="viewDistanceEnter">
        /// The view Distance Enter.
        /// </param>
        /// <param name="viewDistanceExit">
        /// The view Distance Exit.
        /// </param>
        public static void EnterWorld(
            Game game, string worldName, string username, Hashtable properties, float[] position, float[] rotation, float[] viewDistanceEnter, float[] viewDistanceExit)
        {
            var data = new Dictionary<byte, object>
                {
                    { (byte)ParameterCode.WorldName, worldName }, 
                    { (byte)ParameterCode.Username, username }, 
                    { (byte)ParameterCode.Position, position }, 
                    { (byte)ParameterCode.ViewDistanceEnter, viewDistanceEnter }, 
                    { (byte)ParameterCode.ViewDistanceExit, viewDistanceExit }
                };
            if (properties != null)
            {
                data.Add((byte)ParameterCode.Properties, properties);
            }

            if (rotation != null)
            {
                data.Add((byte)ParameterCode.Rotation, rotation);
            }

            game.SendOperation(OperationCode.EnterWorld, data, true, Settings.OperationChannel);
        }

        /// <summary>
        /// The exit world.
        /// </summary>
        /// <param name="game">
        /// The mmo game.
        /// </param>
        public static void ExitWorld(Game game)
        {
            game.SendOperation(OperationCode.ExitWorld, new Dictionary<byte, object>(), true, Settings.OperationChannel);
        }

        /// <summary>
        /// The get properties.
        /// </summary>
        /// <param name="game">
        /// The mmo game.
        /// </param>
        /// <param name="itemId">
        /// The item id.
        /// </param>
        /// <param name="itemType">
        /// The item type.
        /// </param>
        /// <param name="knownRevision">
        /// The known revision.
        /// </param>
        public static void GetProperties(Game game, string itemId, byte itemType, int? knownRevision)
        {
            var data = new Dictionary<byte, object> { { (byte)ParameterCode.ItemId, itemId }, { (byte)ParameterCode.ItemType, itemType } };
            if (knownRevision.HasValue)
            {
                data.Add((byte)ParameterCode.PropertiesRevision, knownRevision.Value);
            }

            game.SendOperation(OperationCode.GetProperties, data, true, Settings.ItemChannel);
        }

        /// <summary>
        /// The move operation.
        /// </summary>
        /// <param name="game">
        /// The mmo game.
        /// </param>
        /// <param name="itemId">
        /// The item id.
        /// </param>
        /// <param name="itemType">
        /// The item type.
        /// </param>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <param name="rotation">
        /// The rotation.
        /// </param>
        /// <param name="sendReliable">
        /// The send Reliable.
        /// </param>
        public static void Move(Game game, string itemId, byte? itemType, float[] position, float[] rotation, bool sendReliable)
        {
            var data = new Dictionary<byte, object> { { (byte)ParameterCode.Position, position } };
            if (itemId != null)
            {
                data.Add((byte)ParameterCode.ItemId, itemId);
            }

            if (itemType.HasValue)
            {
                data.Add((byte)ParameterCode.ItemType, itemType.Value);
            }

            if (rotation != null)
            {
                data.Add((byte)ParameterCode.Rotation, rotation);
            }

            game.SendOperation(OperationCode.Move, data, sendReliable, Settings.ItemChannel);
        }

        /// <summary>
        /// The move camera.
        /// </summary>
        /// <param name="game">
        /// The mmo game.
        /// </param>
        /// <param name="cameraId">
        /// The camera id.
        /// </param>
        /// <param name="position">
        /// The position.
        /// </param>
        public static void MoveInterestArea(Game game, byte cameraId, float[] position)
        {
            var data = new Dictionary<byte, object> { { (byte)ParameterCode.InterestAreaId, cameraId }, { (byte)ParameterCode.Position, position } };

            game.SendOperation(OperationCode.MoveInterestArea, data, true, Settings.ItemChannel);
        }

        /// <summary>
        /// The radar subscribe.
        /// </summary>
        /// <param name="peer">
        /// The photon peer.
        /// </param>
        /// <param name="worldName">
        /// The world Name.
        /// </param>
        public static void RadarSubscribe(PhotonPeer peer, string worldName)
        {
            var data = new Dictionary<byte, object> { { (byte)ParameterCode.WorldName, worldName } };
            peer.OpCustom((byte)OperationCode.RadarSubscribe, data, true, Settings.RadarChannel);
        }

        /// <summary>
        /// The raise generic event.
        /// </summary>
        /// <param name="game">
        /// The mmo game.
        /// </param>
        /// <param name="itemId">
        /// The item id.
        /// </param>
        /// <param name="itemType">
        /// The item type.
        /// </param>
        /// <param name="customEventCode">
        /// The custom event code.
        /// </param>
        /// <param name="eventData">
        /// The event data.
        /// </param>
        /// <param name="eventReliability">
        /// The event reliability.
        /// </param>
        /// <param name="eventReceiver">
        /// The event receiver.
        /// </param>
        public static void RaiseGenericEvent(
            Game game, string itemId, byte? itemType, byte customEventCode, object eventData, byte eventReliability, EventReceiver eventReceiver)
        {
            var data = new Dictionary<byte, object>
                {
                    { (byte)ParameterCode.CustomEventCode, customEventCode }, 
                    { (byte)ParameterCode.EventReliability, eventReliability }, 
                    { (byte)ParameterCode.EventReceiver, (byte)eventReceiver }
                };

            if (eventData != null)
            {
                data.Add((byte)ParameterCode.EventData, eventData);
            }

            if (itemId != null)
            {
                data.Add((byte)ParameterCode.ItemId, itemId);
            }

            if (itemType.HasValue)
            {
                data.Add((byte)ParameterCode.ItemType, itemType.Value);
            }

            game.SendOperation(OperationCode.RaiseGenericEvent, data, true, Settings.ItemChannel);
        }

        /// <summary>
        /// The remove interest area.
        /// </summary>
        /// <param name="game">
        /// The mmo game.
        /// </param>
        /// <param name="cameraId">
        /// The camera id.
        /// </param>
        public static void RemoveInterestArea(Game game, byte cameraId)
        {
            var data = new Dictionary<byte, object> { { (byte)ParameterCode.InterestAreaId, cameraId } };

            game.SendOperation(OperationCode.RemoveInterestArea, data, true, Settings.ItemChannel);
        }

        /// <summary>
        /// The set properties.
        /// </summary>
        /// <param name="game">
        /// The mmo game.
        /// </param>
        /// <param name="itemId">
        /// The item id.
        /// </param>
        /// <param name="itemType">
        /// The item type.
        /// </param>
        /// <param name="propertiesSet">
        /// The properties set.
        /// </param>
        /// <param name="propertiesUnset">
        /// The properties unset.
        /// </param>
        /// <param name="sendReliable">
        /// The send Reliable.
        /// </param>
        public static void SetProperties(Game game, string itemId, byte? itemType, Hashtable propertiesSet, ArrayList propertiesUnset, bool sendReliable)
        {
            var data = new Dictionary<byte, object>();
            if (propertiesSet != null)
            {
                data.Add((byte)ParameterCode.PropertiesSet, propertiesSet);
            }

            if (propertiesUnset != null)
            {
                data.Add((byte)ParameterCode.PropertiesUnset, propertiesUnset);
            }

            if (itemId != null)
            {
                data.Add((byte)ParameterCode.ItemId, itemId);
            }

            if (itemType.HasValue)
            {
                data.Add((byte)ParameterCode.ItemType, itemType.Value);
            }

            game.SendOperation(OperationCode.SetProperties, data, sendReliable, Settings.ItemChannel);
        }

        /// <summary>
        /// The set view distance.
        /// </summary>
        /// <param name="game">
        /// The mmo game.
        /// </param>
        /// <param name="viewDistanceEnter">
        /// The view Distance Enter.
        /// </param>
        /// <param name="viewDistanceExit">
        /// The view Distance Exit.
        /// </param>
        public static void SetViewDistance(Game game, float[] viewDistanceEnter, float[] viewDistanceExit)
        {
            var data = new Dictionary<byte, object>
                {
                    { (byte)ParameterCode.ViewDistanceEnter, viewDistanceEnter }, { (byte)ParameterCode.ViewDistanceExit, viewDistanceExit } 
                };
            game.SendOperation(OperationCode.SetViewDistance, data, true, Settings.ItemChannel);
        }

        /// <summary>
        /// The spawn item.
        /// </summary>
        /// <param name="game">
        /// The mmo game.
        /// </param>
        /// <param name="itemId">
        /// The item id.
        /// </param>
        /// <param name="itemType">
        /// The item type.
        /// </param>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <param name="rotation">
        /// The rotation.
        /// </param>
        /// <param name="properties">
        /// The properties.
        /// </param>
        /// <param name="subscribe">
        /// The subscribe.
        /// </param>
        public static void SpawnItem(Game game, string itemId, byte itemType, float[] position, float[] rotation, Hashtable properties, bool subscribe)
        {
            var data = new Dictionary<byte, object>
                {
                    { (byte)ParameterCode.Position, position }, 
                    { (byte)ParameterCode.ItemId, itemId }, 
                    { (byte)ParameterCode.ItemType, itemType }, 
                    { (byte)ParameterCode.Subscribe, subscribe }
                };
            if (properties != null)
            {
                data.Add((byte)ParameterCode.Properties, properties);
            }

            if (rotation != null)
            {
                data.Add((byte)ParameterCode.Rotation, rotation);
            }

            game.SendOperation(OperationCode.SpawnItem, data, true, Settings.ItemChannel);
        }

        /// <summary>
        /// The subscribe item.
        /// </summary>
        /// <param name="game">
        /// The mmo game.
        /// </param>
        /// <param name="itemId">
        /// The item id.
        /// </param>
        /// <param name="itemType">
        /// The item type.
        /// </param>
        /// <param name="propertiesRevision">
        /// The properties revision.
        /// </param>
        public static void SubscribeItem(Game game, string itemId, byte itemType, int? propertiesRevision)
        {
            var data = new Dictionary<byte, object> { { (byte)ParameterCode.ItemId, itemId }, { (byte)ParameterCode.ItemType, itemType } };
            if (propertiesRevision.HasValue)
            {
                data.Add((byte)ParameterCode.PropertiesRevision, propertiesRevision);
            }

            game.SendOperation(OperationCode.SubscribeItem, data, true, Settings.ItemChannel);
        }

        /// <summary>
        /// The unsubscribe item.
        /// </summary>
        /// <param name="game">
        /// The mmo game.
        /// </param>
        /// <param name="itemId">
        /// The item id.
        /// </param>
        /// <param name="itemType">
        /// The item type.
        /// </param>
        public static void UnsubscribeItem(Game game, string itemId, byte itemType)
        {
            var data = new Dictionary<byte, object> { { (byte)ParameterCode.ItemId, itemId }, { (byte)ParameterCode.ItemType, itemType } };

            game.SendOperation(OperationCode.UnsubscribeItem, data, true, Settings.ItemChannel);
        }
    }
}