﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Security;
using Drexel.Configurables.Contracts;
using Drexel.Configurables.Contracts.Classes;
using Drexel.Configurables.Contracts.Structs;
using Drexel.Configurables.External;
using Drexel.Configurables.Internals.Types;

namespace Drexel.Configurables
{
    /// <summary>
    /// Provides access to the default set of <see cref="RequirementType"/>s. These requirement types are guaranteed
    /// to be supported in all related official libraries (ex. serialization/deserialization or UI frameworks).
    /// </summary>
    public static class Types
    {
        static Types()
        {
            Types.BigInteger = BigIntegerRequirementType.Instance;
            Types.Boolean = BooleanRequirementType.Instance;
            Types.Decimal = DecimalRequirementType.Instance;
            Types.Double = DoubleRequirementType.Instance;
            Types.FilePath = FilePathRequirementType.Instance;
            Types.Int32 = Int32RequirementType.Instance;
            Types.Int64 = Int64RequirementType.Instance;
            Types.SecureString = SecureStringRequirementType.Instance;
            Types.Single = SingleRequirementType.Instance;
            Types.UInt64 = UInt64RequirementType.Instance;
            Types.Uri = UriRequirementType.Instance;

            Types.DefaultSupported = new ReadOnlyCollection<RequirementType>(
                new List<RequirementType>()
                {
                    Types.BigInteger,
                    Types.Boolean,
                    Types.Decimal,
                    Types.Double,
                    Types.FilePath,
                    Types.Int32,
                    Types.Int64,
                    Types.SecureString,
                    Types.Single,
                    Types.String,
                    Types.UInt64,
                    Types.Uri
                });
        }

        /// <summary>
        /// Gets the default <see cref="RequirementType"/> for an inner type of <see cref="System.Numerics.BigInteger"/>.
        /// </summary>
        public static StructRequirementType<BigInteger> BigInteger { get; }

        /// <summary>
        /// Gets the default <see cref="RequirementType"/> for an inner type of <see cref="System.Boolean"/>.
        /// </summary>
        public static StructRequirementType<Boolean> Boolean { get; }

        /// <summary>
        /// Gets the default <see cref="RequirementType"/> for an inner type of <see cref="System.Decimal"/>.
        /// </summary>
        public static StructRequirementType<Decimal> Decimal { get; }

        /// <summary>
        /// Gets the default <see cref="RequirementType"/> for an inner type of <see cref="System.Boolean"/>.
        /// </summary>
        public static StructRequirementType<Double> Double { get; }

        /// <summary>
        /// Gets the default <see cref="RequirementType"/> for an inner type of <see cref="External.FilePath"/>.
        /// </summary>
        public static ClassRequirementType<FilePath> FilePath { get; }

        /// <summary>
        /// Gets the default <see cref="RequirementType"/> for an inner type of <see cref="System.Int32"/>.
        /// </summary>
        public static StructRequirementType<Int32> Int32 { get; }

        /// <summary>
        /// Gets the default <see cref="RequirementType"/> for an inner type of <see cref="System.Int64"/>.
        /// </summary>
        public static StructRequirementType<Int64> Int64 { get; }

        /// <summary>
        /// Gets the default <see cref="RequirementType"/> for an inner type of <see cref="System.Security.SecureString"/>.
        /// </summary>
        public static ClassRequirementType<SecureString> SecureString { get; }

        /// <summary>
        /// Gets the default <see cref="RequirementType"/> for an inner type of <see cref="System.Single"/>.
        /// </summary>
        public static StructRequirementType<Single> Single { get; }

        /// <summary>
        /// Gets the default <see cref="RequirementType"/> for an inner type of <see cref="System.String"/>.
        /// </summary>
        public static ClassRequirementType<String> String { get; }

        /// <summary>
        /// Gets the default <see cref="RequirementType"/> for an inner type of <see cref="System.UInt64"/>.
        /// </summary>
        public static StructRequirementType<UInt64> UInt64 { get; }

        /// <summary>
        /// Gets the default <see cref="RequirementType"/> for an inner type of <see cref="System.Uri"/>.
        /// </summary>
        public static ClassRequirementType<Uri> Uri { get; }

        /// <summary>
        /// Gets the set of default <see cref="RequirementType"/>s exposed by this class.
        /// </summary>
        public static IReadOnlyCollection<RequirementType> DefaultSupported { get; }
    }
}
