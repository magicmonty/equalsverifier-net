using System;
using System.Collections.Generic;
using System.Reflection;
using EqualsVerifier.Util;
using System.Linq;

namespace EqualsVerifier
{
    public class StaticFieldValueStash
    {
        readonly Dictionary<Type, Dictionary<FieldInfo, object>> _stash = new Dictionary<Type, Dictionary<FieldInfo, object>>();

        public void Backup(Type type)
        {
            if (_stash.ContainsKey(type))
                return;

            _stash.Add(type, 
                FieldEnumerable
                .Of(type)
                .Where(f => f.IsStatic)
                .Select(f => new { Field = f, Value = f.GetValue(null)})
                .ToDictionary(f => f.Field, f => f.Value));
        }

        public void RestoreAll()
        {
            foreach (var type in _stash.Keys)
            {
                foreach (var field in FieldEnumerable.Of(type).Where(f => f.IsStatic))
                    field.SetValue(null, _stash[type][field]);
            }
        }
    }
}

