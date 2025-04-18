using fusion.bank.account.domain.Interfaces;
using fusion.bank.core.Autentication;
using fusion.bank.core.Messages.DataContract;
using fusion.bank.core.Messages.Producers;
using fusion.bank.core.Messages.Requests;
using fusion.bank.core.Messages.Responses;
using fusion.bank.core.Model;
using fusion.bank.core.Request;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace fusion.bank.account.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController(IAccountRepository accountRepository, 
        ILogger<AccountController> logger, IPublishEndpoint bus, 
        IRequestClient<NewKeyAccountRequest> requestClient, 
        IConfiguration configuration) : MainController
    {
        [HttpPost("create-account")]
        public async Task<IActionResult> CreateAccount(AccountRequest accountRequest)
        {
            var account = new Account();
            account.CreateAccount(accountRequest);

            await accountRepository.SaveAccount(account);

            await bus.Publish(new NewAccountProducer(account.AccountId,
                account.Name, account.LastName, account.FullName, account.AccountNumber,
                account.Balance, account.TransferLimit, account.SalaryPerMonth,
                account.AccountType, account.BankISBP, account.KeyAccount
            ));

            var token = AutenticationService.GenerateJwtToken(account, configuration);

            var response = new DataContractMessage<string> { Data = token, Success = true };

            return CreateResponse(response);
        }

        [HttpPost("sign-account")]
        public async Task<IActionResult> SignAccount(LoginRequest loginRequest)
        {
            var account = await accountRepository.GetAccountPerTypeAndPassoword(loginRequest);

            if(account is null)
            {
                return CreateResponse(new DataContractMessage<string>(), $"Usuario ou senha inválidos, tente novamente.");
            }

            var token = AutenticationService.GenerateJwtToken(account, configuration);

            return CreateResponse(new DataContractMessage<string> { Data = token, Success = true });
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
