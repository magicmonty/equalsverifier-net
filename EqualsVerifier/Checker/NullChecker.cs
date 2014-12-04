using EqualsVerifier.Util;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace EqualsVerifier.Checker
{
    public class NullChecker<T> : IChecker where T: class
    {
        readonly ClassAccessor _classAccessor;
        readonly ISet<Warning> _warningsToSuppress;

        public NullChecker(ClassAccessor classAccessor, ISet<Warning> warningsToSuppress)
        {
            _warningsToSuppress = new HashSet<Warning>(warningsToSuppress);
            _classAccessor = classAccessor;
        }

        public void Check()
        {
            if (_warningsToSuppress.Contains(Warning.NULL_FIELDS))
                return;

            var inspector = new FieldInspector<T>(_classAccessor);
            inspector.Check(new NullPointerExceptionFieldCheck(_classAccessor));
        }

        class NullPointerExceptionFieldCheck : IFieldCheck
        {
            readonly ClassAccessor _classAccessor;

            public NullPointerExceptionFieldCheck(ClassAccessor classAccessor)
            {
                _classAccessor = classAccessor;
            }

            public void Execute(FieldAccessor referenceAccessor, FieldAccessor changedAccessor)
            {
                var field = referenceAccessor.Field;
                if (field.FieldType.IsPrimitive)
                    return;

                if (_classAccessor.FieldHasAttribute(field, SupportedAttributes.NONNULL))
                    return;

                var reference = referenceAccessor.Object;
                var changed = changedAccessor.Object;

                changedAccessor.DefaultField();

                Handle("Equals", field, () => reference.Equals(changed));
                Handle("Equals", field, () => changed.Equals(reference));
                Handle("GetHashCode", field, () => changed.GetHashCode());

                referenceAccessor.DefaultField();
            }

            static void Handle(string testedMethodName, MemberInfo field, Action r)
            {
                try {
                    r();
                }
                catch (NullReferenceException e) {
                    NullReferenceExceptionThrown(testedMethodName, field, e);
                }
                catch (Exception e) {
                    ExceptionThrown(testedMethodName, field, e);
                }
            }

            static void NullReferenceExceptionThrown(string method, MemberInfo field, NullReferenceException e)
            {
                TestFrameworkBridge.Fail(ObjectFormatter.Of("Non-nullity: %% throws NullReferenceException on field %%.", method, field.Name), e);
            }

            static void ExceptionThrown(string method, MemberInfo field, Exception e)
            {
                TestFrameworkBridge.Fail(ObjectFormatter.Of("%% throws %% when field %% is null.", method, e.GetType().Name, field.Name));
            }
        }
    }
}

