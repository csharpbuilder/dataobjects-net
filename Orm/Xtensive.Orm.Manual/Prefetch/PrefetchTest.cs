// Copyright (C) 2003-2010 Xtensive LLC.
// All rights reserved.
// For conditions of distribution and use, see license.
// Created by: Alex Yakunin
// Created:    2009.12.24

using System;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using Xtensive.Core;
using Xtensive.Orm.Internals.Prefetch;
using Xtensive.Orm.Services;
using Xtensive.Orm.Tests;

namespace Xtensive.Orm.Manual.Prefetch
{
  #region Model

  [Serializable]
  [HierarchyRoot]
  public class Person : Entity
  {
    [Key, Field]
    public int Id { get; private set; }

    [Field(Length = 200)]
    public string Name { get; set; }

    [Field]
    public DateTime BirthDay { get; set; }

    [Field(LazyLoad = true, Length = 65536)]
    public byte[] Photo { get; set; }

    [Field]
    public Person Manager { get; set; }

    [Field]
    [Association(PairTo = "Manager")]
    public EntitySet<Person> Employees { get; private set; }

    public Key ManagerKey { 
      get { return GetReferenceKey(TypeInfo.Fields["Manager"]); } 
    }

    public Person(Session session)
      : base(session)
    {}
  }

  #endregion

  [TestFixture]
  public class PrefetchTest
  {
    [Test]
    public void MainTest()
    {
      var config = DomainConfigurationFactory.CreateWithoutSessionConfigurations();
      config.UpgradeMode = DomainUpgradeMode.Recreate;
      config.Types.Register(typeof(Person).Assembly, typeof(Person).Namespace);
      var domain = Domain.Build(config);

      using (var session = domain.OpenSession())
      using (var transactionScope = session.OpenTransaction()) {

        var employee = new Person(session) {Name = "Employee", Photo = new byte[] {8, 0}};
        var manager  = new Person(session) {Name = "Manager",  Photo = new byte[] {8, 0}};
        manager.Employees.Add(employee);
        transactionScope.Complete();
      }
  
      using (var session = domain.OpenSession())
      using (var transactionScope = session.OpenTransaction()) {
        var persons = session.Query.All<Person>()
          .Prefetch(p => p.Photo) // Lazy load field
          .Prefetch(p => p.Employees // EntitySet Employees
            .Prefetch(e => e.Photo)) // and lazy load field of each of its items
          .Prefetch(p => p.Manager); // Referenced entity
        foreach (var person in persons) {
          // some code here...
        }
        transactionScope.Complete();
      }

      using (var session = domain.OpenSession())
      using (var transactionScope = session.OpenTransaction()) {
        var personIds = session.Query.All<Person>().Select(p => p.Id);
        var prefetchedPersons = session.Query.Many<Person, int>(personIds)
          .Prefetch(p => new { p.Photo, p.Manager }) // Lazy load field and Referenced entity
          .Prefetch(p => p.Employees // EntitySet Employees
            .Prefetch(e => new { e.Photo, e.Manager })); // and lazy load field and referenced entity of each of its items
        foreach (var person in prefetchedPersons) {
          // some code here...
        }
        transactionScope.Complete();
      }

      using (var session = domain.OpenSession())
      using (var transactionScope = session.OpenTransaction()) {
        var persons = session.Query.All<Person>()
          .Prefetch(p => p.Photo) // Lazy load field
          .Prefetch(p => p.Employees.Prefetch(e => e.Photo)) // EntitySet Employees and lazy load field of each of its items with the limit on number of items to be loaded
          .Prefetch(p => p.Manager.Photo); // Referenced entity and lazy load field for each of them
        foreach (var person in persons) {
          Assert.IsTrue(DirectStateAccessor.Get(person).GetFieldState("Photo")==PersistentFieldState.Loaded);
          Assert.IsTrue(DirectStateAccessor.Get(person).GetFieldState("Manager")==PersistentFieldState.Loaded);
          if (person.ManagerKey != null) {
            Assert.IsNotNull(DirectStateAccessor.Get(session)[person.ManagerKey]);
            Assert.IsTrue(DirectStateAccessor.Get(person.Manager).GetFieldState("Photo")==PersistentFieldState.Loaded);
          }
          // some code here...
        }
        transactionScope.Complete();
      }
    }

