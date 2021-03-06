﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using EnvCrypt.Core.EncrypedData.UserStringConverter;
using EnvCrypt.Core.EncryptionAlgo;
using EnvCrypt.Core.EncryptionAlgo.Aes;
using EnvCrypt.Core.Key.Aes;
using EnvCrypt.Core.Verb.DecryptEntry.Audit;
using EnvCrypt.Core.Verb.LoadDat;
using EnvCrypt.Core.Verb.LoadKey;

namespace EnvCrypt.Core.Verb.DecryptEntry.Aes
{
    public class DecryptAesEntryWorkflowBuilder : GenericBuilder, IDecryptAesEntryWorkflowBuilder
    {
        private ISegmentEncryptionAlgo<AesKey> _segmentEncryptionAlgo;
        private IKeyLoader<AesKey, KeyFromFileDetails> _keyLoader;
        private IDatLoader<DatFromFileLoaderOptions> _datLoader;
        private IAuditLogger<AesKey, DecryptEntryWorkflowOptions> _auditLogger;

        private DecryptEntryWorkflow<AesKey, DecryptEntryWorkflowOptions, DatFromFileLoaderOptions> _workflow;

        public DecryptAesEntryWorkflowBuilder()
        {
            _segmentEncryptionAlgo = new AesSegmentEncryptionAlgo(new AesAlgo());
            _keyLoader = LoadKeyFromXmlFileFactory.GetAesKeyLoader();
            _datLoader = DatFromXmlFileFactory.GetDatLoader();
            _auditLogger = new NullAuditLogger<AesKey, DecryptEntryWorkflowOptions>();
        }


        public IDecryptAesEntryWorkflowBuilder WithKeyLoader(IKeyLoader<AesKey, KeyFromFileDetails> keyLoader)
        {
            _keyLoader = keyLoader;
            MarkAsNotBuilt();
            return this;
        }


        public IDecryptAesEntryWorkflowBuilder WithDatLoader(IDatLoader<DatFromFileLoaderOptions> datLoader)
        {
            _datLoader = datLoader;
            MarkAsNotBuilt();
            return this;
        }


        public IDecryptAesEntryWorkflowBuilder WithAuditLogger(IAuditLogger<AesKey, DecryptEntryWorkflowOptions> auditLogger)
        {
            _auditLogger = auditLogger;
            MarkAsNotBuilt();
            return this;
        }


        internal DecryptAesEntryWorkflowBuilder WithAesSegmentEncryptionAlgo(ISegmentEncryptionAlgo<AesKey> segmentEncryptionAlgo)
        {
            Contract.Requires<ArgumentNullException>(segmentEncryptionAlgo != null, "segmentEncryptionAlgo");
            //
            _segmentEncryptionAlgo = segmentEncryptionAlgo;
            MarkAsNotBuilt();
            return this;
        }


        /// <summary>
        /// Prepares the Builder ready for use. This must be called before your first call to the <see cref="Run"/> method.
        /// This method is idempotent.
        /// </summary>
        /// <returns>the same Builder instance</returns>
        public IDecryptAesEntryWorkflowBuilder Build()
        {
            var entriesDecrypter = new EntriesDecrypter<AesKey>(
                new AesKeySuitabilityChecker(),
                new Utf16LittleEndianUserStringConverter(),
                _segmentEncryptionAlgo);

            _workflow = new DecryptEntryUsingKeyWorkflow<AesKey, DecryptEntryWorkflowOptions>(_datLoader, entriesDecrypter, _auditLogger, _keyLoader);
            IsBuilt = true;
            return this;
        }


        public IList<EntriesDecrypterResult<AesKey>> Run(DecryptEntryWorkflowOptions options)
        {
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
