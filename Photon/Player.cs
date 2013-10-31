// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Player.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The player.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

using Photon.MmoDemo.Client;

using UnityEngine;

/// <summary>
/// The player.
/// </summary>
public class Player : MonoBehaviour
{
    /// <summary>
    /// The change text.
    /// </summary>
    private bool changeText = false;

    /// <summary>
    /// The engine.
    /// </summary>
    private Game engine;

    /// <summary>
    /// The last key press.
    /// </summary>
    private float lastKeyPress;

    /// <summary>
    /// The last move position.
    /// </summary>
    private Vector3 lastMovePosition;


    /// <summary>
    /// The last move rotation.
    /// </summary>
    private Vector3 lastMoveRotation;

    /// <summary>
    /// The next move time.
    /// </summary>
    private float nextMoveTime;

    /// <summary>
    /// The name text.
    /// </summary>
    private GUIText nameText;

    /// <summary>
    /// The view text.
    /// </summary>
    private GUIText viewText;

    /// <summary>
    /// The get position.
    /// </summary>
    /// <param name="position">
    /// The position.
    /// </param>
    /// <returns>
    /// the position as float array
    /// </returns>
    public static float[] GetPosition(Vector3 position)
    {
        float[] result = new float[3];
        result[0] = position.x * MmoEngine.PositionFactorHorizonal;
        result[1] = position.z * MmoEngine.PositionFactorVertical;
        result[2] = position.y;
        return result;
    }

    public static float[] GetRotation(Vector3 rotation)
    {
        float[] rotationValue = new float[3];
        rotationValue[0] = rotation.x;
        rotationValue[1] = rotation.y;
        rotationValue[2] = rotation.z;
        return rotationValue;
    }

    /// <summary>
    /// The initialize.
    /// </summary>
    /// <param name="engine">
    /// The engine.
    /// </param>
    public void Initialize(Game engine)
    {
        this.nextMoveTime = 0;
        this.engine = engine;
        this.nameText = (GUIText)GameObject.Find("PlayerNamePrefab").GetComponent("GUIText");
        this.viewText = (GUIText)GameObject.Find("ViewDistancePrefab").GetComponent("GUIText");
    }

    /// <summary>
    /// The start.
    /// </summary>
    public void Start()
    {
    }

    /// <summary>
    /// The update.
    /// </summary>
    public void Update()
    {
        try
        {
            if (this.engine != null)
            {
                this.nameText.text = this.engine.Avatar.Text;
                this.viewText.text = string.Format("{0:0} x {1:0}", this.engine.Avatar.ViewDistanceEnter[0], this.engine.Avatar.ViewDistanceEnter[1]);
                this.Move();
                this.ReadKeyboardInput();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    /// <summary>
    /// The move.
    /// </summary>
    private void Move()
    {
        if (Time.time > this.nextMoveTime)
        {
            Vector3 rotation = this.transform.rotation.eulerAngles;
            if (this.lastMovePosition != this.transform.position || this.lastMoveRotation != rotation)
            {
                this.engine.Avatar.MoveAbsolute(GetPosition(this.transform.position), GetRotation(rotation));
                this.lastMovePosition = this.transform.position;
                this.lastMoveRotation = rotation;
            }

            // up to 10 times per second
            this.nextMoveTime = Time.time + 0.1f;
        }
    }

    /// <summary>
    /// The read keyboard input.
    /// </summary>
    private void ReadKeyboardInput()
    {
        if (this.changeText)
        {
            if (Input.GetKey(KeyCode.Return))
            {
                this.changeText = false;
                return;
            }

            if (Input.GetKey(KeyCode.Backspace))
            {
                if (this.lastKeyPress + 0.1f < Time.time)
                {
                    if (this.engine.Avatar.Text.Length > 0)
                    {
                        this.engine.Avatar.SetText(this.engine.Avatar.Text.Remove(this.engine.Avatar.Text.Length - 1));
                        this.lastKeyPress = Time.time;
                    }
                }

                return;
            }

            this.engine.Avatar.SetText(this.engine.Avatar.Text + Input.inputString);
            return;
        }

        if (Input.GetKey(KeyCode.F1))
        {
            this.changeText = true;
            return;
        }

        // center
        if (Input.GetKey(KeyCode.Keypad5) || Input.GetKey(KeyCode.C))
        {
            if (this.lastKeyPress + 0.1f < Time.time)
            {
                float height = Terrain.activeTerrain.SampleHeight(new Vector3(1000, 0, 1000));
                this.transform.position = new Vector3(1000, height + 1, 1000);
                this.lastKeyPress = Time.time;
            }
        }

        // view distance
        if (Input.GetKey(KeyCode.KeypadPlus))
        {
            if (this.lastKeyPress + 0.05f < Time.time)
            {
                float[] viewDistance = (float[])this.engine.Avatar.ViewDistanceEnter.Clone();
                viewDistance[0] = Mathf.Min(this.engine.WorldData.Width, viewDistance[0] + 1);
                viewDistance[1] = Mathf.Min(this.engine.WorldData.Height, viewDistance[1] + 1);
                InterestArea cam;
                this.engine.TryGetCamera(0, out cam);
                cam.SetViewDistance(viewDistance);
                this.lastKeyPress = Time.time;
            }
        }

        if (Input.GetKey(KeyCode.KeypadMinus))
        {
            if (this.lastKeyPress + 0.05f < Time.time)
            {
                float[] viewDistance = (float[])this.engine.Avatar.ViewDistanceEnter.Clone();
                viewDistance[0] = Mathf.Max(this.engine.WorldData.Width, viewDistance[0] - 1);
                viewDistance[1] = Mathf.Max(this.engine.WorldData.Height, viewDistance[1] - 1);
                InterestArea cam;
                this.engine.TryGetCamera(0, out cam);
                cam.SetViewDistance(viewDistance);
                this.lastKeyPress = Time.time;
            }
        }

        if (Input.GetKey(KeyCode.KeypadEnter))
        {
            if (this.lastKeyPress + 0.05f < Time.time)
            {
                InterestArea cam;
                this.engine.TryGetCamera(0, out cam);
                cam.ResetViewDistance();
                this.lastKeyPress = Time.time;
            }
        }

        if (Input.GetKey(KeyCode.F5) || Input.GetKey(KeyCode.E))
        {
            if (this.lastKeyPress + 0.5f < Time.time)
            {
                InterestArea area;
                if (this.engine.TryGetCamera(1, out area))
                {
                    GameObject.Find("Sphere").GetComponent<RemoteCam>().Destroy();
                }

                Debug.Log("create interest area");
                GameObject actorCube = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                RemoteCam remoteCam = actorCube.AddComponent<RemoteCam>();
                remoteCam.Initialize(this.engine, 1, this.transform.position, UnityEngine.Camera.main.transform.forward);

                this.lastKeyPress = Time.time;
            }
        }
    }
}