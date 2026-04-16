using System.Collections;
using Microsoft.AspNetCore.Mvc;

namespace MyWorksheet.Website.Server.Controllers;

public abstract class ApiControllerBase : ControllerBase
{
    public virtual IActionResult Data()
    {
        return Ok();
    }

    public virtual IActionResult Data(object value)
    {
        if (value == null)
        {
            return NoContent();
        }

        if (value is ICollection collection)
        {
            if (collection.Count == 0)
            {
                return NoContent();
            }

            return Ok(value);
        }

        return Ok(value);
    }

}
