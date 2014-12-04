using EqualsVerifier.Util;
using System.Collections.Generic;
using System;

namespace EqualsVerifier.Checker
{
    public class HierarchyChecker<T> : IChecker
    {
        readonly ClassAccessor _classAccessor;
        readonly ISet<Warning> _warningsToSuppress;
        readonly bool _usingGetClass;
        readonly bool _hasRedefinedSubclass;
        readonly Type _redefinedSubclass;

        public HierarchyChecker(
            ClassAccessor classAccessor, 
            ISet<Warning> warningsToSuppress, 
            bool usingGetClass, 
            bool hasRedefinedSubclass, 
            Type redefinedSubclass)
        {
            _redefinedSubclass = redefinedSubclass;
            _hasRedefinedSubclass = hasRedefinedSubclass;
            _usingGetClass = usingGetClass;           
            _warningsToSuppress = warningsToSuppress;
            _classAccessor = classAccessor;
        }

        public void Check()
        {

        }
    }
}