    [Test]
    public async Task MainAsyncTest()
    {
      var config = DomainConfigurationFactory.CreateWithoutSessionConfigurations();
      config.UpgradeMode = DomainUpgradeMode.Recreate;
      config.Types.Register(typeof(Person).Assembly, typeof(Person).Namespace);
      var domain = Domain.Build(config);

      using (var session = domain.OpenSession())
      using (var transactionScope = session.OpenTransaction()) {
        var employee = new Person(session) {Name = "Employee", Photo = new byte[] {8, 0}};
        var manager = new Person(session) {Name = "Manager", Photo = new byte[] {8, 0}};
        manager.Employees.Add(employee);
        transactionScope.Complete();
      }

      using (var session = domain.OpenSession())
      using (var transactionScope = session.OpenTransaction()) {
        var persons = session.Query.All<Person>()
          .Prefetch(p => p.Photo) // Lazy load field
          .Prefetch(p => p.Employees // EntitySet Employees
            .Prefetch(e => e.Photo)) // and lazy load field of each of its items
          .Prefetch(p => p.Manager) // Referenced entity
          .AsAsync();
        foreach (var person in await persons) {
          // some code here...
        }
        transactionScope.Complete();
      }

      using (var session = domain.OpenSession())
      using (var transactionScope = session.OpenTransaction()) {
        var personIds = session.Query.All<Person>().Select(p => p.Id);
        var prefetchedPersons = session.Query.Many<Person, int>(personIds)
          .Prefetch(p => new {p.Photo, p.Manager}) // Lazy load field and Referenced entity
          .Prefetch(p => p.Employees // EntitySet Employees
            .Prefetch(e => new {e.Photo, e.Manager})) // and lazy load field and referenced entity of each of its items
          .AsAsync();
        foreach (var person in await prefetchedPersons) {
          // some code here...
        }
        transactionScope.Complete();
      }

      using (var session = domain.OpenSession())
      using (var transactionScope = session.OpenTransaction()) {
        var persons = session.Query.All<Person>()
          .Prefetch(p => p.Photo) // Lazy load field
          .Prefetch(p => p.Employees.Prefetch(e => e.Photo)) // EntitySet Employees and lazy load field of each of its items with the limit on number of items to be loaded
          .Prefetch(p => p.Manager.Photo) // Referenced entity and lazy load field for each of them
          .AsAsync();
        foreach (var person in await persons) {
          Assert.IsTrue(DirectStateAccessor.Get(person).GetFieldState("Photo")==PersistentFieldState.Loaded);
          Assert.IsTrue(DirectStateAccessor.Get(person).GetFieldState("Manager")==PersistentFieldState.Loaded);
          if (person.ManagerKey!=null) {
            Assert.IsNotNull(DirectStateAccessor.Get(session)[person.ManagerKey]);
            Assert.IsTrue(DirectStateAccessor.Get(person.Manager).GetFieldState("Photo")==PersistentFieldState.Loaded);
          }
          // some code here...
        }
        transactionScope.Complete();
      }
    }

    [Test]
    public void MultipleBatchesTest()
    {
      var config = DomainConfigurationFactory.CreateWithoutSessionConfigurations();
      config.UpgradeMode = DomainUpgradeMode.Recreate;
      config.Types.Register(typeof (Person).Assembly, typeof (Person).Namespace);
      var domain = Domain.Build(config);

      int count = 1000;

      using (var session = domain.OpenSession())
      using (var transactionScope = session.OpenTransaction()) {
        var random = new Random(10);
        for (int i = 0; i < count; i++)
          new Person(session) {Name = i.ToString(), Photo = new[] {(byte) (i % 256)}};
        var persons = session.Query.All<Person>().OrderBy(p => p.Id).ToArray();
        for (int i = 0; i<count; i++) {
          var person = persons[i];
          if (random.Next(5)>0)
            person.Manager = persons[random.Next(count)];
        }
        transactionScope.Complete();
      }

      using (var session = domain.OpenSession())
      using (var transactionScope = session.OpenTransaction()) {
        var prefetchedPersons = (
          from person in session.Query.All<Person>()
          orderby person.Name
          select person)
          .Take(100)
          .Prefetch(p => p.Photo) // Lazy load field
          .Prefetch(p => p.Employees // EntitySet Employees
            .Prefetch(e => e.Photo)) // and lazy load field of each of its items
          .Prefetch(p => p.Manager); // Referenced entity
        foreach (var person in prefetchedPersons) {
          Assert.IsTrue(DirectStateAccessor.Get(person).GetFieldState("Photo")==PersistentFieldState.Loaded);
          Assert.IsTrue(DirectStateAccessor.Get(person).GetFieldState("Manager")==PersistentFieldState.Loaded);
          Assert.IsTrue(DirectStateAccessor.Get(person.Employees).IsFullyLoaded);
          foreach (var employee in person.Employees)
            Assert.IsTrue(DirectStateAccessor.Get(employee).GetFieldState("Photo")==PersistentFieldState.Loaded);
        }
        transactionScope.Complete();
      }
    }

    [Test]
    public async Task MultipleBatchesAsyncTest()
    {
      var config = DomainConfigurationFactory.CreateWithoutSessionConfigurations();
      config.UpgradeMode = DomainUpgradeMode.Recreate;
      config.Types.Register(typeof(Person).Assembly, typeof(Person).Namespace);
      var domain = Domain.Build(config);

      int count = 1000;

      using (var session = domain.OpenSession())
      using (var transactionScope = session.OpenTransaction()){
        var random = new Random(10);
        for (int i = 0; i < count; i++)
          new Person(session) { Name = i.ToString(), Photo = new[] { (byte)(i % 256) } };
        var persons = session.Query.All<Person>().OrderBy(p => p.Id).ToArray();
        for (int i = 0; i < count; i++) {
          var person = persons[i];
          if (random.Next(5) > 0)
            person.Manager = persons[random.Next(count)];
        }
        transactionScope.Complete();
      }

      using (var session = domain.OpenSession())
      using (var transactionScope = session.OpenTransaction()) {
        var prefetchedPersons = (
          from person in session.Query.All<Person>()
          orderby person.Name
          select person)
          .Take(100)
          .Prefetch(p => p.Photo) // Lazy load field
          .Prefetch(p => p.Employees // EntitySet Employees
            .Prefetch(e => e.Photo)) // and lazy load field of each of its items
          .Prefetch(p => p.Manager) // Referenced entity
          .AsAsync(); 
        foreach (var person in await prefetchedPersons) {
          Assert.IsTrue(DirectStateAccessor.Get(person).GetFieldState("Photo")==PersistentFieldState.Loaded);
          Assert.IsTrue(DirectStateAccessor.Get(person).GetFieldState("Manager")==PersistentFieldState.Loaded);
          Assert.IsTrue(DirectStateAccessor.Get(person.Employees).IsFullyLoaded);
          foreach (var employee in person.Employees)
            Assert.IsTrue(DirectStateAccessor.Get(employee).GetFieldState("Photo")==PersistentFieldState.Loaded);
        }
        transactionScope.Complete();
      }
    }
  }
}