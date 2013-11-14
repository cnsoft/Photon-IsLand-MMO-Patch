// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MyItem.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The mmo item.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Client
{
    using System;
    using System.Collections;

    /// <summary>
    /// The mmo item.
    /// </summary>
    public class MyItem : Item
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MyItem"/> class. 
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
        /// <param name="text">
        /// The text property.
        /// </param>
        [CLSCompliant(false)]
        public MyItem(string id, byte type, Game game, string text)
            : base(id, type, game)
        {
            base.SetColor((int)((uint)new Random(Guid.NewGuid().GetHashCode()).Next(0, int.MaxValue) | 0xFF000000));
            base.SetText(text);
        }

        /// <summary>
        /// Gets a value indicating whether IsMine.
        /// </summary>
        public override bool IsMine
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsMoving.
        /// </summary>
        public bool IsMoving { get; set; }

        /// <summary>
        /// The destroy.
        /// </summary>
        public void Destroy()
        {
            this.IsDestroyed = true;
            Operations.DestroyItem(this.Game, this.Id, this.Type);
        }

        /// <summary>
        /// The enter world.
        /// </summary>
        public void EnterWorld()
        {
            var r = new Random();
            var position = new float[]
                {
                    r.Next((int)this.Game.WorldData.TopLeftCorner[0], (int)this.Game.WorldData.BottomRightCorner[0]), 
                    r.Next((int)this.Game.WorldData.TopLeftCorner[1], (int)this.Game.WorldData.BottomRightCorner[1])
                };
            this.SetPositions(position, position, null, null);
            
            var properties = new Hashtable
                {
                    { PropertyKeyInterestAreaAttached, this.InterestAreaAttached }, 
                    { PropertyKeyViewDistanceEnter, this.ViewDistanceEnter }, 
                    { PropertyKeyViewDistanceExit, this.ViewDistanceExit }, 
                    { PropertyKeyColor, this.Color }, 
                    { PropertyKeyText, this.Text }
                };
            Operations.EnterWorld(this.Game, this.Game.WorldData.Name, this.Id, properties, this.Position, this.Rotation, this.ViewDistanceEnter, this.ViewDistanceExit);
        }

        /// <summary>
        /// The move operation.
        /// </summary>
        /// <param name="newPosition">
        /// The new position.
        /// </param>
        /// <param name="rotation">
        /// The rotation.
        /// </param>
        /// <returns>
        /// The move absolute.
        /// </returns>
        public bool MoveAbsolute(float[] newPosition, float[] rotation)
        {
            if (newPosition[0] < this.Game.WorldData.TopLeftCorner[0])
            {
                return false;
            }

            if (newPosition[0] > this.Game.WorldData.BottomRightCorner[0])
            {
                return false;
            }

            if (newPosition[1] < this.Game.WorldData.TopLeftCorner[1])
            {
                return false;
            }

            if (newPosition[1] > this.Game.WorldData.BottomRightCorner[1])
            {
                return false;
            }

            this.SetPositions(newPosition, this.Position, rotation, this.Rotation);
            Operations.Move(this.Game, this.Id, this.Type, newPosition, rotation, this.Game.Settings.SendReliable);
            return true;
        }

        /// <summary>
        /// The move relative.
        /// </summary>
        /// <param name="offset">
        /// The offset.
        /// </param>
        /// <param name="rotation">
        /// The rotation.
        /// </param>
        /// <returns>
        /// true if moved.
        /// </returns>
        public bool MoveRelative(float[] offset, float[] rotation)
        {
            return this.MoveAbsolute(new[] { this.Position[0] + offset[0], this.Position[1] + offset[1] }, rotation);
        }

        /// <summary>
        /// The set color.
        /// </summary>
        /// <param name="color">
        /// The color.
        /// </param>
        public override void SetColor(int color)
        {
            if (color != this.Color)
            {
                base.SetColor(color);
                Operations.SetProperties(this.Game, this.Id, this.Type, new Hashtable { { PropertyKeyColor, color } }, null, true);
            }
        }

        /// <summary>
        /// The set interest area attached item.
        /// </summary>
        /// <param name="attached">
        /// The attached.
        /// </param>
        public override void SetInterestAreaAttached(bool attached)
        {
            if (attached != this.InterestAreaAttached)
            {
                base.SetInterestAreaAttached(attached);
                Operations.SetProperties(this.Game, this.Id, this.Type, new Hashtable { { PropertyKeyInterestAreaAttached, attached } }, null, true);
            }
        }

        /// <summary>
        /// The set interest area view distance.
        /// </summary>
        /// <param name="viewDistanceEnter">
        /// The view distance enter.
        /// </param>
        /// <param name="viewDistanceExit">
        /// The view distance exit.
        /// </param>
        public override void SetInterestAreaViewDistance(float[] viewDistanceEnter, float[] viewDistanceExit)
        {
            base.SetInterestAreaViewDistance(viewDistanceEnter, viewDistanceExit);
            Operations.SetProperties(
                this.Game, 
                this.Id, 
                this.Type, 
                new Hashtable { { PropertyKeyViewDistanceEnter, viewDistanceEnter }, { PropertyKeyViewDistanceExit, viewDistanceExit } }, 
                null, 
                true);
        }

        /// <summary>
        /// The set view distance.
        /// </summary>
        /// <param name="camera">
        /// The camera.
        /// </param>
        public void SetInterestAreaViewDistance(InterestArea camera)
        {
            this.SetInterestAreaViewDistance(camera.ViewDistanceEnter, camera.ViewDistanceExit);
        }

        /// <summary>
        /// The set text.
        /// </summary>
        /// <param name="text">
        /// The new text.
        /// </param>
        public override void SetText(string text)
        {
            if (text != this.Text)
            {
                base.SetText(text);
                Operations.SetProperties(this.Game, this.Id, this.Type, new Hashtable { { PropertyKeyText, text } }, null, true);
            }
        }

        /// <summary>
        /// The spawn.
        /// </summary>
        /// <param name="position">
        /// The item position.
        /// </param>
        /// <param name="rotation">
        /// The rotation.
        /// </param>
        /// <param name="color">
        /// The item color.
        /// </param>
        /// <param name="subscribe">
        /// The subscribe.
        /// </param>
        public void Spawn(float[] position, float[] rotation, int color, bool subscribe)
        {
            this.SetPositions(position, position, rotation, rotation);
            base.SetInterestAreaViewDistance(new[] { 0f, 0f }, new[] { 0f, 0f });
            base.SetColor(color);
            var properties = new Hashtable
                {
                    { PropertyKeyInterestAreaAttached, false }, 
                    { PropertyKeyViewDistanceEnter, this.ViewDistanceEnter }, 
                    { PropertyKeyViewDistanceExit, this.ViewDistanceExit }, 
                    { PropertyKeyColor, this.Color }, 
                    { PropertyKeyText, this.Text }
                };
            Operations.SpawnItem(this.Game, this.Id, this.Type, position, rotation, properties, subscribe);
        }
    }
}