import yfinance as yf
import pandas as pd


tickers = yf.Tickers('msft aapl goog')

# data = tickers.tickers['AAPL'].history(period="1mo")

data = tickers.tickers['AAPL'].history(period='1mo', start='2022-12-26', end='2023-12-26', interval = '60m')

# print(data.head())
# print(data)

def comma_separator(x):
    return str(x).replace('.', ',')

# Apply the function to the 'Open' column
data['Open'] = data['Open'].apply(comma_separator)

data[['Open']].to_csv('C:\\Eigen data\\Research\\Stock research\\data.csv', index=True, sep=';')