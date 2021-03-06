﻿using System.Collections.Generic;
using Diagnostics.RuntimeHost.Utilities;

namespace Diagnostics.RuntimeHost.Models
{
    /// <summary>
    /// Compilation post body
    /// </summary>
    /// <typeparam name="T">Resource type.</typeparam>
    public class CompilationPostBody<T>
    {
        private string _sanitizedScript;

        /// <summary>
        /// Gets or sets the script.
        /// </summary>
        public string Script
        {
            get
            {
                return _sanitizedScript;
            }
            set
            {
                _sanitizedScript = FileHelper.SanitizeScriptFile(value);
            }
        }

        /// <summary>
        /// Gets or sets the source code reference.
        /// </summary>
        public IDictionary<string, string> References { get; set; }

        /// <summary>
        /// Gets or sets the entity type.
        /// </summary>
        public string EntityType { get; set; }

        /// <summary>
        /// Gets or sets the detector search terms
        /// </summary>
        public string DetectorUtterances { get; set; }

        /// <summary>
        /// Gets or sets the resource.
        /// </summary>
        public T Resource { get; set; }
    }
}
