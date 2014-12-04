using EqualsVerifier.Util;
using System.Collections.Generic;


namespace EqualsVerifier.Checker
{
    public class FieldsChecker<T> : IChecker
    {
        readonly ClassAccessor _classAccessor;
        readonly ISet<Warning> _warningsToSuppress;
        readonly bool _allFieldsShouldBeUsed;
        readonly ISet<string> _allFieldsShouldBeUsedExceptions;
        readonly PrefabValues _prefabValues;

        public FieldsChecker(
            ClassAccessor classAccessor, 
            ISet<Warning> warningsToSuppress, 
            bool allFieldsShouldBeUsed, 
            ISet<string> allFieldsShouldBeUsedExceptions)
        {
            _allFieldsShouldBeUsedExceptions = allFieldsShouldBeUsedExceptions;
            _allFieldsShouldBeUsed = allFieldsShouldBeUsed;
            _classAccessor = classAccessor;
            _warningsToSuppress = new HashSet<Warning>(warningsToSuppress);
            _prefabValues = classAccessor.PrefabValues;
        }

        public void Check()
        {
        }
    }
}

