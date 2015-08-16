namespace Server.Framework.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;

    /// <summary>
    /// Utility class for the API server.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Returns the time for the next featured update.
        /// </summary>
        /// <returns>The time of the next update.</returns>
        public static long NextFeaturedUpdateTime()
        {
            // New "featured" every day.
            var date = DateTimeOffset.UtcNow.Date.AddDays(1).AddHours(1);

            var abslouteTime = date - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return (long)abslouteTime.TotalSeconds;
        }

        /// <summary>
        /// Returns a hash of the password, safe for storage.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <param name="salt">The salt to use.</param>
        /// <returns>The resulting hash.</returns>
        public static string HashPassword(string password, string salt)
        {
            Guard.NotNullOrEmpty(() => password);
            Guard.NotNullOrEmpty(() => salt);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt.GetBytes(), 10000))
            {
                return pbkdf2.GetBytes(128).ToBase64String();
            }
        }

        /// <summary>
        /// Converts a byte array into a base 64 string.
        /// </summary>
        /// <param name="value">The byte array to convert.</param>
        /// <returns>A base 64 string.</returns>
        public static string ToBase64String(this byte[] value)
        {
            Guard.NotNull(() => value);

            return Convert.ToBase64String(value);
        }

        /// <summary>
        /// Gets the raw bytes that represent a given string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns>The raw bytes.</returns>
        public static byte[] GetBytes(this string str)
        {
            Guard.NotNullOrEmpty(() => str);

            byte[] bytes = new byte[str.Length * sizeof(char)];

            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);

            return bytes;
        }
    }
}
