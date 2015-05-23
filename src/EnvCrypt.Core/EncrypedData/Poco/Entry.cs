﻿using System.Collections.Generic;

namespace EnvCrypt.Core.EncrypedData.Poco
{
    public class Entry
    {
        /// <summary>
        /// A list is used to cater to encrypt data that exceeds the maximum 
        /// allowed for the algorithm.
        /// </summary>
        public IList<byte[]> EncryptedValue { get; set; }
        
        /// <summary>
        /// There must only be one non-null key for an entry.
        /// If null, then no encryption is used.
        /// </summary>
        public Key.EnvCryptKey Key { get; set; }
    }
}