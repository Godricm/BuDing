﻿using BuDing.Core.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BuDing.Ioc.Dapper.Repository.UnitOfWork.Tests.TestHelpers
{
    class Brave:Entity<int>
    {
        [ForeignKey("New")]
        public int NewId { get; set; }
        public New New { get; set; }
    }
}