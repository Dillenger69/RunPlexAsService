
namespace RunPlexAsService
{
    using System;
    using System.Runtime.Serialization;

    public class RunPlexAsServiceException : Exception, ISerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RunPlexAsServiceException"/> class.
        /// </summary>
        public RunPlexAsServiceException() : base()
        {
            // Add implementation.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RunPlexAsServiceException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public RunPlexAsServiceException(string message) : base(message)
        {
            // Add implementation.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RunPlexAsServiceException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The inner exception.</param>
        public RunPlexAsServiceException(string message, Exception inner) : base(message, inner)
        {
            // Add implementation.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RunPlexAsServiceException"/> class.
        /// This constructor is needed for serialization.        
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected RunPlexAsServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            // Add implementation.
        }
    }
}
