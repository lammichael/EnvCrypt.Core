﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using EnvCrypt.Core.EncrypedData.UserStringConverter;
using EnvCrypt.Core.EncryptionAlgo.PlainText;
using EnvCrypt.Core.Key.PlainText;
using EnvCrypt.Core.Verb.LoadDat;
using EnvCrypt.Core.Verb.LoadKey;

namespace EnvCrypt.Core.Verb.DecryptEntry.PlainText
{
    public class DecryptPlainTextEntryWorkflowBuilder : GenericBuilder
    {
        private IDatLoader _datLoader;

        private DecryptEntryWorkflow<PlainTextKey, DecryptPlainTextEntryWorkflowOptions> _workflow;

        public DecryptPlainTextEntryWorkflowBuilder()
        {
            _datLoader = DatFromXmlFileFactory.GetDatLoader();
        }


        public DecryptPlainTextEntryWorkflowBuilder WithDatLoader(IDatLoader datLoader)
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
        public DecryptPlainTextEntryWorkflowBuilder Build()
        {
            var entriesDecrypter = new EntriesDecrypter<PlainTextKey>(
                new PlainTextKeySuitabilityChecker(),
                new Utf16LittleEndianUserStringConverter(),
                new PlainTextSegmentEncryptionAlgo());

            _workflow = new DecryptPlainTextEntryWorkflow<PlainTextKey, DecryptPlainTextEntryWorkflowOptions>(_datLoader, entriesDecrypter, LoadKeyFromXmlFileFactory.GetPlainTextKeyLoader());
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
