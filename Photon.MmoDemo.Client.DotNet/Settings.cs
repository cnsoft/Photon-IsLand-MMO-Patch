// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Settings.cs" company="Exit Games GmbH">
//   Copyright (c) Exit Games GmbH.  All rights reserved.
// </copyright>
// <summary>
//   The settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Photon.MmoDemo.Client
{
    /// <summary>
    /// The settings.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Initializes static members of the <see cref="Settings"/> class.
        /// </summary>
        static Settings()
        {
            ChannelCount = 3;

            ////RadarChannel = 1;
            ////DiagnosticsChannel = 1;
            ////DefaultChannel = 1;
            RadarChannel = 0;
            DiagnosticsChannel = 0;
            OperationChannel = 0;
            ItemChannel = 0;
        }

        /// <summary>
        /// Gets or sets ChannelCount.
        /// </summary>
        public static byte ChannelCount { get; set; }

        /// <summary>
        /// Gets or sets DiagnosticsChannel.
        /// </summary>
        public static byte DiagnosticsChannel { get; set; }

        /// <summary>
        /// Gets or sets ItemChannel.
        /// </summary>
        public static byte ItemChannel { get; set; }

        /// <summary>
        /// Gets or sets OperationChannel.
        /// </summary>
        public static byte OperationChannel { get; set; }

        /// <summary>
        /// Gets or sets RadarChannel.
        /// </summary>
        public static byte RadarChannel { get; set; }

        /// <summary>
        /// Gets or sets ApplicationName.
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets GridSize.
        /// </summary>
        public int[] GridSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether SendReliable.
        /// </summary>
        public bool SendReliable { get; set; }

        /// <summary>
        /// Gets or sets ServerAddress.
        /// </summary>
        public string ServerAddress { get; set; }

        /// <summary>
        /// Gets or sets TileDimensions.
        /// </summary>
        public int[] TileDimensions { get; set; }

        /// <summary>
        /// Gets or sets WorldName.
        /// </summary>
        public string WorldName { get; set; }
    }
}