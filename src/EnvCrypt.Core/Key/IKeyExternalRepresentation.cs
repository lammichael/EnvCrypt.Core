﻿namespace EnvCrypt.Core.Key
{
    /// <summary>
    /// Marker for a POCO used to deserialize a key.
    /// </summary>
    public interface IKeyExternalRepresentation<T> where T : KeyBase
    {}
}
