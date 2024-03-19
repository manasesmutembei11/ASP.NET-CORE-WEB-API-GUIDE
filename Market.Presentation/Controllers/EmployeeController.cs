using Entities.LinkModels;
using Market.Presentation.ActionFilters;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;
using System.Text.Json;


namespace Market.Presentation.Controllers
{
    [Route("api/companies/{companyId}/employees")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IServiceManager _service;
        public EmployeeController(IServiceManager service) => _service = service;




        [HttpGet]
        [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
        public async Task<IActionResult> GetEmployeesForCompany(Guid companyId,
        [FromQuery] EmployeeParameters employeeParameters)
        {
            var linkParams = new LinkParameters(employeeParameters, HttpContext);
            var result = await _service.EmployeeService.GetEmployeesAsync(companyId,
            linkParams, trackChanges: false);
            Response.Headers.Add("X-Pagination",
            JsonSerializer.Serialize(result.metaData));
        return result.linkResponse.HasLinks ? Ok(result.linkResponse.LinkedEntities) :
        Ok(result.linkResponse.ShapedEntities);
        }





        [HttpPost]
        public async Task<IActionResult> CreateEmployeeForCompanyAsync(Guid companyId, [FromBody]
        EmployeeForCreationDto employee)
        {
            if (employee is null)
                return BadRequest("EmployeeForCreationDto object is null");
            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);
            var employeeToReturn =
           await _service.EmployeeService.CreateEmployeeForCompanyAsync(companyId, employee, trackChanges:
            false);
            return CreatedAtAction("CreateEmployeeForCompanyAsync", new
            {
                companyId,
                id =
            employeeToReturn.Id
            },
            employeeToReturn);
        }

        [HttpDelete("{id:guid}")]
        public IActionResult DeleteEmployeeForCompany(Guid companyId, Guid id)
        {
            _service.EmployeeService.DeleteEmployeeForCompanyAsync(companyId, id, trackChanges:
            false);
            return NoContent();
        }

        [HttpPut("{id:guid}")]
        public IActionResult UpdateEmployeeForCompany(Guid companyId, Guid id,
        [FromBody] EmployeeForUpdateDto employee)
        {
            if (employee is null)
            return BadRequest("EmployeeForUpdateDto object is null");
            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            _service.EmployeeService.UpdateEmployeeForCompanyAsync(companyId, id, employee,
            compTrackChanges: false, empTrackChanges: true);
            return NoContent();
        }

        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> PartiallyUpdateEmployeeForCompany(Guid companyId, Guid id,
        [FromBody] JsonPatchDocument<EmployeeForUpdateDto> patchDoc)
        {
            if (patchDoc is null)
                return BadRequest("patchDoc object sent from client is null.");

            
            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            var result = await _service.EmployeeService.GetEmployeeForPatchAsync(companyId, id, compTrackChanges: false, empTrackChanges: true);
            TryValidateModel(result.employeeToPatch);
            patchDoc.ApplyTo(result.employeeToPatch);
            _service.EmployeeService.SaveChangesForPatch(result.employeeToPatch,
            result.employeeEntity);
            return NoContent();
        }


    }
}
