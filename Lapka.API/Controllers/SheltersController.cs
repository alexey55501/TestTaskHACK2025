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
    public class SheltersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AnimalsService _animalsService;

        public SheltersController(
            UserManager<ApplicationUser> userManager,
            AnimalsService AnimalsService,
            BlobService blobService)
        {
            _userManager = userManager;
            _animalsService = AnimalsService;
        }

        //[HttpPost]
        //[Route(APIRoutes.V1.Animals.Create)]
        //[Authorize(Roles = GlobalConstants.Roles.ADMIN)]
        //[SwaggerOperation("Create a new Request to Shelter")]
        //public async Task<IActionResult> CreateRequest(
        //    [FromBody] ShelterBase dto)
        //{
        //    try
        //    {
        //        var aid = _animalsService.CreateShelterRequest(dto);
        //        if (aid != -1)
        //            return Ok(aid);
        //        else
        //            return BadRequest();
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest();
        //    }
        //}

        // Paginated Shelters
        // Do request to shelter
        // Do request to give an animal to shelter


        // Create Request to Shelter

        [HttpGet]
        [Authorize(Roles = GlobalConstants.Roles.USER + "," + GlobalConstants.Roles.SHELTER)]
        [Route(APIRoutes.V1.Shelters.GetShelterInfo)]
        [SwaggerOperation("Get shelter info")]
        public async Task<IActionResult> GetShelterInfo(int id)
        {
            try
            {
                var s = _animalsService.GetShelterInfo(id);

                if (s != null)
                {
                    return Ok(s);
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
    }
}

