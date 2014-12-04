using System;
using System.Collections.Generic;
using EqualsVerifier.Util.Exceptions;

namespace EqualsVerifier.Util
{
    public class AttributeAccessor
    {
        readonly IEnumerable<IAttribute> _supportedAttributes;
        readonly Type _type;
        readonly ISet<IAttribute> _classAttributes = new HashSet<IAttribute>();
        readonly Dictionary<string, ISet<IAttribute>> _fieldAttributes = new Dictionary<string, ISet<IAttribute>>();

        bool processed = false;
        bool shortCircuit = false;

        public AttributeAccessor(IEnumerable<IAttribute> supportedAttributes, Type type, bool ignoreFailure)
        {
            _supportedAttributes = supportedAttributes;
            _type = type;
        }

        public bool TypeHas(IAttribute attribute)
        {
            if (shortCircuit)
                return false;

            Process();
            return _classAttributes.Contains(attribute);
        }

        public bool FieldHas(string fieldName, IAttribute attribute)
        {
            if (shortCircuit)
                return false;

            Process();

            if (!_fieldAttributes.ContainsKey(fieldName))
                throw new ReflectionException("Class " + _type.Name + " does not have field " + fieldName);

            var attributes = _fieldAttributes[fieldName];
            return attributes.Contains(attribute);
        }

        void Process()
        {
            if (processed) {
                return;
            }

            Visit();
            processed = true;
        }

        void Visit()
        {
            VisitType(_type, false);
            var i = _type.BaseType;

            while (i != null && i != typeof(object)) {
                VisitType(i, true);
                i = i.BaseType;
            }
        }

        void VisitType(Type type, bool inheriting)
        {
            var attribs = type.GetCustomAttributes(inheriting);
            foreach (var attribute in attribs) {
                var attributeTypeName = CleanAttributeName(attribute.GetType().Name);
                Add(_classAttributes, attributeTypeName, inheriting);
            }

            foreach (var field in type.GetFields(FieldHelper.DeclaredOnly)) {
                if (field.FieldType.FullName.Contains("Castle"))
                    continue;
                if (_fieldAttributes.ContainsKey(field.Name))
                    continue;

                var fieldAttributes = new HashSet<IAttribute>();
                _fieldAttributes.Add(field.Name, fieldAttributes);

                foreach (var attribute in field.GetCustomAttributes(inheriting)) {
                    var attributeTypeName = CleanAttributeName(attribute.GetType().Name);
                    Add(fieldAttributes, attributeTypeName, inheriting);
                }
            }
        }

        static string CleanAttributeName(string attributeName)
        {
            var attributeTypeName = attributeName;
            if (attributeTypeName.EndsWith("Attribute", StringComparison.Ordinal))
                attributeTypeName = attributeTypeName.Substring(0, attributeTypeName.Length - "Attribute".Length);
            return attributeTypeName;
        }

        void Add(ISet<IAttribute> attributes, string annotationDescriptor, bool inheriting)
        {
            foreach (var attribute in _supportedAttributes) {
                if (!inheriting || attribute.Inherits) {
                    foreach (var descriptor in attribute.Descriptors) {
                        if (annotationDescriptor.Contains(descriptor))
                            attributes.Add(attribute);
                    }
                }
            }
        }
    }

}

