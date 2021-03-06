﻿using System;
using System.Collections.Generic;
using System.Text;
using BuDing.Extensions;
using BuDing.Ioc.Dapper.Repository.UnitOfWork.Containers;
using Dapper.FastCrud;

namespace BuDing.Ioc.Dapper.Repository.UnitOfWork.Helpers
{
    public sealed class SqlDialectHelper
    {
        private readonly object _locakSqlDialectUpdate = new object();
        private readonly SqlDialectContainer _container = SqlDialectContainer.Instance;

        public void SetDialogueIfNeeded<TEntity>(Ioc.UnitOfWork.SqlDialect sqlDialect) where TEntity : class
        {
            SetDialogueIfNeeded<TEntity>(EnumExtensions.ConvertEnumToEnum<SqlDialect>(sqlDialect));
        }

        public void SetDialogueIfNeeded<TEntity>(SqlDialect sqlDialect) where TEntity : class
        {
            if (_container.TryEntityIsFroozenOrDialogueIsCorrect<TEntity>())
            {
                return;
            }

            var mapping = OrmConfiguration.GetDefaultEntityMapping<TEntity>();
            if (!mapping.IsFrozen && mapping.Dialect != sqlDialect)
            {
                lock (_locakSqlDialectUpdate)
                {
                    //reload to be true
                    mapping = OrmConfiguration.GetDefaultEntityMapping<TEntity>();
                    if (mapping.IsFrozen || mapping.Dialect == sqlDialect) return;
                    mapping.SetDialect(sqlDialect);
                }
            }
            _container.AddEntityFroozenOrDialogueState<TEntity>(mapping.IsFrozen || mapping.Dialect == sqlDialect);
        }

        public bool? GetEntityState<TEntity>() where TEntity : class
        {
            return _container.GetState<TEntity>();
        }

        public void Reset()
        {
            _container.Clear(); 
        }
    }
}
