using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DatabaseSchemaReader.DataSchema
{
    /// <summary>
    /// Represents a user in the database
    /// </summary>
    [Serializable]
    public partial class DatabaseStatistics : NamedSchemaObject<DatabaseStatistics>
    {
        #region Fields
        //backing fields
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<DatabaseColumn> _columns;
        #endregion


        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseIndex"/> class.
        /// </summary>
        public DatabaseStatistics() {
            _columns = new List<DatabaseColumn>();
        }


        /// <summary>
        /// Gets or sets the name of the table.
        /// </summary>
        /// <value>
        /// The name of the table.
        /// </value>
        public string TableName {
            get; set;
        }

        /// <summary>
        /// Gets the columns of the statistics.
        /// </summary>
        public List<DatabaseColumn> Columns {
            get {
                return _columns;
            }
        }
        
        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Name + " in " + TableName;
        }
    }
}
