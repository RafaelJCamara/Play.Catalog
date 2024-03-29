﻿using PlayCatalog.Service.Dtos;
using PlayCatalog.Service.Entities;

namespace PlayCatalog.Service.Extensions
{
    public static class Extensions
    {
        public static ItemDto AsDto(this Item item)
        {
            return new ItemDto(item.Id, item.Name, item.Description, item.Price, item.CreatedDate);
        }
    }
}
