﻿using EnvCrypt.Core.Verb.AddEntry.PlainText;

namespace EnvCrypt.Core.Verb.AddEntry
{
    public class AddEntryUsingKeyFileWorkflowOptions : AddPlainTextEntryWorkflowOptions
    {
        public string KeyFilePath { get; set; }
    }
}