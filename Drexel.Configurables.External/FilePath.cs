﻿using System;
using System.Runtime.CompilerServices;

namespace Drexel.Configurables.External
{
    /// <summary>
    /// Represents a local file path.
    /// </summary>
    public class FilePath
    {
        /// <summary>
        /// Exception message for when an invalid path is supplied.
        /// </summary>
        internal const string InvalidPath = "The specified path is not a valid fully-qualified path.";

        /// <summary>
        /// Instantiates a new <see cref="FilePath"/> instance.
        /// </summary>
        /// <param name="path">
        /// The local file path, formatted as a <see langword="string"/>. Must be fully-qualified (not relative).
        /// </param>
        /// <param name="interactor">
        /// The path interactor.
        /// </param>
        /// <param name="caseSensitive">
        /// Indicates whether the file path should be treated case-sensitively.
        /// </param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "The System.IO.Path class forces us to do this.")]
        public FilePath(string path, IPathInteractor interactor, bool caseSensitive = false)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (interactor == null)
            {
                throw new ArgumentNullException(nameof(interactor));
            }

            bool valid;
            try
            {
                valid = FilePath.InvariantCultureStringEquals(path, interactor.GetFullPath(path), caseSensitive)
                    && interactor.IsPathRooted(path);
            }
            catch
            {
                // Using exceptions for control flow is bad practice, but such is life.
                valid = false;
            }

            if (!valid)
            {
                throw new ArgumentException(FilePath.InvalidPath, nameof(path));
            }

            this.Path = path;
            this.CaseSensitive = caseSensitive;
        }

        /// <summary>
        /// Indicates whether the path is considered case-sensitive.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if case sensitive; <see langword="false"/> otherwise.
        /// </value>
        public bool CaseSensitive { get; private set; }

        /// <summary>
        /// The local file path.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// Returns a string that represents this <see cref="FilePath"/> instance.
        /// </summary>
        /// <returns>
        /// A string that represents this <see cref="FilePath"/> instance.
        /// </returns>
        public override string ToString()
        {
            return this.Path;
        }

        /// <summary>
        /// Determines whether the specified <see cref="FilePath"/> is equal to the current <see cref="FilePath"/>.
        /// </summary>
        /// <param name="obj">
        /// The <see cref="FilePath"/> to compare with the current <see cref="FilePath"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the specified <see cref="FilePath"/> is equal to the current
        /// <see cref="FilePath"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is FilePath other)
            {
                return FilePath.InvariantCultureStringEquals(
                    this.Path,
                    other.Path,
                    this.CaseSensitive || other.CaseSensitive);
            }

            return false;
        }

        /// <summary>
        /// Returns the hash code for this <see cref="FilePath"/>.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer hash code.
        /// </returns>
        public override int GetHashCode()
        {
            return this.CaseSensitive
                ? this.Path.GetHashCode()
                : this.Path.ToLowerInvariant().GetHashCode();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool InvariantCultureStringEquals(string a, string b, bool caseSensitive)
        {
            return string.Equals(
                a,
                b,
                caseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
