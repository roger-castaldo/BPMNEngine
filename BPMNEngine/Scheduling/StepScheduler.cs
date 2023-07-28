using BPMNEngine.Elements.Processes.Events;
using BPMNEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BPMNEngine.Scheduling
{
    internal class StepScheduler : IScheduleEngine
    {
        public readonly static StepScheduler Instance = new StepScheduler();

        private readonly struct SProcessSuspendEvent
        {
            public ProcessInstance Instance { get; init; }
            public AEvent Event { get; init; }
            public DateTime EndTime { get; init; }
        }

        private readonly struct SProcessDelayedEvent
        {
            public ProcessInstance Instance { get; init; }
            public BoundaryEvent Event { get; init; }
            public DateTime StartTime { get; init; }
            public string SourceID { get; init; }
        }

        private bool disposedValue;
        private bool done;
        private readonly ReaderWriterLockSlim locker;
        private readonly List<SProcessSuspendEvent> suspendEvents;
        private readonly List<SProcessDelayedEvent> delayedEvents;
        private readonly ManualResetEvent backgroundMREEvent;

        private StepScheduler() {
            locker=new();
            suspendEvents=new();
            delayedEvents=new();
            backgroundMREEvent=new(true);
            ProduceBackgroundTask();
        }

        private void ProduceBackgroundTask()
        {
            _=Task.Run(() =>
            {
                while (!done)
                {
                    TimeSpan sleep = TimeSpan.MaxValue;
                    var ts = DateTime.Now;
                    locker.EnterReadLock();
                    if (suspendEvents.Any()||delayedEvents.Any())
                    {
                        sleep = suspendEvents.Select(se=>se.EndTime.Subtract(ts))
                        .Concat(delayedEvents.Select(de=>de.StartTime.Subtract(ts)))
                        .Min();
                    }
                    locker.ExitReadLock();
                    if (sleep==TimeSpan.MaxValue)
                        backgroundMREEvent.WaitOne();
                    else if (sleep>TimeSpan.Zero)
                        backgroundMREEvent.WaitOne(sleep);
                    else
                    {
                        locker.EnterWriteLock();

                        IEnumerable<object> toRemove = Array.Empty<object>();

                        suspendEvents
                            .Where(se => se.EndTime.Ticks<=ts.Ticks)
                            .ForEach(se => {
                                try
                                {
                                    se.Instance.CompleteTimedEvent(se.Event);
                                    toRemove = toRemove.Append(se);
                                }catch(Exception e){ se.Instance.WriteLogException(se.Event, new StackFrame(1, true), DateTime.Now, e); }
                            });
                        suspendEvents.RemoveAll(se=>toRemove.Contains(se));
                        
                        toRemove = Array.Empty<object>();
                        delayedEvents
                            .Where(de=>de.StartTime.Ticks<=ts.Ticks)
                            .ForEach(de => {
                                try
                                {
                                    de.Instance.StartTimedEvent(de.Event,de.SourceID);
                                    toRemove = toRemove.Append(de);
                                }
                                catch (Exception e) { de.Instance.WriteLogException(de.Event, new StackFrame(1, true), DateTime.Now, e); }
                            });
                        delayedEvents.RemoveAll(de => toRemove.Contains(de));

                        locker.ExitWriteLock();
                    }
                    if (!done)
                        backgroundMREEvent.Reset();
                }
            });
        }

        public void AbortDelayedEvent(ProcessInstance process, BoundaryEvent evnt, string sourceID)
        {
            locker.EnterWriteLock();
            var changed = delayedEvents.RemoveAll(de=>
            de.Instance.ID==process.ID 
            && de.SourceID==sourceID
            && de.Event.id==evnt.id)>0;
            locker.ExitWriteLock();
            if (changed)
                backgroundMREEvent.Set();
        }

        public void AbortSuspendedElement(ProcessInstance process, string id)
        {
            locker.EnterWriteLock();
            var changed = suspendEvents.RemoveAll(se =>
            se.Instance.ID==process.ID
            && se.Event.id==id)>0;
            locker.ExitWriteLock();
            if (changed)
                backgroundMREEvent.Set();
        }

        public void DelayStart(TimeSpan value, ProcessInstance process, BoundaryEvent evnt, string sourceID)
        {
            locker.EnterWriteLock();
            delayedEvents.Add(new()
            {
                Event=evnt,
                Instance=process,
                SourceID=sourceID,
                StartTime=DateTime.Now.Add(value)
            });
            locker.ExitWriteLock();
            backgroundMREEvent.Set();
        }

        public void Sleep(TimeSpan value, ProcessInstance process, AEvent evnt)
        {
            locker.EnterWriteLock();
            suspendEvents.Add(new()
            {
                Event=evnt,
                Instance=process,
                EndTime=DateTime.Now.Add(value)
            });
            locker.ExitWriteLock();
            backgroundMREEvent.Set();
        }

        public void UnloadProcess(BusinessProcess process)
        {
            locker.EnterWriteLock();
            var changed = suspendEvents.RemoveAll(se =>
            se.Instance.Process.Equals(process))
            +delayedEvents.RemoveAll(de=>
            de.Instance.Process.Equals(process))>0;
            locker.ExitWriteLock();
            if (changed)
                backgroundMREEvent.Set();
        }

        public void UnloadProcess(ProcessInstance process)
        {
            locker.EnterWriteLock();
            var changed = suspendEvents.RemoveAll(se =>
            se.Instance.Equals(process))
            +delayedEvents.RemoveAll(de =>
            de.Instance.Equals(process))>0;
            locker.ExitWriteLock();
            if (changed)
                backgroundMREEvent.Set();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    done=true;
                    locker.EnterWriteLock();
                    suspendEvents.Clear();
                    delayedEvents.Clear();
                    backgroundMREEvent.Set();
                    locker.ExitWriteLock();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                backgroundMREEvent.Dispose();
                locker.Dispose();
                disposedValue=true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~StepScheduler()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
