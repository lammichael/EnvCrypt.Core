﻿using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Security.Cryptography;
using EnvCrypt.Core.EncryptionAlgo.Rsa.Key;
using EnvCrypt.Core.EncryptionAlgo.Rsa.Utils;
using EnvCrypt.Core.Key;
using EnvCrypt.Core.Utils;
using EnvCrypt.Core.Utils.IO;
using EnvCryptKey = EnvCrypt.Core.Key.Xml.EnvCryptKey;

namespace EnvCrypt.Core.Verb.GenerateKey.Persister
{
    class RsaKeyFilePersister : AsymmetricKeyFilePersister<RsaKey, Key.Xml.EnvCryptKey>
    {
        private readonly IKeyToExternalRepresentationMapper<RsaKey, Key.Xml.EnvCryptKey> _pocoMapper;
        private readonly IXmlSerializationUtils<Key.Xml.EnvCryptKey> _serializationUtils;
        private readonly IStringToFileWriter _writer;

        public RsaKeyFilePersister(
            IKeyToExternalRepresentationMapper<RsaKey, EnvCryptKey> pocoMapper, 
            IXmlSerializationUtils<EnvCryptKey> serializationUtils,
            IStringToFileWriter writer)
        {
            Contract.Requires<ArgumentNullException>(pocoMapper != null, "pocoMapper");
            Contract.Requires<ArgumentNullException>(serializationUtils != null, "serializationUtils");
            Contract.Requires<ArgumentNullException>(writer != null, "writer");
            //
            _pocoMapper = pocoMapper;
            _serializationUtils = serializationUtils;
            _writer = writer;
        }


        public override void WriteToFile(RsaKey thisKey, AsymmetricKeyFilePersisterOptions withOptions)
        {
            if (thisKey.GetKeyType() != AsymmetricKeyType.Private)
            {
                throw new EnvCryptException("key to persist must have all data for a private key");
            }
            if (string.IsNullOrWhiteSpace(withOptions.NewPrivateKeyFullFilePath))
            {
                throw new EnvCryptException("private key full file path cannot be empty");
            }
            if (string.IsNullOrWhiteSpace(withOptions.NewPublicKeyFullFilePath))
            {
                throw new EnvCryptException("private key full file path cannot be empty");
            }
            Contract.EndContractBlock();


            {
                // Write private key
                var privateKeyXmlPoco = new Key.Xml.EnvCryptKey();
                _pocoMapper.Map(thisKey, privateKeyXmlPoco);
                var toWrite = _serializationUtils.Serialize(privateKeyXmlPoco);

                _writer.Write(withOptions.NewPrivateKeyFullFilePath, toWrite,
                    withOptions.OverwriteFileIfExists, _serializationUtils.GetUsedEncoding());
            }

            {
                // Write public key
                var publicKeyXmlPoco = new Key.Xml.EnvCryptKey();
                _pocoMapper.Map(GetPublicKey(thisKey), publicKeyXmlPoco);
                var toWrite = _serializationUtils.Serialize(publicKeyXmlPoco);
                _writer.Write(withOptions.NewPublicKeyFullFilePath, toWrite,
                    withOptions.OverwriteFileIfExists, _serializationUtils.GetUsedEncoding());
            }
        }


        protected override RsaKey GetPublicKey(RsaKey fromPrivateKey)
        {
            var publicKey = new RSAParameters()
            {
                Exponent = fromPrivateKey.Key.Exponent,
                Modulus = fromPrivateKey.Key.Modulus,
            };
            return new RsaKey(publicKey, fromPrivateKey.UseOaepPadding);
        }
    }
}