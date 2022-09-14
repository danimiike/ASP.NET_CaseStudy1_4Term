using Casestudy.DAL;
using Casestudy.DAL.DAO;
using Casestudy.DAL.DomainClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Casestudy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BrandsController : ControllerBase
    {
        AppDbContext _db;
        public BrandsController(AppDbContext context)
        {
            _db = context;
        }
        public async Task<ActionResult<List<Brand>>> Index()
        {
            BrandDAO dao = new BrandDAO(_db);
            List<Brand> allBrands = await dao.GetAll();
            return allBrands;
        }
    }
}
