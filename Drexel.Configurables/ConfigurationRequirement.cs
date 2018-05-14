﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using Drexel.Configurables.Contracts;

namespace Drexel.Configurables
{
    /// <summary>
    /// Validates a supplied <see cref="object"/>.
    /// </summary>
    /// <param name="instance">
    /// The <see cref="object"/> to validate.
    /// </param>
    /// <returns>
    /// <see langword="null"/> if the object passed validation; else, an <see cref="Exception"/> describing why the
    /// supplied <paramref name="instance"/> failed validation.
    /// </returns>
    public delegate Exception Validator(object instance);

    /// <summary>
    /// A simple <see cref="IConfigurationRequirement"/> implementation.
    /// </summary>
    public class ConfigurationRequirement : IConfigurationRequirement
    {
        private const string SuppliedObjectIsOfWrongType = "Supplied object is of wrong type. Expected type: '{0}'.";
        private const string StringMustBeNonWhitespace = "String must not be whitespace.";

        private Validator validator;
        private string cachedToString;

        /// <summary>
        /// Instantiates a new <see cref="ConfigurationRequirement"/> instance using the supplied parameters.
        /// </summary>
        /// <param name="name">
        /// The name of the <see cref="ConfigurationRequirement"/>.
        /// </param>
        /// <param name="description">
        /// The description of the <see cref="ConfigurationRequirement"/>.
        /// </param>
        /// <param name="ofType">
        /// The <see cref="ConfigurationRequirementType"/> of the <see cref="ConfigurationRequirement"/>.
        /// </param>
        /// <param name="isOptional">
        /// Indicates whether this <see cref="ConfigurationRequirement"/> is optional.
        /// </param>
        /// <param name="validator">
        /// Validates <see cref="object"/>s to determine if they satisfy the <see cref="ConfigurationRequirement"/>.
        /// </param>
        /// <param name="collectionInfo">
        /// The <see cref="CollectionInfo"/> describing this <see cref="ConfigurationRequirement"/>.
        /// When <see langword="null"/>, indicates that this <see cref="ConfigurationRequirement"/> is not a
        /// collection.
        /// </param>
        /// <param name="dependsOn">
        /// A collection of <see cref="IConfigurationRequirement"/>s which this <see cref="ConfigurationRequirement"/>
        /// must be supplied alongside.
        /// </param>
        /// <param name="exclusiveWith">
        /// A collection of <see cref="IConfigurationRequirement"/>s which this <see cref="ConfigurationRequirement"/>
        /// must not be supplied alongside with.
        /// </param>
        public ConfigurationRequirement(
            string name,
            string description,
            ConfigurationRequirementType ofType,
            bool isOptional,
            Validator validator,
            CollectionInfo collectionInfo = null,
            IEnumerable<IConfigurationRequirement> dependsOn = null,
            IEnumerable<IConfigurationRequirement> exclusiveWith = null)
        {
            ConfigurationRequirement.ThrowIfBadString(name, nameof(name));
            ConfigurationRequirement.ThrowIfBadString(description, nameof(description));
            this.Name = name;
            this.Description = description;
            this.OfType = ofType;
            this.IsOptional = isOptional;
            this.CollectionInfo = collectionInfo;
            this.DependsOn = dependsOn ?? new IConfigurationRequirement[0];
            this.ExclusiveWith = exclusiveWith ?? new IConfigurationRequirement[0];

            this.validator = validator ?? throw new ArgumentNullException(nameof(validator));
            this.cachedToString = null;
        }

        /// <summary>
        /// The set of <see cref="IConfigurationRequirement"/>s which must be supplied alongside this requirement.
        /// </summary>
        public IEnumerable<IConfigurationRequirement> DependsOn { get; private set; }

        /// <summary>
        /// The description of this requirement.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// The set of <see cref="IConfigurationRequirement"/>s which must not be supplied alongside this requirement.
        /// </summary>
        public IEnumerable<IConfigurationRequirement> ExclusiveWith { get; private set; }

        /// <summary>
        /// <see langword="null"/> if this requirement expects a single instance of
        /// <see cref="ConfigurationRequirementType"/> <see cref="OfType"/>; else, the constraints of the required
        /// collection are described by the <see cref="CollectionInfo"/>.
        /// </summary>
        public CollectionInfo CollectionInfo { get; private set; }

        /// <summary>
        /// <see langword="true"/> if this requirement is optional; <see langword="false"/> if this requirement is
        /// required.
        /// </summary>
        public bool IsOptional { get; private set; }

        /// <summary>
        /// The type of this requirement. This indicates what the expected type of the input to
        /// <see cref="Validate(object)"/> is.
        /// </summary>
        public ConfigurationRequirementType OfType { get; private set; }

