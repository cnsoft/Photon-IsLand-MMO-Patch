// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ForeignItem.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The foreign item.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Client
{
    using System;

    /// <summary>
    /// The foreign item.
    /// </summary>
    public class ForeignItem : Item
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForeignItem"/> class.
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
        public ForeignItem(string id, byte type, Game game)
            : base(id, type, game)
        {
        }

        /// <summary>
        /// Gets a value indicating whether IsMine.
        /// </summary>
        public override bool IsMine
        {
            get
            {
                return false;
            }
        }
    }
}