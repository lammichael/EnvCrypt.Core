﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using EnvCrypt.Core.EncrypedData.Mapper;
using EnvCrypt.Core.EncrypedData.Poco;
using EnvCrypt.Core.EncrypedData.XmlPoco;
using EnvCrypt.Core.Utils;
using EnvCrypt.Core.Utils.IO;

namespace EnvCrypt.Core.Verb.LoadDat
{
    class DatFromXmlFileLoader : IDatLoader<DatFromFileLoaderOptions>
    {
        private readonly IMyFile _myFile;
        private readonly ITextReader _xmlReader;
        private readonly IXmlSerializationUtils<EnvCryptEncryptedData> _xmlSerializationUtils;
        private readonly IExternalRepresentationToDatMapper<EnvCryptEncryptedData> _xmlToPocoMapper;

        public DatFromXmlFileLoader(
            IMyFile myFile,
            ITextReader xmlReader,
            IXmlSerializationUtils<EnvCryptEncryptedData> xmlSerializationUtils,
            IExternalRepresentationToDatMapper<EnvCryptEncryptedData> xmlToPocoMapper)
        {
            Contract.Requires<ArgumentNullException>(myFile != null, "myFile");
            Contract.Requires<ArgumentNullException>(xmlReader != null, "xmlReader");
            Contract.Requires<ArgumentNullException>(xmlSerializationUtils != null, "xmlSerializationUtils");
            Contract.Requires<ArgumentNullException>(xmlToPocoMapper != null, "xmlToPocoMapper");
            //
            _myFile = myFile;
            _xmlReader = xmlReader;
            _xmlSerializationUtils = xmlSerializationUtils;
            _xmlToPocoMapper = xmlToPocoMapper;
        }


        public EnvCryptDat Load(DatFromFileLoaderOptions options)
        {
            if (!_myFile.Exists(options.DatFilePath))
            {
                return new EnvCryptDat()
                {
                    Categories = new List<Category>()
                };
            }

            var xmlFromFile = _xmlReader.ReadAllText(options.DatFilePath);
            if (string.IsNullOrWhiteSpace(xmlFromFile))
            {
                throw new EnvCryptException("key file is empty: {0}", options.DatFilePath);
            }

            var xmlPoco = _xmlSerializationUtils.Deserialize(xmlFromFile);
            return _xmlToPocoMapper.Map(xmlPoco);
        }
    }
}
