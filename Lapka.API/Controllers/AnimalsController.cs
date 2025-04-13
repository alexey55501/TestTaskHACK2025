using AutoMapper;
using Lapka.BLL.Services;
using Lapka.BLL.Services.Animals;
using Lapka.DAL.Models;
using Lapka.SharedModels.Base;
using Lapka.SharedModels.Constants;
using Lapka.SharedModels.DTO;
using Lapka.SharedModels.DTO.Filters;
using Lapka.SharedModels.Routes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace Lapka.API.Controllers
{
    [Authorize(Roles = GlobalConstants.Roles.USER + "," + GlobalConstants.Roles.SHELTER + "," +
        GlobalConstants.Roles.ADMIN,
       AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route(APIRoutes.V1.Animals.Base)]
    [Produces("application/json")]
    public class AnimalsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AnimalsService _animalsService;
        private readonly BlobService _blobService;

        public AnimalsController(
            UserManager<ApplicationUser> userManager,
            AnimalsService AnimalsService,
            BlobService blobService)
        {
            _userManager = userManager;
            _animalsService = AnimalsService;
            _blobService = blobService;
        }

        #region CRUD

        [HttpPost]
        [Route(APIRoutes.V1.Animals.GetPaginated)]
        [SwaggerOperation("Get all Animals (paginated)")]
        public async Task<IActionResult> GetAnimals(
            [FromBody] AnimalsFilterDTO model)
        {
            try
            {
                var requester = await _userManager.GetUserAsync(User);

                var animals = _animalsService.GetPaginatedAnimals(model, requester.Email);

                if (animals != null)
                    return Ok(animals);
                else
                    return BadRequest();
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route(APIRoutes.V1.Animals.Create)]
        [SwaggerOperation("Create a new Animal")]
        public async Task<IActionResult> CreateAnimal(
            [FromBody] AnimalBase dto)
        {
            try
            {
                var requester = await _userManager.GetUserAsync(User);

                var aid = _animalsService.CreateAnimal(dto, requester.Shelter.Id);
                if (aid != -1)
                    return Ok(aid);
                else
                    return BadRequest();
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route(APIRoutes.V1.Animals.Get)]
        [SwaggerOperation("Get a Animal")]
        public async Task<IActionResult> GetAnimal(int id)
        {
            try
            {
                var Animal = Mapper.Map<AnimalDTO>(_animalsService.GetAnimal(id));

                if (Animal != null)
                {
                    return Ok(Animal);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }


        [HttpPost]
        [Route(APIRoutes.V1.Animals.AddToFavorites)]
        [SwaggerOperation("Get Animal to user favorites")]
        public async Task<IActionResult> AddAnimalToFavorites(
           int id)
        {
            try
            {
                if (id > 0)
                {
                    var user = await _userManager.GetUserAsync(User);

                    var res = _animalsService.AddAnimalToFavorites(id, user.Id);

                    if (string.IsNullOrEmpty(res))
                        return Ok();
                    else
                        return BadRequest(res);
                }

                return BadRequest("Retailer not found");
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route(APIRoutes.V1.Animals.RemoveFromFavorites)]
        [SwaggerOperation("Remove Animal from user favorites")]
        public async Task<IActionResult> RemoveAnimalFromFavorites(
           int id)
        {
            try
            {
                if (id > 0)
                {
                    var user = await _userManager.GetUserAsync(User);

                    var res = _animalsService.RemoveAnimalFromFavorites(id, user.Id);

                    if (string.IsNullOrEmpty(res))
                        return Ok();
                    else
                        return BadRequest(res);
                }

                return BadRequest("Retailer not found");
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpPut]
        [Route(APIRoutes.V1.Animals.Update)]
        [SwaggerOperation("Update a Animal")]
        public async Task<IActionResult> UpdateAnimal(
            int id,
            [FromBody] AnimalDTO payload)
        {
            try
            {
                if (_animalsService.UpdateAnimal(payload))
                    return Ok();
                else
                    return BadRequest();
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpDelete]
        [Route(APIRoutes.V1.Animals.Delete)]
        [SwaggerOperation("Delete a new Animal")]
        public async Task<IActionResult> DeleteAnimal(int id)
        {
            try
            {
                if (_animalsService.DeleteAnimal(id))
                    return Ok();
                else
                    return BadRequest();
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        #endregion
    }
}

