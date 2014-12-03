using System;
using System.Collections.Generic;
using System.Reflection;

namespace EqualsVerifier
{
    public class StaticFieldValueStash
    {
        readonly Dictionary<Type, Dictionary<FieldInfo, object>> _stash = new Dictionary<Type, Dictionary<FieldInfo, object>>();

        public void Backup(Type type)
        {
            if (_stash.ContainsKey(type)) {
                return;
            }

            _stash.Add(type, new Dictionary<FieldInfo, object>());
            foreach (var field in type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)) {
                _stash[type].Add(field, field.GetValue(null));
            }
        }

        public void RestoreAll()
        {
            foreach (var type in _stash.Keys) {
                foreach (var field in type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)) {
                    field.SetValue(null, _stash[type][field]);
                }
            }
        }
    }
}

