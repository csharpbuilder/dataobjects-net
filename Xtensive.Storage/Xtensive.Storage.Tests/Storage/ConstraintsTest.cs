// Copyright (C) 2003-2010 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Dmitri Maximov
// Created:    2008.08.18

using System.Reflection;
using NUnit.Framework;
using Xtensive.Core.Testing;
using Xtensive.Storage.Configuration;
using Xtensive.Storage.Tests.Storage.BookAuthorModel;

namespace Xtensive.Storage.Tests.Storage
{
  public class ConstraintsTest : AutoBuildTest
  {
    protected override DomainConfiguration BuildConfiguration()
    {
      DomainConfiguration config = base.BuildConfiguration();
      config.Types.Register(Assembly.GetExecutingAssembly(), "Xtensive.Storage.Tests.Storage.BookAuthorModel");
      return config;
    }

    [Test]
    public void NotNullableViolationTest()
    {
      using (Session.Open(Domain)) {
        using (Transaction.Open()) {
          var book = new Book();

          // Text is nullable so it's OK
          book.Text = null;

          // Title has length constraint (10 symbols) so InvalidOperationException is expected
          AssertEx.ThrowsInvalidOperationException(() => book.Title = "01234567890");
        }
      }
    }

    [Test]
    public void SessionBoundaryViolationTest()
    {
      using (Session.Open(Domain)) {
        using (Transaction.Open()) {
          Author author = new Author();
          using (Session.Open(Domain)) {
            using (Transaction.Open()) {
              Book book = new Book();

              // Author is bound to another session
              AssertEx.ThrowsInvalidOperationException(() => book.Author = author);
            }
          }
        }
      }
    }
  }
}