﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// ParallelExtensionsExtras: https://code.msdn.microsoft.com/ParExtSamples

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceDebugger.Threading
{
    /// <summary>Provides a scheduler that uses STA threads.</summary>
    public sealed class StaTaskScheduler : TaskScheduler, IDisposable
    {
        /// <summary>Stores the queued tasks to be executed by our pool of STA threads.</summary>
        private BlockingCollection<Task> _tasks;

        /// <summary>Initializes a new instance of the StaTaskScheduler class with the specified concurrency level.</summary>
        /// <param name="numberOfThreads">The number of threads that should be created and used by this scheduler.</param>
        public StaTaskScheduler(int numberOfThreads)
        {
            // Validate arguments
            if (numberOfThreads < 1)
                throw new ArgumentOutOfRangeException(nameof(numberOfThreads));

            // Initialize the tasks collection
            _tasks = new BlockingCollection<Task>();

            // Create the threads to be used by this scheduler
            Threads = new ReadOnlyCollection<Thread>(Enumerable.Range(0, numberOfThreads).Select(i =>
            {
                var thread = new Thread(() =>
                {
                    // Continually get the next task and try to execute it.
                    // This will continue until the scheduler is disposed and no more tasks remain.
                    foreach (Task t in _tasks.GetConsumingEnumerable())
                        if (!TryExecuteTask(t))
                            Debug.Assert(t.IsCompleted, "Can't run, not completed");
                });
                thread.IsBackground = true;
                thread.SetApartmentState(ApartmentState.STA);
                return thread;
            }).ToList());

            // Start all of the threads
            foreach (Thread thread in Threads) thread.Start();
        }

        /// <summary>Gets a StaTaskScheduler for the current AppDomain.</summary>
        /// <remarks>
        ///     We use a count of 1, because the editor ends up re-using <see cref="System.Windows.Threading.DispatcherObject" />
        ///     instances between tests, so we need to always use the same thread for our Sta tests.
        /// </remarks>
        public static StaTaskScheduler DefaultSta { get; } = new StaTaskScheduler(1);

        /// <summary>The STA threads used by the scheduler.</summary>
        public ReadOnlyCollection<Thread> Threads { get; }

        /// <summary>Gets the maximum concurrency level supported by this scheduler.</summary>
        public override int MaximumConcurrencyLevel => Threads.Count;

        /// <summary>
        ///     Cleans up the scheduler by indicating that no more tasks will be queued.
        ///     This method blocks until all threads successfully shutdown.
        /// </summary>
        public void Dispose()
        {
            if (_tasks != null)
            {
                // Indicate that no new tasks will be coming in
                _tasks.CompleteAdding();

                // Wait for all threads to finish processing tasks
                foreach (Thread thread in Threads)
                    thread.Join();

                // Cleanup
                _tasks.Dispose();
                _tasks = null;
            }
        }

        /// <summary>Queues a Task to be executed by this scheduler.</summary>
        /// <param name="task">The task to be executed.</param>
        protected override void QueueTask(Task task)
        {
            // Push it into the blocking collection of tasks
            _tasks.Add(task);
        }

        /// <summary>Provides a list of the scheduled tasks for the debugger to consume.</summary>
        /// <returns>An enumerable of all tasks currently scheduled.</returns>
        protected override IEnumerable<Task> GetScheduledTasks()
        {
            // Serialize the contents of the blocking collection of tasks for the debugger
            return _tasks.ToArray();
        }

        /// <summary>Determines whether a Task may be inlined.</summary>
        /// <param name="task">The task to be executed.</param>
        /// <param name="taskWasPreviouslyQueued">Whether the task was previously queued.</param>
        /// <returns>true if the task was successfully inlined; otherwise, false.</returns>
        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            // Try to inline if the current thread is STA
            return
                Thread.CurrentThread.GetApartmentState() == ApartmentState.STA &&
                TryExecuteTask(task);
        }

        public bool IsAnyQueued()
        {
            if (Threads.Count != 1 || Threads[0] != Thread.CurrentThread)
                throw new InvalidOperationException("Operation invalid in this context");

            return _tasks.Count > 0;
        }
    }
}