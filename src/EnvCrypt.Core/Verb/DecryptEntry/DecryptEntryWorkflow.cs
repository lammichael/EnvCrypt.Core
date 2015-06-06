﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using EnvCrypt.Core.Key;
using EnvCrypt.Core.Verb.DecryptEntry.PlainText;
using EnvCrypt.Core.Verb.LoadDat;

namespace EnvCrypt.Core.Verb.DecryptEntry
{
    [ContractClass(typeof(DecryptEntryWorkflowContracts<,>))]
    public abstract class DecryptEntryWorkflow<TKey, TWorkflowOptions>
        where TKey : KeyBase
        where TWorkflowOptions : DecryptPlainTextEntryWorkflowOptions
    {
        private readonly IDatLoader _datLoader;
        private readonly EntriesDecrypter<TKey> _entriesDecrypter;

        protected DecryptEntryWorkflow(IDatLoader datLoader, EntriesDecrypter<TKey> entriesDecrypter)
        {
            Contract.Requires<ArgumentNullException>(datLoader != null, "datLoader");
            Contract.Requires<ArgumentNullException>(entriesDecrypter != null, "encryptWorkflow");
            //
            _datLoader = datLoader;
            _entriesDecrypter = entriesDecrypter;
        }


        public IList<EntriesDecrypterResult> Run(TWorkflowOptions options)
        {
            Contract.Requires<ArgumentNullException>(options != null, "options");

            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(options.DatFilePath), "DAT file path cannot be null or whitespace");

            Contract.Requires<ArgumentException>(Contract.ForAll(options.CategoryEntryPair, t => !string.IsNullOrWhiteSpace(t.Category)),
                "none of the category names can be null or whitespace");
            Contract.Requires<ArgumentException>(Contract.ForAll(options.CategoryEntryPair, t => !string.IsNullOrWhiteSpace(t.Entry)),
                "none of the entry names can be null or whitespace");

            /*Contract.Requires<ArgumentException>(typeof(TKey) == typeof(PlainTextKey) || 
                Contract.ForAll(options.KeyFilePaths, s => !string.IsNullOrWhiteSpace(s)), 
                "key file path cannot be null or whitespace");*/
            //

            var datPoco = _datLoader.Load(options.DatFilePath);
            var loadedKeys = LoadKeys(options);

            return _entriesDecrypter.Decrypt(loadedKeys, datPoco, options.CategoryEntryPair);
        }


        protected abstract List<TKey> LoadKeys(TWorkflowOptions workflowOptions);
    }



    [ContractClassFor(typeof(DecryptEntryWorkflow<,>))]
    internal abstract class DecryptEntryWorkflowContracts<TKey, TWorkflowOptions> : DecryptEntryWorkflow<TKey, TWorkflowOptions>
        where TKey : KeyBase
        where TWorkflowOptions : DecryptPlainTextEntryWorkflowOptions
    {
        private DecryptEntryWorkflowContracts(IDatLoader datLoader, EntriesDecrypter<TKey> entriesDecrypter) : base(datLoader, entriesDecrypter)
        {}


        protected override List<TKey> LoadKeys(TWorkflowOptions workflowOptions)
        {
            Contract.Requires<ArgumentNullException>(workflowOptions != null, "workflowOptions");
            Contract.Ensures(Contract.Result<List<TKey>>() != null);
            Contract.Ensures(Contract.Result<List<TKey>>().Any());

            return default(List<TKey>);
        }
    }
}
