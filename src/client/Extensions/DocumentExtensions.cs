using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Olav.Sanity.Client.Mutators;


namespace Olav.Sanity.Client.Extensions
{
    public static class DocumentExtensions
    {
        /// <summary>
        /// Determines if object is a Sanity draft document by inspecting the Id field.
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static bool IsDraftDocument(this object document)
        {
            if (document == null) return false;
            var id = document.GetId();
            if (id == null) return false;
            return id.StartsWith("drafts.");
        }

        /// <summary>
        /// Returns ID of a document based on conventions: Id, ID, _id, {className}Id etc.
        /// </summary>
        /// <param name="document">Object which is expected to represent a single Sanity document.</param>
        /// <returns></returns>
        public static string GetId(this object document)
        {
            if (document == null) return null;

            // Return Id for documents implementing ISanityDoc
            if (document is ISanityDoc)
            {
                return ((ISanityDoc)document).Id;
            }

            // Return Id for documents implementing IHaveId
            if (document is IHaveId)
            {
                return ((IHaveId)document).Id;
            }

            // Return Id using reflection (based on conventions)
            var idProperty = document.GetType().GetIdProperty();
            if (idProperty != null)
            {
                return idProperty.GetValue(document)?.ToString();
            }

            // ID not found
            return null;
        }

        private static ConcurrentDictionary<Type,PropertyInfo> _idPropertyCache = new ConcurrentDictionary<Type,PropertyInfo>();
        private static PropertyInfo GetIdProperty(this Type type)
        {
            if (!_idPropertyCache.ContainsKey(type))
            {
                // Find Id property by convention (i.e. "Id", "ID", "_id", "{documentTypeName}Id" etc.)
                var props = type.GetProperties();
                var idProperty = props.FirstOrDefault(p => p.Name.Equals("id", StringComparison.InvariantCultureIgnoreCase) ||
                                                           p.Name.Equals("_id", StringComparison.InvariantCultureIgnoreCase) ||
                                                           p.Name.Equals($"{type.Name}id", StringComparison.InvariantCultureIgnoreCase)
                    );
                _idPropertyCache[type] = idProperty;
            }
            return _idPropertyCache[type];
        }

    }
}