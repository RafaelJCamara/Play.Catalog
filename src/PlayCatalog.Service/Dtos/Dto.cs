using System;
using System.ComponentModel.DataAnnotations;

namespace PlayCatalog.Service.Dtos
{
    /*
        DTO retrieved for GET reqeests
     */
    /*
        We use decimal here because we care about precision and are performing money based operations
        Excelent resource to understance the impact of choosing decimal over double (for example)
        (https://exceptionnotfound.net/decimal-vs-double-and-other-tips-about-number-types-in-net/)
     */
    /*
        Example of DateTimeOffset: 5/1/2008 8:06:32 AM -07:00
     */
    public record ItemDto(Guid Id, string Name, string Description, decimal Price, DateTimeOffset CreatedDate);

    /*
        DTO used to create an item
     */
    public record CreateItemDto([Required] string Name, string Description, [Range(0, 100)] decimal Price);

    /*
        DTO used to update an item
     */
    public record UpdateItemDto([Required] string Name, string Description, [Range(0, 100)] decimal Price);
}
