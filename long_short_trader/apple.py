import datetime
import yfinance as yf
import pytz
import random
import math

# start parameters
start_cash = 100000
cash = start_cash
current_timestamp = datetime.datetime.strptime('2023-12-11 09:30:00', '%Y-%m-%d %H:%M:%S')
current_timestamp = pytz.timezone('America/New_York').localize(current_timestamp)
# stop_timestamp = '2023-12-15 15:59:00-05:00'
stop_timestamp = datetime.datetime.strptime('2023-12-11 15:59:00', '%Y-%m-%d %H:%M:%S')
stop_timestamp = pytz.timezone('America/New_York').localize(stop_timestamp)
tickerSymbol = 'AAPL'
weights = [0.2, 0.8]
minimal_position_duration = 1
maximal_position_duration = 5
minimal_percentage_of_cash_for_position = 0.03
maximal_percentage_of_cash_for_position = 0.05

# Get data on this ticker
tickerData = yf.Ticker(tickerSymbol)

# Get the historical prices for this ticker
tickerDf = tickerData.history(period='1d', start='2023-12-10', end='2023-12-17', interval = '1m')

trades = []

def close_trade(trade: tuple):
    global trades
    global cash
    global current_timestamp

    current_stock_price = tickerDf.loc[current_timestamp, 'Open']
    # close trade
    sell_cash =  trade[1] * current_stock_price
    cash = cash + sell_cash
    # print(f"Closed trade for a result of: ${(sell_cash - trade[3]):,.2f}")

    #remove trade
    trades.remove(trade)

def close_expired_trades():
    global trades
    for trade in trades.copy():
        if trade[2] <= current_timestamp: # check for timestamp
            # close trade
            close_trade(trade)


def trade(minutes: int):
    global cash
    global trades
    global current_timestamp
    current_stock_price = tickerDf.loc[current_timestamp, 'Open']
    low_amount = math.floor((minimal_percentage_of_cash_for_position * cash) / current_stock_price)
    high_amount = math.floor((minimal_percentage_of_cash_for_position * cash) / current_stock_price)
    
    if high_amount > 0:
        # choose amount (based on available cash)
        amount = random.randint(low_amount, high_amount)
        buy_cash =(amount * current_stock_price)
        cash = cash - buy_cash
        trades.append(('APPL', amount, current_timestamp + datetime.timedelta(minutes=minutes), buy_cash))

def try_trade():
    # choose whether to make a trade or not
    if random.choices([True, False], weights)[0]:
        # choose minutes to hold position
        trade(random.randint(minimal_position_duration, maximal_position_duration))

def print_profit_loss():
    global cash
    global trades
    global current_timestamp

    capital = cash - start_cash
    current_stock_price = tickerDf.loc[current_timestamp, 'Open']
    for trade in trades:
        capital += current_stock_price * trade[1]
    print(f"Total profit/loss is: ${capital:,.2f}")

for i in range(10):
    current_index = 0
    cash = start_cash
    current_timestamp = datetime.datetime.strptime('2023-12-11 09:30:00', '%Y-%m-%d %H:%M:%S')
    while current_timestamp != stop_timestamp:
        # Get the current timestamp from the DataFrame using the current index
        current_timestamp = tickerDf.index[current_index]

        try_trade()
        close_expired_trades()

        # Increment the current index by 1
        current_index += 1

        # Check if the current index is the last index in the DataFrame
        if current_index >= len(tickerDf):
            print("Reached the end of the DataFrame.")
            break

    print()
    # print(f"Cash: ${cash:,.2f}")
    print_profit_loss()
