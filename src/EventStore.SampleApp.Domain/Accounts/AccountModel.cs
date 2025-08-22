using System.ComponentModel.DataAnnotations;

namespace EventStore.SampleApp.Domain.Accounts;

public class AccountModel
{
    [Key]
    public string Name { get; set; }
    public AccountType Type { get; set; }
    public decimal Balance { get; set; } = decimal.Zero;
    public string CreatedBy { get; set; }
}