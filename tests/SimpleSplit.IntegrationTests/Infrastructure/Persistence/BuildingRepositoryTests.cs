using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SimpleSplit.Domain.Base;
using SimpleSplit.Domain.Exceptions;
using SimpleSplit.Domain.Features.Buildings;
using SimpleSplit.Domain.Features.Expenses;
using SimpleSplit.Infrastructure.Persistence;
using SimpleSplit.Infrastructure.Persistence.Base;
using SimpleSplit.Infrastructure.Persistence.Repositories;
using Xunit;

namespace SimpleSplit.IntegrationTests.Infrastructure.Persistence
{
    public class BuildingRepositoryTests : IClassFixture<InfrastructureFixture>, IDisposable
    {
        private readonly SimpleSplitDbContext _dbContext;
        private readonly IEntityIDFactory     _idFactory;

        public BuildingRepositoryTests(InfrastructureFixture fixture)
        {
            _dbContext = fixture.DbContext;
            _idFactory = fixture.IDFactory;
            
            // Add sample data
            var building = Building.ForTests();
            _dbContext.Add(building);
            _dbContext.SaveChanges();
            _dbContext.ChangeTracker.Clear();
        }

        [Fact]
        public async Task ReadExisting()
        {
            var sut = new BuildingsRepository(_dbContext);
            var expected = Building.ForTests();
            var actual = await sut.GetByID(new BuildingID(expected.ID.Value));
            actual.Should().BeEquivalentTo(expected, options => options
                .Excluding(su => Regex.IsMatch(su.Path, "Apartments\\[.+\\].IsNew"))
                .Excluding(su => Regex.IsMatch(su.Path, "Apartments\\[.+\\].RowVersion"))
                .Excluding(su => su.IsNew)
                .Excluding(su => su.RowVersion)
            );
        }

        [Fact]
        public async Task UpdateBuilding()
        {
            var uow = new UnitOfWork(_dbContext);
            var sut = new BuildingsRepository(_dbContext);
            var expected = await sut.GetByID(new BuildingID(-1));
            expected.Address = new Address("Street", "123", "City", "56789");
            await uow.SaveAsync();
            _dbContext.ChangeTracker.Clear();

            var actual = await sut.GetByID(new BuildingID(-1));
            actual.Address.Should().Be(expected.Address);
        }

        [Fact]
        public async Task UpdateApartment()
        {
            var uow = new UnitOfWork(_dbContext);
            var sut = new BuildingsRepository(_dbContext);
            var expected = await sut.GetByID(new BuildingID(-1));
            var changed = expected.Apartments.FirstOrDefault(a => a.Code == "ΙΣ1")
                ?? throw new Exception("Could not get apartment 'ΙΣ1'");
            changed.SetDweller("Τριανταφύλλου Στέλιος");
            changed.SetOwner("Παπασπυρου Γιώργος");
            await uow.SaveAsync();
            _dbContext.ChangeTracker.Clear();

            var actual = await sut.GetByID(new BuildingID(-1));
            actual.Should().BeEquivalentTo(expected, options => options
                .Excluding(su => Regex.IsMatch(su.Path, "Apartments\\[.+\\].IsNew"))
                .Excluding(su => Regex.IsMatch(su.Path, "Apartments\\[.+\\].RowVersion"))
                .Excluding(su => su.IsNew)
                .Excluding(su => su.RowVersion)
            );
        }

        [Fact]
        public async Task AddApartment()
        {
            var uow = new UnitOfWork(_dbContext);
            var sut = new BuildingsRepository(_dbContext);
            var expected = await sut.GetByID(new BuildingID(-1));
            expected.AddApartment(new Apartment(new ApartmentID(12),
                "NEW",
                "Noone",
                "Someone",
                "RoofGarden",
                new Dictionary<Category.CategoryKind, double>
                {
                    { Category.CategoryKind.Elevator, 100 },
                    { Category.CategoryKind.Heating, 200 },
                    { Category.CategoryKind.Shared, 300 }
                }));
            await uow.SaveAsync();
            _dbContext.ChangeTracker.Clear();

            var actual = await sut.GetByID(new BuildingID(-1));
            actual.Should().BeEquivalentTo(expected, options => options
                .Excluding(su => Regex.IsMatch(su.Path, "Apartments\\[.+\\].IsNew"))
                .Excluding(su => Regex.IsMatch(su.Path, "Apartments\\[.+\\].RowVersion"))
                .Excluding(su => su.IsNew)
                .Excluding(su => su.RowVersion)
            );
        }

        [Fact]
        public async Task DeleteApartment()
        {
            var uow = new UnitOfWork(_dbContext);
            var sut = new BuildingsRepository(_dbContext);
            var expected = await sut.GetByID(new BuildingID(-1));
            expected.RemoveApartment(expected.Apartments.First(a => a.Code == "ΙΣ1"));
            
            await uow.SaveAsync();
            _dbContext.ChangeTracker.Clear();
            var actual = await sut.GetByID(new BuildingID(-1));

            actual.Apartments.Should().HaveCount(8);
            var deleted = actual.Apartments.SingleOrDefault(a => a.Code == "ΙΣ1");
            Assert.Null(deleted);
        }

        [Fact]
        public async void DeleteBuilding()
        {
            var uow = new UnitOfWork(_dbContext);
            var sut = new BuildingsRepository(_dbContext);
            var expected = await sut.GetByID(new BuildingID(-1));
            sut.Delete(expected);
            
            await uow.SaveAsync();
            _dbContext.ChangeTracker.Clear();

            var act = () => sut.GetByID(new BuildingID(-1));
            await act.Should().ThrowAsync<EntityNotFoundException>()
                .WithMessage("Could not find entity Building with BuildingID: -1");
        }
        
        public void Dispose()
        {
            // Cleanup...
            _dbContext.Database.ExecuteSqlRaw("DELETE FROM `building` WHERE `building`.`id` = -1");
            _dbContext.ChangeTracker.Clear();
        }
    }
}