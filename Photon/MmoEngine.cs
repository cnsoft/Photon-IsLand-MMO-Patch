// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MmoEngine.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The mmo engine.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

using ExitGames.Client.Photon;

using Photon.MmoDemo.Client;
using Photon.MmoDemo.Common;

using UnityEngine;

/// <summary>
/// The mmo engine.
/// </summary>
public class MmoEngine : Radar, IGameListener
{
    /// <summary>
    /// EdgeLengthHorizontal / BoxesHorizontal
    /// </summary>
    public const int PositionFactorHorizonal = 10;

    /// <summary>
    /// EdgeLengthVertical / BoxesVertical
    /// </summary>
    public const int PositionFactorVertical = 10;

    /// <summary>
    /// The cam height.
    /// </summary>
    private float camHeight;

    /// <summary>
    /// The engine.
    /// </summary>
    private Game engine;

    /// <summary>
    /// Gets a value indicating whether IsDebugLogEnabled.
    /// </summary>
    public bool IsDebugLogEnabled
    {
        get
        {
            return false;
        }
    }

    /// <summary>
    /// The on application quit.
    /// </summary>
    public void OnApplicationQuit()
    {
        try
        {
            this.engine.Disconnect();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    /// <summary>
    /// The on gui.
    /// </summary>
    public override void OnGUI()
    {
        base.OnGUI();

        if (Event.current.type == EventType.ScrollWheel)
        {
            if (Event.current.delta.y < 0)
            {
                this.IncreaseViewDistance();
            }
            else if (Event.current.delta.y > 0)
            {
                this.DecreaseViewDistance();
            }
        }
        else if (Event.current.type == EventType.MouseDown)
        {
            if (Event.current.button == 2)
            {
                InterestArea cam;
                this.engine.TryGetCamera(0, out cam);
                cam.ResetViewDistance();
            }
        }
    }

    /// <summary>
    /// The start.
    /// </summary>
    public void Start()
    {
        try
        {
            this.style.normal.textColor = Color.white;

            // Make the game run even when in background
            Application.runInBackground = true;

            if (this.IsDebugLogEnabled)
            {
                Debug.Log("Start");
            }

            Settings settings = Settings.GetDefaultSettings();
            this.engine = new Game(this, settings, "Unity");
            this.engine.Avatar.SetText("Unity");

            GameObject player = GameObject.Find("First Person Controller Prefab");
            this.engine.Avatar.MoveAbsolute(Player.GetPosition(player.transform.position), Player.GetRotation(player.transform.rotation.eulerAngles));
            this.engine.Avatar.ResetPreviousPosition();

            Photon.MmoDemo.Client.PhotonPeer peer = new Photon.MmoDemo.Client.PhotonPeer(this.engine, settings.UseTcp);
            this.engine.Initialize(peer);

            RTT rttBehaviour = (RTT)this.gameObject.AddComponent(typeof(RTT));
            rttBehaviour.Initialize(this.engine);

            this.camHeight = Camera.main.transform.position.y - Terrain.activeTerrain.SampleHeight(Camera.main.transform.position);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    /// <summary>
    /// The update.
    /// </summary>
    public void Update()
    {
        try
        {
            this.engine.Update();
            if (this.engine.State == GameState.WorldEntered)
            {
                this.OnRadarUpdate(this.engine.Avatar.Id, this.engine.Avatar.Type, this.engine.Avatar.Position);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    #region Implemented Interfaces

    #region IGameListener

    /// <summary>
    /// The log debug.
    /// </summary>
    /// <param name="game">
    /// The game.
    /// </param>
    /// <param name="message">
    /// The message.
    /// </param>
    public void LogDebug(Game game, string message)
    {
        Debug.Log(message);
    }

    /// <summary>
    /// The log error.
    /// </summary>
    /// <param name="game">
    /// The game.
    /// </param>
    /// <param name="message">
    /// The message.
    /// </param>
    public void LogError(Game game, string message)
    {
        Debug.Log(message);
    }

    /// <summary>
    /// The log error.
    /// </summary>
    /// <param name="game">
    /// The game.
    /// </param>
    /// <param name="errorCode">
    /// The error code.
    /// </param>
    /// <param name="debugMessage">
    /// The debug message.
    /// </param>
    /// <param name="operationCode">
    /// The operation code.
    /// </param>
    public void LogError(Game game, ReturnCode errorCode, string debugMessage, OperationCode operationCode)
    {
        Debug.Log(debugMessage);
    }

    /// <summary>
    /// The log error.
    /// </summary>
    /// <param name="game">
    /// The game.
    /// </param>
    /// <param name="exception">
    /// The exception.
    /// </param>
    public void LogError(Game game, Exception exception)
    {
        Debug.Log(exception.ToString());
    }

    /// <summary>
    /// The log info.
    /// </summary>
    /// <param name="game">
    /// The game.
    /// </param>
    /// <param name="message">
    /// The message.
    /// </param>
    public void LogInfo(Game game, string message)
    {
        Debug.Log(message);
    }

    /// <summary>
    /// The on camera attached.
    /// </summary>
    /// <exception cref="NotImplementedException">
    /// </exception>
    public void OnCameraAttached(string itemId, byte itemType)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// The on camera detached.
    /// </summary>
    /// <exception cref="NotImplementedException">
    /// </exception>
    public void OnCameraDetached()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// The on connect.
    /// </summary>
    /// <param name="game">
    /// The game.
    /// </param>
    public void OnConnect(Game game)
    {
        Debug.Log("connected");
    }

    /// <summary>
    /// The on disconnect.
    /// </summary>
    /// <param name="game">
    /// The game.
    /// </param>
    /// <param name="returnCode">
    /// The return code.
    /// </param>
    public void OnDisconnect(Game game, StatusCode returnCode)
    {
        Debug.Log("disconnected");
    }

    /// <summary>
    /// on item added
    /// </summary>
    /// <param name="game">
    /// The game.
    /// </param>
    /// <param name="item">
    /// The item.
    /// </param>
    public void OnItemAdded(Game game, Item item)
    {
        if (this.engine != null)
        {
            if (this.IsDebugLogEnabled)
            {
                Debug.Log("add item " + item.Id);
            }

            this.CreateActor(game, item);
        }
    }

    /// <summary>
    /// The on item removed.
    /// </summary>
    /// <param name="game">
    /// The game.
    /// </param>
    /// <param name="item">
    /// The item.
    /// </param>
    public void OnItemRemoved(Game game, Item item)
    {
        GameObject actor = GameObject.Find("Item" + item.Id);
        if (actor != null)
        {
            Actor behaviour = (Actor)actor.GetComponent(typeof(Actor));
            behaviour.Destroy();
            if (this.IsDebugLogEnabled)
            {
                Debug.Log("destroy item " + item.Id);
            }
        }
        else
        {
            if (this.IsDebugLogEnabled)
            {
                Debug.Log("destroy item NOT FOUND " + item.Id);
            }
        }
    }

    /// <summary>
    /// The on item spawned.
    /// </summary>
    /// <param name="itemType">
    /// The item type.
    /// </param>
    /// <param name="itemId">
    /// The item id.
    /// </param>
    public void OnItemSpawned(byte itemType, string itemId)
    {
    }

    /// <summary>
    /// The on world entered.
    /// </summary>
    /// <param name="game">
    /// The game.
    /// </param>
    public void OnWorldEntered(Game game)
    {
        Debug.Log("entered world " + game.WorldData.Name);
        GameObject player = GameObject.Find("First Person Controller Prefab");
        Player playerBehaviour = (Player)player.AddComponent(typeof(Player));
        playerBehaviour.Initialize(this.engine);
        this.world = game.WorldData;
        this.selfId = game.Avatar.Id + game.Avatar.Type;
        Operations.RadarSubscribe(game.Peer, game.WorldData.Name);
    }

    #endregion

    #endregion

    /// <summary>
    /// The create actor.
    /// </summary>
    /// <param name="engine">
    /// The engine.
    /// </param>
    /// <param name="actor">
    /// The actor.
    /// </param>
    private void CreateActor(Game engine, Item actor)
    {
        // do not show local actor
        if (actor != engine.Avatar)
        {
            GameObject actorCube = actor.Rotation != null ? GameObject.CreatePrimitive(PrimitiveType.Cube) : GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            Actor actorBehaviour = (Actor)actorCube.AddComponent(typeof(Actor));
            actorBehaviour.Initialize(actor, this.camHeight);
        }
    }

    /// <summary>
    /// The decrease view distance.
    /// </summary>
    private void DecreaseViewDistance()
    {
        InterestArea cam;
        this.engine.TryGetCamera(0, out cam);
        float[] viewDistance = (float[])cam.ViewDistanceEnter.Clone();
        viewDistance[0] = Math.Max(1, viewDistance[0] - (this.engine.WorldData.TileDimensions[0] / 2));
        viewDistance[1] = Math.Max(1, viewDistance[1] - (this.engine.WorldData.TileDimensions[1] / 2));
        cam.SetViewDistance(viewDistance);
    }
   
    /// <summary>
    /// The increase view distance.
    /// </summary>
    private void IncreaseViewDistance()
    {
        InterestArea cam;
        this.engine.TryGetCamera(0, out cam);
        float[] viewDistance = (float[])cam.ViewDistanceEnter.Clone();
        viewDistance[0] = Math.Min(this.engine.WorldData.Width, viewDistance[0] + (this.engine.WorldData.TileDimensions[0] / 2));
        viewDistance[1] = Math.Min(this.engine.WorldData.Height, viewDistance[1] + (this.engine.WorldData.TileDimensions[1] / 2));
        cam.SetViewDistance(viewDistance);
    }
}