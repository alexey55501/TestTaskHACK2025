//using AutoMapper;
//using Lapka.API.DAL.Models;
//using Lapka.BLL.Helpers;
//using Lapka.CORE.Constants;
//using Lapka.DAL.DbContext;
//using Lapka.SharedModels.Constants;
//using Lapka.SharedModels.DTO.Pagination;
//using Microsoft.EntityFrameworkCore;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Lapka.BLL.Services.Shops
//{
//    public class ShopService : BaseService
//    {
//        private readonly LapkaDbContextFactory _factory;
//        private readonly LogService _logService;

//        public ShopService(LapkaDbContextFactory factory, LogService logService)
//        {
//            _factory = factory;
//            _logService = logService;
//        }

//        #region Shop CRUD

//        // Create
//        public bool CreateShop(ShopCreateDTO dto)
//        {
//            try
//            {
//                var parsedConfig = JsonConvert.DeserializeObject<ShopConfig>(dto.Config);

//                using (var db = _factory.CreateDbContext())
//                {
//                    var shop = db.Shops.AsNoTracking().FirstOrDefault(e => e.Link == dto.Link);

//                    if (shop == null)
//                    {
//                        shop = new Shop();
//                        shop.Name = dto.Name;
//                        shop.Link = dto.Link;
//                        shop.ImageLink = dto.ImageLink;
//                        shop.DateCreated = DateTime.Now;
//                        shop.DateModified = shop.DateCreated;
//                        shop.Region = dto.Region;
//                        shop.Config = new ShopParsingConfig
//                        {
//                            Config = dto.Config,
//                            IsCategoriesSelenium = parsedConfig.Category.ScrapperType == SharedModels.Enums.Parser.Task.ParserTaskScrapperType.Browser,
//                            IsProductsSelenium = parsedConfig.Product.ScrapperType == SharedModels.Enums.Parser.Task.ParserTaskScrapperType.Browser
//                        };
//                        db.Shops.Add(shop);
//                    }
//                    else
//                    {
//                        shop.Name = dto.Name;
//                        shop.Link = dto.Link;
//                        shop.ImageLink = dto.ImageLink;
//                        shop.Region = dto.Region;
//                        shop.DateModified = DateTime.Now;

//                        var config = db.ShopParsingConfigs.AsNoTracking().FirstOrDefault(e => e.ShopId == shop.Id);

//                        if (config == null)
//                        {
//                            db.ShopParsingConfigs.Add(new ShopParsingConfig
//                            {
//                                ShopId = shop.Id,
//                                Config = dto.Config,
//                                IsCategoriesSelenium = parsedConfig.Category.ScrapperType == SharedModels.Enums.Parser.Task.ParserTaskScrapperType.Browser,
//                                IsProductsSelenium = parsedConfig.Product.ScrapperType == SharedModels.Enums.Parser.Task.ParserTaskScrapperType.Browser
//                            });
//                        }
//                        else
//                        {
//                            config.Config = dto.Config;
//                            config.IsCategoriesSelenium = parsedConfig.Category.ScrapperType == SharedModels.Enums.Parser.Task.ParserTaskScrapperType.Browser;
//                            config.IsProductsSelenium = parsedConfig.Product.ScrapperType == SharedModels.Enums.Parser.Task.ParserTaskScrapperType.Browser;
//                            db.ShopParsingConfigs.Update(config);
//                        }

//                        db.Shops.Update(shop);
//                    }

//                    db.SaveChanges();
//                }
//            }
//            catch (Exception ex)
//            {
//                return false;
//            }
//            return true;
//        }

//        public bool IsShopExists(int id)
//        {
//            using (var db = _factory.CreateDbContext())
//            {
//                return db.Shops.Any(t => t.Id == id);
//            }
//        }

//        // Get shop
//        public Shop GetShop(int id)
//        {
//            using (var db = _factory.CreateDbContext())
//            {
//                return db.Shops.Where(t => t.Id == id).FirstOrDefault();
//            }
//        }

//        // Get shop by url
//        public Shop GetShopByUrl(string url)
//        {
//            using (var db = _factory.CreateDbContext())
//            {
//                return db.Shops.Where(t => t.Link == url).FirstOrDefault();
//            }
//        }

//        public int? GetShopIdByUrl(string url)
//        {
//            using (var db = _factory.CreateDbContext())
//            {
//                return db.Shops.Where(t => t.Link == url).FirstOrDefault()?.Id;
//            }
//        }

//        public List<ShopCreateDTO> GetShopsForExport()
//        {
//            using (var db = _factory.CreateDbContext())
//            {
//                return db.Shops.Select(s => new ShopCreateDTO
//                {
//                    Name = s.Name,
//                    Link = s.Link,
//                    ImageLink = s.ImageLink,
//                    Config = s.Config.Config
//                }).ToList();
//            }
//        }

