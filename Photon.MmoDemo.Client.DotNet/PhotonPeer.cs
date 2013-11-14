// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhotonPeer.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The photon peer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Client
{
    using System;

    using ExitGames.Client.Photon;

    /// <summary>
    /// The photon peer.
    /// </summary>
    [CLSCompliant(false)]
    public class PhotonPeer : ExitGames.Client.Photon.PhotonPeer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PhotonPeer"/> class.
        /// </summary>
        /// <param name="listener">
        /// The listener.
        /// </param>
        /// <param name="useTcp">
        /// The use tcp.
        /// </param>
        public PhotonPeer(IPhotonPeerListener listener, bool useTcp)
            : base(listener, useTcp)
        {
            this.ChannelCount = Settings.ChannelCount;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PhotonPeer"/> class.
        /// </summary>
        /// <param name="listener">
        /// The listener.
        /// </param>
        public PhotonPeer(IPhotonPeerListener listener)
            : base(listener)
        {
            this.ChannelCount = Settings.ChannelCount;
        }
    }
}