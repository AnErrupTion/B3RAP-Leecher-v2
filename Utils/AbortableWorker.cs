using System.ComponentModel;
using System.Threading;

namespace B3RAP_Leecher_v2.Utils
{
    /// <summary>
    /// Background Worker that can be aborted
    /// </summary>
    public class AbortableWorker : BackgroundWorker
    {
        /// <summary>
        /// Thread to assign
        /// </summary>
        private Thread worker;

        /// <summary>
        /// OnDoWork event that supports aborting.
        /// </summary>
        /// <param name="e">The DoWorkEventArgs</param>
        protected override void OnDoWork(DoWorkEventArgs e)
        {
            try
            {
                // Assign the thread
                worker = Thread.CurrentThread;

                // Do the basic work
                base.OnDoWork(e);
            }
            catch (ThreadAbortException)
            {
                // Cancel must be true
                e.Cancel = true;

                // Prevents propagation
                Thread.ResetAbort();
            }
        }

        /// <summary>
        /// Abort will stop the thread behind the worker
        /// </summary>
        public void Abort()
        {
            if (worker != null && worker.IsAlive && MainForm._stop)
            {
                CancelAsync();
                worker.Abort();
                worker = null;
            }
        }
    }
}
