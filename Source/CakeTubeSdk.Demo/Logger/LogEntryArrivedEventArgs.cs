// <copyright file="LogEntryArrivedEventArgs.cs" company="AnchorFree Inc.">
// Copyright (c) AnchorFree Inc. All rights reserved.
// </copyright>
// <summary>Describes a LogEntryArrived EventArgs.</summary>

namespace CakeTubeSdk.Demo.Logger
{
    using System;

    /// <summary>
    /// <see cref="EventLoggerListener.LogEntryArrived"/> event arguments.
    /// </summary>
    internal class LogEntryArrivedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntryArrivedEventArgs"/> class.
        /// </summary>
        /// <param name="entry">Log entry message.</param>
        public LogEntryArrivedEventArgs(string entry)
        {
            this.Entry = entry;
        }

        /// <summary>
        /// Gets log entry message.
        /// </summary>
        public string Entry { get; }
    }
}