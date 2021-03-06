﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace ThreadAccessToFileViaMutex01
{
    class Program
    {
        private const string FILEPATH = @"D:\David\test\test.txt";

        static void Main(string[] args)
        {
            StartThread01();
            StartThread02();
            WaitThread01End();
            WaitThread02End();
        }

        private static void StartThread01()
        {
            m_thread_01 = new Thread(new ThreadStart(ThreadMethod));
            m_thread_01.Start();
        }        

        private static void StartThread02()
        {
            m_thread_02 = new Thread(new ThreadStart(ThreadMethod));
            m_thread_02.Start();
        }

        // This method represents the entry-point for both
        // threads.
        private static void ThreadMethod()
        {
            try
            {
                // Each thread will perform the following
                // (in a loop iterating 10 times):
                //
                // 1. Acquire ownership of the mutex.
                // This allows the thread to access the
                // file.
                //
                // If the mutex is currently owned by another
                // thread, the current thread will wait until
                // ownership is available (e.g. when the other
                // thread releases the mutex).
                //
                // 2. Write a line to the file. This line shows
                // the id of the thread currently owning the 
                // mutex.
                //
                // 3. Release the mutex. This allows other threads
                // to gain ownership of the mutex.
                //
                // As each thread runs, it will continually acquire
                // and release ownership of the mutex. As a result,
                // access to the file will alternate between the
                // two threads.
                for (int i = 0; i < 10; i++)
                {
                    AcquireFileMutex();
                    File.AppendAllText(FILEPATH, "Thread "
                        + Convert.ToString(Thread.CurrentThread.ManagedThreadId)
                        + " has the Mutex.\r\n");
                    ReleaseFileMutex();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occurred on Thread [{0:D}] : " + ex.Message,
                    Convert.ToString(Thread.CurrentThread.ManagedThreadId));
            }
            finally
            {

            }
        }

        // Allows the calling thread to gain 
        // ownership of the mutex.
        private static void AcquireFileMutex()
        {
            // Note that WaitOne() will block the
            // current thread until the mutex
            // is available for ownership.
            //
            // For more information, please refer
            // to the following:
            // https://msdn.microsoft.com/en-us/library/system.threading.mutex.waitone(v=vs.110).aspx
            m_file_access_mutex.WaitOne();
        }

        // Allows the calling thread to release 
        // the mutex. The calling thread must
        // currently own the mutex.
        private static void ReleaseFileMutex()
        {
            m_file_access_mutex.ReleaseMutex();
        }

        // In this method, the calling thread
        // will simply wait for Thread 01 to
        // finish running.
        private static void WaitThread01End()
        {
            m_thread_01.Join();
        }

        // In this method, the calling thread
        // will simply wait for Thread 02 to
        // finish running.
        private static void WaitThread02End()
        {
            m_thread_02.Join();
        }

        private static Thread m_thread_01 = null;
        private static Thread m_thread_02 = null;
        private static Mutex m_file_access_mutex = new Mutex();
    }
}