        /// <summary>
        /// The name of this requirement.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Creates a new <see cref="ConfigurationRequirement"/> of type
        /// <see cref="ConfigurationRequirementType.String"/>.
        /// </summary>
        /// <param name="name">
        /// The name of the <see cref="ConfigurationRequirement"/>.
        /// </param>
        /// <param name="description">
        /// The description of the <see cref="ConfigurationRequirement"/>.
        /// </param>
        /// <param name="collectionInfo">
        /// The <see cref="CollectionInfo"/> describing this <see cref="ConfigurationRequirement"/>.
        /// When <see langword="null"/>, indicates that this <see cref="ConfigurationRequirement"/> is not a
        /// collection.
        /// </param>
        /// <param name="dependsOn">
        /// A collection of <see cref="IConfigurationRequirement"/>s which this <see cref="ConfigurationRequirement"/>
        /// must be supplied alongside.
        /// </param>
        /// <param name="exclusiveWith">
        /// A collection of <see cref="IConfigurationRequirement"/>s which this <see cref="ConfigurationRequirement"/>
        /// must not be supplied alongside with.
        /// </param>
        /// <returns>
        /// A <see cref="ConfigurationRequirement"/> of type <see cref="ConfigurationRequirementType.String"/> with the
        /// supplied properties.
        /// </returns>
        public static IConfigurationRequirement String(
            string name,
            string description,
            CollectionInfo collectionInfo = null,
            IEnumerable<IConfigurationRequirement> dependsOn = null,
            IEnumerable<IConfigurationRequirement> exclusiveWith = null)
        {
            return new ConfigurationRequirement(
                name,
                description,
                ConfigurationRequirementType.String,
                false,
                instance =>
                {
                    if (instance == null)
                    {
                        return new ArgumentNullException(nameof(instance));
                    }
                    else if (!ConfigurationRequirementType.String.Type.IsAssignableFrom(instance.GetType()))
                    {
                        return new InvalidCastException(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                ConfigurationRequirement.SuppliedObjectIsOfWrongType,
                                ConfigurationRequirementType.String.Type.ToString()));
                    }

                    return null;
                },
                collectionInfo,
                dependsOn,
                exclusiveWith);
        }

