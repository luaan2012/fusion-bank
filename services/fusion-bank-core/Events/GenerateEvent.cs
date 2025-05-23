﻿using System.Globalization;
using fusion.bank.core.Enum;
using fusion.bank.core.Events;

namespace fusion.bank.core
{
    public static class GenerateEvent
    {
        private static string FormatAmount(decimal? amount)
        {
            return amount.HasValue
                ? amount.Value.ToString("C", CultureInfo.GetCultureInfo("pt-BR"))
                : "-";
        }

        private static string GetLastFourChars(string value)
        {
            if (string.IsNullOrEmpty(value)) return "-";
            return value.Length >= 4 ? value.Substring(value.Length - 4) : value.PadLeft(4, '0');
        }

        public static EventMessage CreateLoginEvent(string accountId)
        {
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));

            return new EventMessage
            {
                Title = "Login efetuado",
                Date = DateTime.UtcNow,
                Action = NotificationType.LOGIN,
                AccountId = accountId,
                Service = ServiceType.ACCOUNT,
                Details = "Você fez login com sucesso!"
            };
        }

        public static EventMessage CreateAccountEditedEvent(string accountId)
        {
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));

            return new EventMessage
            {
                Title = "Conta atualizada",
                Date = DateTime.UtcNow,
                Action = NotificationType.ACCOUNT_EDITED,
                AccountId = accountId,
                Service = ServiceType.ACCOUNT,
                Details = "Sua conta foi atualizada com sucesso!"
            };
        }

        public static EventMessage CreateBilletDueEvent(string accountId, string codeGenerate)
        {
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));
            if (string.IsNullOrEmpty(codeGenerate))
                throw new ArgumentNullException(nameof(codeGenerate));

            return new EventMessage
            {
                Title = "Boleto vencido",
                Date = DateTime.UtcNow,
                Action = NotificationType.BILLET_DUE,
                AccountId = accountId,
                CodeGenerate = GetLastFourChars(codeGenerate),
                Service = ServiceType.DEPOSIT,
                Details = "Atenção: um boleto no seu nome está vencido e pode gerar juros. Regularize agora!"
            };
        }

        public static EventMessage CreateBilletWillExpireEvent(string accountId, string codeGenerate)
        {
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));
            if (string.IsNullOrEmpty(codeGenerate))
                throw new ArgumentNullException(nameof(codeGenerate));

            var codeLastFour = GetLastFourChars(codeGenerate);
            return new EventMessage
            {
                Title = "Boleto prestes a vencer",
                Date = DateTime.UtcNow,
                Action = NotificationType.BILLET_WILL_EXPIRE,
                AccountId = accountId,
                CodeGenerate = codeLastFour,
                Service = ServiceType.DEPOSIT,
                Details = $"Um boleto com número final {codeLastFour} vence em breve. Pague antes do vencimento!"
            };
        }

        public static EventMessage CreateBilletGeneratedEvent(string accountId, string codeGenerate)
        {
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));
            if (string.IsNullOrEmpty(codeGenerate))
                throw new ArgumentNullException(nameof(codeGenerate));

            return new EventMessage
            {
                Title = "Boleto gerado",
                Date = DateTime.UtcNow,
                Action = NotificationType.BILLET_GENERATED,
                AccountId = accountId,
                CodeGenerate = codeGenerate,
                Service = ServiceType.DEPOSIT,
                Details = $"Um novo boleto com número {codeGenerate} foi gerado em seu nome."
            };
        }

        public static EventMessage CreateCreditCardRequestedEvent(string accountId)
        {
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));

            return new EventMessage
            {
                Title = "Cartão solicitado",
                Date = DateTime.UtcNow,
                Action = NotificationType.CREDITCARD_REQUESTED,
                AccountId = accountId,
                Service = ServiceType.CREDITCARD,
                Details = "Sua solicitação de cartão de crédito foi enviada com sucesso!"
            };
        }

        public static EventMessage CreateCreditCardFailedEvent(string accountId, DateTime date)
        {
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));

            return new EventMessage
            {
                Title = "Erro na sua solicitação",
                Date = DateTime.UtcNow,
                Action = NotificationType.CREDITCARD_FAILED,
                AccountId = accountId,
                Service = ServiceType.CREDITCARD,
                Details = $"Nesse momento, nao conseguimos fornecer um cartao para voce. Mas nao desanima, tente novamente em {date}!"
            };
        }

        public static EventMessage CreateInvestmentEvent(string accountId, string name, decimal amount, InvestmentType investmentType, int? quantity = null)
        {
            var formattedAmount = FormatAmount(amount);
            var isStockOrFII = investmentType == InvestmentType.STOCK || investmentType == InvestmentType.FII;
            var details = isStockOrFII && quantity.HasValue
                ? $"Parabéns! Você investiu {formattedAmount} em {quantity} cotas de {name}."
                : $"Parabéns! Você investiu {formattedAmount} em {name}.";

            return new EventMessage
            {
                Title = "Investimento realizado",
                Date = DateTime.UtcNow,
                Action = NotificationType.INVESTMENT_CREATE,
                AccountId = accountId,
                Amount = formattedAmount,
                Investment = investmentType.ToString(),
                Service = ServiceType.INVESTMENT,
                Details = details
            };
        }

        public static EventMessage CreateInvestmentPurchaseEvent(string accountId, string name, decimal amount, InvestmentType investmentType, int? quantity = null)
        {
            var formattedAmount = FormatAmount(amount);
            var isStockOrFII = investmentType == InvestmentType.STOCK || investmentType == InvestmentType.FII;
            var details = isStockOrFII && quantity.HasValue
                ? $"Você adquiriu mais {quantity} cotas de {name} por {formattedAmount}."
                : $"Você investiu mais {formattedAmount} em {name}.";

            return new EventMessage
            {
                Title = "Compra adicional realizada",
                Date = DateTime.UtcNow,
                Action = NotificationType.INVESTMENT_HANDLE,
                AccountId = accountId,
                Amount = formattedAmount,
                Investment = investmentType.ToString(),
                Service = ServiceType.INVESTMENT,
                Details = details
            };
        }

        public static EventMessage CreateInvestmentRescueEvent(string accountId, string name, decimal amount, InvestmentType investmentType, int? quantity = null)
        {
            var formattedAmount = FormatAmount(amount);
            var isStockOrFII = investmentType == InvestmentType.STOCK || investmentType == InvestmentType.FII;
            var details = isStockOrFII && quantity.HasValue
                ? $"Você resgatou {quantity} cotas de {name} no valor de {formattedAmount}."
                : $"Você resgatou {formattedAmount} de {name}.";

            return new EventMessage
            {
                Title = "Resgate realizado",
                Date = DateTime.UtcNow,
                Action = NotificationType.INVESTMENT_RESCUE,
                AccountId = accountId,
                Amount = formattedAmount,
                Investment = investmentType.ToString(),
                Service = ServiceType.INVESTMENT,
                Details = details
            };
        }

        public static EventMessage CreateTransferMadeEvent(string accountId, decimal amount, string userReceive, TransferType transferType)
        {
            var formattedAmount = FormatAmount(amount);

            return new EventMessage
            {
                Title = "Transferencia enviada",
                Date = DateTime.UtcNow,
                Action = NotificationType.TRANSFER_MADE,
                AccountId = accountId,
                TransferType = transferType,
                Amount = formattedAmount,
                UserReceive = userReceive,
                Service = ServiceType.TRANSFER,
                Details = $"Transferência de {formattedAmount} para a conta {userReceive} realizada com sucesso!"
            };
        }

        public static EventMessage CreateTransferFailedEvent(string accountId, decimal amount, string userReceive, string account, string agency, TransferType transferType)
        {
            var formattedAmount = FormatAmount(amount);

            return new EventMessage
            {
                Title = "Erro na transferência",
                Date = DateTime.UtcNow,
                Action = NotificationType.TRANSFER_FAILED,
                AccountId = accountId,
                TransferType = transferType,
                Amount = formattedAmount,
                UserReceive = userReceive,
                Service = ServiceType.TRANSFER,
                Details = $"A transferência {transferType} para {userReceive}, conta {account} da agência {agency}, no valor de {amount}, não pôde ser concluída devido a um erro."
            };
        }

        public static EventMessage CreateTransferReceivedEvent(string accountId, decimal amount, string UserOwner, TransferType transferType)
        {
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));
            if (string.IsNullOrEmpty(UserOwner))
                throw new ArgumentNullException(nameof(UserOwner));

            var formattedAmount = FormatAmount(amount);

            return new EventMessage
            {
                Title = "Transferencia recebida",
                Date = DateTime.UtcNow,
                Action = NotificationType.TRANSFER_RECEIVE,
                TransferType = transferType,
                AccountId = accountId,
                Amount = formattedAmount,
                UserOwner = UserOwner,
                Service = ServiceType.TRANSFER,
                Details = $"Você recebeu uma transferência de {formattedAmount} enviada por {UserOwner}."
            };
        }

        public static EventMessage CreateTransferProcessingEvent(string accountId, decimal amount, string UserOwner, TransferType transferType)
        {
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));
            if (string.IsNullOrEmpty(UserOwner))
                throw new ArgumentNullException(nameof(UserOwner));

            var formattedAmount = FormatAmount(amount);

            return new EventMessage
            {
                Title = "Transferencia em processamento",
                Date = DateTime.UtcNow,
                Action = NotificationType.TRANSFER_RECEIVE,
                TransferType = transferType,
                AccountId = accountId,
                Amount = formattedAmount,
                UserOwner = UserOwner,
                Service = ServiceType.TRANSFER,
                Details = $"Sua transferencia {transferType} entrou em processamento, levará entre 10 a 60 segundos para ser concluída."
            };
        }

        public static EventMessage CreateAccountCreatedEvent(string accountId)
        {
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));

            return new EventMessage
            {
                Title = "Conta criada",
                Date = DateTime.UtcNow,
                Action = NotificationType.ACCOUNT_CREATED,
                AccountId = accountId,
                Service = ServiceType.ACCOUNT,
                Details = "Bem-vindo! Sua conta foi criada e está pronta para uso."
            };
        }

        public static EventMessage CreateTransferScheduledEvent(string accountId, decimal amount, string dateSchedule, TransferType transferType)
        {
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));
            if (string.IsNullOrEmpty(dateSchedule))
                throw new ArgumentNullException(nameof(dateSchedule));

            var formattedAmount = FormatAmount(amount);
            return new EventMessage
            {
                Title = "Transferencia agendada",
                Date = DateTime.UtcNow,
                Action = NotificationType.TRANSFER_SCHEDULED,
                AccountId = accountId,
                TransferType = transferType,
                Amount = formattedAmount,
                DateSchedule = dateSchedule,
                Service = ServiceType.TRANSFER,
                Details = $"Transferência de {formattedAmount} agendada para {dateSchedule} com sucesso."
            };
        }

        public static EventMessage CreateKeyCreatedEvent(string accountId, string keyAccount)
        {
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));
            if (string.IsNullOrEmpty(keyAccount))
                throw new ArgumentNullException(nameof(keyAccount));

            return new EventMessage
            {
                Title = "Chave criada",
                Date = DateTime.UtcNow,
                Action = NotificationType.KEY_CREATED,
                AccountId = accountId,
                KeyAccount = keyAccount,
                Service = ServiceType.ACCOUNT,
                Details = $"A chave {keyAccount} foi registrada com sucesso. Suas transferências serão direcionadas para sua conta."
            };
        }

        public static EventMessage CreateKeyDeletedEvent(string accountId, string keyAccount)
        {
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));
            if (string.IsNullOrEmpty(keyAccount))
                throw new ArgumentNullException(nameof(keyAccount));

            return new EventMessage
            {
                Title = "Chave deletada",
                Date = DateTime.UtcNow,
                Action = NotificationType.KEY_DELETED,
                AccountId = accountId,
                KeyAccount = keyAccount,
                Service = ServiceType.ACCOUNT,
                Details = $"A chave {keyAccount} foi deletada com sucesso."
            };
        }

        public static EventMessage CreateKeyEditedEvent(string accountId, string keyAccountOld, string keyAccountNew)
        {
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));
            if (string.IsNullOrEmpty(keyAccountOld))
                throw new ArgumentNullException(nameof(keyAccountOld));

            return new EventMessage
            {
                Title = "Chave editada",
                Date = DateTime.UtcNow,
                Action = NotificationType.KEY_EDITED,
                AccountId = accountId,
                KeyAccount = keyAccountOld,
                Service = ServiceType.ACCOUNT,
                Details = $"A chave {keyAccountOld} foi alterada para {keyAccountNew} com sucesso."
            };
        }

        public static EventMessage CreateCreditCardResponsedEvent(string accountId, string creditCardType, string creditCardNumber, decimal creditCardLimit)
        {
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));
            if (string.IsNullOrEmpty(creditCardType))
                throw new ArgumentNullException(nameof(creditCardType));
            if (string.IsNullOrEmpty(creditCardNumber))
                throw new ArgumentNullException(nameof(creditCardNumber));

            var cardLastFour = GetLastFourChars(creditCardNumber);
            var formattedLimit = FormatAmount(creditCardLimit);
            return new EventMessage
            {
                Title = "Cartão de crédito liberado",
                Date = DateTime.UtcNow,
                Action = NotificationType.CREDITCARD_RESPONSED,
                AccountId = accountId,
                CreditCardType = creditCardType,
                CreditCardNumber = cardLastFour,
                CreditCardLimit = formattedLimit,
                Service = ServiceType.CREDITCARD,
                Details = $"Parabéns! Seu cartão de crédito {creditCardType} com final {cardLastFour} e limite de {formattedLimit} está liberado para uso."
            };
        }

        public static EventMessage CreateDepositCreateEvent(string accountId, decimal amount, TransferType transferType)
        {
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));

            var formattedAmount = FormatAmount(amount);
            return new EventMessage
            {
                Title = "Deposito em processamento",
                Date = DateTime.UtcNow,
                Action = NotificationType.DEPOSIT_CREATE,
                TransferType = transferType,
                AccountId = accountId,
                Amount = formattedAmount,
                Service = ServiceType.DEPOSIT,
                Details = $"Depósito de {formattedAmount} já esta em processamento."
            };
        }

        public static EventMessage CreateBilletCreateEvent(string accountId, decimal amount, TransferType transferType)
        {
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));

            var formattedAmount = FormatAmount(amount);
            return new EventMessage
            {
                Title = "Boleto pago",
                Date = DateTime.UtcNow,
                Action = NotificationType.BILLET_MADE,
                TransferType = transferType,
                AccountId = accountId,
                Amount = formattedAmount,
                Service = ServiceType.DEPOSIT,
                Details = $"Boleto pago com sucesso."
            };
        }


        public static EventMessage CreateDirectDepositCreateEvent(string accountId, decimal amount, TransferType transferType)
        {
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));

            var formattedAmount = FormatAmount(amount);
            return new EventMessage
            {
                Title = "Deposito em processamento",
                Date = DateTime.UtcNow,
                Action = NotificationType.DEPOSIT_DIRECT_CREATE,
                TransferType = transferType,
                AccountId = accountId,
                Amount = formattedAmount,
                Service = ServiceType.DEPOSIT,
                Details = $"Depósito de {formattedAmount} creditado em sua conta com sucesso."
            };
        }

        public static EventMessage CreateDirectDepositMadeEvent(string accountId, decimal amount, TransferType transferType)
        {
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));

            var formattedAmount = FormatAmount(amount);
            return new EventMessage
            {
                Title = "Deposito concluido",
                Date = DateTime.UtcNow,
                Action = NotificationType.DEPOSIT_DIRECT_MADE,
                TransferType = transferType,
                AccountId = accountId,
                Amount = formattedAmount,
                Service = ServiceType.DEPOSIT,
                Details = $"Recebemos seu depósito no valor de {amount} e já está na sua conta."
            };
        }
    }
}