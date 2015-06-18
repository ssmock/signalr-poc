using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace signalr_poc.StockTicker
{
    public class StockTicker
    {
        private IHubConnectionContext<dynamic> Clients { get; set; }

        private readonly static Lazy<StockTicker> _instance =
            new Lazy<StockTicker>(() => new StockTicker(
                GlobalHost.ConnectionManager.GetHubContext<StockTickerHub>().Clients));

        private readonly ConcurrentDictionary<string, Stock> _stocks = 
            new ConcurrentDictionary<string, Stock>();

        private readonly object _updateStockPricesLock = new object();

        private readonly double _rangePercent = .002;

        private readonly TimeSpan _updateInterval = 
            TimeSpan.FromMilliseconds(250);

        private readonly Random _updateOrNotRandom = new Random();

        private readonly Timer _timer;

        private volatile bool _isUpdatingPrices = false;

        private StockTicker(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;

            _stocks.Clear();

            var stocks = new List<Stock> {
                new Stock {Symbol = "MSFT", Price = 30.31M},
                new Stock {Symbol = "APPL", Price = 578.18M},
                new Stock {Symbol="GOOG", Price=570.30M}
            };

            stocks.ForEach(stock => _stocks.TryAdd(stock.Symbol, stock));

            _timer = new Timer(
                UpdateStockPrices, null, _updateInterval, _updateInterval);
        }

        public static StockTicker Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        private void UpdateStockPrices(object state)
        {
            lock(_updateStockPricesLock)
            {
                if(!_isUpdatingPrices)
                {
                    _isUpdatingPrices = true;

                    foreach(var stock in _stocks.Values)
                    {
                        if(TryUpdateStockPrice(stock))
                        {
                            BroadcastStockPrice(stock);
                        }
                    }

                    _isUpdatingPrices = false;
                }
            }
        }

        private bool TryUpdateStockPrice(Stock stock)
        {
            var r = _updateOrNotRandom.NextDouble();

            if(r > .1)
            {
                return false;
            }

            var random = new Random((int)Math.Floor(stock.Price));
            var percentChange = random.NextDouble() * _rangePercent;
            var isPositive = random.NextDouble() > .51;
            var change = Math.Round(stock.Price * (decimal)percentChange, 2);

            change = isPositive ? change : -change;

            stock.Price += change;

            return true;
        }

        private void BroadcastStockPrice(Stock stock)
        {
            Clients.All.UpdateStockPrice(stock);
        }

        internal IEnumerable<Stock> GetAllStocks()
        {
            return _stocks.Values;
        }
    }
}