//        public ShopStatisticsDTO GetShopStatistics(int id)
//        {
//            var result = new ShopStatisticsDTO();

//            using (var db = _factory.CreateDbContext())
//            {
//                var searches = db.Searches.AsNoTracking().Where(se => se.ShopId == id).Select(se => se.DateCreated).ToList();

//                result.TotalSearches = searches.Count();
//                result.Last90DaysSearches = searches.Where(e => e >= DateTime.Now.AddDays(-90)).Count();
//                result.Last30DaysSearches = searches.Where(e => e >= DateTime.Now.AddDays(-30)).Count();
//                result.ProductsCount = db.Products.AsNoTracking().Where(p => p.ShopId == id).Count();
//                result.VAChecks = db.ProductPairs.AsNoTracking().Where(pp => pp.Status != ProductPairStatus.Unset && pp.RetailerId == id).Count();
//            }

//            return result;
//        }

//        // Get shops
//        public PaginatedResponse<AdminShopDTO> GetPaginatedShops(AdminShopFilter filter)
//        {
//            var dateTo = DateTime.Now.AddDays(-30);

//            using (var db = _factory.CreateDbContext())
//            {
//                var items = db.Shops.AsNoTracking()
//                                .Where(t => (string.IsNullOrEmpty(filter.SearchQuery) || t.Name.Contains(filter.SearchQuery)) &&
//                                   (filter.Region == null || t.Region == filter.Region))
//                                .Select(s => new AdminShopDTO
//                                {
//                                    Id = s.Id,
//                                    Deals = s.ExpectedProfitable,
//                                    IsHidden = s.IsHidden,
//                                    Link = s.Link,
//                                    IsUnderMaintenance = s.IsUnderMaintenance,
//                                    ImageLink = s.ImageLink,
//                                    Name = s.Name,
//                                    Region = s.Region,
//                                    HasIssues = false//db.Issues.AsNoTracking().Any(i => i.ShopId == s.Id)
//                                });

//                if (filter.IsHavingIsuues)
//                    items = items.Where(e => e.HasIssues);

//                var count = items.Count();

//                switch (filter.Order)
//                {
//                    case ShopFilterSorting.NameASC:
//                        items = items.OrderBy(p => p.Name);
//                        break;
//                    case ShopFilterSorting.NameDESC:
//                        items = items.OrderByDescending(p => p.Name);
//                        break;
//                    case ShopFilterSorting.LinkASC:
//                        items = items.OrderBy(p => p.Link);
//                        break;
//                    case ShopFilterSorting.LinkDESC:
//                        items = items.OrderByDescending(p => p.Link);
//                        break;
//                    case ShopFilterSorting.RegionASC:
//                        items = items.OrderBy(p => p.Region);
//                        break;
//                    case ShopFilterSorting.RegionDESC:
//                        items = items.OrderByDescending(p => p.Region);
//                        break;
//                    case ShopFilterSorting.IsHiddenASC:
//                        items = items.OrderBy(p => p.IsHidden);
//                        break;
//                    case ShopFilterSorting.IsHiddenDESC:
//                        items = items.OrderByDescending(p => p.IsHidden);
//                        break;
//                    case ShopFilterSorting.DealsASC:
//                        items = items.OrderBy(p => p.Deals);
//                        break;
//                    case ShopFilterSorting.DealsDESC:
//                        items = items.OrderByDescending(p => p.Deals);
//                        break;
//                    case ShopFilterSorting.ProductsASC:
//                        items = items.OrderBy(p => p.Products);
//                        break;
//                    case ShopFilterSorting.ProductsDESC:
//                        items = items.OrderByDescending(p => p.Products);
//                        break;
//                    case ShopFilterSorting.IsUnderMaintenanceASC:
//                        items = items.OrderBy(p => p.IsUnderMaintenance);
//                        break;
//                    case ShopFilterSorting.IsUnderMaintenanceDESC:
//                        items = items.OrderByDescending(p => p.IsUnderMaintenance);
//                        break;
//                    //case ShopFilterSorting.VAChecksASC:
//                    //    items = items.OrderBy(p => p.VAChecks);
//                    //    break;
//                    //case ShopFilterSorting.VAChecksDESC:
//                    //    items = items.OrderByDescending(p => p.VAChecks);
//                    //    break;
//                    //case ShopFilterSorting.Searches30DaysASC:
//                    //    items = items.OrderBy(p => p.Searches30Days);
//                    //    break;
//                    //case ShopFilterSorting.Searches30DaysDESC:
//                    //    items = items.OrderByDescending(p => p.Searches30Days);
//                    //    break;
//                    //case ShopFilterSorting.LastIssueASC:
//                    //    items = items.OrderBy(p => p.LastIssue);
//                    //    break;
//                    //case ShopFilterSorting.LastIssueDESC:
//                    //    items = items.OrderByDescending(p => p.LastIssue);
//                    //    break;
//                    default:
//                        break;
//                }

