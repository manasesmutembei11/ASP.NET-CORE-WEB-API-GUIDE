﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Core;
using Service.Contracts;
using Shared.DataTransferObjects;
using Market.Presentation.ModelBinders;
using Newtonsoft.Json.Linq;
using static System.Collections.Specialized.BitVector32;
using System.Runtime.Intrinsics.X86;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authorization;

namespace Market.Presentation.Controllers
{
    
    [Route("api/companies")]
    [ApiController]
    [Authorize]
    // [ResponseCache(CacheProfileName = "120SecondsDuration")]

    public class CompaniesController : ControllerBase
    {
        private readonly IServiceManager _service;
        public CompaniesController(IServiceManager service) => _service = service;
        
        
       
        [HttpGet(Name = "GetCompanies")]
        
        public async Task<IActionResult> GetCompanies()
        {
            var companies = await
            _service.CompanyService.GetAllCompaniesAsync(trackChanges: false);
            return Ok(companies);
        }



        [HttpGet("{id:guid}", Name = "CompanyById")]
        public async Task<IActionResult> GetCompany(Guid id)
        {
            var company = await _service.CompanyService.GetCompanyAsync(id, trackChanges:
            false);
            return Ok(company);
        }

        

        [HttpPost(Name = "CreateCompany")]

        public async Task<IActionResult> CreateCompany([FromBody] CompanyForCreationDto company)
        {

            var createdCompany = await _service.CompanyService.CreateCompanyAsync(company);
            return CreatedAtRoute("CompanyById", new { id = createdCompany.Id },
            createdCompany);
        }


        [HttpGet("collection/({ids})", Name = "CompanyCollection")]
        public async Task<IActionResult> GetCompanyCollection
        ([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            var companies = await _service.CompanyService.GetByIdsAsync(ids, trackChanges:
            false);
            return Ok(companies);
        }


        [HttpPost("collection")]
        public async Task<IActionResult> CreateCompanyCollection
        ([FromBody] IEnumerable<CompanyForCreationDto> companyCollection)
        {
            var result = await
            _service.CompanyService.CreateCompanyCollectionAsync(companyCollection);
            return CreatedAtRoute("CompanyCollection", new { result.ids },
            result.companies);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteCompany(Guid id)
        {
            await _service.CompanyService.DeleteCompanyAsync(id, trackChanges: false);
            return NoContent();
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] CompanyForUpdateDto company)
        {
            if (company is null)
                return BadRequest("CompanyForUpdateDto object is null");
            await _service.CompanyService.UpdateCompanyAsync(id, company, trackChanges:
            true);
            return NoContent();
        }



    }

}
