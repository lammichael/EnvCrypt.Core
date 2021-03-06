﻿using System.Diagnostics.Contracts;
using EnvCrypt.Core.Key.Mapper.Xml.ToXmlPoco;
using EnvCrypt.Core.Key.Rsa;
using EnvCrypt.Core.Key.XmlPoco;
using EnvCrypt.Core.Utils;
using EnvCrypt.Core.Utils.IO;
using EnvCrypt.Core.Utils.IO.StringWriter;
using EnvCrypt.Core.Verb.GenerateKey.Rsa;

namespace EnvCrypt.Core.Verb.GenerateKey.Persister.Asymetric
{
    public static class AsymmetricKeyFilePersisterFactory
    {
        public static AsymmetricKeyFilePersister<RsaKey, EnvCryptKey, StringToFileWriterOptions> GetRsaKeyPersister()
        {
            Contract.Ensures(Contract.Result<AsymmetricKeyFilePersister<RsaKey, EnvCryptKey, StringToFileWriterOptions>>() != null);
            //
            return GetRsaKeyPersister(new StringToFileWriter(new MyDirectory(), new MyFile()));
        }


        public static AsymmetricKeyFilePersister<RsaKey, EnvCryptKey, StringToFileWriterOptions> GetRsaKeyPersister(IStringWriter<StringToFileWriterOptions> writer)
        {
            Contract.Ensures(Contract.Result<AsymmetricKeyFilePersister<RsaKey, EnvCryptKey, StringToFileWriterOptions>>() != null);
            //
            return new RsaKeyPersister(
                new RsaKeyToXmlMapper(new KeyDetailsPersistConverter()),
                new XmlSerializationUtils<EnvCryptKey>(),
                writer);
        }
    }
}
