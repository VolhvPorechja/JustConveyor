﻿using System.Collections.Generic;

namespace JustConveyor.Contracts.Settings
{
	/// <summary>
    /// Settings for service.
    /// </summary>
    public class MetricsServiceSettings
    {
        /// <summary>
        /// Base address for service.
        /// </summary>
        public string BaseAddress { get; set; }

        /// <summary>
        /// Allowed addresses by CORS policy.
        /// </summary>
        public List<string> CorsAddresses { get; set; }

		/// <summary>
		/// Config for collecting metrics.
		/// </summary>
		public MetricsConfig MetricsConfig { get; set; }
    }
}