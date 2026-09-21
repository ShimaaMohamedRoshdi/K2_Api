namespace AccountDocApi.DTOs
{
    /// <summary>
    /// Represents the Account and Customer information returned to Nintex / API clients.
    /// </summary>
    public class AccountDto
    {
        /// <summary>
        /// Unique Account Number
        /// </summary>
        public string AccountNo { get; set; } = string.Empty;

        /// <summary>
        /// Customer Identifier
        /// </summary>
        public string CustomerId { get; set; } = string.Empty;

        /// <summary>
        /// Full Name of the Customer
        /// </summary>
        public string CustomerName { get; set; } = string.Empty;

        /// <summary>
        /// Type of Account (e.g. Savings, Checking, Corporate)
        /// </summary>
        public string AccountType { get; set; } = string.Empty;

        /// <summary>
        /// Current status of the Account (e.g. Active, Pending, Suspended)
        /// </summary>
        public string AccountStatus { get; set; } = string.Empty;

        /// <summary>
        /// Branch name where account is registered
        /// </summary>
        public string Branch { get; set; } = string.Empty;
    }
}
