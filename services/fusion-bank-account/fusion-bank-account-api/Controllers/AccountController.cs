using fusion.bank.account.domain.Interfaces;
using fusion.bank.core.Enum;
using fusion.bank.core.Messages.DataContract;
using fusion.bank.core.Messages.Producers;
using fusion.bank.core.Messages.Requests;
using fusion.bank.core.Messages.Responses;
using fusion.bank.core.Model;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace fusion.bank.account.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController(IAccountRepository accountRepository, ILogger<AccountController> logger, IPublishEndpoint bus, IRequestClient<NewKeyAccountRequest> requestClient) : MainController
    {
        [HttpPost("create-account")]
        public async Task<IActionResult> CreateAccount(string name, string lastName, decimal salaryPerMonth, AccountType accountType, string bankISBP, string bankName)
        {
            var account = new Account();
            account.CreateAccount(name, lastName, salaryPerMonth, accountType, bankISBP);

            await accountRepository.SaveAccount(account);

            await bus.Publish(new NewAccountProducer(account.AccountId,
                account.Name, account.LastName, account.FullName, account.AccountNumber,
                account.Balance, account.TransferLimit, account.SalaryPerMonth,
                account.AccountType, account.BankISBP, account.KeyAccount
            ));

            var response = new DataContractMessage<string> { Data = $"Obrigado por ser inscrever no {bankName}. Estamos analizando todas informações e te notificaremos em breve sobre o status de criação da sua conta.", Success = true };

            return CreateResponse(response);
        }

        [HttpGet("list-account")]
        public async Task<IActionResult> ListAccount()
        {
            var response = new DataContractMessage<IEnumerable<Account>> { Data = await accountRepository.ListAllAccount(), Success = true };

            return CreateResponse(response);
        }

        [HttpGet("list-account-by-id/id:guid")]
        public async Task<IActionResult> ListAccountById(Guid id)
        {
            return Ok(await accountRepository.ListAccountById(id));
        }

        [HttpGet("list-account-by-key-account/key:string")]
        public async Task<IActionResult> ListAccountById(string keyAccount)
        {
            return Ok(await accountRepository.ListAccountByKey(keyAccount));
        }

        [HttpPost("register-key-account/id:guid/key:string")]
        public async Task<IActionResult> ListAccountById(Guid id, string keyAccount)
        {
            var account = await accountRepository.ListAccountById(id);

            if(account is null)
            {
                return BadRequest("Account not found");
            }

            var response = await requestClient.GetResponse<DataContractMessage<CreatedKeyAccountResponse>>(new NewKeyAccountRequest(account.AccountId, keyAccount));

            if(!response.Message.Success)
            {
                return BadRequest(response.Message.Error);
            }
            
            await accountRepository.SaveKeyByAccount(id, keyAccount);

            return Ok();
        }

    }
}
