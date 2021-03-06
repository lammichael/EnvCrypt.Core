﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using EnvCrypt.Core.EncrypedData.UserStringConverter;
using EnvCrypt.Core.EncryptionAlgo;
using EnvCrypt.Core.Key;
using EnvCrypt.Core.Verb.LoadKey;

namespace EnvCrypt.Core.Verb.AddEntry
{
    public class EncryptWorkflow<TKey, TKeyLoadDetails>
        where TKey : KeyBase
    {
        private readonly IKeyLoader<TKey, TKeyLoadDetails> _keyLoader;
        private readonly IKeySuitabilityChecker<TKey> _keySuitabilityChecker;
        private readonly IUserStringConverter _userStringConverter;
        private readonly ISegmentEncryptionAlgo<TKey> _segmentEncrypter;

        public EncryptWorkflow(IKeyLoader<TKey, TKeyLoadDetails> keyLoader, 
            IKeySuitabilityChecker<TKey> keySuitabilityChecker,
            IUserStringConverter userStringConverter,
            ISegmentEncryptionAlgo<TKey> segmentEncrypter)
        {
            Contract.Requires<ArgumentNullException>(keyLoader != null, "keyLoader");
            Contract.Requires<ArgumentNullException>(keySuitabilityChecker != null, "keyChecker");
            Contract.Requires<ArgumentNullException>(userStringConverter != null, "userStringConverter");
            Contract.Requires<ArgumentNullException>(segmentEncrypter != null, "segmentEncrypter");
            //
            _keyLoader = keyLoader;
            _keySuitabilityChecker = keySuitabilityChecker;
            _userStringConverter = userStringConverter;
            _segmentEncrypter = segmentEncrypter;
        }


        public IList<byte[]> GetEncryptedSegments(TKeyLoadDetails usingKeyDetails, string toEncrypt, out TKey withKey)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(toEncrypt), "toEncrypt");
            Contract.Ensures(Contract.ValueAtReturn(out withKey) != null);
            //
            var key = _keyLoader.Load(usingKeyDetails);
            
            if(!_keySuitabilityChecker.IsEncryptingKey(key))
            {
                throw new EnvCryptException("impossible to encrypt using this {0} key. Name: {1}", key.Algorithm, key.Name);
            }

            var binaryToEncrypt = _userStringConverter.Encode(toEncrypt);

            withKey = key;
            return _segmentEncrypter.Encrypt(binaryToEncrypt, key);
        }
    }
}
