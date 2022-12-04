using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Contracts;
using Play.Common.Repository;
using PlayCatalog.Service.Dtos;
using PlayCatalog.Service.Entities;
using PlayCatalog.Service.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayCatalog.Service.Controllers
{
    [ApiController]
    [Route("items")]
    ////by specifying the role here, we are saying that only tokens that contain the role claim with value admin are going to be granted access
    //[Authorize(Roles = "Admin")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<Item> itemsRepository;
        private readonly IPublishEndpoint publishEndpoint;

        public ItemsController(IRepository<Item> itemsRepository, IPublishEndpoint publishEndpoint)
        {
            this.itemsRepository = itemsRepository;
            this.publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        /*
            This means that a token must comply with the Read policy, defined in the startup class
         */
        [Authorize(Policies.Policies.Read)]
        public async Task<IEnumerable<ItemDto>> Get()
        {
            return (await itemsRepository.GetAllAsync()).Select(element => element.AsDto());
        }

        [HttpGet("{id}")]
        [Authorize(Policies.Policies.Read)]
        public async Task<ActionResult<ItemDto>> GetById(Guid id)
        {
            var existingItem = await itemsRepository.GetAsync(id);
            if (existingItem == null)
            {
                return NotFound();
            }
            return existingItem.AsDto();
        }

        [HttpPost]
        [Authorize(Policies.Policies.Write)]
        public async Task<ActionResult<ItemDto>> Post(CreateItemDto createItemDto)
        {
            var item = new Item
            {
                Id = Guid.NewGuid(), 
                Name = createItemDto.Name, 
                Description = createItemDto.Description, 
                Price = createItemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await itemsRepository.CreateAsync(item);

            await publishEndpoint.Publish(new CatalogItemCreated(item.Id, item.Name, item.Description));

            /*
                With CreatedAtAction, in the headers field, we will have a location attribute that will give us the path/url to find the item
             */
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item.AsDto());
        }

        [HttpPut("{id}")]
        [Authorize(Policies.Policies.Write)]
        public async Task<IActionResult> Put(Guid id, UpdateItemDto updateItemDto)
        {
            var existingItem = await itemsRepository.GetAsync(id);

            if (existingItem == null)
            {
                return NotFound();
            }

            existingItem.Name = updateItemDto.Name;
            existingItem.Description = updateItemDto.Description;
            existingItem.Price = updateItemDto.Price;

            await itemsRepository.UpdateAsync(existingItem);

            await publishEndpoint.Publish(new CatalogItemUpdated(existingItem.Id, existingItem.Name, existingItem.Description));

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policies.Policies.Write)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await itemsRepository.GetAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            await itemsRepository.RemoveAsync(item.Id);

            await publishEndpoint.Publish(new CatalogItemDeleted(item.Id));

            return NoContent();
        }

    }
}
