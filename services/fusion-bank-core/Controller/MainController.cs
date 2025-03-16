using fusion.bank.core.Messages.DataContract;
using Microsoft.AspNetCore.Mvc;

public class MainController : ControllerBase
{
    protected IActionResult CreateResponse<T>(DataContractMessage<T> message, string optionMessage = "")
    {
        if (message.Success)
        {
            // Cria um novo message com string, se optionMessage for fornecido
            if (!string.IsNullOrEmpty(optionMessage))
            {
                var newMessage = new DataContractMessage<string>(optionMessage, true);
                return Ok(newMessage);
            }

            return message.Data is null ? NotFound(message) : Ok(message);
        }

        return BadRequest(message);
    }
}