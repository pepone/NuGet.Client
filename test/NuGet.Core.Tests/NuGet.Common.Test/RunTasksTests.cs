// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace NuGet.Common.Test
{
    public class RunTasksTests
    {
        [Theory]
        [InlineData(0, 0, 0, true)]
        [InlineData(0, 0, 0, false)]
        [InlineData(1, 1, 0, true)]
        [InlineData(1, 1, 1, true)]
        [InlineData(1, 1, 64, true)]
        [InlineData(10000, 2, 64, true)]
        [InlineData(100, 10, 8, true)]
        [InlineData(10000, 2, 64, false)]

        public async Task RunTasks_VerifyTaskResults(int max, int delay, int maxThreads, bool useTaskRun)
        {
            var tasks = new List<Func<Task<int>>>(max);

            for (var i = 0; i < max; i++)
            {
                var cur = i;
                tasks.Add(new Func<Task<int>>(() =>
                {
                    var taskCurrent = cur;
                    return Run(taskCurrent, delay);
                }));
            }

            var results = await ConcurrencyUtilities.RunAsync(tasks,
                maxThreads: maxThreads,
                useTaskRun: useTaskRun);

            var expected = new int[max];

            results.Length.Should().Be(max);

            for (var i = 0; i < max; i++)
            {
                results[i].Should().Be(i * 2);
            }
        }

        private static async Task<int> Run(int x, int delay)
        {
            await Task.Delay(delay);
            return x * 2;
        }
    }
}
