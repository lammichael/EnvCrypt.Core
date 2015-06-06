﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using EnvCrypt.Core.EncrypedData.UserStringConverter;
using EnvCrypt.Core.EncryptionAlgo.Aes;
using EnvCrypt.Core.Key.Aes;
using EnvCrypt.Core.Verb.LoadDat;
using EnvCrypt.Core.Verb.LoadKey;

namespace EnvCrypt.Core.Verb.DecryptEntry.Aes
{
    public class DecryptAesEntryWorkflowBuilder : GenericBuilder
    {
        private IKeyLoader<AesKey, KeyFromFileDetails> _keyLoader;
        private IDatLoader _datLoader;

        private DecryptEntryWorkflow<AesKey, DecryptEntryWorkflowOptions> _workflow;

        public DecryptAesEntryWorkflowBuilder()
        {
            _keyLoader = LoadKeyFromXmlFileFactory.GetAesKeyLoader();
            _datLoader = DatFromXmlFileFactory.GetDatLoader();
        }


        public DecryptAesEntryWorkflowBuilder WithKeyLoader(IKeyLoader<AesKey, KeyFromFileDetails> keyLoader)
        {
            Contract.Requires<ArgumentNullException>(keyLoader != null, "keyLoader");
            //
            _keyLoader = keyLoader;
            MarkAsNotBuilt();
            return this;
        }


        public DecryptAesEntryWorkflowBuilder WithDatLoader(IDatLoader datLoader)
        {
            Contract.Requires<ArgumentNullException>(datLoader != null, "datLoader");
            //
            _datLoader = datLoader;
            MarkAsNotBuilt();
            return this;
        }


        /// <summary>
        /// Prepares the Builder ready for use. This must be called before your first call to the <see cref="Run"/> method.
        /// This method is idempotent.
        /// </summary>
        /// <returns>the same Builder instance</returns>
        public DecryptAesEntryWorkflowBuilder Build()
        {
            var entriesDecrypter = new EntriesDecrypter<AesKey>(
                new AesKeySuitabilityChecker(),
                new Utf16LittleEndianUserStringConverter(),
                new AesSegmentEncryptionAlgo(new AesAlgo()));

            _workflow = new DecryptEntryUsingKeyWorkflow<AesKey, DecryptEntryWorkflowOptions>(_datLoader, entriesDecrypter, _keyLoader);
            IsBuilt = true;
            return this;
        }


        public IList<EntriesDecrypterResult> Run(DecryptEntryWorkflowOptions options)
        {
            Contract.Requires<ArgumentNullException>(options != null, "options");

            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(options.DatFilePath), "DAT file path cannot be null or whitespace");

            Contract.Requires<ArgumentException>(Contract.ForAll(options.CategoryEntryPair, t => !string.IsNullOrWhiteSpace(t.Category)),
                "none of the category names can be null or whitespace");
            Contract.Requires<ArgumentException>(Contract.ForAll(options.CategoryEntryPair, t => !string.IsNullOrWhiteSpace(t.Entry)),
                "none of the entry names can be null or whitespace");

            Contract.Requires<ArgumentException>(Contract.ForAll(options.KeyFilePaths, s => !string.IsNullOrWhiteSpace(s)),
                "key file path cannot be null or whitespace");
            //
            if (!IsBuilt)
            {
                throw new EnvCryptException("workflow cannot be run because it has not been built");
            }
            return _workflow.Run(options);
        }


        protected override void SetWorkflowToNull()
        {
            _workflow = null;
        }

        protected override bool IsWorkflowNull()
        {
            return _workflow == null;
        }

        [ContractInvariantMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(IsBuilt == (_workflow != null));
        }
    }
}
