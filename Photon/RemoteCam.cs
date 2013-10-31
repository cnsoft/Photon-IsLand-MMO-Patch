// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteCam.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The remote cam.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Photon.MmoDemo.Client;

using UnityEngine;

/// <summary>
/// The remote cam.
/// </summary>
public class RemoteCam : MonoBehaviour
{
    /// <summary>
    /// The interest area.
    /// </summary>
    private InterestArea interestArea;

    /// <summary>
    /// The next move time.
    /// </summary>
    private float nextMoveTime;


    private Vector3 direction;

    private float height;

    /// <summary>
    /// The destroy.
    /// </summary>
    public void Destroy()
    {
        this.interestArea.Remove();
        Destroy(this.gameObject);
        Destroy(this);
        Debug.Log("destroy interest area");
    }

    /// <summary>
    /// The initialize.
    /// </summary>
    /// <param name="game">
    /// The game.
    /// </param>
    /// <param name="cameraId">
    /// The camera Id.
    /// </param>
    /// <param name="position">
    /// The position.
    /// </param>
    /// <param name="moveDirection">
    /// The direction of movement
    /// </param>
    public void Initialize(Game game, byte cameraId, Vector3 position, Vector3 moveDirection)
    {
        this.interestArea = new InterestArea(cameraId, game, Player.GetPosition(position));
        this.interestArea.ResetViewDistance();
        this.direction = moveDirection;
        this.nextMoveTime = Time.time + 0.05f;
        this.interestArea.Create();
        this.transform.position = position;

        float terrainHeight = Terrain.activeTerrain.SampleHeight(position);
        this.height = position.y - terrainHeight;
    }

    /// <summary>
    /// The update.
    /// </summary>
    public void Update()
    {
        if (Vector3.Distance(this.transform.position, GameObject.Find("First Person Controller Prefab").transform.position) > 1000)
        {
            this.Destroy();
            return;
        }

        Vector3 targetPos = this.transform.position + this.direction;
        targetPos.y = Terrain.activeTerrain.SampleHeight(targetPos) + this.height;

        this.transform.position = Vector3.Lerp(this.transform.position, targetPos, Time.time / 2f);

        if (Time.time > this.nextMoveTime)
        {
            this.interestArea.Move(Player.GetPosition(this.transform.position));

            // up to 20 times per second
            this.nextMoveTime = Time.time + 0.05f;
        }
    }
}