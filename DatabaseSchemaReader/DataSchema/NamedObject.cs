﻿using System;
using System.Data;
using System.Linq;

namespace DatabaseSchemaReader.DataSchema
{
    /// <summary>
    /// A database object that should have an unique name within the collection
    /// </summary>
    [Serializable]
    public abstract class NamedObject<T> : IEquatable<T>, INamedObject where T : NamedObject<T>
    {

        /// <summary>
        /// Gets or sets the name (original database format)
        /// </summary>
        /// <value>
        /// The table name.
        /// </value>
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>
        /// The tag.
        /// </value>
        public object Tag
        {
            get; set;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        public virtual bool Equals(T other)
        {
            if (other == null)
            {
                return false;
            }
            if (Name == null && other.Name == null)
                return Equals(this, other);
            return (Name == other.Name);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var o = (NamedObject<T>) obj;
            if (Name == null && o.Name == null)
                return base.Equals(obj);
            return (Name == o.Name);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            if (string.IsNullOrEmpty(Name))
                return base.GetHashCode(); //transient instance
            return Name.GetHashCode();
        }

        /// <summary>
        /// Additional database-specific properties of this object
        /// </summary>
        public SerializableAdditionalProperties AdditionalProperties { get; set; }

        /// <summary>
        /// constructor for derived classes
        /// </summary>
        protected NamedObject()
        {
            AdditionalProperties = new SerializableAdditionalProperties();
        }

        /// <summary>
        /// Add additional properties from database result
        /// </summary>
        /// <param name="record">Database record read</param>
        /// <param name="additionalPropertyNames">Properties to be extracted</param>
        /// <exception cref="Exception">If column with property name is not present in record</exception>
        public void AddAdditionalProperties(IDataRecord record, string[] additionalPropertyNames)
        {
            foreach (var s in additionalPropertyNames ?? Enumerable.Empty<string>())
            {
                int ix = record.GetOrdinal(s);
                AdditionalProperties.AddOrReplace(s, record.IsDBNull(ix) ? null : record.GetValue(ix));
            }
        }

    }
}
