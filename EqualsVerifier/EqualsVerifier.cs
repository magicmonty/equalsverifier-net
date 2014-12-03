using System;
using EqualsVerifier.Util.Exceptions;
using EqualsVerifier.Util;
using System.Diagnostics.Contracts;
using System.Diagnostics;

namespace EqualsVerifier
{
    public class EqualsVerifier
    {
        readonly Type _type;
        readonly StaticFieldValueStash _stash;

        public static EqualsVerifier ForType(Type type)
        {
            return new EqualsVerifier(type);
        }

        EqualsVerifier(Type type)
        {
            _type = type;
            _stash = new StaticFieldValueStash();
        }

        public void Verify()
        {
            try {
                _stash.Backup(_type);
                PerformVerification();
            }
            catch (InternalException e) {
                HandleError(e, e.InnerException);
            }
            catch (Exception e) {
                HandleError(e, e);
            }
            finally {
                _stash.RestoreAll();
            }
        }

        static void HandleError(Exception messageContainer, Exception trueCause)
        {
            var showCauseExceptionInMessage = trueCause != null && trueCause.Equals(messageContainer);

            var message = ObjectFormatter.Of(
                              "%%%%",
                              showCauseExceptionInMessage 
                    ? trueCause.GetType().Name + ": " 
                    : string.Empty,                    
                              messageContainer.Message == null 
                    ? string.Empty 
                : messageContainer.Message);

            throw new AssertionException(message, trueCause);
        }

        void PerformVerification()
        {
            if (_type.IsEnum)
                return;
                
            VerifyWithoutExamples(_type);
            EnsureUnequalExamples(_type);
            VerifyWithExamples(_type);
        }

        void VerifyWithoutExamples(Type type)
        {
            throw new NotImplementedException();
        }

        void EnsureUnequalExamples(Type type)
        {
            throw new NotImplementedException();
        }

        void VerifyWithExamples(Type type)
        {
            throw new NotImplementedException();
        }
    }
}

