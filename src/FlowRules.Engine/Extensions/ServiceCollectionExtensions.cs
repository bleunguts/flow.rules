﻿using System;
using FlowRules.Engine.Interfaces;
using FlowRules.Engine.Models;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FlowRules.Engine.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFlowRules<T>(
            this IServiceCollection services,
            Func<Policy<T>> ruleAction,
            Action<FlowRulesOptions> setupAction = null)
            where T : class
        {
            FlowRulesOptions options = new();

            setupAction?.Invoke(options);

            Policy<T> policy = ruleAction();
            services.AddSingleton(policy);

            services.AddSingleton<IPolicyManager<T>, PolicyManager<T>>();

            if (options.ResultsRepository != null)
            {
                //    if (!options.ResultsRepository.GetInterfaces()
                //            .Any(x =>
                //                x.IsGenericType &&
                //                x.GetGenericTypeDefinition() == (typeof(IPolicyResultsRepository<T>))))
                //    {
                //        throw new InvalidOperationException($"[{options.ResultsRepository.Name}] is not of type [{nameof(IPolicyResultsRepository<T>)}]");
                //    }
                services.TryAddSingleton(typeof(IPolicyResultsRepository<T>), options.ResultsRepository);
            }
            else
            {
                services.TryAddSingleton(typeof(NullPolicyResultsRepository<T>));
            }

            return services;
        }
    }
}