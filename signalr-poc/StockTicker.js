// A simple templating method for replacing placeholders enclosed in curly braces.
if (!String.prototype.supplant) {
    String.prototype.supplant = function (o) {
        return this.replace(/{([^{}]*)}/g,
            function (a, b) {
                var r = o[b];
                return typeof r === 'string' || typeof r === 'number' ? r : a;
            }
        );
    };
}

$(function () {
    var ticker = $.connection.StockTicker;
    var up = "▲";
    var down = "▼";
    var table = $("#stockTable");
    var body = table.find("tbody");
    var rowTemplate = "<tr data-symbol='{Symbol}'><td>{Symbol}</td><td>{Price}</td><td>{DayOpen}</td><td>{Direction} {Change}</td><td>{PercentChange}</td></tr>";

    function formatStock(stock) {
        return $.extend(stock, {
            Price: stock.Price.toFixed(2),
            PercentChange: (stock.PercentChange * 100).toFixed(2) + "%",
            Direction: stock.Change === 0 ? "" : stock.Change > 0 ? up : down
        });
    }

    function init() {
        ticker.server.getAllStocks().done(function (stocks) {
            body.empty();

            $.each(stocks, function () {
                var stock = formatStock(this);

                body.append(rowTemplate.supplant(stock));
            });
        });
    }

    ticker.client.UpdateStockPrice = function (stock) {
        var display = formatStock(stock);
        var row = $(rowTemplate.supplant(display));

        body.find("tr[data-symbol=" + stock.Symbol + "]")
            .replaceWith(row);
    };

    //$.connection.hub.logging = true;
    $.connection.hub.start().done(init);
});