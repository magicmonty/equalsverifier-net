using EqualsVerifier.Util;

namespace EqualsVerifier.Checker
{
    public interface IFieldCheck
    {
        void Execute(FieldAccessor referenceAccessor, FieldAccessor changedAccessor);
    }

    public class FieldInspector<T>
    {
        readonly ClassAccessor _classAccessor;

        public FieldInspector(ClassAccessor classAccessor)
        {
            _classAccessor = classAccessor;
        }

        public void Check(IFieldCheck check)
        {
            foreach (var field in _classAccessor.Type.GetFields(FieldHelper.AllFields)) {
                var reference = _classAccessor.GetRedAccessor();
                var changed = _classAccessor.GetRedAccessor();

                check.Execute(
                    reference.FieldAccessorFor(field), 
                    changed.FieldAccessorFor(field));
            }
        }
    }
}

