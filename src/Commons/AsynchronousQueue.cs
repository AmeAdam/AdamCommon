using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Commons
{
    public class AsynchronousQueue<T> : IDisposable
    {
        private readonly object sync = new object();
        private bool isProcessing;
        private volatile bool keepRunning = true;
        private List<T> list = new List<T>();
        private List<T> workingList = new List<T>();
        public Exception LastError;
        private Task task;
        private readonly Action<T> action;

        public AsynchronousQueue(Action<T> action)
        {
            this.action = action;
        }

        public void Add(T t)
        {
            if (!keepRunning)
                return;

            lock (sync)
            {
                list.Add(t);
                if (isProcessing)
                    return;
                isProcessing = true;
                CleanUpPreviousTask(task);
                task = Task.Factory.StartNew(ExecuteAll);
            }
        }

        public void WaitForComplete()
        {
            var t = task;
            if (t == null)
                return;
            t.Wait();
        }

        private static void CleanUpPreviousTask(Task task)
        {
            if (task == null)
                return;
            try
            {
                task.Wait();
            }
            catch (AggregateException)
            {
                return;
            }
            finally
            {
                task.Dispose();
            }
        }

        public void Dispose()
        {
            keepRunning = false;
            var t = task;
            if (t != null)
                t.ContinueWith(previousTask => previousTask.Exception);
        }

        private bool PrepareWorkingList()
        {
            lock (sync)
            {
                if (list.Count == 0)
                    return false;
                var temp = workingList;
                workingList = list;
                list = temp;
                return true;
            }
        }

        void ExecuteAll()
        {
            PrepareWorkingList();
            while (keepRunning && workingList.Count > 0)
            {
                try
                {
                    foreach (var item in workingList)
                    {
                        action(item);
                        if (!keepRunning)
                            break;
                    }
                }
                catch (Exception e)
                {
                    LastError = e;
                }
                finally
                {
                    workingList.Clear();
                }

                lock (sync)
                {
                    try
                    {
                        if (!keepRunning || !PrepareWorkingList())
                        {
                            isProcessing = false;
                            return;
                        }
                    }
                    catch (Exception)
                    {
                        isProcessing = false;
                        throw;
                    }
                }
            }
        }
    }
}