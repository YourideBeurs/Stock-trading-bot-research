import pandas as pd
import numpy as np
import yfinance as yf
import math

tickerSymbol = 'AAPL'

tickerData = yf.Ticker(tickerSymbol)

df = tickerData.history(period='1d', start='2023-12-10', end='2023-12-17', interval='1m')

# init
cash = 100000
max_position_size = 5000
max_positions = 4
open_positions = []
closed_positions = []

def open_position(timestamp, price, position_type):
    global cash
    global open_positions
    global max_position_size
    
    shares = math.floor(max_position_size / price)
    cash -= shares * price
    open_positions.append({'timestamp': timestamp, 'price': price, 'type': position_type, 'shares': shares})

def close_position(timestamp, price, position):
    global cash
    global open_positions
    global closed_positions
    
    cash += position['shares'] * price
    profit = (price - position['price']) * position['shares']
    if position['type'] == 'short':
        profit = -profit  # Invert profit for short positions
    closed_positions.append({'open_timestamp': position['timestamp'], 'close_timestamp': timestamp, 'open_price': position['price'], 'close_price': price, 'type': position['type'], 'profit': profit})
    open_positions.remove(position)

current_index = 0

# open start position
current_timestamp = df.index[current_index]
current_stock_price = df.loc[current_timestamp, 'Open']
open_position(current_timestamp, current_stock_price, 'long')
open_position(current_timestamp, current_stock_price, 'short')

while current_index < len(df):
    current_timestamp = df.index[current_index]
    current_stock_price = df.loc[current_timestamp, 'Open']

    open_new_long_position = False
    open_new_short_position = False
    
    # Check voor long en short posities
    for position in open_positions:
        # Close positions met winst of verlies
        if position['type'] == 'long':
            if current_stock_price >= position['price'] * 1.003:
                close_position(current_timestamp, current_stock_price, position)
            elif current_stock_price <= position['price'] * 0.97:
                close_position(current_timestamp, current_stock_price, position)
        elif position['type'] == 'short':
            if current_stock_price <= position['price'] * 0.97:
                close_position(current_timestamp, current_stock_price, position)
            elif current_stock_price >= position['price'] * 1.03:
                close_position(current_timestamp, current_stock_price, position)

        # Set parameters voor nieuwe posities
        if position['type'] == 'long' and current_stock_price >= position['price'] * 1.015:
            open_new_long_position = True
        elif position['type'] == 'short' and current_stock_price <= position['price'] * 0.985:
            open_new_short_position = True

    # Open nieuwe posities
    if open_new_long_position and len(open_positions) < max_positions and cash >= max_position_size:
        open_position(current_timestamp, current_stock_price, 'long')
    elif open_new_short_position and len(open_positions) < max_positions and cash >= max_position_size:
        open_position(current_timestamp, current_stock_price, 'short')

    if len(open_positions) == 0:
        open_position(current_timestamp, current_stock_price, 'long')

    current_index += 1

# Sluit alle openstaande posities aan het einde
last_timestamp = df.index[-1]
last_price = df['Open'].iloc[-1]
for position in open_positions:
    close_position(last_timestamp, last_price, position)

# Print de hoeveelheid cash en sla alle gesloten posities op
print(f"Hoeveelheid cash: ${cash:,.2f}")
closed_positions_df = pd.DataFrame(closed_positions)
closed_positions_df.to_csv('gesloten_posities.csv', index=False)
