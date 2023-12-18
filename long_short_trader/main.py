import yfinance as yf
from position import Position

tickerSymbol = 'AAPL'

tickerData = yf.Ticker(tickerSymbol)

tickerDf = tickerData.history(period='1d', start='2023-12-10', end='2023-12-17', interval = '1m')

open_positions = []
closed_positions = []
cash = 100000
current_index = 1

long_target = 1.003
short_target = 0.97

long_stoploss = 0.97
short_stoploss = 1.03

def open_start_position():
    #globals
    global cash
    global long_target
    global long_stoploss
    global current_index
    global open_positions

    #open long position to start
    current_timestamp = tickerDf.index[current_index]
    current_stock_price = tickerDf.loc[current_timestamp, 'Open']
    amount = 5
    start_price = current_stock_price
    target_price = current_stock_price * long_target
    stoploss_price = current_stock_price * long_stoploss
    position = Position(ticker=tickerSymbol, amount=amount, start_timestamp=current_timestamp, start_price=start_price,target_price=target_price, stoploss_price=stoploss_price, type='long')
    open_positions.append(position)
    cash = cash - current_stock_price * amount

def try_open_new_position():
    global open_positions
    global current_index
    global cash

    # Do we need new positions?
    new_long_price = 0
    new_short_position = False

    current_timestamp = tickerDf.index[current_index]
    current_stock_price = tickerDf.loc[current_timestamp, 'Open']
    for position in open_positions:
        if position.type == 'long':
            new_long_price = position.require_new_position(current_stock_price, new_long_price)

    if new_long_price != 0:
        stoploss_price = current_stock_price * long_stoploss
        new_position = Position(ticker=tickerSymbol, amount=5, start_timestamp=current_timestamp, start_price=current_stock_price, target_price=new_long_price, stoploss_price=stoploss_price, type='long')
        if len(open_positions) < 3:
            open_positions.append(new_position)
            cash = cash - new_position.amount * current_stock_price

def close_position(position: Position):
    global open_positions
    global closed_position
    global current_index
    global tickerDf
    global cash

    open_positions.remove(position)
    closed_positions.append(position)

    # no short support yet
    if position.type == 'long':
        current_timestamp = tickerDf.index[current_index]
        current_stock_price = tickerDf.loc[current_timestamp, 'Open']
        cash += position.amount * current_stock_price

def check_open_positions():
    global open_positions
    global current_index
    
    current_timestamp = tickerDf.index[current_index]
    current_stock_price = tickerDf.loc[current_timestamp, 'Open']
    
    for position in open_positions.copy():
        if position.try_action(current_stock_price) == 'SELL':
            close_position(position)

    # Do we need new positions?
    try_open_new_position()

open_start_position()
while current_index < len(tickerDf):
    check_open_positions()
    current_index += 1

print(f"Cash: ${cash:,.2f}")
print(f'Open positions: {len(open_positions)}')
print(f'Closed positions: {len(closed_positions)}')