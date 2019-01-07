﻿using System;

namespace Drexel.Configurables.Contracts
{
    /// <summary>
    /// Represents information about a set value restriction.
    /// </summary>
    public abstract class SetRestrictionInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetRestrictionInfo"/> class.
        /// </summary>
        /// <param name="minimumTimesAllowed">
        /// The minimum number of times the associated value must appear, if such a restriction exists.
        /// </param>
        /// <param name="maximumTimesAllowed">
        /// The maximum number of times the associated value is allowed to appear, if such a restriction exists.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="minimumTimesAllowed"/> is less than 0, <paramref name="maximumTimesAllowed"/>
        /// is less than 1, or <paramref name="maximumTimesAllowed"/> is less than
        /// <paramref name="minimumTimesAllowed"/>.
        /// </exception>
        private protected SetRestrictionInfo(int? minimumTimesAllowed = null, int? maximumTimesAllowed = null)
        {
            this.MaximumTimesAllowed = maximumTimesAllowed;
            this.MinimumTimesAllowed = minimumTimesAllowed;

            if (minimumTimesAllowed.HasValue && minimumTimesAllowed.Value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minimumTimesAllowed));
            }

            if (maximumTimesAllowed.HasValue
                && (maximumTimesAllowed.Value < 1 // If you don't want it to appear in the set, don't include it
                    || (minimumTimesAllowed.HasValue // Maximum count cannot be less than minimum count
                        && maximumTimesAllowed.Value < minimumTimesAllowed.Value)))
            {
                throw new ArgumentOutOfRangeException(nameof(maximumTimesAllowed));
            }
        }

        /// <summary>
        /// Gets the maximum number of times the associated value is allowed to appear, if such a restriction exists.
        /// </summary>
        public int? MaximumTimesAllowed { get; }

        /// <summary>
        /// Gets the minimum number of times the associated value must appear, if such a restriction exists.
        /// </summary>
        public int? MinimumTimesAllowed { get; }

        /// <summary>
        /// Returns a value indicating whether the specified count is above the allowed range.
        /// </summary>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the count is above the allowed range; <see langword="false"/> otherwise.
        /// </returns>
        public bool IsAboveRange(int count) =>
            this.MaximumTimesAllowed.HasValue && count > this.MaximumTimesAllowed.Value;

        /// <summary>
        /// Returns a value indicating whether the specified count is below the allowed range.
        /// </summary>
        /// <param name="count">
        /// The count.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the count is below the allowed range; <see langword="false"/> otherwise.
        /// </returns>
        public bool IsBelowRange(int count) =>
            this.MinimumTimesAllowed.HasValue && count < this.MinimumTimesAllowed.Value;
    }
}