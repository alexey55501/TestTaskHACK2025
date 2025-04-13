using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Lapka.API.DAL.Migrations;
using Lapka.API.DAL.Models;
using Lapka.BLL.Helpers;
using Lapka.DAL.DbContext;
using Lapka.SharedModels.Base;
using Lapka.SharedModels.DTO;
using Lapka.SharedModels.DTO.Filters;
using Lapka.SharedModels.DTO.Filters.Base;
using Lapka.SharedModels.DTO.Pagination;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lapka.BLL.Services.Animals
{
    public class AnimalsService : BaseService
    {
        private readonly LapkaDbContext _db;

        public AnimalsService(LapkaDbContext db)
        {
            _db = db;
        }

        #region CRUD

        // Create
        public int CreateAnimal(AnimalBase dto, int shelterId)
        {
            var animal = new Animal();
            try
            {
                animal = Mapper.Map<Animal>(dto);
                animal.AdditionDate = DateTime.Now;
                animal.ShelterId = shelterId;
                _db.Animals.Add(animal);

                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                return -1;
            }
            return animal.Id;
        }

        public bool IsAnimalExists(int id)
        {
            return _db.Animals.Any(t => t.Id == id);
        }

        // Get animal
        public Animal GetAnimal(int id)
        {
            return _db.Animals.Where(t => t.Id == id).FirstOrDefault();
        }
        public ShelterBase GetShelterInfo(int shelterId)
        {
            return Mapper.Map<ShelterBase>(_db.Shelters.Where(t => t.Id == shelterId).FirstOrDefault());
        }

        // Get animals
        public PaginatedResponse<AnimalDTO> GetPaginatedAnimals(AnimalsFilterDTO filter, string requesterId)
        {
            var items = _db.Animals.AsNoTracking();

            items = items.Where(t =>
                (string.IsNullOrEmpty(filter.SearchQuery) || t.Name.Contains(filter.SearchQuery))).AsQueryable();

            if (filter.FavoritesOnly)
            {
                var favs = _db.UserFavoritesAnimals.Where(t => t.UserId == requesterId).Select(t => t.Animal).AsQueryable();
                items = items.Where(t => favs.Contains(t)).AsQueryable();
            }
            if (filter.MinAge != 0)
                items = items.Where(t => filter.MinAge <= t.Age).AsQueryable();
            if (filter.MaxAge != 0)
                items = items.Where(t => filter.MaxAge <= t.Age).AsQueryable();
            if (filter.HealthStatus != HealthStatus.None)
                items = items.Where(t => filter.HealthStatus == t.HealthStatus).AsQueryable();
            if (!string.IsNullOrEmpty(filter.Location))
                items = items.Where(t => filter.Location.Contains(t.Location)).AsQueryable();
            if (filter.Type != AnimalType.None)
                items = items.Where(t => filter.Type == t.Type).AsQueryable();
            if(filter.HasPassport != null)
                items = items.Where(t => filter.HasPassport == t.HasPassport).AsQueryable();
            if (filter.Sex != null)
                items = items.Where(t => filter.Sex == t.Sex).AsQueryable();
            if(filter.IsSterilized != null)
                items = items.Where(t => filter.IsSterilized == t.IsSterilized).AsQueryable();

            var count = items.Count();

            switch (filter.OrderByField.ToLower())
            {
                case "name":
                    if (filter.OrderBy == Order.ASC)
                        items = items.OrderBy(p => p.Name);
                    else
                        items = items.OrderByDescending(p => p.Name);
                    break;
                case "age":
                    if (filter.OrderBy == Order.ASC)
                        items = items.OrderBy(p => p.Age);
                    else
                        items = items.OrderByDescending(p => p.Age);
                    break;
                case "additiondate":
                default:
                    if (filter.OrderBy == Order.ASC)
                        items = items.OrderBy(p => p.AdditionDate);
                    else
                        items = items.OrderByDescending(p => p.AdditionDate);
                    break;
                    break;
            }


            return new PaginatedResponse<AnimalDTO>()
            {
                Items = Mapper.Map<List<AnimalDTO>>(items.Skip(filter.Skip)
                             .Take(filter.Take)
                             .ToList()),
                CurrentPage = filter?.Page.Value ?? 0,
                PagesCount = PaginationHelper.PagesCount(filter?.AmountOnPage ?? 1, count),
            };
        }

        // Update Animal 
        public bool UpdateAnimal(AnimalDTO payload)
        {

            var animal = _db.Animals.Where(t => t.Id == payload.Id).FirstOrDefault();
            if (animal != null)
            {
                animal = Mapper.Map<Animal>(payload);
                _db.SaveChanges();
                return true;
            }
            return false;
        }

        // Delete Animal
        public bool DeleteAnimal(int id)
        {
            var animal = _db.Animals.Where(t => t.Id == id).FirstOrDefault();
            if (animal != null)
            {
                animal.IsDeleted = true;
                animal.UserFavorites.Clear();
                _db.SaveChanges();
                return true;
            }
            return false;
        }
        #endregion

        public string AddAnimalToFavorites(int animalId, string userId)
        {
            if (!_db.UserFavoritesAnimals.Any(e => e.AnimalId == animalId && e.UserId == userId))
            {
                try
                {
                    _db.UserFavoritesAnimals.Add(new UserFavorites
                    {
                        UserId = userId,
                        AnimalId = animalId
                    });

                    _db.SaveChanges();

                    return string.Empty;
                }
                catch (Exception e)
                {
                    return "Помилка при додаванні до Обраних.";
                }
            }

            return string.Empty;
        }
        public string RemoveAnimalFromFavorites(int animalId, string userId)
        {
            var item = _db.UserFavoritesAnimals.FirstOrDefault(e => e.AnimalId == animalId && e.UserId == userId);

            if (item != null)
            {
                try
                {
                    _db.UserFavoritesAnimals.Remove(item);

                    _db.SaveChanges();
                }
                catch (Exception e)
                {
                    return "Помилка при видаленні.";
                }
            }

            return string.Empty;
        }

        public int CreateShelterRequest(ShelterBase dto)
        {
            throw new NotImplementedException();
        }
    }
}
