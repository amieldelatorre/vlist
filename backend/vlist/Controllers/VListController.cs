﻿using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using vlist.Data;
using vlist.Models.VList;
using vlist.Validation;

namespace vlist.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VListController : ControllerBase
    {
        private readonly IRepo _vListRepo;
        public VListController(IRepo vListRepo) =>
            _vListRepo = vListRepo;

        [HttpGet("{id:length(24)}")]
        public async Task Get(string id) =>
            await _vListRepo.GetAsync(id);

        [HttpPost]
        public async Task<IActionResult> Post(VListCreate vListCreate)
        {  
            VList newVList = new VList(
                vListCreate.Title.Trim(),
                vListCreate.Description.Trim(),
                vListCreate.CreatedBy.Trim(),
                DateValidation.ParseDate(vListCreate.Expiry).ToUniversalTime(),
                vListCreate.PassPhrase
                );

            await _vListRepo.CreateAsync(newVList);
            
            return CreatedAtAction(nameof(Get), new { id = newVList.Id }, newVList);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, VList updatedVlist)
        {
            var vList = await _vListRepo.GetAsync(id);
            if (vList == null)
            {
                return NotFound();
            }

            updatedVlist.Id = id;
            await _vListRepo.UpdateAsync(id, updatedVlist);
            return NoContent();
        }
    }
}