//                //foreach (var item in result)
//                //{
//                //    item.Products = _db.Products.AsNoTracking().Where(p => p.ShopId == item.Id).Count();
//                //    item.VAChecks = _db.ProductPairs.AsNoTracking().Include(pp => pp.ProductRetailer)
//                //        .Where(pp => pp.Status != ProductPairStatus.Unset && pp.ProductRetailer.ShopId == item.Id).Count();
//                //    item.Searches30Days = _db.Searches.AsNoTracking().Where(se => se.ShopId == item.Id && se.DateCreated >= dateTo).Count();
//                //    item.LastIssue = _db.Issues.Where(e => e.ShopId == item.Id).OrderByDescending(e => e.DateCreated).FirstOrDefault()?.DateCreated;
//                //}

//                return new PaginatedResponse<AdminShopDTO>()
//                {
//                    Items = items.Skip(filter.Skip)
//                        .Take(filter.Take)
//                        .ToList(),
//                    CurrentPage = filter?.Page.Value ?? 0,
//                    PagesCount = PaginationHelper.PagesCount(filter?.AmountOnPage ?? 1, count),
//                };
//            }
//        }

//        public List<ChartJSChartItem> GetShopProductUpdateDates(int shopId)
//        {
//            using (var db = _factory.CreateDbContext())
//            {
//                var dates = db.Products.AsNoTracking().Where(e => e.ShopId == shopId).Select(e => e.DateModified).ToList();

//                return dates.GroupBy(e => e.Date).Select(e => new ChartJSChartItem { X = e.Key, Y = e.Count() }).OrderBy(e => e.X).ToList();
//            }
//        }

//        public List<ShopDTO> GetShops(AccountRegion region)
//        {
//            using (var db = _factory.CreateDbContext())
//            {
//                return db.Shops.AsNoTracking().Where(e => !e.IsHidden && e.Region == region).OrderByDescending(e => e.ExpectedProfitable).Select(s => Mapper.Map<ShopDTO>(s)).ToList();
//            }
//        }

//        public List<ShopIssuesListItem> GetShopsIssuesList()
//        {
//            var result = new List<ShopIssuesListItem>();

//            //using (var db = _factory.CreateDbContext())
//            //{
//            //    var shops = db.Shops.AsNoTracking().ToList();

//            //    foreach (var shop in shops)
//            //    {
//            //        var count = db.Issues.AsNoTracking().Where(e => e.ShopId == shop.Id).Count();

//            //        if (count > 0)
//            //            result.Add(new ShopIssuesListItem
//            //            {
//            //                Id = shop.Id,
//            //                Name = shop.Name,
//            //                ImageUrl = shop.ImageLink,
//            //                Count = count
//            //            });
//            //    }
//            //}

//            return result;
//        }

//        public PaginatedResponse<ParserIssueDTO> GetShopsIssuesDetails(ShopIssuesFilterModel filter)
//        {
//            return new PaginatedResponse<ParserIssueDTO>();
//            //using (var db = _factory.CreateDbContext())
//            //{
//            //    var issues = db.Issues.AsNoTracking().Where(e => e.ShopId == filter.ShopId).OrderByDescending(e => e.DateCreated).AsQueryable();

//            //    var count = issues.Count();

//            //    issues = issues.Skip(filter.Skip).Take(filter.Take).AsQueryable();

//            //    var result = new PaginatedResponse<ParserIssueDTO>()
//            //    {
//            //        Items = issues.ToList()
//            //                           .Select(e => new ParserIssueDTO
//            //                           {
//            //                               Id = e.Id,
//            //                               DateCreated = e.DateCreated,
//            //                               Content = e.Content
//            //                           })
//            //                           .ToList(),
//            //        ItemsCount = count,
//            //        CurrentPage = filter?.Page.Value ?? 0,
//            //        PagesCount = PaginationHelper.PagesCount(filter?.AmountOnPage ?? 1, count),
//            //    };

//            //    return result;
//            //}
//        }

