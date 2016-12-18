function CurrencyCalculator(fromCurrency, toCurrency, amount) {
    this.fromCurrency = fromCurrency;
    this.toCurrency = toCurrency;
    this.amount = amount;

    this.calculate = function (resultCallback) {
        var url = "https://api.fixer.io/latest?base=" + this.fromCurrency;

        $.ajax({
            url: url,
            dataType: "json",
            context: this
        }).then(function (data) {
            var exchangeRate = data.rates[this.toCurrency];
            resultCallback(exchangeRate * amount);
        }, function (err) {
            console.log(err);
        });
    }
}