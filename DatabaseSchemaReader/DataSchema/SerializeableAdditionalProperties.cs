using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DatabaseSchemaReader.DataSchema {
    [Serializable]
    public class SerializableAdditionalProperties {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<string> _additionalPropertyNames;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<object> _additionalPropertyValues;

        internal SerializableAdditionalProperties()
        {
            _additionalPropertyNames = new List<string>();
            _additionalPropertyValues = new List<object>();
        }

        public void Add(string name, object value)
        {
            // TODO: Remove existing name and value? - because some objects are "doubly filled", e.g. constraints (FindConstraint, then fill).

            _additionalPropertyNames.Add(name);
            _additionalPropertyValues.Add(value);
        }

        public object Get(string name)
        {
            int ix = _additionalPropertyNames.FindIndex(s => s == name);
            return ix < 0 ? null : _additionalPropertyValues[ix];
        }

        public List<string> AllNames => _additionalPropertyNames;

        public List<object> AllValues => _additionalPropertyValues;
    }
}