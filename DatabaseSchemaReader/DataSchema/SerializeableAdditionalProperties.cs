using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DatabaseSchemaReader.DataSchema {
    [Serializable]
    public class SerializableAdditionalProperties {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<string> _additionalPropertyNames;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<object> _additionalPropertyValues;

        private Dictionary<string, object> _cache;

        internal SerializableAdditionalProperties() {
            _additionalPropertyNames = new List<string>();
            _additionalPropertyValues = new List<object>();
        }

        public void Add(string name, object value) {
            // TODO: Remove existing name and value? - because some objects are "doubly filled", e.g. constraints (FindConstraint, then fill).

            _additionalPropertyNames.Add(name);
            _additionalPropertyValues.Add(value);
            _cache = null;
        }

        public object Get(string name) {
            if (_cache == null) {
                _cache = _additionalPropertyNames
                    .Select((n, ix) => new {
                        Name = n, Ix = ix
                    })
                    .ToDictionary(nix => nix.Name, nix => _additionalPropertyValues[nix.Ix]);
            }
            object value;
            _cache.TryGetValue(name, out value);
            return value;
        }

        public List<string> AllNames => _additionalPropertyNames;

        public List<object> AllValues => _additionalPropertyValues;
    }
}