﻿using GlobalPayments.Api.Builders;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.PaymentMethods;

namespace GlobalPayments.Api.Terminals.Builders {
    public class TerminalAuthBuilder : TerminalBuilder<TerminalAuthBuilder> {
        internal Address Address { get; set; }
        internal bool AllowDuplicates { get; set; }
        internal decimal? Amount { get; set; }
        internal string AuthCode {
            get {
                if (PaymentMethod is TransactionReference)
                    return (PaymentMethod as TransactionReference).AuthCode;
                return null;
            }
        }
        internal CurrencyType? Currency { get; set; }
        internal decimal? Gratuity { get; set; }
        internal string InvoiceNumber { get; set; }
        internal bool RequestMultiUseToken { get; set; }
        internal bool SignatureCapture { get; set; }
        internal string TransactionId {
            get {
                if (PaymentMethod is TransactionReference)
                    return (PaymentMethod as TransactionReference).TransactionId;
                return null;
            }
        }

        public TerminalAuthBuilder WithAddress(Address address) {
            Address = address;
            return this;
        }
        public TerminalAuthBuilder WithAllowDuplicates(bool allowDuplicates) {
            AllowDuplicates = allowDuplicates;
            return this;
        }
        public TerminalAuthBuilder WithAmount(decimal? amount) {
            Amount = amount;
            return this;
        }
        public TerminalAuthBuilder WithAuthCode(string value) {
            if (PaymentMethod == null || !(PaymentMethod is TransactionReference))
                PaymentMethod = new TransactionReference();
            (PaymentMethod as TransactionReference).AuthCode = value;
            return this;
        }
        public TerminalAuthBuilder WithCurrency(CurrencyType? value) {
            Currency = value;
            return this;
        }
        public TerminalAuthBuilder WithGratuity(decimal? gratuity) {
            Gratuity = gratuity;
            return this;
        }
        public TerminalAuthBuilder WithInvoiceNumber(string invoiceNumber) {
            this.InvoiceNumber = invoiceNumber;
            return this;
        }
        public TerminalAuthBuilder WithPaymentMethod(IPaymentMethod method) {
            PaymentMethod = method;
            return this;
        }
        public TerminalAuthBuilder WithRequestMultiUseToken(bool requestMultiUseToken) {
            RequestMultiUseToken = requestMultiUseToken;
            return this;
        }
        public TerminalAuthBuilder WithSignatureCapture(bool signatureCapture) {
            SignatureCapture = signatureCapture;
            return this;
        }
        public TerminalAuthBuilder WithToken(string value) {
            if (PaymentMethod == null || !(PaymentMethod is CreditCardData))
                PaymentMethod = new CreditCardData();
            (PaymentMethod as CreditCardData).Token = value;
            return this;
        }
        public TerminalAuthBuilder WithTransactionId(string value) {
            if (PaymentMethod == null || !(PaymentMethod is TransactionReference))
                PaymentMethod = new TransactionReference();
            (PaymentMethod as TransactionReference).TransactionId = value;
            return this;
        }

        internal TerminalAuthBuilder(TransactionType type, PaymentMethodType paymentType) : base(type, paymentType) {
        }

        public override TerminalResponse Execute() {
            base.Execute();

            var device = ServicesContainer.Instance.GetDeviceController();
            return device.ProcessTransaction(this);
        }

        protected override void SetupValidations() {
            Validations.For(TransactionType.Sale | TransactionType.Auth).Check(() => Amount).IsNotNull();
            Validations.For(TransactionType.Refund).Check(() => Amount).IsNotNull();
            Validations.For(TransactionType.Refund)
                .With(PaymentMethodType.Credit)
                .When(() => TransactionId).IsNotNull()
                .Check(() => AuthCode).IsNotNull();
            Validations.For(PaymentMethodType.Gift).Check(() => Currency).IsNotNull();
            Validations.For(TransactionType.AddValue).Check(() => Amount).IsNotNull();
        }
    }
}
