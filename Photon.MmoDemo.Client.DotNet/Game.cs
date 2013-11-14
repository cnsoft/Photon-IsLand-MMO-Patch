// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Game.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The game logic.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Client
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using ExitGames.Client.Photon;

    using Photon.MmoDemo.Client.GameStateStrategies;
    using Photon.MmoDemo.Common;

    /// <summary>
    ///   The game logic.
    /// </summary>
    [CLSCompliant(false)]
    public class Game : IPhotonPeerListener
    {
        #region Constants and Fields

        /// <summary>
        ///   The move down.
        /// </summary>
        public static readonly float[] MoveDown = new float[] { 0, 1 };

        /// <summary>
        ///   The move down left.
        /// </summary>
        public static readonly float[] MoveDownLeft = new float[] { -1, 1 };

        /// <summary>
        ///   The move down right.
        /// </summary>
        public static readonly float[] MoveDownRight = new float[] { 1, 1 };

        /// <summary>
        ///   The move left.
        /// </summary>
        public static readonly float[] MoveLeft = new float[] { -1, 0 };

        /// <summary>
        ///   The move right.
        /// </summary>
        public static readonly float[] MoveRight = new float[] { 1, 0 };

        /// <summary>
        ///   The move up.
        /// </summary>
        public static readonly float[] MoveUp = new float[] { 0, -1 };

        /// <summary>
        ///   The move up left.
        /// </summary>
        public static readonly float[] MoveUpLeft = new float[] { -1, -1 };

        /// <summary>
        ///   The move up right.
        /// </summary>
        public static readonly float[] MoveUpRight = new float[] { 1, -1 };

        /// <summary>
        ///   The avatar.
        /// </summary>
        private readonly MyItem avatar;

        /// <summary>
        ///   The camera.
        /// </summary>
        private readonly Dictionary<byte, InterestArea> cameras = new Dictionary<byte, InterestArea>();

        /// <summary>
        ///   The my item cache.
        /// </summary>
        private readonly Dictionary<byte, Dictionary<string, Item>> itemCache = new Dictionary<byte, Dictionary<string, Item>>();

        /// <summary>
        ///   The listener.
        /// </summary>
        private readonly IGameListener listener;

        /// <summary>
        ///   The settings.
        /// </summary>
        private readonly Settings settings;

        /// <summary>
        ///   The outgoing operation count.
        /// </summary>
        private int outgoingOperationCount;

        /// <summary>
        ///   The photon peer.
        /// </summary>
        private PhotonPeer peer;

        /// <summary>
        ///   The event dispatcher.
        /// </summary>
        private IGameLogicStrategy stateStrategy;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Game" /> class.
        /// </summary>
        /// <param name = "listener">
        ///   The listener.
        /// </param>
        /// <param name = "settings">
        ///   The settings.
        /// </param>
        /// <param name = "avatarName">
        ///   The avatar Name.
        /// </param>
        public Game(IGameListener listener, Settings settings, string avatarName)
        {
            this.listener = listener;
            this.settings = settings;

            this.avatar = new MyItem(Guid.NewGuid().ToString(), (byte)ItemType.Avatar, this, avatarName);
            this.avatar.AddVisibleInterestArea(0);

            this.AddItem(this.Avatar);
            this.AddCamera(new InterestArea(0, this, this.avatar));
            this.WorldData = new WorldData
                {
                    TopLeftCorner = new[] { 1f, 1f }, 
                    BottomRightCorner = new float[] { this.settings.GridSize[0], this.settings.GridSize[1] }, 
                    Name = this.settings.WorldName, 
                    TileDimensions = new float[] { this.settings.TileDimensions[0], this.settings.TileDimensions[1] }
                };

            this.stateStrategy = Disconnected.Instance;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets Avatar.
        /// </summary>
        public MyItem Avatar
        {
            get
            {
                return this.avatar;
            }
        }

        /// <summary>
        ///   Gets Items.
        /// </summary>
        public Dictionary<byte, Dictionary<string, Item>> Items
        {
            get
            {
                return this.itemCache;
            }
        }

        /// <summary>
        ///   Gets Listener.
        /// </summary>
        public IGameListener Listener
        {
            get
            {
                return this.listener;
            }
        }

        /// <summary>
        ///   Gets Peer.
        /// </summary>
        public PhotonPeer Peer
        {
            get
            {
                return this.peer;
            }
        }

        /// <summary>
        ///   Gets Settings.
        /// </summary>
        public Settings Settings
        {
            get
            {
                return this.settings;
            }
        }

        /// <summary>
        ///   Gets State.
        /// </summary>
        public GameState State
        {
            get
            {
                return this.stateStrategy.State;
            }
        }

        /// <summary>
        ///   Gets WorldData.
        /// </summary>
        public WorldData WorldData { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///   The add camera.
        /// </summary>
        /// <param name = "camera">
        ///   The camera.
        /// </param>
        public void AddCamera(InterestArea camera)
        {
            this.cameras.Add(camera.Id, camera);
        }

        /// <summary>
        ///   The add item.
        /// </summary>
        /// <param name = "item">
        ///   The added item.
        /// </param>
        public void AddItem(Item item)
        {
            Dictionary<string, Item> typedItems;
            if (this.Items.TryGetValue(item.Type, out typedItems) == false)
            {
                typedItems = new Dictionary<string, Item>();
                this.Items.Add(item.Type, typedItems);
            }

            typedItems.Add(item.Id, item);
            this.listener.OnItemAdded(this, item);
        }

        /// <summary>
        ///   The disconnect.
        /// </summary>
        public void Disconnect()
        {
            this.peer.Disconnect();
        }

        /// <summary>
        ///   The initialize.
        /// </summary>
        /// <param name = "photonPeer">
        ///   The photon peer.
        /// </param>
        public void Initialize(PhotonPeer photonPeer)
        {
            this.peer = photonPeer;
            this.stateStrategy = WaitingForConnect.Instance;
            photonPeer.Connect(this.settings.ServerAddress, this.settings.ApplicationName);
        }

        /// <summary>
        ///   The on camera attached.
        /// </summary>
        /// <param name = "itemId">
        ///   The item Id.
        /// </param>
        /// <param name = "itemType">
        ///   The item Type.
        /// </param>
        public void OnCameraAttached(string itemId, byte itemType)
        {
            this.listener.OnCameraAttached(itemId, itemType);
            this.avatar.AddVisibleInterestArea(0);
        }

        /// <summary>
        ///   The on camera detached.
        /// </summary>
        public void OnCameraDetached()
        {
            this.listener.OnCameraDetached();
            this.avatar.RemoveVisibleInterestArea(0);
        }

        /// <summary>
        ///   The on item spawned.
        /// </summary>
        /// <param name = "itemType">
        ///   The item type.
        /// </param>
        /// <param name = "itemId">
        ///   The item id.
        /// </param>
        public void OnItemSpawned(byte itemType, string itemId)
        {
            this.listener.OnItemSpawned(itemType, itemId);
        }

        /// <summary>
        ///   The on unexpected event receive.
        /// </summary>
        /// <param name = "event">
        ///   The event.
        /// </param>
        public void OnUnexpectedEventReceive(EventData @event)
        {
            this.listener.LogError(this, string.Format("{0}: unexpected event {1}", this.avatar.Text, @event.Code));
        }

        /// <summary>
        /// The on unexpected operation error.
        /// </summary>
        /// <param name="operationResponse">
        /// The operation Response.
        /// </param>
        public void OnUnexpectedOperationError(OperationResponse operationResponse)
        {
            string message = string.Format(
                "{0}: unexpected operation error {1} from operation {2} in state {3}", 
                this.avatar.Text, 
                operationResponse.DebugMessage, 
                operationResponse.ReturnCode, 
                this.stateStrategy.State);
            this.listener.LogError(this, message);
        }

        /// <summary>
        /// The on unexpected photon return.
        /// </summary>
        /// <param name="operationResponse">
        /// The operation Response.
        /// </param>
        public void OnUnexpectedPhotonReturn(OperationResponse operationResponse)
        {
            this.listener.LogError(this, string.Format("{0}: unexpected return {1}", this.avatar.Text, operationResponse.OperationCode));
        }

        /// <summary>
        ///   The remove camera.
        /// </summary>
        /// <param name = "cameraId">
        ///   The camera id.
        /// </param>
        /// <returns>
        ///   true if camera was found.
        /// </returns>
        public bool RemoveCamera(byte cameraId)
        {
            return this.cameras.Remove(cameraId);
        }

        /// <summary>
        ///   The remove item.
        /// </summary>
        /// <param name = "item">
        ///   The removed item.
        /// </param>
        /// <returns>
        ///   true if item was found.
        /// </returns>
        public bool RemoveItem(Item item)
        {
            Dictionary<string, Item> typedItems;
            if (this.itemCache.TryGetValue(item.Type, out typedItems))
            {
                if (typedItems.Remove(item.Id))
                {
                    if (typedItems.Count == 0)
                    {
                        this.itemCache.Remove(item.Type);
                    }

                    this.listener.OnItemRemoved(this, item);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///   The send operation.
        /// </summary>
        /// <param name = "operationCode">
        ///   The operation code.
        /// </param>
        /// <param name = "parameter">
        ///   The parameter.
        /// </param>
        /// <param name = "sendReliable">
        ///   The send reliable.
        /// </param>
        /// <param name = "channelId">
        ///   The channel Id.
        /// </param>
        public void SendOperation(OperationCode operationCode, Dictionary<byte, object> parameter, bool sendReliable, byte channelId)
        {
            if (this.listener.IsDebugLogEnabled)
            {
                var builder = new StringBuilder();
                builder.AppendFormat("{0}: send operation {1}:", this.avatar.Id, operationCode);
                foreach (var entry in parameter)
                {
                    builder.AppendFormat(" {0}=", (ParameterCode)entry.Key);
                    if (entry.Value is float[])
                    {
                        builder.Append("float[");
                        foreach (float number in (float[])entry.Value)
                        {
                            builder.AppendFormat("{0:0.00},", number);
                        }

                        builder.Append("]");
                    }
                    else
                    {
                        builder.Append(entry.Value);
                    }
                }

                this.listener.LogDebug(this, builder.ToString());
            }

            this.stateStrategy.SendOperation(this, operationCode, parameter, sendReliable, channelId);

            // avoid operation congestion (QueueOutgoingUnreliableWarning)
            this.outgoingOperationCount++;
            if (this.outgoingOperationCount > 10)
            {
                this.peer.SendOutgoingCommands();
                this.outgoingOperationCount = 0;
            }
        }

        /// <summary>
        ///   The set connected.
        /// </summary>
        public void SetConnected()
        {
            this.stateStrategy = Connected.Instance;
            this.listener.OnConnect(this);
        }

        /// <summary>
        ///   The set disconnected.
        /// </summary>
        /// <param name = "returnCode">
        ///   The return code.
        /// </param>
        public void SetDisconnected(StatusCode returnCode)
        {
            this.stateStrategy = Disconnected.Instance;
            this.listener.OnDisconnect(this, returnCode);
        }

        /// <summary>
        ///   The set state world entered.
        /// </summary>
        /// <param name = "worldData">
        ///   The world Data.
        /// </param>
        public void SetStateWorldEntered(WorldData worldData)
        {
            this.WorldData = worldData;
            this.stateStrategy = WorldEntered.Instance;
            InterestArea camera;
            this.cameras.TryGetValue(0, out camera);
            camera.ResetViewDistance();

            var r = new Random();
            var position = new float[]
                {
                    r.Next((int)this.WorldData.TopLeftCorner[0], (int)this.WorldData.BottomRightCorner[0]), 
                    r.Next((int)this.WorldData.TopLeftCorner[1], (int)this.WorldData.BottomRightCorner[1])
                };
            this.avatar.SetPositions(position, position, null, null);

            this.listener.OnWorldEntered(this);
        }

        /// <summary>
        ///   The try get camera.
        /// </summary>
        /// <param name = "cameraId">
        ///   The camera id.
        /// </param>
        /// <param name = "camera">
        ///   The camera.
        /// </param>
        /// <returns>
        ///   true if camera was found.
        /// </returns>
        public bool TryGetCamera(byte cameraId, out InterestArea camera)
        {
            return this.cameras.TryGetValue(cameraId, out camera);
        }

        /// <summary>
        ///   The try get item.
        /// </summary>
        /// <param name = "itemType">
        ///   The item type.
        /// </param>
        /// <param name = "itemid">
        ///   The itemid.
        /// </param>
        /// <param name = "item">
        ///   The result item.
        /// </param>
        /// <returns>
        ///   true if item was found.
        /// </returns>
        public bool TryGetItem(byte itemType, string itemid, out Item item)
        {
            Dictionary<string, Item> typedItems;
            if (this.itemCache.TryGetValue(itemType, out typedItems))
            {
                return typedItems.TryGetValue(itemid, out item);
            }

            item = null;
            return false;
        }

        /// <summary>
        ///   The update.
        /// </summary>
        public void Update()
        {
            this.stateStrategy.OnUpdate(this);
        }

        #endregion

        #region Implemented Interfaces

        #region IPhotonPeerListener

        /// <summary>
        ///   The debug return.
        /// </summary>
        /// <param name = "debugLevel">
        ///   The debug Level.
        /// </param>
        /// <param name = "debug">
        ///   The debug.
        /// </param>
        public void DebugReturn(DebugLevel debugLevel, string debug)
        {
            if (this.listener.IsDebugLogEnabled)
            {
                // we don't use debugLevel here - just log what's there
                this.listener.LogDebug(this, string.Concat(this.avatar.Id, ": ", debug));
            }
        }

        public void OnEvent(EventData ev)
        {
            if (this.listener.IsDebugLogEnabled)
            {
                var builder = new StringBuilder();
                builder.AppendFormat("{0}: received event {1}:", this.avatar.Id, (EventCode)ev.Code);
                foreach (var entry in ev.Parameters)
                {
                    builder.AppendFormat(" {0}=", (ParameterCode)entry.Key);
                    if (entry.Value is float[])
                    {
                        builder.Append("float[");
                        foreach (float number in (float[])entry.Value)
                        {
                            builder.AppendFormat("{0:0.00},", number);
                        }

                        builder.Append("]");
                    }
                    else
                    {
                        builder.Append(entry.Value);
                    }
                }

                this.listener.LogDebug(this, builder.ToString());
            }

            this.stateStrategy.OnEventReceive(this, ev);
        }

        public void OnOperationResponse(OperationResponse operationResponse)
        {
            try
            {
                if (this.listener.IsDebugLogEnabled)
                {
                    this.listener.LogDebug(this, string.Format("{0}: received return {1}", this.avatar.Id, operationResponse.ReturnCode));
                }

                this.stateStrategy.OnOperationReturn(this, operationResponse);
            }
            catch (Exception e)
            {
                this.listener.LogError(this, e);
            }
        }

        public void OnStatusChanged(StatusCode returnCode)
        {
            try
            {
                if (this.listener.IsDebugLogEnabled)
                {
                    this.listener.LogDebug(this, string.Format("{0}: received callback {1}", this.avatar.Id, returnCode));
                }

                this.stateStrategy.OnPeerStatusCallback(this, returnCode);
            }
            catch (Exception e)
            {
                this.listener.LogError(this, e);
            }
        }

        #endregion

        #endregion
    }
}