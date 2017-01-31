using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DatabaseSchemaReader.DataSchema {
    /// <summary>
    /// Holder for additional properties of all schema objects to store database-specific flat information.
    /// </summary>
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

        /// <summary>
        /// Add or replace the value of a top level property. The internal cache is cleared in this operation.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddOrReplace(string name, object value) {
            int ix = _additionalPropertyNames.FindIndex(s => s == name);
            if (ix >= 0) {
                _additionalPropertyValues[ix] = value;
            } else {
                _additionalPropertyNames.Add(name);
                _additionalPropertyValues.Add(value);
            }
            _cache = null;
        }

        /// <summary>
        /// Get value of a property. This sets up an internal cache for all properties unless the cache
        /// is already present.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
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

        /// <summary>
        /// All the recorded property names
        /// </summary>
        public List<string> AllNames => _additionalPropertyNames;

        /// <summary>
        /// All the recorded property values, in the same order as <see cref="AllNames"/>.
        /// </summary>
        public List<object> AllValues => _additionalPropertyValues;
    }
}