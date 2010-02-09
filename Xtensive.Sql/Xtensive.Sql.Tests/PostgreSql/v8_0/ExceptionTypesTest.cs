// Copyright (C) 2010 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Denis Krjuchkov
// Created:    2010.02.08

using NUnit.Framework;

namespace Xtensive.Sql.Tests.PostgreSql.v8_0
{
  [TestFixture, Explicit]
  public class ExceptionTypesTest : Tests.ExceptionTypesTest
  {
    protected override string Url { get { return TestUrl.PostgreSql80LowTimeout; } }
  }
}