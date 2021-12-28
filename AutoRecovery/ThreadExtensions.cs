using System;
using System.Threading;

//        #region ------------- Implementation example -------------------------------
//
//        using Abraham.Threading;
//
//        private ThreadExtensions _SupervisorThread;
//
//        private void StartSupervisorThread()
//        {
//            _SupervisorThread = new ThreadExtensions(SupervisorThreadProc);
//            _SupervisorThread.thread.Start();
//        }
//
//        private void StopSupervisorThread()
//        {
//            _SupervisorThread.SendStopSignalAndWait();
//        }
//
//        private void SupervisorThreadProc()
//        {
//            do
//			{
//				try
//				{
//				}
//				catch (Exception ex)
//				{
//					System.Diagnostics.Debug.WriteLine($"SupervisorThread: {ex.ToString()}");
//				}
//			}
//			while (_SupervisorThread.Run);
//        }
//
//        #endregion



namespace Abraham.Threading
{
    public class ThreadExtensions
    {
        #region ------------- Properties ----------------------------------------------------------

        public Thread thread { get; set; }

        public int Timeout_Seconds { get; set; }

        /// <summary>
        /// The Thread Procedure should check this flag often and stop working if false
        /// </summary>
        public volatile bool Run;

        public CancellationTokenSource CancellationToken_Source { get; private set; }
        #endregion



        #region ------------- Fields --------------------------------------------------------------
        #endregion



        #region ------------- Init ----------------------------------------------------------------

        public ThreadExtensions(ThreadStart threadProc, string name = "MyThread")
        {
            CancellationToken_Source = new CancellationTokenSource();
            thread = new Thread(threadProc);
            thread.Name = name;
            System.Diagnostics.Debug.WriteLine($"Started new Thread '{name} with ManagedThreadId={thread.ManagedThreadId}");
            Timeout_Seconds = 1;
            Run = true;
        }

        public ThreadExtensions(ParameterizedThreadStart threadProc, string name = "MyThread")
        {
            CancellationToken_Source = new CancellationTokenSource();
            thread = new Thread(threadProc);
            thread.Name = name;
            System.Diagnostics.Debug.WriteLine($"Started new Thread '{name} with ManagedThreadId={thread.ManagedThreadId}");
            Timeout_Seconds = 1;
            Run = true;
        }

        #endregion



        #region ------------- Methods -------------------------------------------------------------

        public void SendStopSignalAndWait()
        {
			System.Diagnostics.Debug.WriteLine($"SendStopSignalAndWait");
            Run = false;
            CancellationToken_Source.Cancel();

            int i = 0;
            while (thread.IsAlive && i < (10 * Timeout_Seconds))
            {
                Thread.Sleep(100);
                i++;
            }

            if (thread.IsAlive)
			{
				System.Diagnostics.Debug.WriteLine($"SendStopSignalAndWait thread didn't respond, aborting now");
                try
                {
                    thread.Abort();
                }
                catch (Exception)
                { 
                }
				System.Diagnostics.Debug.WriteLine($"SendStopSignalAndWait abort finished");
			}
			else
			{
				System.Diagnostics.Debug.WriteLine($"SendStopSignalAndWait thread has ended normally.");
			}
        }

        /// <summary>
        /// Safe method to wait a certain time, but stopping immediately when the thread stop is requested
        /// </summary>
        public void Sleep(int milliseconds)
        {
            CancellationToken_Source.Token.WaitHandle.WaitOne(milliseconds);
        }

        #endregion



        #region ------------- Implementation ------------------------------------------------------
        #endregion
    }
}
