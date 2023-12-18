import pandas as pd
import numpy as np
import yfinance as yf


tickerSymbol = 'AAPL'

tickerData = yf.Ticker(tickerSymbol)

df = tickerData.history(period='1d', start='2023-12-10', end='2023-12-17', interval = '1m')


# Initialisatie
cash = 100000
position_size = 5000
max_positions = 4
open_positions = []
closed_positions = []

# Functie om een positie te openen
def open_position(timestamp, price, position_type):
    global cash
    global open_positions
    
    shares = position_size / price
    cash -= position_size
    open_positions.append({'timestamp': timestamp, 'price': price, 'type': position_type, 'shares': shares})

# Functie om een positie te sluiten
def close_position(timestamp, price, position):
    global cash
    global open_positions
    global closed_positions
    
    cash += position['shares'] * price
    closed_positions.append({'open_timestamp': position['timestamp'], 'close_timestamp': timestamp, 'open_price': position['price'], 'close_price': price, 'type': position['type'], 'profit': (price - position['price']) * position['shares']})
    open_positions.remove(position)

# Aangepaste iteratie over het dataframe met een current_index variabele
current_index = 0
while current_index < len(df):
    current_timestamp = df.index[current_index]
    current_stock_price = df.loc[current_timestamp, 'Open']
    
    # Check voor long posities
    for position in open_positions:
        if current_stock_price >= position['price'] * 1.03:
            close_position(current_timestamp, current_stock_price, position)
        elif current_stock_price >= position['price'] * 1.015:
            open_position(current_timestamp, current_stock_price, 'long')
    
    # Check voor short posities
    for position in open_positions:
        if position['type'] == 'short':
            if current_stock_price <= position['price'] * 0.97:
                close_position(current_timestamp, current_stock_price, position)
            elif current_stock_price <= position['price'] * 0.985:
                open_position(current_timestamp, current_stock_price, 'short')
    
    # Open een nieuwe long positie
    if len(open_positions) < max_positions and cash >= position_size:
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
