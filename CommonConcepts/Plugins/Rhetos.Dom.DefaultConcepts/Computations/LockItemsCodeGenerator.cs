﻿/*
    Copyright (C) 2014 Omega software d.o.o.

    This file is part of Rhetos.

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as
    published by the Free Software Foundation, either version 3 of the
    License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Text;
using Rhetos.Compiler;
using Rhetos.Dsl;
using Rhetos.Dsl.DefaultConcepts;
using Rhetos.Extensibility;
using Rhetos.Utilities;

namespace Rhetos.Dom.DefaultConcepts
{
    [Export(typeof(IConceptCodeGenerator))]
    [ExportMetadata(MefProvider.Implements, typeof(LockItemsInfo))]
    public class LockItemsCodeGenerator : IConceptCodeGenerator
    {
        public static readonly CsTag<LockItemsInfo> UserMessageAppend = "UserMessage";

        public void GenerateCode(IConceptInfo conceptInfo, ICodeBuilder codeBuilder)
        {
            var info = (LockItemsInfo)conceptInfo;
            codeBuilder.InsertCode(CheckLockedItemsSnippet(info), WritableOrmDataStructureCodeGenerator.OldDataLoadedTag, info.Source);
            codeBuilder.AddReferencesFromDependency(typeof(UserException));
        }

        private static string CheckLockedItemsSnippet(LockItemsInfo info)
        {
            return string.Format(
@"            if (updated.Length > 0 || deleted.Length > 0)
            {{
                {0}[] changedItems = updated.Concat(deleted).ToArray();
                var lockedItems = _domRepository.{0}.Filter(changedItems.AsQueryable(), new {1}());
                if (lockedItems.Count() > 0)
                    throw new Rhetos.UserException({2}, ""DataStructure:{0},ID:"" + lockedItems.First().ID.ToString(){3});

                // Workaround to restore NH proxies if NHSession.Clear() is called inside filter.
                for (int i=0; i<updated.Length; i++) updated[i] = _executionContext.NHibernateSession.Load<{0}>(updated[i].ID);
                for (int i=0; i<deleted.Length; i++) deleted[i] = _executionContext.NHibernateSession.Load<{0}>(deleted[i].ID);
            }}
",
                info.Source.GetKeyProperties(),
                info.FilterType,
                CsUtility.QuotedString(info.Title),
                UserMessageAppend.Evaluate(info));
        }
    }
}