//        // Update Shop 
//        public bool UpdateShop(ShopUpdateDTO payload)
//        {
//            using (var db = _factory.CreateDbContext())
//            {
//                var shop = db.Shops.Where(t => t.Id == payload.Id).FirstOrDefault();
//                if (shop != null)
//                {
//                    shop.Name = payload.Name;
//                    shop.Link = payload.Link;
//                    shop.ImageLink = payload.ImageLink;
//                    shop.IsTop = payload.IsTop;
//                    shop.IsHidden = payload.IsHidden;
//                    shop.IsUnderMaintenance = payload.IsUnderMaintenance;
//                    shop.Region = payload.Region;
//                    shop.DateModified = DateTime.Now;
//                    shop.LastFailedSearchDate = null;
//                    db.SaveChanges();
//                    return true;
//                }
//                return false;
//            }
//        }

//        public string AddShopToFavorites(int shopId, string userId)
//        {
//            using (var db = _factory.CreateDbContext())
//            {
//                if (db.UserFavorites.Where(e => e.UserId == userId && e.Shop != null).Count() < GlobalConstants.Values.MAX_FAVORITE_SHOPS)
//                {
//                    if (!db.UserFavorites.Any(e => e.ShopId == shopId && e.UserId == userId))
//                    {
//                        try
//                        {
//                            db.UserFavorites.Add(new UserFavorites
//                            {
//                                UserId = userId,
//                                ShopId = shopId
//                            });

//                            db.SaveChanges();

//                            return string.Empty;
//                        }
//                        catch (Exception e)
//                        {
//                            return "Error while adding favorite retailer";
//                        }
//                    }
//                    else
//                        return string.Empty;
//                    //return "This retailer is already favorite";
//                }
//                return $"Maximum {GlobalConstants.Values.MAX_FAVORITE_SHOPS} favorite retailers";
//            }
//        }
//        public string RemoveShopFromFavorites(int shopId, string userId)
//        {
//            using (var db = _factory.CreateDbContext())
//            {
//                var item = db.UserFavorites.FirstOrDefault(e => e.ShopId == shopId && e.UserId == userId);

//                if (item != null)
//                {
//                    try
//                    {
//                        db.UserFavorites.Remove(item);

//                        db.SaveChanges();
//                    }
//                    catch (Exception e)
//                    {
//                        return "Error while removing favorite retailer";
//                    }
//                }

//                return string.Empty;
//            }
//        }
//        public void UpdateTopShops(List<string> topShopsUrls)
//        {
//            using (var db = _factory.CreateDbContext())
//            {
//                var shops = db.Shops.AsNoTracking().Where(e => !e.IsDeleted).ToList();

//                foreach (var shop in shops)
//                {
//                    if (topShopsUrls.Contains(shop.Link))
//                    {
//                        if (!shop.IsTop)
//                        {
//                            shop.IsTop = true;

//                            db.Shops.Update(shop);
//                        }
//                    }
//                    else if (shop.IsTop)
//                    {
//                        shop.IsTop = false;

//                        db.Shops.Update(shop);
//                    }
//                }

//                db.SaveChanges();
//            }
//        }
//        // Delete Shop
//        public bool DeleteShop(int id)
//        {
//            using (var db = _factory.CreateDbContext())
//            {
//                var shop = db.Shops.Where(t => t.Id == id).FirstOrDefault();
//                if (shop != null)
//                {
//                    shop.IsDeleted = true;
//                    shop.UserFavorites.Clear();
//                    db.SaveChanges();
//                    return true;
//                }
//                return false;
//            }
//        }


//        public List<int> GetExpiringShops()
//        {
//            var results = new List<int>();

//            using (var db = _factory.CreateDbContext())
//            {
//                var ids = db.Shops
//                    .AsNoTracking()
//                    .Where(e => !e.IsHidden && e.Link != GlobalConstants.Values.AMAZON_UK_URL && e.Link != GlobalConstants.Values.AMAZON_US_URL)
//                    .Select(e => e.Id)
//                    .ToList();

//                var dateTo = DateTime.Now.AddDays(AppConstants.Search.MAX_DAYS_OLD_RETAILER_PRODUCT);

//                foreach (var id in ids)
//                {
//                    if (db.Products.AsNoTracking().Any(e => e.ShopId == id))
//                    {
//                        var dateMax = db.Products.AsNoTracking().Where(e => e.ShopId == id).Max(e => e.DateModified);
//                        if (dateMax <= dateTo)
//                        {
//                            results.Add(id);
//                        }
//                    }
//                }

//                db.Shops.AsNoTracking().Where(e => !results.Contains(e.Id)).ExecuteUpdate(e => e.SetProperty(p => p.IsUnderMaintenance, false));
//                db.Shops.AsNoTracking().Where(e => results.Contains(e.Id)).ExecuteUpdate(e => e.SetProperty(p => p.IsUnderMaintenance, true));
//            }

//            return results;
//        }
//        #endregion
//    }
//}
