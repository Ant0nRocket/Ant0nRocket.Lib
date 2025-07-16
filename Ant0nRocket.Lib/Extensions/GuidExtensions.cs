using Ant0nRocket.Lib.Logging;
using System;
using System.Security.Cryptography;
using System.Threading;

namespace Ant0nRocket.Lib.Extensions
{
    /// <summary>
    /// Extensions for <see cref="Guid"/>.
    /// </summary>
    public static class GuidExtensions
    {
        const long UNIX_BASE_TICKS = 621_355_968_000_000_000;

        private static long _lastUnixEpochMsValue;

        /// <summary>
        /// If current GUID is UUIDv8 variant 8 (custom UUIDs) then
        /// ticks since 1970 Jan 1 (encoded in it) will be returned.
        /// Else - zero.
        /// </summary>
        public static long GetTicksSinceUnixEpoch(this Guid guid)
        {
            var array = guid.ToByteArray();
            var customGuidByte = array[7];
            customGuidByte >>= 4;
            customGuidByte <<= 4;
            if (customGuidByte != 0b1000_0000) // 0x80 for custom GUIDs
                return 0;

            var ticksSpanArray = new byte[] {
                array[4], array[5],
                array[0], array[1], array[2], array[3],
                0, 0 };

            var ticks = BitConverter.ToInt64(ticksSpanArray, 0) * 10_000;

            return ticks;
        }

        /// <summary>
        /// Returnes a <see cref="DateTime"/> (UTC) value encoded in first 48 bits of GUID.
        /// </summary>
        public static DateTime GetDateTimeUtc(this Guid guid)
        {
            var ticks = guid.GetTicksSinceUnixEpoch() + UNIX_BASE_TICKS;
            var date = new DateTime(ticks, DateTimeKind.Utc);
            return date;
        }

        /// <summary>
        /// Returnes a <see cref="DateTime"/> (Local) value encoded in first 48 bits of GUID.
        /// </summary>
        public static DateTime GetDateTime(this Guid guid) => guid.GetDateTimeUtc().ToLocalTime();

        /// <summary>
        /// Generates a sequential GUID using provided one and keeping in mind
        /// that first 48 bits should represent ticks since 1970 Jan 1.
        /// If empty GUID provided - random number generator will create a new
        /// 16 byte array with random numbers, othervise - only first 48 bits will
        /// be replaced.
        /// </summary>
        public static Guid ToUnixEpochGuid(this Guid guid, DateTime? dateTimeUtcNow = default)
        {
            /*
            (1) The 48-bit unix_ts_ms field is dedicated to the Unix timestamp in milliseconds.
            (2) The 4-bit ver field is set at 0111.
            (3) The 42-bit counter field accommodates a counter that ensures the increasing 
                order of IDs generated within a millisecond. The counter is incremented by one 
                for each new ID and is reset to a random number when the unix_ts_ms changes.
            (4) The 2-bit var field is set at 10.
            (5) The remaining 32 rand bits are filled with a cryptographically strong random number.

            N.B.! We respect rules 2 and 4, but rule 3 were dropped 'cause we do very interesting
                  thing: if empty GUID provided then random number generator will populate 16-bytes
                  length array and apply sequential algorythm on it, else (if some GUID provided)
                  all operations will be performed on it (left 48 bytes will become datetime-ralated).
             */

            var now = dateTimeUtcNow ?? DateTime.UtcNow; // fix current moment
            var unixEpochTicks = now.Ticks - UNIX_BASE_TICKS;

            var unixEpochMs = unixEpochTicks / 10_000; // milliseconds since 1970 Jan 1

            while (unixEpochMs <= _lastUnixEpochMsValue) unixEpochMs++; // protect against duplication

            var guidBytes = guid.Equals(Guid.Empty) ?
                RandomNumberGenerator.GetBytes(16) : guid.ToByteArray(); // Prepare GUID byte array

            // (1)
            var msBytes = BitConverter.GetBytes(unixEpochMs);
            Array.Reverse(msBytes);
            Array.Copy(msBytes[2..], 0, guidBytes, 0, 6); // we need only 48-bit (6 bytes).

            // (2-4)
            guidBytes[6] <<= 4;   // to drop left 4 bytes
            guidBytes[6] >>= 4;   // to assign left 4 bytes zero
            guidBytes[6] |= 0b1000_0000; // 8 - for custom UNIX-timestamp related algo

            guidBytes[11] >>= 2;
            guidBytes[11] <<= 2;
            guidBytes[11] |= 0b0000_0010; // group 11 must end with 10 according to standard
            // 6        7        8        9        10       11
            // 00000000 00000000 00000000 00000000 00000000 00000000 
            // 1000xxxx xxxxxxxx xxxxxxxx xxxxxxxx xxxxxxxx xxxxxx10

            var resultGuid = new Guid(guidBytes, true);
            _lastUnixEpochMsValue = unixEpochMs;
            return resultGuid;
        }
    }
}
