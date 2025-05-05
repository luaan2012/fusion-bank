using fusion.bank.account.domain.Interfaces;
using fusion.bank.account.domain.Request;
using fusion.bank.account.domain.Response;
using fusion.bank.core;
using fusion.bank.core.Autentication;
using fusion.bank.core.Messages.DataContract;
using fusion.bank.core.Messages.Producers;
using fusion.bank.core.Messages.Requests;
using fusion.bank.core.Messages.Responses;
using fusion.bank.core.Model;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace fusion.bank.account.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class AccountController(IAccountRepository accountRepository, 
        ILogger<AccountController> logger, IPublishEndpoint bus, 
        IRequestClient<NewKeyAccountRequest> requestClient, 
        IPublishEndpoint publishEndpoint,
        IConfiguration configuration) : MainController
    {
        [AllowAnonymous]
        [HttpPost("create-account")]
        public async Task<IActionResult> CreateAccount(AccountRequest accountRequest)
        {
            var account = new Account();

            account.CreateAccount(accountRequest.Name, accountRequest.LastName, accountRequest.AccountType, accountRequest.BankISBP,
                accountRequest.DocumentType, accountRequest.Document, accountRequest.Email, accountRequest.Password, accountRequest.SalaryPerMonth);

            await accountRepository.SaveAccount(account);

            await bus.Publish(new NewAccountProducer(account.AccountId,
                account.Name, account.LastName, account.FullName, account.AccountNumber, account.Agency,
                account.Balance, account.TransferLimit, account.SalaryPerMonth,
                account.AccountType, account.BankISBP, account.KeyAccount
            ));

            var token = AutenticationService.GenerateJwtToken(account, configuration);

            await publishEndpoint.Publish(GenerateEvent.CreateAccountCreatedEvent(account.AccountId.ToString()));

            return CreateResponse(new DataContractMessage<LoginResponse> { Data = new LoginResponse { Token = token, Account = account}, Success = true });
        }

        [AllowAnonymous]
        [HttpPost("sign-account")]
        public async Task<IActionResult> SignAccount(LoginRequest loginRequest)
        {
            var account = await accountRepository.GetAccountPerTypeAndPassoword(loginRequest);

            if(account is null)
            {
                return CreateResponse(new DataContractMessage<string>(), $"Usuario ou senha inválidos, tente novamente.");
            }

            var token = AutenticationService.GenerateJwtToken(account, configuration);

            await publishEndpoint.Publish(GenerateEvent.CreateLoginEvent(account.AccountId.ToString()));

            return CreateResponse(new DataContractMessage<LoginResponse> { Data = new LoginResponse { Token = token, Account = account }, Success = true });
        }

        [HttpGet("list-account")]
        [AllowAnonymous]
        public async Task<IActionResult> ListAccount()
        {
            var response = new DataContractMessage<IEnumerable<Account>> { Data = await accountRepository.ListAllAccount(), Success = true };

            return CreateResponse(response);
        }

        [HttpGet("get-account-by-id/{id}")]
        public async Task<IActionResult> GetAccountById(Guid id)
        {
            return CreateResponse(new DataContractMessage<Account> { Data = await accountRepository.ListAccountById(id), Success = true });
        }

        [HttpGet("get-account-by-key-account")]
        public async Task<IActionResult> GetAccountByKey(Guid accountId, string keyAccount)
        {
            return CreateResponse(new DataContractMessage<Account> { Data = await accountRepository.ListAccountByKey(keyAccount), Success = true });
        }

        [HttpPut("edit-account/{key}")]
        public async Task<IActionResult> EditAccountByKey(string keyAccountt, AccountEditRequest accountEditRequest)
        {
            var response = await accountRepository.EditAccountByKey(keyAccountt, accountEditRequest);

            if(response is null) return CreateResponse(new DataContractMessage<Account> { Success = false }, "Aconteceu um erro ao tentar atualizar a sua conta, tente novamente mais tarde.");

            await publishEndpoint.Publish(GenerateEvent.CreateAccountEditedEvent(response.AccountId.ToString()));

            return CreateResponse(new DataContractMessage<Account> { Data = response, Success = true });
        }

        [HttpDelete("delete-account/{accountId}")]
        public async Task<IActionResult> EditAccountByKey(Guid accountId)
        {
            return CreateResponse(new DataContractMessage<bool> { Data = await accountRepository.DeleteAccount(accountId), Success = true });
        }

        [HttpDelete("delete-key-account/{accountId}")]
        public async Task<IActionResult> DeleteKeyAccount(Guid accountId)
        {
            var response = await accountRepository.DeleteKeyAccount(accountId);

            if (response is null) return CreateResponse(new DataContractMessage<Account> { Success = false }, "Aconteceu um erro ao tentar excluir sua chave, tente novamente mais tarde.");

            await publishEndpoint.Publish(GenerateEvent.CreateKeyDeletedEvent(response.AccountId.ToString(), response.KeyAccount));

            return CreateResponse(new DataContractMessage<Account> { Data = response, Success = true });
        }

        [HttpPut("edit-key-account/{accountId}")]
        public async Task<IActionResult> EditKeyAccount(Guid accountId, string keyAccount)
        {
            var response = await accountRepository.EditKeyAccount(accountId, keyAccount);

            if (response is null) return CreateResponse(new DataContractMessage<Account> { Success = false }, "Aconteceu um erro ao tentar editar sua chave, tente novamente mais tarde.");

            await publishEndpoint.Publish(GenerateEvent.CreateKeyEditedEvent(response.AccountId.ToString(), response.KeyAccount, keyAccount));

            return CreateResponse(new DataContractMessage<Account> { Data = response, Success = true });
        }

        [HttpPut("set-dark-mode/{accountId}")]
        public async Task<IActionResult> SetDarkMode(Guid accountId, bool darkMode)
        {
            await accountRepository.SetDarkMode(accountId, darkMode);

            return CreateResponse(new DataContractMessage<string> { Success = true }, "DarkMode atualizado com sucesso");
        }

        [HttpPost("register-key-account/{id}/{key}")]
        public async Task<IActionResult> ListAccountById(Guid id, string keyAccount)
        {
            var account = await accountRepository.ListAccountById(id);

            if(account is null)
            {
                return CreateResponse(new DataContractMessage<string> { Success = false}, "Account not found");
            }

            var response = await requestClient.GetResponse<DataContractMessage<CreatedKeyAccountResponse>>(new NewKeyAccountRequest(account.AccountId, keyAccount));

            if(!response.Message.Success)
            {
                return CreateResponse(new DataContractMessage<string> { Success = false, Error = response.Message.Error });
            }
            
            await accountRepository.SaveKeyByAccount(id, keyAccount);

            await publishEndpoint.Publish(GenerateEvent.CreateKeyCreatedEvent(account.AccountId.ToString(), keyAccount));

            return CreateResponse(new DataContractMessage<string> { Success = true });
        }

    }
}