        /// <summary>
        /// Creates a new <see cref="ConfigurationRequirement"/> of type
        /// <see cref="ConfigurationRequirementType.Path"/>.
        /// </summary>
        /// <param name="name">
        /// The name of the <see cref="ConfigurationRequirement"/>.
        /// </param>
        /// <param name="description">
        /// The description of the <see cref="ConfigurationRequirement"/>.
        /// </param>
        /// <param name="collectionInfo">
        /// The <see cref="CollectionInfo"/> describing this <see cref="ConfigurationRequirement"/>.
        /// When <see langword="null"/>, indicates that this <see cref="ConfigurationRequirement"/> is not a
        /// collection.
        /// </param>
        /// <param name="dependsOn">
        /// A collection of <see cref="IConfigurationRequirement"/>s which this <see cref="ConfigurationRequirement"/>
        /// must be supplied alongside.
        /// </param>
        /// <param name="exclusiveWith">
        /// A collection of <see cref="IConfigurationRequirement"/>s which this <see cref="ConfigurationRequirement"/>
        /// must not be supplied alongside with.
        /// </param>
        /// <returns>
        /// A <see cref="ConfigurationRequirement"/> of type <see cref="ConfigurationRequirementType.Path"/> with the
        /// supplied properties.
        /// </returns>
        public static IConfigurationRequirement Path(
            string name,
            string description,
            CollectionInfo collectionInfo = null,
            IEnumerable<IConfigurationRequirement> dependsOn = null,
            IEnumerable<IConfigurationRequirement> exclusiveWith = null)
        {
            return new ConfigurationRequirement(
                name,
                description,
                ConfigurationRequirementType.Path,
                false,
                instance =>
                {
                    if (instance == null)
                    {
                        return new ArgumentNullException(nameof(instance));
                    }
                    else if (!ConfigurationRequirementType.Path.Type.IsAssignableFrom(instance.GetType()))
                    {
                        return new InvalidCastException(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                ConfigurationRequirement.SuppliedObjectIsOfWrongType,
                                ConfigurationRequirementType.Path.Type.ToString()));
                    }

                    return null;
                },
                collectionInfo,
                dependsOn,
                exclusiveWith);
        }

        /// <summary>
        /// Creates a new <see cref="ConfigurationRequirement"/> of type
        /// <see cref="ConfigurationRequirementType.Int64"/>.
        /// </summary>
        /// <param name="name">
        /// The name of the <see cref="ConfigurationRequirement"/>.
        /// </param>
        /// <param name="description">
        /// The description of the <see cref="ConfigurationRequirement"/>.
        /// </param>
        /// <param name="collectionInfo">
        /// The <see cref="CollectionInfo"/> describing this <see cref="ConfigurationRequirement"/>.
        /// When <see langword="null"/>, indicates that this <see cref="ConfigurationRequirement"/> is not a
        /// collection.
        /// </param>
        /// <param name="dependsOn">
        /// A collection of <see cref="IConfigurationRequirement"/>s which this <see cref="ConfigurationRequirement"/>
        /// must be supplied alongside.
        /// </param>
        /// <param name="exclusiveWith">
        /// A collection of <see cref="IConfigurationRequirement"/>s which this <see cref="ConfigurationRequirement"/>
        /// must not be supplied alongside with.
        /// </param>
        /// <returns>
        /// A <see cref="ConfigurationRequirement"/> of type <see cref="ConfigurationRequirementType.Int64"/> with the
        /// supplied properties.
        /// </returns>
        public static IConfigurationRequirement Int64(
            string name,
            string description,
            CollectionInfo collectionInfo = null,
            IEnumerable<IConfigurationRequirement> dependsOn = null,
            IEnumerable<IConfigurationRequirement> exclusiveWith = null)
        {
            return new ConfigurationRequirement(
                name,
                description,
                ConfigurationRequirementType.Int64,
                false,
                instance =>
                {
                    if (instance == null)
                    {
                        return new ArgumentNullException(nameof(instance));
                    }
                    else if (!ConfigurationRequirementType.Int64.Type.IsAssignableFrom(instance.GetType()))
                    {
                        return new InvalidCastException(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                ConfigurationRequirement.SuppliedObjectIsOfWrongType,
                                ConfigurationRequirementType.Int64.Type.ToString()));
                    }

                    return null;
                },
                collectionInfo,
                dependsOn,
                exclusiveWith);
        }

        /// <summary>
        /// Validates the supplied <see cref="object"/> for this requirement.
        /// </summary>
        /// <param name="instance">
        /// The <see cref="object"/> to perform validation upon.
        /// </param>
        /// <returns>
        /// <see langword="null"/> if the supplied <see cref="object"/> <paramref name="instance"/> passed validation;
        /// an <see cref="Exception"/> describing the validation failure otherwise.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "By design, no exception should escape the validation call.")]
        public Exception Validate(object instance)
        {
            try
            {
                return this.validator.Invoke(instance);
            }
            catch (Exception e)
            {
                return e;
            }
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents the current <see cref="ConfigurationRequirement"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that represents the current <see cref="ConfigurationRequirement"/>.
        /// </returns>
        public override string ToString()
        {
            const string newline = "\r\n";

            string JsonEscape(string toEscape)
            {
                StringBuilder builder = new StringBuilder();
                foreach (char @char in toEscape)
                {
                    switch (@char)
                    {
                        case '\b':
                            builder.Append(@"\b");
                            break;
                        case '\f':
                            builder.Append(@"\f");
                            break;
                        case '\n':
                            builder.Append(@"\n");
                            break;
                        case '\r':
                            builder.Append(@"\r");
                            break;
                        case '\t':
                            builder.Append(@"\t");
                            break;
                        case '\"':
                            builder.Append("\\\"");
                            break;
                        case '\\':
                            builder.Append(@"\");
                            break;
                        default:
                            builder.Append(@char);
                            break;
                    }
                }

                return builder.ToString();
            }
            void AppendValue(StringBuilder builder, string name, string value)
            {
                builder.Append('"');
                builder.Append(name);
                builder.Append("\": ");
                builder.Append('"');
                builder.Append(value);
                builder.Append('"');
            }

            const string @null = "<null>";

            if (this.cachedToString == null)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append('{');

                builder.Append(newline);
                AppendValue(builder, nameof(this.Name), JsonEscape(this.Name));
                builder.Append(',');
                builder.Append(newline);
                AppendValue(builder, nameof(this.OfType), JsonEscape(this.OfType.ToString()));
                builder.Append(',');
                builder.Append(newline);
                AppendValue(builder, nameof(this.IsOptional), this.IsOptional.ToString());
                builder.Append(',');
                builder.Append(newline);
                AppendValue(builder, nameof(this.Description), JsonEscape(this.Description));
                builder.Append(',');
                builder.Append(newline);
                AppendValue(
                    builder,
                    nameof(this.CollectionInfo),
                    this.CollectionInfo == null
                        ? "null"
                        : string.Format(
                            CultureInfo.InvariantCulture,
                            "{{ \"{0}\": \"{1}\", \"{2}\": \"{3}\" }}",
                            nameof(this.CollectionInfo.MaximumCount),
                            this.CollectionInfo.MaximumCount,
                            nameof(this.CollectionInfo.MinimumCount),
                            this.CollectionInfo.MinimumCount));
                builder.Append(',');
                builder.Append(newline);
                AppendValue(
                    builder,
                    nameof(this.DependsOn),
                    JsonEscape(string.Join(", ", this.DependsOn.Select(x => "\"" + (x?.Name ?? @null) + "\""))));
                builder.Append(',');
                builder.Append(newline);
                AppendValue(
                    builder,
                    nameof(this.ExclusiveWith),
                    JsonEscape(string.Join(", ", this.ExclusiveWith.Select(x => "\"" + (x?.Name ?? @null) + "\""))));
                builder.Append(newline);

                builder.Append('}');

                cachedToString = builder.ToString();
            }

            return this.cachedToString;
        }

        [DebuggerHidden]
        private static void ThrowIfBadString(string @string, string name)
        {
            if (@string == null)
            {
                throw new ArgumentNullException(name);
            }
            else if (string.IsNullOrWhiteSpace(@string))
            {
                throw new ArgumentException(ConfigurationRequirement.StringMustBeNonWhitespace, name);
            }
        }
    }
}
