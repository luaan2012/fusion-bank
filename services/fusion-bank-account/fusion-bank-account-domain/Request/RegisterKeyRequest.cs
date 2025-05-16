using fusion.bank.core.Enum;

namespace fusion.bank.account.domain.Request
{
    public record RegisterKeyRequest(Guid AccountId, string KeyPix, KeyType KeyTypePix);
}
