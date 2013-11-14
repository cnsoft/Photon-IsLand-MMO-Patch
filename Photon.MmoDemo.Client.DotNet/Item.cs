// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Item.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The item base.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Client
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The item base.
    /// </summary>
    public abstract class Item
    {
        /// <summary>
        /// The property key color.
        /// </summary>
        public static readonly string PropertyKeyColor = "color";

        /// <summary>
        /// The property key interest area attached.
        /// </summary>
        public static readonly string PropertyKeyInterestAreaAttached = "attached";

        /// <summary>
        /// The property key text.
        /// </summary>
        public static readonly string PropertyKeyText = "text";

        /// <summary>
        /// The property key view distance enter.
        /// </summary>
        public static readonly string PropertyKeyViewDistanceEnter = "enter";

        /// <summary>
        /// The property key view distance exit.
        /// </summary>
        public static readonly string PropertyKeyViewDistanceExit = "exit";

        /// <summary>
        /// The mmo game.
        /// </summary>
        private readonly Game game;

        /// <summary>
        /// The item id.
        /// </summary>
        private readonly string id;

        /// <summary>
        /// The subscribed interest areas.
        /// </summary>
        private readonly List<byte> subscribedInterestAreas;

        /// <summary>
        /// The item type.
        /// </summary>
        private readonly byte type;

        /// <summary>
        /// The visible interest areas.
        /// </summary>
        private readonly List<byte> visibleInterestAreas;

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class. 
        /// </summary>
        /// <param name="id">
        /// The item id.
        /// </param>
        /// <param name="type">
        /// The item type.
        /// </param>
        /// <param name="game">
        /// The mmo game.
        /// </param>
        [CLSCompliant(false)]
        protected Item(string id, byte type, Game game)
        {
            this.id = id;
            this.game = game;
            this.type = type;
            this.visibleInterestAreas = new List<byte>();
            this.subscribedInterestAreas = new List<byte>();
        }

        /// <summary>
        /// The moved.
        /// </summary>
        public event Action<Item> Moved;

        /// <summary>
        /// Gets Color.
        /// </summary>
        public int Color { get; private set; }

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
        /// Gets item Id.
        /// </summary>
        public string Id
        {
            get
            {
                return this.id;
            }
        }

        /// <summary>
        /// Gets a value indicating whether InterestAreaAttached.
        /// </summary>
        public bool InterestAreaAttached { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsDestroyed.
        /// </summary>
        public bool IsDestroyed { get; set; }

        /// <summary>
        /// Gets a value indicating whether IsMine.
        /// </summary>
        public abstract bool IsMine { get; }

        /// <summary>
        /// Gets a value indicating whether IsVisible.
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return this.visibleInterestAreas.Count > 0;
            }
        }

        /// <summary>
        /// Gets Position.
        /// </summary>
        public float[] Position { get; private set; }

        /// <summary>
        /// Gets Rotation.
        /// </summary>
        public float[] Rotation { get; private set; }

        /// <summary>
        /// Gets PreviousPosition.
        /// </summary>
        public float[] PreviousPosition { get; private set; }

        /// <summary>
        /// Gets PreviousRotation.
        /// </summary>
        public float[] PreviousRotation { get; private set; }

        /// <summary>
        /// Gets or sets PropertyRevision.
        /// </summary>
        public int PropertyRevision { get; set; }

        /// <summary>
        /// Gets Text.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Gets Type.
        /// </summary>
        public byte Type
        {
            get
            {
                return this.type;
            }
        }

        /// <summary>
        /// Gets ViewDistanceEnter.
        /// </summary>
        public float[] ViewDistanceEnter { get; private set; }

        /// <summary>
        /// Gets BViewDistanceExit.
        /// </summary>
        public float[] ViewDistanceExit { get; private set; }

        /// <summary>
        /// The add subscribed interest area.
        /// </summary>
        /// <param name="cameraId">
        /// The camera id.
        /// </param>
        /// <returns>
        /// false if added before.
        /// </returns>
        public bool AddSubscribedInterestArea(byte cameraId)
        {
            if (this.subscribedInterestAreas.Contains(cameraId))
            {
                return false;
            }

            this.subscribedInterestAreas.Add(cameraId);
            return true;
        }

        /// <summary>
        /// The add visible interest area.
        /// </summary>
        /// <param name="cameraId">
        /// The camera id.
        /// </param>
        /// <returns>
        /// false if added before.
        /// </returns>
        public bool AddVisibleInterestArea(byte cameraId)
        {
            if (this.visibleInterestAreas.Contains(cameraId))
            {
                return false;
            }

            this.visibleInterestAreas.Add(cameraId);
            return true;
        }

        /// <summary>
        /// The get initial properties.
        /// </summary>
        public void GetInitialProperties()
        {
            Operations.GetProperties(this.game, this.id, this.type, null);
        }

        /// <summary>
        /// The get properties.
        /// </summary>
        public void GetProperties()
        {
            Operations.GetProperties(this.game, this.id, this.type, this.PropertyRevision);
        }

        /// <summary>
        /// The make visible to subscribed interest areas.
        /// </summary>
        public void MakeVisibleToSubscribedInterestAreas()
        {
            this.subscribedInterestAreas.ForEach(b => this.AddVisibleInterestArea(b));
        }

        /// <summary>
        /// The remove subscribed interest area.
        /// </summary>
        /// <param name="cameraId">
        /// The camera id.
        /// </param>
        /// <returns>
        /// true if found.
        /// </returns>
        public bool RemoveSubscribedInterestArea(byte cameraId)
        {
            return this.subscribedInterestAreas.Remove(cameraId);
        }

        /// <summary>
        /// The remove visible interest area.
        /// </summary>
        /// <param name="cameraId">
        /// The camera id.
        /// </param>
        /// <returns>
        /// true if found.
        /// </returns>
        public bool RemoveVisibleInterestArea(byte cameraId)
        {
            return this.visibleInterestAreas.Remove(cameraId);
        }

        /// <summary>
        /// The reset previous position.
        /// </summary>
        public void ResetPreviousPosition()
        {
            this.PreviousPosition = null;
        }

        /// <summary>
        /// The set color.
        /// </summary>
        /// <param name="color">
        /// The color.
        /// </param>
        public virtual void SetColor(int color)
        {
            this.Color = color;
        }

        /// <summary>
        /// The set interest area attached item.
        /// </summary>
        /// <param name="attached">
        /// The attached.
        /// </param>
        public virtual void SetInterestAreaAttached(bool attached)
        {
            this.InterestAreaAttached = attached;
        }

        /// <summary>
        /// The set interest are view distance.
        /// </summary>
        /// <param name="viewDistanceEnter">
        /// The view distance enter.
        /// </param>
        /// <param name="viewDistanceExit">
        /// The view distance exit.
        /// </param>
        public virtual void SetInterestAreaViewDistance(float[] viewDistanceEnter, float[] viewDistanceExit)
        {
            this.ViewDistanceEnter = viewDistanceEnter;
            this.ViewDistanceExit = viewDistanceExit;
        }

        /// <summary>
        /// The set positions.
        /// </summary>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <param name="previousPosition">
        /// The previous position.
        /// </param>
        /// <param name="rotation">
        /// The rotation.
        /// </param>
        /// <param name="previousRotation">
        /// The previous Rotation.
        /// </param>
        public void SetPositions(float[] position, float[] previousPosition, float[] rotation, float[] previousRotation)
        {
            this.Position = position;
            this.PreviousPosition = previousPosition;
            this.Rotation = rotation;
            this.PreviousRotation = previousPosition;

            this.OnMoved();
        }

        /// <summary>
        /// The set text.
        /// </summary>
        /// <param name="text">
        /// The item text.
        /// </param>
        public virtual void SetText(string text)
        {
            this.Text = text;
        }

        /// <summary>
        /// The on moved.
        /// </summary>
        private void OnMoved()
        {
            if (this.Moved != null)
            {
                this.Moved(this);
            }
        }
    }
}