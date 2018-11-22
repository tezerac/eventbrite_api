﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventCatalogAPI.Data;
using EventCatalogAPI.Domain;
using EventCatalogAPI.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EventCatalogAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Catalog")]
    public class CatalogController : ControllerBase
    {
        private readonly CatalogContext _catalogContext;
        private readonly IConfiguration _configuration;

        public CatalogController(CatalogContext catalogContext,
            IConfiguration Configuration)
        {
            _catalogContext = catalogContext;
            _configuration = Configuration;
        }

        //Get for subCatagories

        [HttpGet]
        [Route("[action]")]

        public async Task<IActionResult> SubCatagories()
        {
            var events = await _catalogContext.SubCatagories.ToListAsync();
            return Ok(events);
        }

        // Get for catagories
        [HttpGet]
        [Route("[action]")]

        public async Task<IActionResult> Catagories()
        {
            var events = await _catalogContext.Catagories.ToListAsync();
            return Ok(events);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Events(
             [FromQuery] int pageSize = 9,
             [FromQuery]int pageIndex = 0
            )
        {
            var totalEvents = await
                _catalogContext.CatalogEvents.LongCountAsync();

            var eventsOnPage = await _catalogContext.CatalogEvents
                 .OrderBy(c => c.EventName)
                 .Skip(pageIndex * pageSize)
                 .Take(pageSize)
                 .ToListAsync();

            eventsOnPage = ChangeUrlPlaceholder(eventsOnPage);

            var model = new PaginatedItemsViewModel<Event>(
               pageIndex, pageSize, totalEvents, eventsOnPage
                );
            return Ok(model);
        }

        // get Event by location

        [HttpGet]
        [Route("[action]/Location/{Location:minlength(1)}")]
        public async Task<IActionResult> GetEventByLocation(
            string location,
             [FromQuery] int pageSize = 9,
             [FromQuery]int pageIndex = 0
            )
        {
            var totalEvents = await
                _catalogContext.CatalogEvents.LongCountAsync();

            var eventsOnPage = await _catalogContext.CatalogEvents
                .Where(c => c.Location.StartsWith(location))
                 .OrderBy(c => c.Location)
                 .Skip(pageIndex * pageSize)
                 .Take(pageSize)
                 .ToListAsync();

            eventsOnPage = ChangeUrlPlaceholder(eventsOnPage);
            var model = new PaginatedItemsViewModel<Event>(
               pageIndex, pageSize, totalEvents, eventsOnPage
                );
            return Ok(model);
        }


        //Get event by EventName

        [HttpGet]
        [Route("[action]/EventName/{EventName:minlength(1)}")]
        public async Task<IActionResult> Events(
            string EventName,
             [FromQuery] int pageSize = 9,
             [FromQuery]int pageIndex = 0
            )
        {
            var totalEvents = await
                _catalogContext.CatalogEvents.LongCountAsync();

            var eventsOnPage = await _catalogContext.CatalogEvents
                .Where(c => c.EventName.StartsWith(EventName))
                 .OrderBy(c => c.EventName)
                 .Skip(pageIndex * pageSize)
                 .Take(pageSize)
                 .ToListAsync();

            eventsOnPage = ChangeUrlPlaceholder(eventsOnPage);
            var model = new PaginatedItemsViewModel<Event>(
              pageIndex, pageSize, totalEvents, eventsOnPage
               );
            return Ok(model);
        }

        // Search By type / brand

        [HttpGet]
        //items/type/1/brand/3 --- example EventCategoryId=1, EventSubCatagoryId=2
        [Route("[action]/brand/{eventcategoryid}/type/{eventsubcatagoryid}")]
        public async Task<IActionResult> Events(
            // ? nullable type-- allowing null value
            //Example -- items/type//brand/3
            int? eventcategoryid,
            int? eventsubcatagoryid,
            [FromQuery] int pageSize = 9,
            [FromQuery]int pageIndex = 0
          )
        {
            //Iqueryable is just converting to Query
            var root = (IQueryable<Event>)_catalogContext.CatalogEvents;
            if (eventcategoryid.HasValue)
            {
                root = root.Where(c => c.EventCategoryId == eventcategoryid);
            }
            if (eventsubcatagoryid.HasValue)
            {
                root = root.Where(c => c.EventSubCatagoryId == eventsubcatagoryid);
            }

            // root is a query
            var totalEvents = await root.LongCountAsync();

            var eventsOnPage = await root
                 .OrderBy(c => c.EventName)
                 .Skip(pageIndex * pageSize)
                 .Take(pageSize)
                 .ToListAsync();

           eventsOnPage = ChangeUrlPlaceholder(eventsOnPage);

            var model = new PaginatedItemsViewModel<Event>(
                pageIndex, pageSize, totalEvents, eventsOnPage);
            return Ok(model);
    }

        //post
        [HttpPost]
        [Route("events")]

        public async Task<IActionResult> CreateEvent(
            [FromBody] Event product)
        {
            var newevent = new Event
            {
                EventCategoryId = product.EventCategoryId,
                EventSubCatagoryId = product.EventSubCatagoryId,
                Description = product.Description,
                EventName = product.EventName,
                EventStartDate = product.EventStartDate,
                EventEndDate = product.EventEndDate,
                EventStartTime =product.EventStartTime,
                EventEndTime = product.EventEndTime,
                EventImageUrl=product.EventImageUrl,
                Fee =product.Fee,
                Address=product.Address,
                Location=product.Location,
                Created =product.Created

            };
            _catalogContext.CatalogEvents.Add(newevent);
           await _catalogContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetEventById), new { eventid = newevent.Id});
        }
        [HttpPut]
        [Route("Events")]
        public async Task<IActionResult> UpdateProduct(
                   [FromBody] Event productToUpdate)
        {
            var catalogEvent = await _catalogContext.CatalogEvents
                              .SingleOrDefaultAsync
                              (i => i.Id == productToUpdate.Id);
            if (catalogEvent == null)
            {
                return NotFound(new { Message = $"Item with id {productToUpdate.Id} not found." });
            }
            catalogEvent = productToUpdate;
            _catalogContext.CatalogEvents.Update(catalogEvent);
            await _catalogContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEventById), new { id = productToUpdate.Id });
        }


        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _catalogContext.CatalogEvents
                .SingleOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();

            }
            _catalogContext.CatalogEvents.Remove(product);
            await _catalogContext.SaveChangesAsync();
            return NoContent();

        }


        // Get event by event Id
        [HttpGet]
        [Route("Events/{eventid:int}")]

     public async Task<IActionResult> GetEventById(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            // this method return one item
            var newevent = await _catalogContext.CatalogEvents
                  .SingleOrDefaultAsync(c => c.Id == id);
            if (newevent != null)
            {
                newevent.EventImageUrl = newevent.EventImageUrl
                    .Replace("http://externalcatalogbaseurltobereplaced",
                _configuration["ExternalCatalogBaseurl"]);
                return Ok(newevent);
            }
            return NotFound();
        }

        private List<Event> ChangeUrlPlaceholder(
            List<Event> Events)
        {
            Events.ForEach(
                x => x.EventImageUrl =
                x.EventImageUrl.Replace("http://externalcatalogbaseurltobereplaced",
                _configuration["ExternalCatalogBaseurl"]));
               return Events;
        }

    }
}