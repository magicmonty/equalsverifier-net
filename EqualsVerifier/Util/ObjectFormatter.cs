using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Linq;

namespace EqualsVerifier.Util
{
    public class ObjectFormatter
    {
        readonly string message;
        readonly object[] objects;

        ///
        /// Factory method.
        ///
        /// <param name="message"> 
        /// The string that will be formatted.
        /// The substring %% represents the location where each
        /// object's will string representation will be inserted. 
        /// </param>
        /// <param name="objects">
        /// The objects whose string representation will be inserted into the message string.
        /// </param>
        /// <returns>A code Formatter</returns>
        public static ObjectFormatter Of(string message, params object[] objects)
        {
            return new ObjectFormatter(message, objects);
        }

        /// <summary>
        /// Private constructor. Call <see cref="Of"/> to instantiate.
        /// </summary>
        /// <param name="message">
        /// The string that will be formatted.
        /// The substring %% represents the location where each
        /// object's will string representation will be inserted. 
        /// </param>
        /// <param name="objects">
        /// The objects whose string representation will be inserted into the message string.
        /// </param>
        ObjectFormatter(string message, object[] objects)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            this.message = message;
            this.objects = objects;
        }

        /**
     * Formats the message with the given objects.
     * 
     * @return The message, with the given objects's string representations
     *          inserted into it.
     * @throws IllegalStateException if the number of %%'s in the message does
     *          not match the number of objects.
     */
        public String Format()
        {
            var result = message;
            var replacement = new Regex(Regex.Escape("%%"), RegexOptions.Compiled);
            foreach (var obj in objects)
            {
                var s = replacement.Replace(result, Stringify(obj), 1);

                if (result.Equals(s))
                    throw new InvalidOperationException("Too many parameters");

                result = s;
            }

            if (result.Contains("%%"))
                throw new InvalidOperationException("Not enough parameters");

            return result;
        }

        string Stringify(object obj)
        {
            if (obj == null)
                return "null";

            try
            {
                return obj.ToString();
            }
            catch (Exception e)
            {
                return new StringBuilder()
                    .Append(StringifyByReflection(obj))
                    .Append("-throws ")
                    .Append(e.GetType().Name)
                    .Append("(")
                    .Append(e is NotImplementedException && e.Message.Contains("DynamicProxy") ? "" : e.Message)
                    .Append(")")
                    .ToString();
            }
        }

        string StringifyByReflection(object obj)
        {
            var result = new StringBuilder();

            var type = obj.GetType();
            if (type.FullName.Contains("Castle") && type.FullName.Contains("Proxy"))
                type = type.BaseType;

            result.AppendFormat("[{0}", type.Name);

            FieldEnumerable
                .Of(type)
                .Select(field => new { field.Name, Value = field.GetValue(obj) })
                .ToList()
                .ForEach(f => result.AppendFormat(" {0}={1}", f.Name, Stringify(f.Value)));

            return result.Append("]").ToString();
        }
    }
}

