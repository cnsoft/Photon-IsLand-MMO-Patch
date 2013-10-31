// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Radar.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The radar.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

using Photon.MmoDemo.Client;

using UnityEngine;

/// <summary>
/// The radar.
/// </summary>
public class Radar : MonoBehaviour
{
    // radar! by oPless from the original javascript by PsychicParrot, 
    // who in turn adapted it from a Blitz3d script found in the
    // public domain online somewhere ....

    /// <summary>
    /// The blip.
    /// </summary>
    public Texture blip;

    ////public Transform centerObject;
    ////public Vector2 mapCenter = new Vector2(50, 50);
    ////public float mapScale = 0.3f;
    ////public string tagFilter = "enemy";
    ////public float maxDist = 200;
    public WorldData world;
    public Vector2 mapPosition = new Vector2(10, 10);
    public Vector2 mapSize = new Vector2(100, 100);
    ////public Texture radarBG;
    public Texture blibSelf;
    public string selfId;
    public GUIStyle style = new GUIStyle(GUIStyle.none);


    /// <summary>
    /// The item positions.
    /// </summary>
    private readonly Dictionary<string, float[]> itemPositions = new Dictionary<string, float[]>();

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
    public void OnRadarUpdate(string itemId, byte itemType, float[] position)
    {
        itemId += itemType;
        if (position == null)
        {
            this.itemPositions.Remove(itemId);
            return;
        }

        if (!this.itemPositions.ContainsKey(itemId))
        {
            this.itemPositions.Add(itemId, position);
            return;
        }

        this.itemPositions[itemId] = position;
    }


    /// <summary>
    /// The on gui.
    /// </summary>
    public virtual void OnGUI()
    {
        try
        {
            if (this.itemPositions.Count == 0) return;

            float scaleX = this.mapSize.x / this.world.Width;
            float scaleY = this.mapSize.y / this.world.Height;

            Vector3 north = new Vector3(0, 0, -1);
            Vector3 direction = new Vector3(Camera.mainCamera.transform.forward.x, 0, Camera.mainCamera.transform.forward.z);
            Vector3 whichWay = Vector3.Cross(Camera.mainCamera.transform.forward, north - direction);
            float angle = Vector3.Angle(north, direction);
            if (whichWay.y < 0) angle *= -1;
          

            ////GUI.Label(new Rect(10, 20, 100, 100), angle.ToString());
            ////GUI.Label(new Rect(10, 40, 100, 100), whichWay.ToString());

            Vector2 center = new Vector2(this.mapPosition.x + (this.mapSize.x / 2), this.mapPosition.y + (this.mapSize.y / 2));
            Matrix4x4 oldMatrix = GUI.matrix;
            GUIUtility.RotateAroundPivot(angle, center);

            GUI.BeginGroup(new Rect(this.mapPosition.x, this.mapPosition.y, this.mapSize.x, this.mapSize.y));
            
            ////GUI.DrawTexture(new Rect(this.mapPosition.x, this.mapPosition.y, this.mapSize.x, this.mapSize.y), this.radarBG);
            foreach (KeyValuePair<string, float[]> pair in this.itemPositions)
            {
                float x = this.world.BottomRightCorner[0] - pair.Value[0];
                float y = pair.Value[1] - this.world.TopLeftCorner[1];
                x *= scaleX;
                y *= scaleY;

                GUI.DrawTexture(new Rect(x - 1, y - 1, 2, 2), this.blip);
            }

            const int LabelSize = 20;
            this.style.alignment = TextAnchor.UpperCenter;
            GUI.Label(new Rect((this.mapSize.x / 2f) - (LabelSize / 2f), 0, LabelSize, LabelSize), "N", this.style);
            this.style.alignment = TextAnchor.LowerCenter;
            GUI.Label(new Rect((this.mapSize.x / 2f) - (LabelSize / 2f), this.mapSize.y - LabelSize, LabelSize, LabelSize), "S", this.style);
            this.style.alignment = TextAnchor.MiddleLeft;
            GUI.Label(new Rect(0, (this.mapSize.y / 2f) - (LabelSize / 2f), LabelSize, LabelSize), "W", this.style);
            this.style.alignment = TextAnchor.MiddleRight;
            GUI.Label(new Rect(this.mapSize.x - LabelSize, (this.mapSize.y / 2f) - (LabelSize / 2f), LabelSize, LabelSize), "E", this.style);

            float myX = this.world.BottomRightCorner[0] - this.itemPositions[selfId][0];
            float myY = this.itemPositions[selfId][1] - this.world.TopLeftCorner[1];
            GUI.DrawTexture(new Rect((myX * scaleX) - 1, (myY * scaleY) - 1, 3, 3), this.blibSelf);

            GUI.EndGroup();

            GUI.matrix = oldMatrix;
            this.style.alignment = TextAnchor.MiddleLeft;
            GUI.Label(new Rect(this.mapPosition.x, this.mapPosition.y + this.mapSize.y, 100, 20), string.Format("Online: {0}", this.itemPositions.Count), this.style);


        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
}