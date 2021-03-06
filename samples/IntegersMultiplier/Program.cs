﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NLog;
using JustConveyor;
using JustConveyor.Contracts.Dependencies;
using JustConveyor.Contracts.Settings;
using JustConveyor.Contracts.Utils;
using JustConveyor.Injection;

namespace IntegersMultiplier
{
    internal class Program
    {
        // Boostraping conveyor
        private static Finalizer BootstrapContainer()
        {
            var logger = LogManager.GetCurrentClassLogger();
            var container = new IoCContainer();
            container.SetLogger(logger);
            Injection.RegisterInjectionProvider(container);

            // Preparing jobs and finalizer
            // This will be our jobs
            var processingInts = Enumerable.Range(0, 1000).ToList();
            container.RegisterSingle<IEnumerable<int>>(processingInts);
            // And in "collector" we will accumulate results.
            container.RegisterSingle("collector", new ConcurrentBag<int>());
            // To find out when we can close application we use CountFinalizer
            Action inTheEnd =
                () => logger.Info($"Result: {string.Join(",", container.Get<IEnumerable<int>>("collector"))}");

            var finalizer = new CountFinalizer(processingInts.Count, inTheEnd);

            // And boostrapping Conveyor itself
            Conveyor.Init(logger)
                .ScanForBlueprints()
                .WithMetricsService(new MetricsServiceSettings
                {
                    BaseAddress = "http://localhost:5001/",
                    CorsAddresses = new List<string> {"http://localhost/*"},
                    MetricsConfig = new MetricsConfig
                    {
                        IncludeLastLogsFrom = new List<string> {"mainLogFile"},
                        CountOfLogLines = 100
                    }
                })
                .WithSupplier("IntsSupplier", Injection.InjectionProvider.Get<IntegersSupplier>())
                .WithFinalizer(finalizer)
                .Start();

            return finalizer;
        }

        private static void Main(string[] args)
        {
            BootstrapContainer().GetWaitTask().Wait();
        }
    }
}