using System;
using System.Threading;

namespace Azure.WebRole.CapacityTesting.HttpHandlers
{
    /// <summary>
    /// Class that represents an asynchronous result that contains a simple value.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="Result"/>.</typeparam>
    public class SimpleAsyncResult<T> : IAsyncResult
    {
        /// <summary>
        /// Backing field for the <see cref="AsyncState"/> property.
        /// </summary>
        private readonly object _asyncState;

        /// <summary>
        /// Backing field for the <see cref="Exception"/> property.
        /// </summary>
        private Exception _exception;

        /// <summary>
        /// Backing field of the <see cref="Result"/> property.
        /// </summary>
        private T _result;

        /// <summary>
        /// Constructs the <see cref="SimpleAsyncResult{T}"/> class with the <paramref name="asyncState"/> state.
        /// </summary>
        /// <param name="asyncState">A user-defined object that qualifies or contains information about an asynchronous operation.</param>
        public SimpleAsyncResult(object asyncState)
        {
            _asyncState = asyncState;
        }

        /// <summary>
        /// The result of the async operation.
        /// </summary>
        public T Result
        {
            get { return _result; }
            set
            {
                _result = value;

                // When the result is set, set the completed flag.
                IsCompleted = true;
            }
        }

        /// <summary>
        /// The <see cref="T:System.Exception"/> that occured.
        /// </summary>
        public Exception Exception
        {
            get { return _exception; }
            set
            {
                _exception = value;

                // When the exception is set, set the completed flag.
                IsCompleted = true;
            }
        }

        #region Implementation of IAsyncResult

        /// <summary>
        /// Gets a value that indicates whether the asynchronous operation has completed.
        /// </summary>
        /// <returns>
        /// true if the operation is complete; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool IsCompleted { get; protected set; }

        /// <summary>
        /// Gets a <see cref="WaitHandle"/> that is used to wait for an asynchronous operation to complete.
        /// </summary>
        /// <returns>
        /// A <see cref="WaitHandle"/> that is used to wait for an asynchronous operation to complete.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public WaitHandle AsyncWaitHandle { get { return null; } }

        /// <summary>
        /// Gets a user-defined object that qualifies or contains information about an asynchronous operation.
        /// </summary>
        /// <returns>
        /// A user-defined object that qualifies or contains information about an asynchronous operation.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public object AsyncState { get { return _asyncState; } }

        /// <summary>
        /// Gets a value that indicates whether the asynchronous operation completed synchronously.
        /// </summary>
        /// <returns>
        /// true if the asynchronous operation completed synchronously; otherwise, false.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public bool CompletedSynchronously { get; set; }

        #endregion Implementation of IAsyncResult
    }
}
