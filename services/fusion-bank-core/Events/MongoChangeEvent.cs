using System.Globalization;
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
                Date = DateTime.UtcNow,
                Action = NotificationType.LOGIN,
                UserOwner = accountId,
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
                Date = DateTime.UtcNow,
                Action = NotificationType.ACCOUNT_EDITED,
                UserOwner = accountId,
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
                Date = DateTime.UtcNow,
                Action = NotificationType.BILLET_DUE,
                UserOwner = accountId,
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
                Date = DateTime.UtcNow,
                Action = NotificationType.BILLET_WILL_EXPIRE,
                UserOwner = accountId,
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
                Date = DateTime.UtcNow,
                Action = NotificationType.BILLET_GENERATED,
                UserOwner = accountId,
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
                Date = DateTime.UtcNow,
                Action = NotificationType.CREDITCARD_REQUESTED,
                UserOwner = accountId,
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
                Date = DateTime.UtcNow,
                Action = NotificationType.CREDITCARD_FAILED,
                UserOwner = accountId,
                Service = ServiceType.CREDITCARD,
                Details = $"Nesse momento, nao conseguimos fornecer um cartao para voce. Mas nao desanima, tente novamente em {date}!"
            };
        }

        public static EventMessage CreateInvestmentEvent(string accountId, decimal amount, string investmentType)
        {
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));
            if (string.IsNullOrEmpty(investmentType))
                throw new ArgumentNullException(nameof(investmentType));

            var formattedAmount = FormatAmount(amount);
            return new EventMessage
            {
                Date = DateTime.UtcNow,
                Action = NotificationType.INVESTMENT,
                UserOwner = accountId,
                Amount = formattedAmount,
                Investment = investmentType,
                Service = ServiceType.INVESTMENT,
                Details = $"Você investiu {formattedAmount} em {investmentType}."
            };
        }

        public static EventMessage CreateTransferMadeEvent(string accountId, decimal amount, string UserOwnerReceive)
        {
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));
            if (string.IsNullOrEmpty(UserOwnerReceive))
                throw new ArgumentNullException(nameof(UserOwnerReceive));

            var formattedAmount = FormatAmount(amount);

            return new EventMessage
            {
                Date = DateTime.UtcNow,
                Action = NotificationType.TRANSFER_MADE,
                UserOwner = accountId,
                Amount = formattedAmount,
                UserReceive = UserOwnerReceive,
                Service = ServiceType.TRANSFER,
                Details = $"Transferência de {formattedAmount} para a conta {UserOwnerReceive} realizada com sucesso!"
            };
        }

        public static EventMessage CreateTransferReceivedEvent(string accountId, decimal amount, string UserOwnerOwner)
        {
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));
            if (string.IsNullOrEmpty(UserOwnerOwner))
                throw new ArgumentNullException(nameof(UserOwnerOwner));

            var formattedAmount = FormatAmount(amount);

            return new EventMessage
            {
                Date = DateTime.UtcNow,
                Action = NotificationType.TRANSFER_RECEIVE,
                UserOwner = accountId,
                Amount = formattedAmount,
                UserReceive = UserOwnerOwner,
                Service = ServiceType.TRANSFER,
                Details = $"Você recebeu uma transferência de {formattedAmount} enviada por {UserOwnerOwner}."
            };
        }

        public static EventMessage CreateAccountCreatedEvent(string accountId)
        {
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));

            return new EventMessage
            {
                Date = DateTime.UtcNow,
                Action = NotificationType.ACCOUNT_CREATED,
                UserOwner = accountId,
                Service = ServiceType.ACCOUNT,
                Details = "Bem-vindo! Sua conta foi criada e está pronta para uso."
            };
        }

        public static EventMessage CreateTransferScheduledEvent(string accountId, decimal amount, string dateSchedule)
        {
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));
            if (string.IsNullOrEmpty(dateSchedule))
                throw new ArgumentNullException(nameof(dateSchedule));

            var formattedAmount = FormatAmount(amount);
            return new EventMessage
            {
                Date = DateTime.UtcNow,
                Action = NotificationType.TRANSFER_SCHEDULED,
                UserReceive = accountId,
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
                Date = DateTime.UtcNow,
                Action = NotificationType.KEY_CREATED,
                UserOwner = accountId,
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
                Date = DateTime.UtcNow,
                Action = NotificationType.KEY_DELETED,
                UserOwner = accountId,
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
                Date = DateTime.UtcNow,
                Action = NotificationType.KEY_EDITED,
                UserOwner = accountId,
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
                Date = DateTime.UtcNow,
                Action = NotificationType.CREDITCARD_RESPONSED,
                UserOwner = accountId,
                CreditCardType = creditCardType,
                CreditCardNumber = cardLastFour,
                CreditCardLimit = formattedLimit,
                Service = ServiceType.CREDITCARD,
                Details = $"Parabéns! Seu cartão de crédito {creditCardType} com final {cardLastFour} e limite de {formattedLimit} está liberado para uso."
            };
        }

        public static EventMessage CreateDepositEvent(string accountId, decimal amount)
        {
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));

            var formattedAmount = FormatAmount(amount);
            return new EventMessage
            {
                Date = DateTime.UtcNow,
                Action = NotificationType.DEPOSIT,
                UserOwner = accountId,
                Amount = formattedAmount,
                Service = ServiceType.DEPOSIT,
                Details = $"Depósito de {formattedAmount} realizado com sucesso em sua conta. Entre 1 a 30 min o valor será creditado."
            };
        }
    }
}