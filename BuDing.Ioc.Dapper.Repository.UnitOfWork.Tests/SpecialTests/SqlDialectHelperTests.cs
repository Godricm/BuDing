﻿using BuDing.Ioc.Dapper.Repository.UnitOfWork.Helpers;
using BuDing.Ioc.Dapper.Repository.UnitOfWork.Tests.ExampleTests.Repository;
using BuDing.Ioc.Dapper.Repository.UnitOfWork.Tests.TestHelpers;
using BuDing.Ioc.Dapper.Repository.UnitOfWork.Tests.TestHelpers.Migrations;
using BuDing.Ioc.UnitOfWork.Interfaces;
using Dapper.FastCrud;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace BuDing.Ioc.Dapper.Repository.UnitOfWork.Tests.SpecialTests
{
    [TestFixture]
    public class SqlDialectHelperTests:CommonTestDataSetup
    {
        [Test]
        public static void SetDialogueIfNeeded_AddsMappedIsFroozenToDictionary()
        {
            var target = new SqlDialectHelper();
            target.SetDialogueIfNeeded<Brave>(SqlDialect.SqLite);
            var result = target.GetEntityState<Brave>();
            Assert.That(result.HasValue, Is.True);
            Assert.That(OrmConfiguration.GetDefaultEntityMapping<Brave>().Dialect, Is.EqualTo(SqlDialect.SqLite));
        }

        [Test,Category("Integration")]
        public static void SetDialogueIfNeeded_SetsIsFroozenInDictionary()
        {
            var repo = new BraveRepository(Factory);
            repo.GetByKey<ITestSession>(1);
            var target = new SqlDialectHelper();
            var result = target.GetEntityState<Brave>();
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.True);
            Assert.That(OrmConfiguration.GetDefaultEntityMapping<Brave>().Dialect, Is.EqualTo(SqlDialect.SqLite));
        }

        [Test, Category("Integration")]
        public static void Reset_ClearDictionary()
        {
            var repo = new BraveRepository(Factory);
            repo.GetByKey<ITestSession>(1);
            var target = new SqlDialectHelper();
            target.Reset();
            var result = target.GetEntityState<Brave>();
            Assert.That(result.HasValue, Is.True);
        }

        [Test, Category("Integration")]
        public static void Query_Wont_MakeFrozen()
        {
            var connection = new TestSessionMemory(A.Fake<IDbFactory>());
            new MigrateDb(connection);
            var target = new SqlDialectHelper();
            target.Reset();
            OrmConfiguration.RegisterEntity<Brave>();
            connection.Query("SELECT * FROM Braves");
            var result = target.GetEntityState<Brave>();
            Assert.That(OrmConfiguration.GetDefaultEntityMapping<Brave>().IsFrozen, Is.False);
            Assert.That(result.HasValue, Is.False);
        }

        [Test, Category("Integration")]
        public static void Table_Will_SetDialect()
        {
            var connection = CreateSession(null);
            var sql = SqlInstance.Instance;
            connection.Query($"SELECT * FROM {sql.Table<Brave>(connection.SqlDialect)}");
            var target = new SqlDialectHelper();
            var result = target.GetEntityState<Brave>();
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.True);
            Assert.That((int)OrmConfiguration.GetDefaultEntityMapping<Brave>().Dialect, Is.EqualTo((int)connection.SqlDialect));

        }
    }
}
