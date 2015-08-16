namespace Server.Framework.Structs
{
    using System;

    /// <summary>
    /// Represents a globally unique identifier (GUID) with a shorter string value.
    /// </summary>
    public struct ShortGuid
    {
        /// <summary>
        /// A read-only instance of the <see cref="ShortGuid" /> class whose value is guaranteed to be all zeroes.
        /// </summary>
        public static readonly ShortGuid Empty = new ShortGuid(Guid.Empty);

        /// <summary>
        /// The structs internal GUID.
        /// </summary>
        private Guid internalGuid;

        /// <summary>
        /// The structs base64 representation.
        /// </summary>
        private string internalStringValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShortGuid" /> struct.
        /// </summary>
        /// <param name="value">The encoded GUID as a base64 string</param>
        public ShortGuid(string value)
        {
            this.internalStringValue = value;

            this.internalGuid = Decode(value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShortGuid" /> struct.
        /// </summary>
        /// <param name="value">The GUID to encode</param>
        public ShortGuid(Guid value)
        {
            this.internalStringValue = Encode(value);

            this.internalGuid = value;
        }

        /// <summary>
        /// Gets or sets the underlying GUID.
        /// </summary>
        public Guid Guid
        {
            get
            {
                return this.internalGuid;
            }

            set
            {
                if (value != this.internalGuid)
                {
                    this.internalGuid = value;

                    this.internalStringValue = Encode(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the underlying base64 encoded string.
        /// </summary>
        public string Value
        {
            get 
            {
                return this.internalStringValue;
            }

            set
            {
                if (value != this.internalStringValue)
                {
                    this.internalStringValue = value;

                    this.internalGuid = Decode(value);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShortGuid" /> struct.
        /// </summary>
        /// <returns>A <see cref="ShortGuid" />.</returns>
        public static ShortGuid NewGuid()
        {
            return new ShortGuid(Guid.NewGuid());
        }

        /// <summary>
        /// Creates a new instance of a GUID using the string value, then returns the base64 encoded version of the GUID.
        /// </summary>
        /// <param name="value">An actual GUID string (i.e. not a <see cref="ShortGuid" />).</param>
        /// <returns>Base64 string of the <see cref="ShortGuid" />.</returns>
        public static string Encode(string value)
        {
            Guid guid = new Guid(value);

            return Encode(guid);
        }

        /// <summary>
        /// Encodes the given GUID as a base64 string that is 22 characters long.
        /// </summary>
        /// <param name="value">The GUID to encode.</param>
        /// <returns>Base64 string of the <see cref="ShortGuid" />.</returns>
        public static string Encode(Guid value)
        {
            string encoded = Convert.ToBase64String(value.ToByteArray());

            encoded = encoded
                .Replace("/", "_")
                .Replace("+", "-");

            return encoded.Substring(0, 22);
        }

        /// <summary>
        /// Decodes the given base64 string.
        /// </summary>
        /// <param name="value">The base64 encoded string of a GUID.</param>
        /// <returns>A new GUID.</returns>
        public static Guid Decode(string value)
        {
            Guard.NotNullOrEmpty(() => value);

            value = value
                .Replace("_", "/")
                .Replace("-", "+");
            byte[] buffer = Convert.FromBase64String(value + "==");
            return new Guid(buffer);
        }

        /// <summary>
        /// Determines if both the <see cref="ShortGuid" /> have the same underlying GUID value.
        /// </summary>
        /// <param name="leftGuid">The left GUID.</param>
        /// <param name="rightGuid">The right GUID.</param>
        /// <returns>True if the objects are equal.</returns>
        public static bool operator ==(ShortGuid leftGuid, ShortGuid rightGuid)
        {
            if ((object)leftGuid == null)
            {
                return (object)rightGuid == null;
            }

            return leftGuid.internalGuid == rightGuid.internalGuid;
        }

        /// <summary>
        /// Determines if both the <see cref="ShortGuid" /> do not have the same underlying GUID value.
        /// </summary>
        /// <param name="leftGuid">The left GUID.</param>
        /// <param name="rightGuid">The right GUID.</param>
        /// <returns>True if the objects are not equal.</returns>
        public static bool operator !=(ShortGuid leftGuid, ShortGuid rightGuid)
        {
            return !(leftGuid == rightGuid);
        }

        /// <summary>
        /// Implicitly converts the <see cref="ShortGuid" /> to it's string equivalent.
        /// </summary>
        /// <param name="shortGuid">The <see cref="ShortGuid" /> to convert.</param>
        /// <returns></returns>
        public static implicit operator string(ShortGuid shortGuid)
        {
            return shortGuid.internalStringValue;
        }

        /// <summary>
        /// Implicitly converts the <see cref="ShortGuid" /> to it's GUID equivalent.
        /// </summary>
        /// <param name="shortGuid">The <see cref="ShortGuid" /> to convert.</param>
        /// <returns>A GUID.</returns>
        public static implicit operator Guid(ShortGuid shortGuid)
        {
            return shortGuid.internalGuid;
        }

        /// <summary>
        /// Implicitly converts the string to a <see cref="ShortGuid" />.
        /// </summary>
        /// <param name="shortGuid">The string value to convert.</param>
        /// <returns>A <see cref="ShortGuid" />.</returns>
        public static implicit operator ShortGuid(string shortGuid)
        {
            return new ShortGuid(shortGuid);
        }

        /// <summary>
        /// Implicitly converts the GUID to a <see cref="ShortGuid" />.
        /// </summary>
        /// <param name="value">The GUID to convert.</param>
        /// <returns>The converted GUID as a <see cref="ShortGuid" />.</returns>
        public static implicit operator ShortGuid(Guid value)
        {
            return new ShortGuid(value);
        }

        /// <summary>
        /// Returns the base64 encoded GUID as a string.
        /// </summary>
        /// <returns>The base64 encoded GUID.</returns>
        public override string ToString()
        {
            return this.internalStringValue;
        }

        /// <summary>
        /// Returns a value indicating whether this instance and a specified Object represent the same type and value.
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>True if the objects are of the same value and type.</returns>
        public override bool Equals(object obj)
        {
            if (obj is ShortGuid)
            {
                return this.internalGuid.Equals(((ShortGuid)obj).internalGuid);
            }

            if (obj is Guid)
            {
                return this.internalGuid.Equals((Guid)obj);
            }

            if (obj is string)
            { 
                return this.internalGuid.Equals(((ShortGuid)obj).internalGuid);
            }

            return false;
        }

        /// <summary>
        /// Returns the HashCode for underlying GUID.
        /// </summary>
        /// <returns>The objects hash code.</returns>
        public override int GetHashCode()
        {
            return this.internalGuid.GetHashCode();
        }
    }
}
