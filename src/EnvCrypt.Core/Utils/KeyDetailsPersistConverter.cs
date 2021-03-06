﻿using System;

namespace EnvCrypt.Core.Utils
{
    /// <summary>
    /// "You should absolutely not use an Encoding to convert arbitrary binary data to text. 
    /// Encoding is for when you've got binary data which genuinely is encoded text"
    /// http://haacked.com/archive/2012/01/30/hazards-of-converting-binary-data-to-a-string.aspx/
    /// </summary>
    class KeyDetailsPersistConverter : IKeyDetailsPersistConverter
    {
        public string Encode(byte[] dataToPersist)
        {
            return Convert.ToBase64String(dataToPersist);
        }

        public byte[] Decode(string persistedStr)
        {
            return Convert.FromBase64String(persistedStr);
        }
    }
}