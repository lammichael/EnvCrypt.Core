﻿using EnvCrypt.Core.EncryptionAlgo;

namespace EnvCrypt.Console.AddEntry
{
    class AddEntryCommandLineProcessor : VerbCommandLineProcessor<AddEntryVerbOptions>
    {
        protected override bool ReportErrors(AddEntryVerbOptions options)
        {
            var hasErrors = false;
            var algorithm = options.GetAlgorithm();
            if (algorithm == null)
            {
                System.Console.Error.WriteLine("Unrecognised algorithm: {0}", options.AlgorithmToUse);
                hasErrors = true;
            }
            else
            {
                // PlainText encryption doesn't require any key
                if (algorithm.Value != EnvCryptAlgoEnum.PlainText)
                {
                    if (string.IsNullOrWhiteSpace(options.KeyFile))
                    {
                        System.Console.Error.WriteLine("Key file path (encryption key) not defined.");
                        hasErrors = true;
                    }
                }
            }
            if (string.IsNullOrWhiteSpace(options.DatFile))
            {
                System.Console.Error.WriteLine("DAT file path not defined.");
                hasErrors = true;
            }
            if (string.IsNullOrWhiteSpace(options.Category))
            {
                System.Console.Error.WriteLine("Category not defined.");
                hasErrors = true;
            }
            if (string.IsNullOrWhiteSpace(options.NewEntryName))
            {
                System.Console.Error.WriteLine("Entry name not defined.");
                hasErrors = true;
            }
            if (string.IsNullOrWhiteSpace(options.StringToEncrypt))
            {
                System.Console.Error.WriteLine("String to encrypt not defined.");
                hasErrors = true;
            }

            return hasErrors;
        }


        protected override void RunWorkflow(AddEntryVerbOptions options)
        {
            new AddEntryWorkflow().Run(options);
        }
    }
}
