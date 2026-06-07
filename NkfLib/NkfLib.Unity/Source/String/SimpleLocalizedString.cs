using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using static UnityEngine.EventSystems.EventTrigger;

namespace NkfLib.Unity
{
    [Serializable]
    public class SimpleLocalizedString
    {
        [SerializeField]
        public string StringTable;
        [SerializeField]
        public string Key;
        public SimpleLocalizedString() { }

        public SimpleLocalizedString(string stringTable, string key)
        {
            StringTable = stringTable;
            Key = key;
        }

        public override string ToString()
        {
            return LocalizationSettings.StringDatabase.GetTableEntry(StringTable, Key).Entry?.Value;
        }
    }
}
