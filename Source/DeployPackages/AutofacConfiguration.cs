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

using Autofac;
using Rhetos.Configuration.Autofac;
using Rhetos.Deployment;
using Rhetos.Extensibility;
using Rhetos.Logging;
using Rhetos.Security;
using Rhetos.Utilities;

namespace DeployPackages
{
    public class AutofacConfiguration : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Specific registrations and initialization:
            PluginsUtility.SetLogProvider(new NLogProvider());
            builder.RegisterModule(new DatabaseGeneratorModuleConfiguration());
            builder.RegisterType<DataMigration>();
            builder.RegisterType<DatabaseCleaner>();

            // General registrations:
            builder.RegisterModule(new Rhetos.Configuration.Autofac.DefaultAutofacConfiguration(generate: true));

            // Specific registrations override:
            builder.RegisterType<ProcessUserInfo>().As<IUserInfo>();

            base.Load(builder);
        }
    }
}