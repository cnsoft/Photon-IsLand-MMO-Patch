// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Settings.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

/// <summary>
/// The settings.
/// </summary>
public class Settings : Photon.MmoDemo.Client.Settings
{
    /// <summary>
    /// The get default settings.
    /// </summary>
    /// <returns>
    /// default settings
    /// </returns>
    public static Settings GetDefaultSettings()
    {
        // terrain is 2000x2000
        const int BoxesVertical = 20;
        const int BoxesHorizontal = 20;
        const int EdgeLengthVertical = 1000;
        const int EdgeLengthHorizontal = 1000;

        const int IntervalSend = 30;
        const int Velocity = 1;
        const bool SendReliable = false;

        const bool UseTcp = false;
        const string ServerAddress = "localhost:5055";
        const string ApplicationName = "MmoDemo";
        const string WorldName = "Unity3d-Island";

        int[] tileDimensions = new int[2];
        tileDimensions[0] = EdgeLengthHorizontal;
        tileDimensions[1] = EdgeLengthVertical;

        int[] gridSize = new int[2];
        gridSize[0] = BoxesHorizontal * EdgeLengthHorizontal;
        gridSize[1] = BoxesVertical * EdgeLengthVertical;

        Settings result = new Settings();

        // photon
        result.ServerAddress = ServerAddress;
        result.UseTcp = UseTcp;
        result.ApplicationName = ApplicationName;

        // grid
        result.WorldName = WorldName;
        result.TileDimensions = tileDimensions;
        result.GridSize = gridSize;

        // game engine
        result.SendInterval = IntervalSend;
        result.SendReliable = SendReliable;
        result.MoveVelocity = Velocity;

        return result;
    }


    /// <summary>
    /// The auto move velocity.
    /// </summary>
    private int moveVelocity;

    /// <summary>
    /// The send interval.
    /// </summary>
    private int sendInterval;

    /// <summary>
    /// The use tcp.
    /// </summary>
    private bool useTcp;

    /// <summary>
    /// Gets or sets MoveVelocity.
    /// </summary>
    public int MoveVelocity
    {
        get
        {
            return this.moveVelocity;
        }

        set
        {
            this.moveVelocity = value;
        }
    }

    /// <summary>
    /// Gets or sets IntervalSend.
    /// </summary>
    public int SendInterval
    {
        get
        {
            return this.sendInterval;
        }

        set
        {
            this.sendInterval = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether UseTcp.
    /// </summary>
    public bool UseTcp
    {
        get
        {
            return this.useTcp;
        }

        set
        {
            this.useTcp = value;
        }
    }
}