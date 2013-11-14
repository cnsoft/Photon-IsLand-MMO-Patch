// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InterestArea.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The camera.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Client
{
    using System;

    /// <summary>
    /// The camera.
    /// </summary>
    public class InterestArea
    {
        /// <summary>
        /// The camera id.
        /// </summary>
        private readonly byte cameraId;

        /// <summary>
        /// The mmo game.
        /// </summary>
        private readonly Game game;

        /// <summary>
        /// Initializes a new instance of the <see cref="InterestArea"/> class. 
        /// </summary>
        /// <param name="cameraId">
        /// The camera Id.
        /// </param>
        /// <param name="game">
        /// The  mmo game.
        /// </param>
        /// <param name="avatar">
        /// The avatar.
        /// </param>
        [CLSCompliant(false)]
        public InterestArea(byte cameraId, Game game, MyItem avatar)
            : this(cameraId, game, avatar.Position)
        {
            this.AttachedItem = avatar;
            avatar.Moved += this.OnItemMoved;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InterestArea"/> class.
        /// </summary>
        /// <param name="cameraId">
        /// The camera id.
        /// </param>
        /// <param name="game">
        /// The mmo game.
        /// </param>
        /// <param name="position">
        /// The position.
        /// </param>
        [CLSCompliant(false)]
        public InterestArea(byte cameraId, Game game, float[] position)
        {
            this.game = game;
            this.cameraId = cameraId;
            this.Position = position;
        }

        /// <summary>
        /// Gets AttachedItem.
        /// </summary>
        public MyItem AttachedItem { get; private set; }

        /// <summary>
        /// Gets Game.
        /// </summary>
        [CLSCompliant(false)]
        public Game Game
        {
            get
            {
                return this.game;
            }
        }

        /// <summary>
        /// Gets camera Id.
        /// </summary>
        public byte Id
        {
            get
            {
                return this.cameraId;
            }
        }

        /// <summary>
        /// Gets Position.
        /// </summary>
        public float[] Position { get; private set; }

        /// <summary>
        /// Gets ViewDistanceEnter.
        /// </summary>
        public float[] ViewDistanceEnter { get; private set; }

        /// <summary>
        /// Gets ViewDistanceExit.
        /// </summary>
        public float[] ViewDistanceExit { get; private set; }

        /// <summary>
        /// The attach item.
        /// </summary>
        /// <param name="item">
        /// The mmo item.
        /// </param>
        public void AttachItem(MyItem item)
        {
            if (this.AttachedItem != null)
            {
                this.AttachedItem.Moved -= this.OnItemMoved;
                this.AttachedItem = null;
            }

            this.AttachedItem = item;
            item.Moved += this.OnItemMoved;

            Operations.AttachInterestArea(this.game, item.Id, item.Type);
            item.SetInterestAreaAttached(true);
        }

        /// <summary>
        /// The create.
        /// </summary>
        public void Create()
        {
            this.game.AddCamera(this);
            Operations.AddInterestArea(this.game, this.Id, this.Position, this.ViewDistanceEnter, this.ViewDistanceExit);
        }

        /// <summary>
        /// The detach.
        /// </summary>
        public void Detach()
        {
            if (this.AttachedItem != null)
            {
                this.AttachedItem.Moved -= this.OnItemMoved;
                this.AttachedItem.SetInterestAreaAttached(false);
                this.AttachedItem = null;
            }

            Operations.DetachInterestArea(this.game);
        }

        /// <summary>
        /// The move camera operation.
        /// </summary>
        /// <param name="newPosition">
        /// The new position.
        /// </param>
        public void Move(float[] newPosition)
        {
            if (this.AttachedItem == null)
            {
                this.Position = newPosition;
                Operations.MoveInterestArea(this.game, this.cameraId, newPosition);
                return;
            }

            throw new InvalidOperationException("cannot move attached interest area manually");
        }

        /// <summary>
        /// The remove.
        /// </summary>
        public void Remove()
        {
            Operations.RemoveInterestArea(this.game, this.Id);
            this.game.RemoveCamera(this.cameraId);
        }

        /// <summary>
        /// The reset view distance.
        /// </summary>
        public void ResetViewDistance()
        {
            this.SetViewDistance(new[] { (this.game.WorldData.TileDimensions[0] / 2) + 1, (this.game.WorldData.TileDimensions[1] / 2) + 1 });
        }

        /// <summary>
        /// The set view distance.
        /// </summary>
        /// <param name="viewDistance">
        /// The view Distance.
        /// </param>
        public void SetViewDistance(float[] viewDistance)
        {
            if (viewDistance[0] < 0)
            {
                viewDistance[0] = 0;
            }

            if (viewDistance[1] < 0)
            {
                viewDistance[1] = 0;
            }

            this.ViewDistanceEnter = viewDistance;

            this.ViewDistanceExit = new[]
                {
                    Math.Max(this.ViewDistanceEnter[0] + this.game.WorldData.TileDimensions[0], 1.5f * this.ViewDistanceEnter[0]), 
                    Math.Max(this.ViewDistanceEnter[1] + this.game.WorldData.TileDimensions[1], 1.5f * this.ViewDistanceEnter[1])
                };

            Operations.SetViewDistance(this.game, this.ViewDistanceEnter, this.ViewDistanceExit);
            this.game.Avatar.SetInterestAreaViewDistance(this);
        }

        /// <summary>
        /// The on item moved.
        /// </summary>
        /// <param name="item">
        /// The mmo item.
        /// </param>
        private void OnItemMoved(Item item)
        {
            this.Position = item.Position;
        }
    }
}