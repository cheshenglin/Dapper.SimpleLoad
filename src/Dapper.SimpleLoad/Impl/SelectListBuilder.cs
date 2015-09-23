﻿using System.Text;
using Dapper.SimpleSave;

namespace Dapper.SimpleLoad.Impl
{
    public static class SelectListBuilder
    {
        public static string BuildSelectListFor(
            DtoMetadata metadata,
            string tableAlias = null)
        {
            var buffer = new StringBuilder();
            AppendSelectListAndGetFirstColumnFor(buffer, metadata, tableAlias);
            return buffer.ToString();
        }

        public static string AppendSelectListAndGetFirstColumnFor(
            StringBuilder buffer,
            DtoMetadata metadata,
            string tableAlias = null)
        {
            return AppendSelectListAndGetFirstColumnFor(buffer, metadata, false, tableAlias);
        }

        public static string AppendSelectListAndGetFirstColumnFor(
            StringBuilder buffer,
            DtoMetadata metadata,
            bool iNeedALeadingComma,
            string tableAlias = null)
        {
            string firstColumn = null;
            foreach (var property in metadata.AllProperties)
            {
                //  At the moment this isn't sophisticated enough to drill down through tables.
                //  We might want to add this in future but, given it's currently only used to retrieve
                //  data to populate lists and dropdowns it seems unnecessary.
                if (!property.Prop.CanWrite
                    || property.HasAttribute<OneToManyAttribute>()
                    || property.HasAttribute<ManyToManyAttribute>()
                    || property.HasAttribute<SimpleLoadIgnoreAttribute>()
                    || property.IsEnumerable)
                {
                    continue;
                }

                if (buffer.Length > 0 || iNeedALeadingComma)
                {
                    buffer.Append(", ");
                }

                if (!string.IsNullOrEmpty(tableAlias))
                {
                    buffer.Append('[');
                    buffer.Append(tableAlias);
                    buffer.Append("].");
                }

                buffer.Append('[');
                buffer.Append(property.ColumnName);
                buffer.Append(']');

                if (firstColumn == null)
                {
                    firstColumn = property.ColumnName;
                }
            }
            return firstColumn;
        }
    }
}
