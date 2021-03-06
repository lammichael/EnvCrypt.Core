﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using EnvCrypt.Core.EncrypedData.Poco;
using EnvCrypt.Core.EncrypedData.XmlPoco;
using EnvCrypt.Core.EncryptionAlgo;
using EnvCrypt.Core.Utils;

namespace EnvCrypt.Core.EncrypedData.Mapper.Xml.ToDatPoco
{
    class XmlToDatMapper : IExternalRepresentationToDatMapper<EnvCryptEncryptedData>
    {
        private readonly IEncryptedDetailsPersistConverter _detailConverter;

        public XmlToDatMapper(IEncryptedDetailsPersistConverter detailConverter)
        {
            Contract.Requires<ArgumentNullException>(detailConverter != null, "detailConverter");
            //
            _detailConverter = detailConverter;
        }


        /// <summary>
        /// If there is no Decryption element in XML, then don't
        /// </summary>
        /// <param name="fromExternalRepresentationPoco"></param>
        /// <returns></returns>
        public EnvCryptDat Map(EnvCryptEncryptedData fromExternalRepresentationPoco)
        {
            // When the file is empty, return an empty list.
            if (fromExternalRepresentationPoco.Items == null || 
                fromExternalRepresentationPoco.Items.Length == 0)
            {
                return new EnvCryptDat()
                {
                    Categories = new List<Category>()
                };
            }

            var categories = new List<Category>(fromExternalRepresentationPoco.Items.Length);
            // Each category
            for (uint catI = 0; catI < fromExternalRepresentationPoco.Items.Length; catI++)
            {
                var currentXmlCategory = fromExternalRepresentationPoco.Items[catI];
                if (currentXmlCategory == null)
                {
                    continue;
                }

                var categoryToAdd = new Category();

                if (string.IsNullOrWhiteSpace(currentXmlCategory.Name))
                {
                    throw new EnvCryptException("category name in XML is null");
                }
                categoryToAdd.Name = currentXmlCategory.Name;

                categoryToAdd.Entries = new List<Entry>();
                if (currentXmlCategory.Entry == null || currentXmlCategory.Entry.Length == 0)
                {
                    continue;
                }

                // Each entry in category
                for (uint entryI = 0; entryI < currentXmlCategory.Entry.Length; entryI++)
                {
                    var currentXmlEntry = currentXmlCategory.Entry[entryI];
                    if (currentXmlEntry == null)
                    {
                        continue;
                    }

                    var entryToAdd = new Entry();
                    
                    if (string.IsNullOrWhiteSpace(currentXmlEntry.Name))
                    {
                        throw new EnvCryptException("entry name in XML is empty in category: {0}", currentXmlCategory.Name);
                    }
                    entryToAdd.Name = currentXmlEntry.Name;


                    var currentEntryDecryption = currentXmlEntry.Decryption;
                    if (currentEntryDecryption == null)
                    {
                        entryToAdd.EncryptionAlgorithm = EnvCryptAlgoEnum.PlainText;
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(currentEntryDecryption.KeyName))
                        {
                            throw new EnvCryptException("key name in XML is empty in Category: {0}  Entry: {1}", currentXmlCategory.Name, currentXmlEntry.Name);
                        }
                        entryToAdd.KeyName = currentEntryDecryption.KeyName;

                        entryToAdd.KeyHash = currentEntryDecryption.Hash;

                        EnvCryptAlgoEnum parsedAlgoType;
                        if (!Enum.TryParse(currentEntryDecryption.Algo, true, out parsedAlgoType))
                        {
                            throw new EnvCryptException("{0} is an unrecognised EnvCrypt encryption algo name in Category: {0}  Entry: {1}", currentXmlCategory.Name, currentXmlEntry.Name);
                        }
                        entryToAdd.EncryptionAlgorithm = parsedAlgoType;
                    }


                    if (currentXmlEntry.EncryptedValue != null)
                    {
                        entryToAdd.EncryptedValue = new List<byte[]>(currentXmlEntry.EncryptedValue.Length);
                        for (uint evI = 0; evI < currentXmlEntry.EncryptedValue.Length; evI++)
                        {
                            var currentEncrypedSegment = currentXmlEntry.EncryptedValue[evI].Value;
                            entryToAdd.EncryptedValue.Add(
                                string.IsNullOrWhiteSpace(currentEncrypedSegment)
                                    ? new byte[0]
                                    : _detailConverter.Decode(currentEncrypedSegment, entryToAdd.EncryptionAlgorithm));
                        }
                    }

                    categoryToAdd.Entries.Add(entryToAdd);
                }

                categories.Add(categoryToAdd);
            }


            return new EnvCryptDat()
            {
                Categories = categories
            };
        }
    }
}
